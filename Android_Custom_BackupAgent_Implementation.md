# Android Cloud Backup with Custom BackupAgent

## Overview
This implementation uses a custom Android BackupAgent to provide full control over cloud backups to Google Drive. The BackupAgent only backs up data when the user has explicitly enabled cloud backups, ensuring a privacy-first approach.

## Implementation Architecture

### Custom BackupAgent Approach
Instead of using static `backup_rules.xml`, we use `KidsIdKitBackupAgent` which:
- Dynamically checks user preferences before every backup/restore
- Provides programmatic control over which files to backup
- Can reject backup/restore operations based on runtime conditions
- Enables detailed logging and debugging

## Files Created/Modified

### 1. KidsIdKitBackupAgent.cs (NEW)
**Location**: `KidsIdKit.Mobile/Platforms/Android/KidsIdKitBackupAgent.cs`

Custom `BackupAgentHelper` that extends Android's backup framework:

```csharp
public class KidsIdKitBackupAgent : BackupAgentHelper
{
	// Checks for preference file to determine if backup is enabled
	private bool IsCloudBackupEnabled()

	// Returns list of files to backup (excludes preference file)
	private string[] GetFilesToBackup()

	// Sets up backup helpers only if enabled
	public override void OnCreate()

	// Performs backup only if enabled
	public override void OnBackup(...)

	// Restores data only if enabled
	public override void OnRestore(...)
}
```

**Key Features**:
- Only backs up when `KidsIdKit_CloudBackupEnabled` file exists
- Excludes the preference file itself from backup
- Writes empty state when backup is disabled
- Logs all backup operations for debugging

### 2. AndroidManifest.xml (MODIFIED)
Added custom backup agent reference:
```xml
<application 
	android:allowBackup="true" 
	android:backupAgent="kidsidkit.mobile.platforms.android.KidsIdKitBackupAgent"
	...>
```

### 3. CloudBackupService.cs (MODIFIED)
Added Android-specific backup triggering:
- `EnableCloudBackupAsync()`: Calls `RequestAndroidBackup()` to trigger immediate backup
- `DisableCloudBackupAsync()`: Logs that existing backups remain
- `RequestAndroidBackup()`: Uses `BackupManager.DataChanged()` to schedule backup

### 4. FileStorageService.cs (MODIFIED)
Added Android backup request after file writes:
```csharp
#elif ANDROID
	if (isCloudBackupEnabled)
	{
		var backupManager = new Android.App.Backup.BackupManager(context);
		backupManager.DataChanged(); // Notifies Android of data change
	}
#endif
```

## How It Works

### Backup Flow (When Enabled)

1. **User Enables Backup**
   ```
   User → Settings → Enable Cloud Backup
   ↓
   CloudBackupService.EnableCloudBackupAsync()
   ↓
   Creates: KidsIdKit_CloudBackupEnabled file
   ↓
   Calls: BackupManager.DataChanged()
   ```

2. **Android Schedules Backup**
   ```
   BackupManager receives notification
   ↓
   Android schedules backup (within 24 hours or immediately if conditions met)
   ↓
   Calls: KidsIdKitBackupAgent.OnCreate()
   ```

3. **Agent Prepares Backup**
   ```
   OnCreate() checks: IsCloudBackupEnabled()
   ↓
   If true: Creates FileBackupHelper for app files
   ↓
   Calls: GetFilesToBackup() to get file list
   ↓
   Sets up: AddHelper(FilesBackupKey, fileBackupHelper)
   ```

4. **Backup Executes**
   ```
   Android calls: OnBackup(oldState, data, newState)
   ↓
   Agent checks: IsCloudBackupEnabled() again
   ↓
   If true: base.OnBackup() performs actual backup
   ↓
   Data uploaded to Google Drive (encrypted)
   ```

5. **Subsequent File Changes**
   ```
   App writes file
   ↓
   FileStorageService.SetFileBackupAsync()
   ↓
   Calls: BackupManager.DataChanged()
   ↓
   Process repeats from step 2
   ```

### Backup Flow (When Disabled)

1. **User Disables Backup**
   ```
   User → Settings → Disable Cloud Backup
   ↓
   CloudBackupService.DisableCloudBackupAsync()
   ↓
   Deletes: KidsIdKit_CloudBackupEnabled file
   ```

2. **Next Backup Attempt**
   ```
   Android calls: OnCreate()
   ↓
   IsCloudBackupEnabled() returns false
   ↓
   No backup helpers added
   ```

3. **OnBackup Called**
   ```
   OnBackup() checks: IsCloudBackupEnabled()
   ↓
   Returns false
   ↓
   Writes empty state to newState
   ↓
   No data backed up
   ```

### Restore Flow

1. **App Installed on New Device**
   ```
   User installs app
   ↓
   Android detects backup in Google Drive
   ↓
   Calls: KidsIdKitBackupAgent.OnRestore()
   ```

2. **Agent Checks Preference**
   ```
   OnRestore() checks: IsCloudBackupEnabled()
   ↓
   If false: Does nothing (no restore)
   ↓
   If true: base.OnRestore() restores files
   ```

3. **Restore Completes**
   ```
   OnRestoreFinished() called
   ↓
   Logs completion
   ↓
   App has restored data
   ```

## Testing Guide

### 1. Enable Backup Test
```bash
# 1. Enable backup in app
# 2. Add test data
# 3. Force backup
adb shell bmgr backupnow com.missingchildrenmn.kidsidkit

# 4. Check backup status
adb shell dumpsys backup com.missingchildrenmn.kidsidkit
```

### 2. View Agent Logs
```bash
# View backup agent debug output
adb logcat | grep "KidsIdKit:"
```

### 3. Test Restore
```bash
# Uninstall (keeps backup)
adb uninstall com.missingchildrenmn.kidsidkit

# Reinstall
adb install path/to/app.apk

# Restore
adb shell bmgr restore com.missingchildrenmn.kidsidkit
```

### 4. Test Disabled Backup
```bash
# 1. Disable backup in app
# 2. Modify data
# 3. Force backup attempt
adb shell bmgr backupnow com.missingchildrenmn.kidsidkit

# 4. Check logs - should show empty state written
adb logcat | grep "KidsIdKit:"
```

## Privacy & Security

### Privacy Features
- **Opt-in Only**: Backup disabled by default, user must enable
- **Runtime Checks**: Agent checks preference before every backup/restore
- **Selective Backup**: Only app data files backed up, preference excluded
- **User Control**: User can disable at any time
- **No MCM Access**: Missing Children Minnesota cannot access backups

### Security Features
- **Google Encryption**: All data encrypted by Google before upload
- **Personal Account**: Backed up to user's own Google Drive
- **Quota Limited**: 25MB limit prevents excessive data collection
- **Restore Control**: Agent can reject restore if preferences don't allow

## Advantages Over backup_rules.xml

| Feature | backup_rules.xml | Custom BackupAgent |
|---------|------------------|-------------------|
| User Preference Check | ❌ Static rules | ✅ Runtime check |
| Dynamic Behavior | ❌ Fixed at build | ✅ Changes at runtime |
| Restore Control | ❌ Always restores | ✅ Can reject restore |
| Debugging | ❌ Limited logging | ✅ Full logging |
| File Selection | ❌ Path patterns | ✅ Programmatic logic |
| Conditional Logic | ❌ None | ✅ Full C# code |

## Platform Comparison

| Feature | iOS | Android (Custom Agent) |
|---------|-----|------------------------|
| Cloud Provider | iCloud | Google Drive |
| Default State | Excluded | Excluded |
| User Control | File attribute | Preference file check |
| Enable Method | SetSkipBackupAttribute(false) | Create preference file |
| Disable Method | SetSkipBackupAttribute(true) | Delete preference file |
| Manual Trigger | Automatic | BackupManager.DataChanged() |
| Delete Backups | Automatic | Manual (via Google) |
| Restore Control | Automatic | Agent can reject |

## Troubleshooting

### Backup Not Working
1. Check preference file exists: `ls /data/data/com.missingchildrenmn.kidsidkit/files/`
2. Check logcat for agent logs: `adb logcat | grep KidsIdKit`
3. Verify manifest has correct agent name
4. Check Google account on device

### Restore Not Working
1. Verify backup exists: `adb shell dumpsys backup`
2. Check agent logs during restore
3. Ensure preference file allows restore
4. Verify app signature matches

### Quota Exceeded
1. Check data size: Should be < 25MB
2. View OnQuotaExceeded logs
3. Consider excluding large files
4. Implement data compression

## Future Enhancements

Possible improvements:
1. Add user notification when quota exceeded
2. Implement selective file backup (e.g., exclude images)
3. Add backup compression
4. Provide backup statistics to user
5. Add backup verification/integrity checks
6. Implement incremental backups

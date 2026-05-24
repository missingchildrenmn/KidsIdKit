# Android Backup Agent Testing Guide

## Why the Backup Agent Isn't Called Automatically

The Android backup agent **does not run automatically** during normal app usage. Android's backup system runs:
- On a schedule determined by the Android OS (usually every 24 hours)
- When triggered manually via ADB commands
- When the user performs a device backup

## Testing the Backup Agent

### Prerequisites
1. Enable USB Debugging on your Android device/emulator
2. Connect device via ADB
3. Ensure the app is installed

### Step 1: Enable Cloud Backup in the App
1. Launch the KidsIdKit app
2. Go to Settings
3. Toggle "Allow Cloud Backups" to ON
4. Confirm the warning dialog

### Step 2: Manually Trigger a Backup via ADB

```powershell
# Replace com.missingchildrenmn.kidsidkit with your actual package name
$packageName = "com.missingchildrenmn.kidsidkit"

# Initialize the backup manager (only needed once per device)
adb shell bmgr enable true

# Request a backup for your app (this will call OnBackup)
adb shell bmgr backupnow $packageName

# Alternative: Force run the backup (this will call OnBackup)
adb shell bmgr run

# Check backup status
adb shell bmgr list transports
```

### Step 3: View Debug Output

In Visual Studio:
1. Open **View → Output**
2. Select **Show output from: Debug**
3. Look for log messages starting with "KidsIdKit:"

Expected log output:
```
KidsIdKit: BackupAgent constructor called
KidsIdKit: BackupAgent.OnCreate called
KidsIdKit: Cloud backup is enabled, setting up FileBackupHelper
KidsIdKit: Preparing to backup X files
KidsIdKit: BackupAgent.OnBackup called
KidsIdKit: Performing backup
```

### Step 4: Test Restore

```powershell
# List available backup sets
adb shell bmgr list sets

# Restore from backup (this will call OnRestore)
adb shell bmgr restore $packageName

# Alternative: Restore from a specific backup set token
adb shell bmgr restore <token> $packageName
```

### Step 5: Verify Backup Data

```powershell
# Check what's in the backup
adb shell dumpsys backup

# View backup data for your app specifically
adb shell dumpsys backup $packageName
```

## Troubleshooting

### Issue: "KidsIdKit: BackupAgent constructor called" never appears

**Solution 1: Verify manifest merge**
```powershell
# After building, check the merged manifest
# Look in: obj/Debug/net10.0-android/AndroidManifest.xml
# Verify it contains: android:backupAgent="..."
```

**Solution 2: Check backup is enabled**
```powershell
# Verify backup is enabled on the device
adb shell bmgr enabled
# Should output: Backup Manager currently enabled
```

**Solution 3: Rebuild and reinstall**
```powershell
# Clean and rebuild
dotnet clean
dotnet build

# Uninstall old version
adb uninstall $packageName

# Reinstall
# (Use Visual Studio or: adb install path/to/apk)
```

### Issue: "Cloud backup is disabled, skipping backup setup"

This means:
- The cloud backup preference file doesn't exist
- The file path is incorrect
- User has not enabled cloud backup in Settings

**Check the preference file:**
```powershell
# View app's files directory
adb shell run-as $packageName ls -la files/

# Should show: KidsIdKit_CloudBackupEnabled
```

### Issue: Backup runs but no files are backed up

**Check files in the app directory:**
```powershell
adb shell run-as $packageName ls -la files/
```

If empty, create test data:
1. Add a family in the app
2. Take a photo
3. Save data
4. Then trigger backup again

## Development Tips

1. **Add more logging:** The backup agent already has extensive logging. Watch the Debug output window.

2. **Test both enabled and disabled states:**
   - Enable cloud backup → trigger backup → verify files are backed up
   - Disable cloud backup → trigger backup → verify empty state is written

3. **Test restore:**
   - Back up data
   - Uninstall app
   - Reinstall app
   - Trigger restore
   - Verify data is restored

4. **Emulator vs Real Device:**
   - Emulators may have backup disabled by default
   - Real devices connected to Google accounts work better
   - Some emulators don't have Google Play Services

## Production Behavior

In production, the backup agent will:
- Run automatically when Android schedules a backup (typically daily)
- Backup to the user's Google account (if signed in)
- Restore automatically when the app is reinstalled on a new device
- Not require any manual ADB commands

## Common ADB Commands Reference

```powershell
# Enable backup system
adb shell bmgr enable true

# Backup now
adb shell bmgr backupnow com.missingchildrenmn.kidsidkit

# Full backup run
adb shell bmgr run

# List transports (backup destinations)
adb shell bmgr list transports

# Check backup status
adb shell dumpsys backup

# Restore
adb shell bmgr restore com.missingchildrenmn.kidsidkit

# Wipe backup data (for testing)
adb shell bmgr wipe com.android.localtransport com.missingchildrenmn.kidsidkit
```

## Expected Behavior Summary

✅ **When Cloud Backup is ENABLED:**
- `OnCreate()` sets up FileBackupHelper
- `OnBackup()` backs up all files in app's files directory
- Files sync to Google Drive
- `OnRestore()` restores files on new devices

✅ **When Cloud Backup is DISABLED:**
- `OnCreate()` skips FileBackupHelper setup
- `OnBackup()` writes empty state (prevents backup)
- No files sync to Google Drive
- Data remains local-only


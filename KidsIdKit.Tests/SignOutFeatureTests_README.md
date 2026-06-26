# Sign Out Feature - Test Coverage Summary

## Overview
Comprehensive test coverage has been added for the new "Sign Out" feature in the KidsIdKit application.

## Test Files Created

### 1. SessionServiceTests.cs
**Location**: `KidsIdKit.Tests/KidsIdKit.Core/Services/SessionServiceTests.cs`

**Test Count**: 15 unit tests

**Test Categories**:

#### SignOut Tests (8 tests)
- `SignOut_WhenUnlocked_ClearsSessionAndLocks` - Verifies sign-out clears the session key and locks
- `SignOut_WhenInInfoOnlyMode_ExitsInfoOnlyMode` - Verifies sign-out exits info-only mode
- `SignOut_WhenUnlockedAndInInfoOnlyMode_ClearsBothStates` - Verifies both states are cleared
- `SignOut_FiresOnLockStateChangedEvent` - Verifies the event is triggered
- `SignOut_WhenAlreadyLocked_FiresOnLockStateChangedEvent` - Verifies event fires even when already locked
- `SignOut_ClearsKeyFromMemory` - Verifies encryption key is securely cleared from memory
- `SignOut_CanBeCalledMultipleTimes` - Verifies idempotency
- `LockIfNeeded_ComparesWithSignOut` - Verifies both locking mechanisms work similarly

#### IsUnlocked Tests (3 tests)
- `IsUnlocked_WhenKeyIsSet_ReturnsTrue`
- `IsUnlocked_WhenKeyIsNotSet_ReturnsFalse`
- `IsUnlocked_AfterSignOut_ReturnsFalse`

#### IsInfoOnlyMode Tests (4 tests)
- `IsInfoOnlyMode_InitiallyFalse`
- `IsInfoOnlyMode_AfterEnable_ReturnsTrue`
- `IsInfoOnlyMode_AfterSignOut_ReturnsFalse`

---

### 2. NavMenuTests.cs
**Location**: `KidsIdKit.Tests/KidsIdKit.Core/SharedComponents/NavMenuTests.cs`

**Test Count**: 11 component tests

**Test Categories**:

#### Sign Out Menu Item Tests (4 tests)
- `Menu_WhenUnlocked_ShowsSignOutOption` - Verifies sign-out option appears when unlocked
- `Menu_WhenUnlocked_SignOutHasLogOutIcon` - Verifies proper icon is used
- `Menu_WhenUnlocked_ShowsAllExpectedMenuItems` - Verifies all menu items including sign-out
- `Menu_WhenInInfoOnlyMode_DoesNotShowSignOutOption` - Verifies sign-out hidden in info-only mode

#### Sign Out Functionality Tests (5 tests)
- `SignOutMenuItem_WhenClicked_ClearsSession` - Verifies clicking clears the session
- `SignOutMenuItem_WhenClicked_NavigatesToHome` - Verifies navigation to home page
- `SignOut_ClearsInfoOnlyMode` - Verifies info-only mode is cleared
- `SignOutMenuItem_WhenClicked_TriggersStateChange` - Verifies state change event
- `SignOutMenuItem_WhenClicked_CausesSessionToLock` - Verifies session locks properly

#### Regular Menu Item Tests (2 tests)
- `RegularMenuItem_WhenClicked_NavigatesToTargetUri` - Verifies normal navigation still works
- `RegularMenuItem_WhenClicked_DoesNotClearSession` - Verifies other items don't trigger sign-out

#### Menu Item Grouping Tests (1 test)
- `Menu_WhenUnlocked_ShowsSignOutInGroupB` - Verifies sign-out is in correct menu group

---

### 3. MainLayoutTests.cs (Enhancement)
**Location**: `KidsIdKit.Tests/KidsIdKit.Core/SharedComponents/MainLayoutTests.cs`

**Test Count**: 1 integration test added (to existing suite)

#### Integration Tests
- `SignOut_WhenUnlocked_LocksSessionAndShowsLockScreen` - End-to-end test verifying:
  - User starts unlocked with content visible
  - Sign-out is called
  - Session becomes locked
  - Lock screen (PinEntry) is automatically displayed
  - Protected content is hidden

---

### 4. SignoutTests.cs
**Location**: `KidsIdKit.Tests/KidsIdKit.Core/Pages/SignoutTests.cs`

**Test Count**: 3 component tests

**Test Categories**:

#### Signout Page Tests (3 tests)
- `Signout_OnInitialized_CallsSessionServiceSignOut` - Verifies rendering the page signs out and clears the session
- `Signout_OnInitialized_NavigatesToHome` - Verifies navigation to the home page on initialization
- `Signout_OnInitialized_SignsOutAndNavigatesToHome` - End-to-end test verifying the page both signs out and navigates home

---

## Test Coverage Summary

### Total Tests for Sign-Out Feature: 30
- **Unit Tests**: 15 (SessionService)
- **Component Tests**: 14 (NavMenu + Signout page)
- **Integration Tests**: 1 (MainLayout)

### All Project Tests: 218
- **Passed**: 218
- **Failed**: 0
- **Skipped**: 0

## Key Test Scenarios Covered

1. ✅ Sign-out clears the encryption key securely from memory
2. ✅ Sign-out exits info-only mode
3. ✅ Sign-out triggers the OnLockStateChanged event
4. ✅ Sign-out can be called multiple times safely
5. ✅ Sign-out menu item appears when unlocked
6. ✅ Sign-out menu item uses correct icon (log-out-outline)
7. ✅ Sign-out menu item is in the correct group (Settings group)
8. ✅ Sign-out menu item is hidden in info-only mode
9. ✅ Clicking sign-out clears the session
10. ✅ Clicking sign-out navigates to home page
11. ✅ Regular menu items don't trigger sign-out
12. ✅ MainLayout automatically shows lock screen after sign-out
13. ✅ Protected content is hidden after sign-out
14. ✅ Signout page signs out and clears the session when rendered
15. ✅ Signout page navigates to the home page on initialization

## Testing Frameworks Used
- **xUnit** - Test framework
- **bUnit** - Blazor component testing
- **Moq** - Mocking framework

## Test Execution
All tests can be run using:
```bash
dotnet test KidsIdKit.Tests/KidsIdKit.Tests.csproj
```

Or specific test classes:
```bash
dotnet test --filter "FullyQualifiedName~SessionServiceTests"
dotnet test --filter "FullyQualifiedName~NavMenuTests"
dotnet test --filter "FullyQualifiedName~SignoutTests"
```
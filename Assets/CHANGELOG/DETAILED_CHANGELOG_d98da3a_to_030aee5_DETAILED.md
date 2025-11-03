# Detailed Changelog: d98da3a to 030aee5

**Generated:** 2025-11-03
**Commit Range:** d98da3a..030aee5 (5 commits)
**Author:** Sotatek-ThangDo (thangdd@sotanext.com)

---

## Summary of Changes

This changelog documents bug fixes and improvements related to:
1. **Lockdown State Handling** - Added comprehensive lockdown checks across multiple character classes
2. **Network Disconnection Handling** - Improved popup detection for network errors
3. **Wave Record Management** - Enhanced play record tracking for wave completion and field boss scenarios
4. **Ranking System** - Adjusted player ranking logic to remove boss kill consideration

---

## Commits Overview

### 1. **030aee5** - addition check for lockdown
**Date:** 2025-11-03 13:36:28 +0700

Added lockdown state checks across multiple character classes to prevent actions during lockdown periods.

**Files Modified:**
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterBard.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterFlora.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterHighnickel.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterKanade.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterLulu.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterRio.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterTrickster.cs`
- `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterYuri.cs`

---

### 2. **6063ffe** - addition check for popup disconnected
**Date:** 2025-11-03 11:48:00 +0700

Added validation to prevent duplicate system notice popups when the player is already viewing battle result screens.

**Files Modified:**
- `Assets/02.Scripts/Manager/NetworkConnect.cs`
- `Assets/02.Scripts/UI/Popup/PopupTemplate.cs`

---

### 3. **2173a16** - addition call record end last wave
**Date:** 2025-11-02 19:20:49 +0700

Modified wave completion flow to ensure play record data is set before network RPC call.

**Files Modified:**
- `Assets/02.Scripts/Manager/MonsterSpawner.cs`

---

### 4. **bfb19fa** - addition fill remain level record when other reached field boss first
**Date:** 2025-11-02 18:56:13 +0700

Added logic to fill remaining wave records when transitioning to field boss wave, and improved debug logging.

**Files Modified:**
- `Assets/02.Scripts/Manager/GameManager.cs`

---

### 5. **0d74821** - addition change log
**Date:** 2025-11-02 17:42:40 +0700

Added previous detailed changelog and updated project files.

**Files Modified:**
- `Assets/CHANGELOG/DETAILED_CHANGELOG_aa07756_to_d98da3a_DETAILED.md` (Added)
- `Assets/CHANGELOG/DETAILED_CHANGELOG_aa07756_to_d98da3a_DETAILED.md.meta` (Added)
- `Assets/AddressableAssetsData/link.xml` (Deleted)
- `Assets/AddressableAssetsData/link.xml.meta` (Deleted)
- `Assets/StreamingAssets/build_info`
- `Assets/08.Materials/HalfToneTransition.mat`

---

## Detailed File Changes

### Character System - Lockdown State Implementation

#### **CharacterBard.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterBard.cs`

**Changes:**
```csharp
// Line ~95 - SetSpeedBuffCharacter method
- if (!isSummoned) return;
+ if (!isSummoned || IsLockdown) return;
```

**Impact:** Prevents speed buff application when character is in lockdown state.

---

#### **CharacterFlora.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterFlora.cs`

**Changes:**
```csharp
// Line ~22 - ActionSequence coroutine
- while (isSummoned)
+ while (isSummoned && !IsLockdown)
```

**Impact:** Stops attack animation loop when lockdown is active.

---

#### **CharacterHighnickel.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterHighnickel.cs`

**Changes:**
```csharp
// Lines 22-45 - ActionSequence coroutine completely wrapped in lockdown check
+ if (!IsLockdown)
+ {
    // Entire attack sequence (animation, projectile spawning)
+ }
```

**Impact:** Prevents entire attack sequence execution during lockdown, including:
- Attack animation
- Target cache iteration
- Projectile spawning and initialization
- Animation completion waiting

---

#### **CharacterKanade.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterKanade.cs`

**Changes:**
```csharp
// Line ~34 - ActionSequence coroutine
- while (isSummoned)
+ while (isSummoned && !IsLockdown)
```

**Impact:** Prevents bomb placement during lockdown.

---

#### **CharacterLulu.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterLulu.cs`

**Changes:**
```csharp
// Line ~50 - ActionSequence coroutine
- while (isSummoned)
+ while (isSummoned && !IsLockdown)
```

**Impact:** Prevents bubble shield creation during lockdown.

---

#### **CharacterRio.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterRio.cs`

**Changes:**
```csharp
// Line ~27 - ActionSequence coroutine
- while (isSummoned)
+ while (isSummoned && !IsLockdown)
```

**Impact:** Prevents attack sequence during lockdown.

---

#### **CharacterTrickster.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterTrickster.cs`

**Changes:**
```csharp
// Line ~123 - CharacterAction lambda
- if (isSummoned && targetedMonster.Count > 0 && !isStun)
+ if (isSummoned && targetedMonster.Count > 0 && !isStun && !IsLockdown)
```

**Impact:** Adds lockdown check to character action trigger condition.

---

#### **CharacterYuri.cs**
**Location:** `Assets/02.Scripts/DefenseObject/Character/Characters/CharacterYuri.cs`

**Changes:**
```csharp
// Line ~30 - ActionSequence coroutine
- while (isSummoned)
+ while (isSummoned && !IsLockdown)

// Line ~32 - Inner condition
- if (targetedMonster.Count > 0 && characterState == CharacterState.DETECT)
+ if (targetedMonster.Count > 0 && characterState == CharacterState.DETECT && !IsLockdown)
```

**Impact:** Double lockdown protection - both in main loop and attack trigger condition.

---

### Network & UI System

#### **NetworkConnect.cs**
**Location:** `Assets/02.Scripts/Manager/NetworkConnect.cs`

**Changes:**

1. **Enhanced Lobby Connection Logging (Line ~96)**
```csharp
- Debug.Log($"[NetworkConnect] Connecting to lobby... ~ Is Friendly Match: {isFriendlyMatch}");
+ Debug.Log($"[NetworkConnect] Connecting to lobby... ~ Is Friendly Match: {isFriendlyMatch} ~ RoomName: {roomName} ~ RoomPassword: {roomPassword}");
```

2. **Fixed Room Password (Line ~271)**
```csharp
// Custom room properties
- ["password"] = UserInfoManager.Instance.userId,
+ ["password"] = roomPassword,
```

3. **Popup Duplication Prevention (Lines ~643-645)**
```csharp
+ if (UIManager.Instance.battleResultPopup.IsActived || UIManager.Instance.battleResultTablePopup.IsActived || UIManager.Instance.battleResultPromotePopup.IsActived)
+     return;
```

4. **Reconnection Logic (Line ~848)**
```csharp
- ConnectToLobby(isFriendlyMatch);
+ ConnectToLobby(isFriendlyMatch, UserInfoManager.Instance.userId, UserInfoManager.Instance.userId);
```

5. **Ranking Logic Update (Lines ~1310-1313)**
```csharp
normalPlayers = normalPlayers
    .OrderByDescending(p => p.waveCount)          // Higher wave better
-   .ThenByDescending(p => p.monsterBossKilled)   // Then boss kills
+                                                  // Removed boss kill consideration
    .ThenByDescending(p => p.monsterKilled)       // Then normal kills
```

**Impact:**
- Improved debugging with more detailed logging
- Fixed room password to use provided parameter instead of hardcoded userId
- Prevents system notice popup when battle result screens are already displayed
- Ensures proper room credentials on reconnection
- Simplified ranking to focus on wave progression and total monster kills

---

#### **PopupTemplate.cs**
**Location:** `Assets/02.Scripts/UI/Popup/PopupTemplate.cs`

**Changes:**
```csharp
+ public bool IsActived => canvasGroup.alpha == 1 && canvasGroup.blocksRaycasts && canvasGroup.interactable;
```

**Impact:** Added property to check if a popup is currently active and visible to the user.

---

### Game Manager & Wave System

#### **GameManager.cs**
**Location:** `Assets/02.Scripts/Manager/GameManager.cs`

**Changes:**

1. **Enhanced Logging (Line ~866)**
```csharp
- Debug.Log($"SetPlayRecord Data in stage {gameState}");
+ Debug.Log($"SetPlayRecord Data in stage {gameState} ~ wave {waveIdx}");
```

2. **Fill Remaining Wave Records (Lines ~1049-1056)**
```csharp
+ for (int i = waveIdx; i < roundId; i++)
+ {
+     if (i == waveIdx)
+         SetPlayRecordData(monsterSpawner.killedBossLevel, i);
+     else
+         SetPlayRecordData(0, i);
+ }
```

**Impact:**
- Better debugging information with wave index
- Ensures all wave records are filled when jumping to field boss wave
- First record gets actual boss kill data, remaining waves get zero-filled records

---

#### **MonsterSpawner.cs**
**Location:** `Assets/02.Scripts/Manager/MonsterSpawner.cs`

**Changes:**
```csharp
// Lines ~527-528 - Reordered operation sequence
- NetworkConnect.Instance.networkGameManager.Rpc_WaveComplete(NetworkConnect.Instance.playerIdx, currentWaveIdx, totalKilledMonsterCount, totalKilledBossMonsterCount);
  GameManager.Instance.SetPlayRecordData(killedBossLevel, currentWaveIdx);
+ NetworkConnect.Instance.networkGameManager.Rpc_WaveComplete(NetworkConnect.Instance.playerIdx, currentWaveIdx, totalKilledMonsterCount, totalKilledBossMonsterCount);
```

**Impact:** Ensures local play record is saved before network synchronization occurs.

---

### Material & Asset Changes

#### **HalfToneTransition.mat**
**Location:** `Assets/08.Materials/HalfToneTransition.mat`

**Changes:**
```yaml
- _DotScale: 0
+ _DotScale: 0.00099051
- _X_directionToggleLeft: 1
- _X_directionToggleRight: 0
+ _X_directionToggleLeft: 0
+ _X_directionToggleRight: 1
```

**Impact:** Adjusted transition effect parameters for visual polish.

---

#### **Addressable Assets**
**Removed Files:**
- `Assets/AddressableAssetsData/link.xml`
- `Assets/AddressableAssetsData/link.xml.meta`

**Impact:** Cleanup of generated/outdated addressable asset configuration files (214 lines removed).

---

## Statistics

- **Total Commits:** 5
- **Files Changed:** 18
- **Insertions:** 1,695
- **Deletions:** 254
- **Net Change:** +1,441 lines

### Breakdown by Category:

| Category | Files Modified | Description |
|----------|---------------|-------------|
| Character System | 8 | Lockdown state implementation |
| Network/UI | 2 | Popup handling & network fixes |
| Game Management | 2 | Wave record tracking improvements |
| Assets | 4 | Material updates & cleanup |
| Documentation | 2 | Changelog addition |

---

## Technical Notes

### Lockdown State Pattern
The lockdown state check has been implemented consistently across character classes using two patterns:

1. **Early Return Pattern** (e.g., CharacterBard):
```csharp
if (!isSummoned || IsLockdown) return;
```

2. **Loop Condition Pattern** (e.g., CharacterFlora, Kanade, Lulu, Rio):
```csharp
while (isSummoned && !IsLockdown)
```

3. **Block Wrapping Pattern** (CharacterHighnickel):
```csharp
if (!IsLockdown) { /* entire action sequence */ }
```

### Wave Record Management
The wave record system now handles edge cases where players reach field boss waves at different times:
- Fills intermediate wave records automatically
- Preserves boss kill data for current wave
- Zero-fills skipped waves to maintain data consistency

### Network Improvements
- Room password now correctly uses provided parameter
- Prevents duplicate error popups during battle result display
- Simplified ranking algorithm focuses on wave progression

---

## Testing Recommendations

1. **Lockdown State**
   - Verify all 8 modified characters stop actions during lockdown
   - Test lockdown activation/deactivation transitions
   - Confirm no lingering effects after lockdown ends

2. **Wave Records**
   - Test field boss wave transitions with players at different progress
   - Verify wave record data integrity
   - Check database entries for completeness

3. **Network & UI**
   - Test disconnect scenarios during battle result screens
   - Verify room password functionality in friendly matches
   - Validate player ranking calculation

4. **Visual Effects**
   - Verify HalfToneTransition material changes in-game

---

## Migration Notes

No migration required. All changes are backward compatible.

---

**End of Changelog**

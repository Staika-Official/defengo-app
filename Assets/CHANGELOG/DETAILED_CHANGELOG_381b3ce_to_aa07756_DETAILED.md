# DETAILED Changelog: 381b3ce → aa07756

**Period**: Unity Version Update and UI Language Fix
**Date Range**: 381b3ce (Save FieldBoss Feature) → aa07756 (Addition UI fix - Language)
**Major Theme**: Unity 2022.3.62f2 Update, Friend System UI Improvements, Language Support Enhancement

---

## 📊 Statistics Overview

```
Total Commits:           3 commits
Total Files Changed:     17 files
C# Scripts Modified:     1 file (+24 lines, -7 lines)
Prefabs Modified:        1 file (+283 lines, -87 lines)
Language Files:          4 files (+16 new entries)
Unity Version:           Updated to 2022.3.62f2
Changelog Files:         +2 new changelog files (+1,740 lines)
```

---

## 📝 COMMIT BREAKDOWN

### Commit 1: f59df9b - "addition change log"
**Changes**: Added detailed changelog documentation
**Files**:
- `DETAILED_CHANGELOG_c6c1652_to_381b3ce.md` (490 lines)
- `DETAILED_CHANGELOG_c6c1652_to_381b3ce_DETAILED.md` (1,250 lines)

**Purpose**: Documentation of Field Boss Feature development

---

### Commit 2: f4cd2f6 - "Addition UNITY VERSION 2022.3.62f2"
**Changes**: Unity version upgrade and package updates

**Files Modified**:
1. **ProjectSettings/ProjectVersion.txt**
   - Unity version: 2022.3.51f1 → **2022.3.62f2**
   - Serialization: 2022.3.51f1 → **2022.3.62f2**

2. **Packages/manifest.json** - Package version updates:
   ```diff
   - "com.unity.2d.animation": "9.1.0"
   + "com.unity.2d.animation": "9.1.1"

   - "com.unity.2d.psdimporter": "8.0.4"
   + "com.unity.2d.psdimporter": "8.0.5"

   - "com.unity.collab-proxy": "2.3.1"
   + "com.unity.collab-proxy": "2.4.4"

   - "com.unity.test-framework": "1.3.9"
   + "com.unity.test-framework": "1.4.5"
   ```

3. **Packages/packages-lock.json** - Locked package versions updated (64 lines changed)

4. **Assets/AddressableAssetsData/link.xml** - DELETED
   - Removed entire file (214 lines removed)
   - File was auto-generated and no longer needed with new Unity version

**Purpose**: Keep Unity engine up to date with latest LTS patch

---

### Commit 3: aa07756 - "Addition UI fix - Language" ⭐ **MAIN CHANGES**
**Changes**: Friend system UI improvements and language support

---

## 🔧 DETAILED SCRIPT CHANGES

### 1. **FriendPopup.cs** - Friend System UI Enhancements
**File**: `Assets/02.Scripts/UI/Popup/FriendPopup.cs`
**Lines Changed**: 31 (+24 additions, -7 deletions)

#### **Change 1.1: NEW Property - Search Result Panel (Line 24)**
```diff
  public Button button_Search;
  public FriendItem searchFriendItem;
+ public GameObject goSearchFriendResult;

  public GameObject goListFriend;
```

**What Changed**: Added reference to search result container panel

**Purpose**: Separate control of search results vs recommended friends display

---

#### **Change 1.2: Input Field Clear Handler Enhancement (Lines 65-68)**
```diff
  {
      button_Search.interactable = !string.IsNullOrEmpty(txt);
      if (string.IsNullOrEmpty(txt))
-         searchFriendItem.transform.parent.gameObject.SetActive(false);
+     {
+         goSearchFriendResult.SetActive(false);
+         goRecommendedFriend.transform.parent.gameObject.SetActive(true);
+     }
  });
```

**What Changed**:
- Hide search results panel
- Re-show recommended friends panel

**Before**: Only hid search result
**After**: Hides search result AND restores recommended friends

**User Flow**:
```
1. User types UID → Search button enabled
2. User clears input field → Search results hidden + Recommended friends shown
```

---

#### **Change 1.3: Online Friend Search Result Display (Lines 117-118)**
```diff
  await NetworkManager.Instance.SearchFriends(input_UID.text, (ReqSearchFriendsData data) =>
  {
-     searchFriendItem.transform.parent.gameObject.SetActive(true);
+     goSearchFriendResult.SetActive(true);
+     goRecommendedFriend.transform.parent.gameObject.SetActive(false);
      searchFriendItem.gameObject.SetActive(true);
      searchFriendItem.SetFriendData(data);
  }, (string err) =>
```

**What Changed**: When online search succeeds
- Show search result panel
- Hide recommended friends panel

**Purpose**: Clear UX - don't show both search results and recommendations

---

#### **Change 1.4: Local Friend Search Enhancement (Lines 132-139)**
```diff
  if (friend != null)
  {
-     searchFriendItem.transform.parent.gameObject.SetActive(true);
+     goSearchFriendResult.SetActive(true);
+     goRecommendedFriend.transform.parent.gameObject.SetActive(false);
      searchFriendItem.gameObject.SetActive(true);
      searchFriendItem.SetFriendData(friend, true, isBattleInvite);
  }
+ else
+ {
+     SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
+     popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Invalid_UID"));
+ }
```

**What Changed**:
- Same panel management as online search
- **NEW**: Show error message if UID not found

**Before**: Silent failure when UID not found
**After**: Shows localized error: "No friends found"

**Error Message**: Uses new language key `UI_Invalid_UID` (see Language section below)

---

#### **Change 1.5: Send Energy Button Fix (Line 203)**
```diff
  var itm = listFriendItems.Find(x => x.friendData.userId == userId);
  if (itm != null)
-     itm.button_sendEnergy.SetInterectible(false);
+     itm.button_sendEnergy.gameObject.SetActive(false);
```

**What Changed**: Hide button instead of disabling

**Before**: Button grayed out (still visible, confusing)
**After**: Button completely hidden (clearer UX)

**Visual Impact**:
```
Before: [Send Energy] (grayed out, clickable but does nothing)
After:  (button completely gone)
```

---

#### **Change 1.6: Delete Friend Enhancement (Lines 217-227)**
```diff
  public void OnDeleteFriend(string userId)
  {
      var itm = listFriendItems.Find(x => x.friendData.userId == userId);
+     var dat = friendDatas.Find(x => x.userId == userId);
+
      if (itm != null)
      {
          listFriendItems.Remove(itm);
          Destroy(itm.gameObject);
      }
+     if (dat != null)
+         friendDatas.Remove(dat);
      text_FriendCount.text = $"{listFriendItems.Count}/30";
+     input_UID.text = "";
  }
```

**What Changed**:
1. Also remove from data list (not just UI list)
2. Clear search input field

**Before**: Friend removed from UI but data remained, causing bugs
**After**: Complete cleanup of both UI and data

**Bug Fix**: ⭐ **CRITICAL** - Prevented data inconsistency where deleted friends could reappear

**Flow**:
```
1. User deletes friend
2. Remove from UI list (listFriendItems)
3. Remove from data list (friendDatas) ← NEW
4. Update counter
5. Clear search box ← NEW (ready for new search)
```

---

## 🎨 PREFAB CHANGES

### 2. **Popup - Friend.prefab** - UI Layout Restructure
**File**: `Assets/03.Prefabs/InGamePrefabs/05.UIPrefabs/UI_Prefab(New)/UI - Popup/Popup - Friend.prefab`
**Lines Changed**: 370 (+283 additions, -87 deletions)

#### **Change 2.1: NEW GameObject - Panel - SearchResult**

**Added Components**:
```yaml
GameObject: "Panel - SearchResult"
  FileID: 4918266389970036244
  Layer: 5 (UI)
  Active: false (hidden by default)

  RectTransform:
    AnchorMin: [0, 0]
    AnchorMax: [1, 1]
    AnchoredPosition: [0, -150]
    SizeDelta: [0, -215]
    Pivot: [0.5, 1]

  Children:
    - Label - SearchResult (6801786791978987119)
    - Recommended Friend Item Container (6033632864339069488)
```

**Purpose**: Container for search results, separate from main friend list

---

#### **Change 2.2: NEW Label - SearchResult**

**Component Details**:
```yaml
GameObject: "Label - SearchResult"
  FileID: 6801786791978987119

  RectTransform:
    AnchorMin: [0, 1]
    AnchorMax: [0, 1]
    AnchoredPosition: [110, 0]
    SizeDelta: [320.49, 50]
    Pivot: [0, 1]

  TextMeshProUGUI:
    Text: "Search for friends"
    Font: UI Font Asset
    FontSize: 36
    Color: rgba(47, 108, 133, 255)  # #2F6C85 - Teal blue
    Alignment: Left + Middle
    HorizontalFit: PreferredSize (auto-width)

  LocalizeStringEvent:
    TableReference: UI_Text
    EntryReference: 385583545304920064
    Key: UI_Search_For_Friends
    UpdateString:
      Calls:
        - Target: TextMeshProUGUI
          Method: set_text
```

**Purpose**: Localized header for search results section

**Language Support**: Tied to `UI_Search_For_Friends` language key

---

#### **Change 2.3: Recommended Friend Item Container Repositioned**

**Changed**:
```diff
  GameObject: Recommended Friend Item
  FileID: 6033632864339069488

  RectTransform:
-   m_Father: 7358352492295015957  # Was child of main panel
+   m_Father: 8514885136223331019  # Now child of Panel - SearchResult

-   m_AnchoredPosition: [0, -67.5]
+   m_AnchoredPosition: [0, -25]

-   m_SizeDelta: [-130, -265]
+   m_SizeDelta: [-130, -50]

-   m_IsActive: 0
+   m_IsActive: 1
```

**What Changed**:
- Moved into new search result panel
- Repositioned within panel
- Now visible by default (parent panel controls visibility)

---

#### **Change 2.4: FriendPopup Component Updated**

**Script Reference Updated**:
```diff
  MonoBehaviour: FriendPopup (Framework.UI)
    input_UID: {fileID: 1577428569316977870}
    button_Search: {fileID: 4056180127389520631}
    searchFriendItem: {fileID: 2969760411179993206}
+   goSearchFriendResult: {fileID: 4918266389970036244}  # NEW
    goListFriend: {fileID: 3712779400562815865}
    scrollFriends: {fileID: 6544387988788267785}
```

**What Changed**: Connected new panel to script

---

#### **Change 2.5: Friend Count Label - Removed Localization**

**Removed Component**:
```diff
  GameObject: Text - FriendCount
  FileID: 3813183844486017313

- MonoBehaviour: LocalizeStringEvent
-   TableReference: UI_Text
-   EntryReference: 371867285492772864
-   UpdateString: ...
```

**What Changed**: Removed auto-localization from friend count

**Reason**: Friend count is dynamic (`10/30`), not a static translatable string

**Before**: Localization system tried to translate "10/30" (bug)
**After**: Script directly sets text (correct)

---

#### **Change 2.6: Content Fitter Removed**

**Removed Component**:
```diff
  GameObject: Recommended Friend Item

- MonoBehaviour: ContentSizeFitter
-   m_HorizontalFit: 0
-   m_VerticalFit: 0
```

**What Changed**: No longer auto-resize based on content

**Reason**: Fixed size layout with scroll rect

---

#### **Change 2.7: Pivot Changed for Scroll Content**

**Changed**:
```diff
  GameObject: Content (friend list container)
  FileID: 3641765798395253952

  RectTransform:
-   m_Pivot: [0.5, 0.5]
+   m_Pivot: [0.5, 1]  # Top-center pivot
```

**What Changed**: Scroll content now anchors to top

**Purpose**: Standard scroll view behavior (items appear from top down)

---

## 🌍 LANGUAGE CHANGES

### 3. **UI_Text Shared Data.asset** - New Language Keys
**File**: `Assets/13.Language/UI_Text Shared Data.asset`
**Lines Added**: 8 (+2 new entries)

#### **New Entry 1: UI_Invalid_UID**
```yaml
- m_Id: 385580652539588608
  m_Key: UI_Invalid_UID
  m_Metadata:
    m_Items: []
```

**Purpose**: Error message when searched UID not found

---

#### **New Entry 2: UI_Search_For_Friends**
```yaml
- m_Id: 385583545304920064
  m_Key: UI_Search_For_Friends
  m_Metadata:
    m_Items: []
```

**Purpose**: Label for search results section

---

### 4. **UI_Text.csv** - CSV Language Data
**File**: `Assets/13.Language/UI_Text.csv`
**Lines Added**: 4 (+2 new rows)

```diff
+ "UI_Invalid_UID",385580652539588608,"No friends found",
+ "UI_Search_For_Friends",385583545304920064,"Search for friends",
```

**Format**: Key, ID, English Text

---

### 5. **UI_Text_en-US.asset** - English Localization Asset
**File**: `Assets/13.Language/UI_Text_en-US.asset`
**Lines Added**: 8 (+2 new entries)

#### **Entry 1: UI_Invalid_UID**
```yaml
- m_Id: 385580652539588608
  m_Localized: No friends found
  m_Metadata:
    m_Items: []
```

---

#### **Entry 2: UI_Search_For_Friends**
```yaml
- m_Id: 385583545304920064
  m_Localized: Search for friends
  m_Metadata:
    m_Items: []
```

---

### 6. **UI_Text.csv** (Root) - Duplicate Entry
**File**: `Assets/UI_Text.csv`
**Lines Added**: Same 2 entries as above

**Note**: This appears to be a duplicate/backup file

---

## 📦 TEMPORARY FILES

### 7. **temp_changelog.txt**
**File**: `temp_changelog.txt`
**Lines**: 16,295 lines (massive diff dump)

**Content**: Complete git diff output from previous development cycle

**Purpose**: Temporary file for changelog generation (should be in .gitignore)

---

### 8. **temp_character_diff.txt**
**File**: `temp_character_diff.txt`
**Lines**: 128 lines

**Content**: Character-specific diff data

**Purpose**: Temporary working file

---

## 🔄 BUILD INFO

### 9. **StreamingAssets/build_info**
**File**: `Assets/StreamingAssets/build_info`

```diff
- Build Date: [Previous Date]
+ Build Date: [Updated Date]
```

**Purpose**: Auto-updated build timestamp

---

## 🎯 IMPACT SUMMARY

### Critical Bug Fixes ⭐
1. **Friend Data Inconsistency** - Fixed friend deletion not removing from data list
2. **Missing Error Feedback** - Added "No friends found" message for invalid UID search
3. **UI State Confusion** - Improved panel visibility management (search vs recommendations)

### User Experience Improvements
1. **Clearer UI States** - Search results and recommendations no longer overlap
2. **Better Feedback** - Energy button hidden instead of grayed out
3. **Localization Support** - Added language keys for search UI

### Technical Improvements
1. **Unity Version** - Updated to 2022.3.62f2 LTS
2. **Package Updates** - Animation, Test Framework, Collab packages updated
3. **Code Quality** - Removed unused localization from dynamic text

---

## 🧪 TESTING CHECKLIST

### Friend Search Flow
- [ ] Search for valid UID → Should show result panel + hide recommendations
- [ ] Search for invalid UID → Should show "No friends found" error
- [ ] Clear search input → Should hide result panel + show recommendations
- [ ] Delete friend → Should remove from both UI and data lists
- [ ] Delete friend → Should clear search input field

### Language Testing
- [ ] Verify "Search for friends" label appears correctly
- [ ] Verify "No friends found" error message appears correctly
- [ ] Test with different language settings (if available)

### UI State Management
- [ ] Only one panel visible at a time (search result OR recommendations)
- [ ] Send energy button properly hidden after use
- [ ] Friend count shows as "X/30" without localization issues

---

## 📊 FINAL STATISTICS

```
Functionality Changes:
  - Friend deletion: Now properly cleans up data
  - Search UX: Improved panel management
  - Error handling: Added missing feedback

Code Quality:
  - Lines added: +24 (C#) + +283 (Prefab YAML)
  - Lines removed: -7 (C#) + -87 (Prefab YAML)
  - Net change: +213 lines

Localization:
  - New keys: 2
  - Total language files updated: 4

Unity Environment:
  - Unity version: 2022.3.51f1 → 2022.3.62f2
  - Packages updated: 4
```

---

## 🔗 RELATED COMMITS

**Previous**: 381b3ce - "Save FieldBoss Feature"
- Field Boss system implementation
- Parasite, Boomber, Emberon bosses
- Infection mechanics

**Current**: aa07756 - "Addition UI fix - Language"
- Friend system polish
- Unity version update
- Language support

---

## 👨‍💻 DEVELOPER NOTES

### Code Review Notes
1. Consider moving temp files to .gitignore
2. Duplicate UI_Text.csv in root and Assets/13.Language - consolidate?
3. Friend data structure could use a refactor (two separate lists is error-prone)

### Future Improvements
1. Consider reactive pattern for friend list management
2. Add unit tests for friend search logic
3. Create language key constants to avoid magic strings

---

**Changelog Generated**: 2025-10-30
**Format Version**: 2.0 (Detailed)
**Generated By**: Claude Code

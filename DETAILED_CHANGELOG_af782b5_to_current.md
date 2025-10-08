# DefenGo - Detailed Change Documentation
## From Commit `af782b5` to `3814538` (HEAD)

**Analysis Date:** 2025-10-03
**Development Period:** ~6 weeks
**Developer:** Sotatek-ThangDo
**Version:** 3.00.3 → 3.00.4

---

## 📊 Statistical Overview

| Category | New Files | Modified Files | Total Changes |
|----------|-----------|----------------|---------------|
| **C# Scripts** | 30 | 32 | 62 |
| **Prefabs** | 36 | 24 | 60 |
| **Animations** | 35 | 1 | 36 |
| **Data Assets** | 10 | 2 | 12 |
| **Text/JSON** | 3 | 1 | 4 |

**Total:** 215,153 lines added, 8,200 lines deleted

---

# 📁 PREFABS - Detailed Breakdown

## ✨ NEW PREFABS (36 files)

### UI Buttons & Controls (3)
1. **Button - GiveUp.prefab**
   - Surrender/Give up button for battle mode
   - Appears at wave 5+ in PvP matches

2. **Button - Rank.prefab**
   - Quick access button to ranking screen

3. **Button - SummonCharacter.prefab**
   - New summon button component with enhanced functionality

### Boss & Monster (1)
4. **BoomberBoss.prefab** (420 lines)
   - New boss type with bomb mechanics
   - Associated script: `BoomberBossMonster.cs`

### Visual Effects (6)
5. **BossEf_Bomb1.prefab** (324 lines)
   - Small bomb visual effect

6. **BossEf_Bomb2.prefab** (324 lines)
   - Large bomb visual effect

7. **BossEf_BombExplosion1.prefab** (9,793 lines)
   - Small bomb explosion particle system

8. **BossEf_BombExplosion2.prefab** (24,418 lines)
   - Large bomb explosion particle system
   - Complex multi-layered particle effect

9. **BossEf_Soti.prefab** (19,560 lines)
   - Sotty boss special effect (soot/pollution visual)

10. **MonsterDisappear.prefab** (4,921 lines)
    - Monster death/disappear particle effect

### UI Popups (11)
11. **Popup - BattleResult.prefab** (8,645 lines)
    - End-of-battle results screen
    - Shows final ranking and statistics

12. **Popup - BattleResultPromote.prefab** (16,826 lines)
    - Rank promotion/demotion animation screen
    - Tier progression visuals

13. **Popup - BattleResultTable.prefab** (2,246 lines)
    - Detailed battle statistics table
    - Player-by-player comparison

14. **Popup - BossRewardSelect.prefab** (3,438 lines)
    - Boss reward selection interface
    - Field boss loot distribution

15. **Popup - BossSelect.prefab** (1,854 lines)
    - Boss selection for battle waves
    - Shows available boss types

16. **Popup - Friend.prefab** (3,518 lines)
    - Friends list management
    - Add/remove friends, send energy

17. **Popup - IngameRank.prefab** (1,352 lines)
    - Real-time ranking display during battle
    - Live player standings

18. **Popup - Matching.prefab** (2,191 lines)
    - Matchmaking queue interface
    - Shows other players joining

19. **Popup - ModeBattle.prefab** (6,845 lines)
    - Battle mode selection screen
    - Choose between ranked/friendly matches

20. **Popup - ModeSingle.prefab** (3,073 lines)
    - Single player mode selection screen

21. **Boss.prefab** (287 lines)
    - Boss UI display component

### UI Items/Components (8)
22. **Item - Friend.prefab** (1,885 lines)
    - Individual friend list item

23. **PlayButton - Battle.prefab** (482 lines)
    - Main battle mode entry button

24. **BattleResultTableItem.prefab**
    - Row item for battle results table

25. **BossRewardItem.prefab**
    - Individual reward selection item

26. **Cell_matching.prefab**
    - Matchmaking cell display

27. **IngameRank.prefab**
    - In-game rank display element

28. **NetBossSummon.prefab**
    - Network-synchronized boss summon UI

29. **PvpJewel.prefab**
    - PvP rank jewel/badge display

30. **PvpResultItem.prefab**
    - Individual PvP result item

31. **PvpScore.prefab**
    - Score display for PvP

32. **StatusMessage.prefab**
    - In-game status message banner

### Network Prefabs (2)
33. **NetworkConnection.prefab**
    - Photon Fusion network runner object

34. **NetworkGameManager.prefab**
    - Network game state manager

### Photon Fusion Statistics (6)
35. **FusionStatsSimpleButton.prefab**
36. **NOStatGraph.prefab**
37. **SingleStatistics.prefab**
38. **StatisticsRenderGraph.prefab**
39. **FusionStatsRenderPanel.prefab**
40. **NetworkObjectStatistics.prefab**

---

## 🔧 MODIFIED PREFABS (24 files)

### Characters (4)
1. **Beosk.prefab**
   - Minor adjustments (59 lines changed)

2. **Fortis.prefab**
   - Significant changes (+321 lines)
   - Enhanced character properties

3. **Highnickel.prefab**
   - Moderate changes (+145 lines)
   - Updated stats/abilities

4. **Turtlia.prefab**
   - Updated (+103 lines)
   - Character property adjustments

### Monsters (1)
5. **CarBoss.prefab**
   - Minor updates (+14 lines)
   - Clone monster support added

### UI Popups (9)
6. **Popup - BattlePass.prefab**
   - Layout adjustments (+216 lines)

7. **Popup - BattlePassCrystalBuy.prefab**
   - UI improvements (+108 lines)

8. **Popup - Crystal.prefab**
   - Minor adjustments (+94 lines)

9. **Popup - Energy.prefab**
   - Updates (+83 lines)

10. **Popup - Guide.prefab**
    - Content removed (-32 lines)

11. **Popup - Inbox.prefab**
    - Enhanced inbox features (+124 lines)

12. **Popup - LeaderBoardReward.prefab**
    - Battle leaderboard integration (+106 lines)

13. **Popup - Setting.prefab**
    - Network settings added (+1,240 lines)

14. **Popup - UserProfileDetail.prefab**
    - Friend profile integration (+817 lines)

### UI Screens (2)
15. **Screen - Home.prefab**
    - Battle mode integration
    - Mode selection buttons

16. **Screen - Ranking.prefab**
    - Battle leaderboard tab
    - Enhanced ranking display

### UI Components (3)
17. **RankInfoItem.prefab**
    - Updated rank display (+126 lines changed)

18. **StaticMenu.prefab**
    - Menu additions for battle mode (+880 lines changed)

19. **Cell_Articles_Inbox.prefab**
    - Friend request support

### Projectiles (4)
20. **IceProjectile.prefab**
21. **SnowProjectile.prefab**
22. **TornadoProjectile.prefab**
23. **WaterSlowProjectile.prefab**
    - All updated for network synchronization

### Other (1)
24. **MainGameCanvas.prefab**
    - Battle UI elements added

---

# 💻 C# SCRIPTS - Detailed Breakdown

## ✨ NEW SCRIPTS (30 files)

### Data Classes (1)
#### 1. **BossData.cs** (17 lines)
```csharp
// New data structure for boss configurations
public class BossData
{
    public int bossIndex;
    public FieldBossMonster bossType;
    public float[] uniqueValue;
    public int bossHealth;
    public float bossSpeed;
    // Boss-specific configuration data
}

public enum FieldBossMonster
{
    TRUSH,
    SMOKER,
    SOTTY,
    LOCKY,
    PARASITE,
    BOOMBER,
    EMBEREON
}
```

### Monster Classes (4)
#### 2. **BoomberBossMonster.cs** (141 lines)
```csharp
public class BoomberBossMonster : Monster
{
    // New Methods:
    public override void Abillity()
    public void SetBomb(Character target)
    public void BombExplosion()
    public override void EndOfUse()
    public override void DeathSequence()
}
```
**Features:**
- Bomb placement on characters
- Area-of-effect damage
- Two bomb variants (based on character star grade)
- Explosion particle effects

#### 3. **SottyBossMonster.cs** (119 lines)
```csharp
public class SottyBossMonster : Monster
{
    // New Methods:
    public override void Abillity()
    public override void EndOfUse()
    public override void DeathSequence()

    // Special mechanics for pollution/soot
}
```

#### 4. **LockyBossMonster.cs** (152 lines)
```csharp
public class LockyBossMonster : Monster
{
    public List<Character> lockedCharacters;

    // New Methods:
    public override void Abillity()
    public void LockdownCharacters()
    public void ReleaseLockdown()
    public override void EndOfUse()
    public override void DeathSequence()
}
```
**Features:**
- Character lockdown mechanic
- Disables character attacks temporarily

#### 5. **ParasiteBossMonster.cs** (127 lines)
```csharp
public class ParasiteBossMonster : Monster
{
    public List<Character> infectedCharacters;

    // New Methods:
    public override void Abillity()
    public void InfectCharacters()
    public void ReleaseInfection()
    public override void EndOfUse()
    public override void DeathSequence()
}
```
**Features:**
- Infection/parasite mechanic
- Character debuffs

### Networking Classes (3)
#### 6. **NetworkConnect.cs** (845 lines) ⭐ MAJOR NEW FILE
```csharp
public class NetworkConnect : SimulationBehaviour, INetworkRunnerCallbacks
{
    // Network State
    public bool isHost;
    public int hostIdx;
    public int metaScore;
    public NetworkRunner runner;
    public Dictionary<int, NetworkBattleData> dic_PlayerData;
    public NetworkBattleStatus networkBattleStatus;
    public int playerIdx;
    public string nickname;
    public int minPlayerCount = 2;
    public int maxPlayerCount = 3;
    public bool isFriendlyMatch;
    public string roomName;
    public string roomPassword;
    public int playId;
    public string roomUuid;
    public int sessionId;
    public string userId;
    public NetworkGameManager networkGameManager;
    public MyBattleLeaderboardInfo myCurrentRank;

    // Key Methods:
    public bool IsCurrentHost()
    public async void ConnectToLobby(bool isFriendlyMatch, string roomName, string roomPassword)
    public async void JoinSession(string sessionName)
    public async void JoinFriendlySession(string sessionName)
    public async void CreateSession()
    public async void CreateFriendlySession()
    public void InitializeCheck(int playerIdx, string json)
    public void FieldBossRewardCheck(int playerIdx)
    public List<NetworkBattleData> GetSortedDictPlayerData()

    // INetworkRunnerCallbacks implementation:
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    // ... + 20 more callback methods
}
```
**Purpose:** Complete multiplayer infrastructure
- Matchmaking system
- Session management (create/join rooms)
- Host migration support
- Player data synchronization
- Lobby management

#### 7. **NetworkGameManager.cs** (240 lines)
```csharp
public class NetworkGameManager : NetworkBehaviour, IAfterSpawned
{
    public int rewardGroupIndex;
    public int gameOverPlayerCount;
    public int rank;

    // RPC Methods (Client → Host):
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestInitializeComplete(int playerIdx, string json)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestCountStart()

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestSpawnBoss(string nickname, float bossHealth)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestWaveComplete(int playerId, int roundId, int monsterKilled, int monsterBossKilled)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestGameOver(int playerId, int roundId, bool isAbnormal)

    // Host → All Broadcasts:
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcInitializeComplete(int playerIdx, string json)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcCountStart()

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSummaryBattle()

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_SpawnBoss(string nickname, float bossHealth)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_WaveComplete(int playerId, int roundId, int monsterKilled, int monsterBossKilled)

    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void Rpc_GameOver(int playerId, int roundId, bool isAbnormal)
}
```
**Purpose:** Real-time game state synchronization via Photon Fusion RPCs

#### 8. **NetworkUserInfo.cs** (84 lines) (decrapted)
```csharp
// public class NetworkUserInfo
// {
//     public string userId;
//     public string nickname;
//     public int playerIdx;
//     public int playId;
//     public string sessionId;
//     public int waveCount;
//     public int monsterKilled;
//     public int monsterBossKilled;
//     public bool isGameOver;
//     public bool isAbnormalExit;
//     public int rank;
// }
```

### UI Popup Classes (11)
#### 9. **BattleResultPopup.cs** (90 lines)
```csharp
public class BattleResultPopup : MonoBehaviour
{
    public TextMeshProUGUI text_rank;
    public TextMeshProUGUI text_wave;
    public TextMeshProUGUI text_kill;

    public void SetResultInfo(int rank, int wave, int kill)
    public void OnClick_Home()
}
```

#### 10. **BattleResultPromotePopup.cs** (321 lines)
```csharp
public class BattleResultPromotePopup : MonoBehaviour
{
    public Animator animator;
    public Image image_CurrentTier;
    public Image image_NextTier;
    public TextMeshProUGUI text_CurrentTierName;
    public TextMeshProUGUI text_NextTierName;
    public TextMeshProUGUI text_Score;

    public void Initialize(MyBattleLeaderboardInfo beforeRank, MyBattleLeaderboardInfo afterRank)
    public IEnumerator ShowPromoteAnimation()
    public IEnumerator ShowDemoteAnimation()
    public IEnumerator ShowSameRankAnimation()
}
```

#### 11. **BattleResultTablePopup.cs** (76 lines)
```csharp
public class BattleResultTablePopup : MonoBehaviour
{
    public List<BattleResultTableItem> battleResultItems;

    public void SetTableData(List<NetworkBattleData> playerDataList)
}
```

#### 12. **BossSelectPopup.cs** (138 lines)
```csharp
public class BossSelectPopup : MonoBehaviour
{
    public List<BossSelectItem> bossSelectItems;
    public BossData[] TempbossList;

    public void Initialize()
    public void OnClick_SelectBoss(int bossIdx)
}
```

#### 13. **FieldBossRewardPopup.cs** (174 lines)
```csharp
public class FieldBossRewardPopup : MonoBehaviour
{
    public List<FieldBossRewardItem> rewardItems;
    public int selectedRewardIndex;

    public void Initialize(int rewardGroupIndex)
    public void OnClick_SelectReward(int index)
    public void OnClick_Confirm()
}
```

#### 14. **FriendPopup.cs** (213 lines)
```csharp
public class FriendPopup : MonoBehaviour
{
    public List<FriendItem> friendItems;
    public GameObject recommendedFriendsPanel;
    public GameObject myFriendsPanel;

    public async void Initialize()
    public async void LoadRecommendedFriends()
    public async void LoadMyFriends()
    public void OnClick_SearchFriend()
    public void OnClick_SendFriendRequest(string friendId)
    public void OnClick_DeleteFriend(string friendId)
}
```

#### 15. **InGameRankPopup.cs** (59 lines)
```csharp
public class InGameRankPopup : MonoBehaviour
{
    public List<InGameRankItem> rankItems;

    public void Initialize()
    public void SortPlayerData()
    public void UpdateRankDisplay()
}
```

#### 16. **MatchMakingPopup.cs** (144 lines)
```csharp
public class MatchMakingPopup : MonoBehaviour
{
    public List<MatchingUserInfoItem> userInfoItems;
    public TextMeshProUGUI text_MatchingStatus;
    public GameObject loadingIndicator;

    public void Initialize()
    public void StartMatchmaking()
    public void OnSessionListUpdated(List<SessionInfo> sessions)
    public void OnClick_Cancel()
    public void Shutdown()
}
```

#### 17. **StartUpBattleModePopup.cs** (144 lines)
```csharp
public class StartUpBattleModePopup : MonoBehaviour
{
    public ButtonComponent button_RankedMatch;
    public ButtonComponent button_FriendlyMatch;

    public void OnClick_RankedMatch()
    public void OnClick_FriendlyMatch()
    public void OnClick_CreateRoom()
    public void OnClick_JoinRoom()
}
```

#### 18. **StartUpSingleModePopup.cs** (62 lines)
```csharp
public class StartUpSingleModePopup : MonoBehaviour
{
    public ButtonComponent button_Start;

    public void OnClick_Start()
}
```

#### 19. **UserProfileDetailPopup.cs** - Enhanced (+70 lines)
```csharp
// Added:
public void ShowFriendProfile(string friendId)
public void OnClick_SendFriendRequest()
public void OnClick_DeleteFriend()
public void OnClick_InviteToBattle()
```

### UI Item Classes (11)
#### 20. **BattleResultTableItem.cs** (107 lines)
```csharp
public class BattleResultTableItem : MonoBehaviour
{
    public TextMeshProUGUI text_Rank;
    public TextMeshProUGUI text_Nickname;
    public TextMeshProUGUI text_Wave;
    public TextMeshProUGUI text_Kills;
    public Image image_Background;

    public void SetData(NetworkBattleData data, bool isMe)
}
```

#### 21. **BossSelectItem.cs** (34 lines)
```csharp
public class BossSelectItem : MonoBehaviour
{
    public Image image_BossIcon;
    public ButtonComponent button_Select;
    public int bossIndex;

    public void Initialize(BossData bossData)
    public void OnClick_Select()
}
```

#### 22. **BossWaveItem.cs** (38 lines)
```csharp
public class BossWaveItem : MonoBehaviour
{
    public Slider slider_BossHealth;
    public TextMeshProUGUI text_BossName;

    public void Initialize(float maxHealth, int bossIdx)
    public void UpdateHealth(float currentHealth)
}
```

#### 23. **FieldBossRewardItem.cs** (260 lines)
```csharp
public class FieldBossRewardItem : MonoBehaviour
{
    public Image image_RewardIcon;
    public TextMeshProUGUI text_RewardAmount;
    public GameObject selectedFrame;
    public int rewardIndex;

    public void Initialize(RewardData data, int index)
    public void OnClick_Select()
    public void SetSelected(bool selected)
}
```

#### 24. **FriendItem.cs** (120 lines)
```csharp
public class FriendItem : MonoBehaviour
{
    public TextMeshProUGUI text_Nickname;
    public TextMeshProUGUI text_Level;
    public Image image_ProfileIcon;
    public ButtonComponent button_SendEnergy;
    public ButtonComponent button_Delete;
    public ButtonComponent button_Profile;

    public void Initialize(FriendData data)
    public void OnClick_SendEnergy()
    public void OnClick_Delete()
    public void OnClick_Profile()
    public void OnClick_InviteToBattle()
}
```

#### 25. **InGameRankItem.cs** (121 lines)
```csharp
public class InGameRankItem : MonoBehaviour
{
    public TextMeshProUGUI text_Rank;
    public TextMeshProUGUI text_Nickname;
    public TextMeshProUGUI text_Wave;
    public Image image_Background;

    public void SetData(NetworkBattleData data, int rank, bool isMe)
}
```

#### 26. **InfiniteHorizontalScroll.cs** (452 lines)
```csharp
public class InfiniteHorizontalScroll : MonoBehaviour
{
    // Optimized scrolling for large lists
    public RectTransform content;
    public GameObject itemPrefab;
    public float itemWidth;

    public void Initialize(int totalItems)
    public void UpdateVisibleItems()
    public void ScrollTo(int index)
}
```

#### 27. **IngameStatusMessage.cs** (41 lines)
```csharp
public class IngameStatusMessage : MonoBehaviour
{
    public TextMeshProUGUI text_Message;
    public TextMeshProUGUI text_PlayerName;

    public void SetMessage(string message, string playerName)
    public IEnumerator HideAfterDelay()
}
```

#### 28. **MatchingUserInfoItem.cs** (78 lines)
```csharp
public class MatchingUserInfoItem : MonoBehaviour
{
    public TextMeshProUGUI text_Nickname;
    public TextMeshProUGUI text_Rank;
    public Image image_TierIcon;

    public void SetData(NetworkBattleData data)
}
```

#### 29. **PvpBonusScoreItem.cs** (10 lines)
```csharp
public class PvpBonusScoreItem : MonoBehaviour
{
    public TextMeshProUGUI text_Bonus;

    public void SetBonus(int bonus)
}
```

#### 30. **SummonButton.cs** (145 lines)
```csharp
public class SummonButton : MonoBehaviour
{
    public ButtonComponent button;
    public Image image_Icon;
    public TextMeshProUGUI text_Cost;
    public ParticleSystem particleEffect;

    public void Initialize()
    public void UpdateCost(int cost)
    public void SetInteractable(bool interactable)
    public void PlayParticle()
}
```

### UI Tools (1)
#### 31. **CustomListButtons.cs** (95 lines)
```csharp
public class CustomListButtons : MonoBehaviour
{
    public List<ButtonComponent> buttons;
    public int selectedIndex;

    public void Initialize()
    public void SelectButton(int index)
    public void OnButtonClicked(int index)
}
```

---

## 🔧 MODIFIED SCRIPTS (32 files)

### Data Classes (2)

#### 1. **GameData.cs** (+69 lines)
**New Enums:**
```csharp
public enum NetworkBattleStatus
{
    LOBBY,
    WAITING,
    PLAYING,
    ENDED
}

public enum FriendRequestStatus
{
    PENDING,
    ACCEPTED,
    REJECTED
}
```

**New Classes:**
```csharp
public class ReadyBattlePayload
{
    public string roomName;
    public string userId;
}

public class StartBattlePayload
{
    public int sessionId;
    public string userId;
    public int slotNumber;
}

public class EndBattlePayload
{
    public int sessionId;
    public int playId;
    public string userId;
    public int lastWave;
    public int requestGo;
    public int slotNumber;
}

public class SurrenderBattlePayload
{
    public string sessionId;
    public int leavePlayId;
    public string leaveUserId;
}

public class BattleRecord
{
    public int waveNumber;
    public int requestGo;
    public int killedMonster;
    public string completedMissions;
    public int bossLevel;
    public int playId;
    public string characterInfo;
    public MonsterInfo[] monsterInfo;
    public string sessionId;
}

public class NetworkBattleData
{
    public string userId;
    public string nickname;
    public int playerIdx;
    public int playId;
    public string sessionId;
    public int waveCount;
    public int monsterKilled;
    public int monsterBossKilled;
    public bool isGameOver;
    public bool isAbnormalExit;
    public int rank;
}

public class RankTierConfig
{
    public int id;
    public string tierName;
    public int minScore;
    public int maxScore;
    public string iconUrl;
}

public class MyBattleLeaderboardInfo
{
    public string userId;
    public int score;
    public int rank;
    public string tierName;
    public int tierLevel;
}

public class FriendData
{
    public string userId;
    public string nickname;
    public int level;
    public string profileIconUrl;
    public bool canSendEnergy;
}

public class FriendlyBattleInviteInfo
{
    public string roomName;
    public string roomPassword;
    public string inviterId;
    public string inviterNickname;
}

public class BattleConfig
{
    public int maxPlayers;
    public int minPlayers;
    public float bossCooldown;
}
```

#### 2. **UIPropertyData.cs** (+19 lines)
**New Fields:**
```csharp
public Sprite sprite_RankButton;
public Sprite sprite_GiveUpButton;
public Sprite sprite_BattleModeButton;
public Color color_MyRank;
public Color color_OtherRank;
```

### Core Game Systems (6)

#### 3. **GameManager.cs** (+518 lines) ⭐ MAJOR CHANGES
**New Enum:**
```csharp
public enum GameMode
{
    SINGLE,
    BATTLE,
    TUTORIAL
}
```

**New Variables:**
```csharp
public GameMode gameMode;
public ObscuredInt summonCostBuffValue = 0;
public ObscuredFloat relocationCostDiscount = 1;
public ObscuredFloat missionRewardBuffValue = 1;
public ObscuredFloat bossRewardBuffValue = 1;
public ObscuredFloat monsterRewardBuffValue = 0;
public ObscuredFloat upgradeCostBuffValue = 0;
public ObscuredFloat attackAllBuffValue = 1;
public ObscuredFloat attackRangeLessBuffValue = 1;
public ObscuredFloat attackRangeLessBuffTrigger = 0;
public ObscuredFloat attackRangeMoreBuffValue = 1;
public ObscuredFloat attackRangeMoreBuffTrigger = 0;
public ObscuredFloat attackSpeedBuffValue = 1;
public ObscuredFloat attackRangeBuffValue = 0;
public ObscuredInt bossKillCount;
public UnityAction BossDataAction;
public float tempBossAddHealth;
public IEnumerator countNextWave;
```

**New Methods:**
```csharp
public void BattleInitialize()
public void TutorialInitialize() // Made public
public void SummonFixedCharacter(CharacterIndex characterIndex, int starGrade)
public void MissionGemReward()
public void DecreaseRelocationCost(float value)
public void BuffSummonCost(int buff)
public void BuffUpgradeCost(float buff)
public static Transform GetGameWorldTransform()
public void GameOverHome()
public void SingleGameOver()
public void BattleGameOver()
public void ReachBossWave()
public float SetAddBossHealth(int roundId)
public void BossWaveSeqeunce(int roundId, string nickname, int bossIdx)
public void SpawnPlasticMonster(int remainingPlayer, float bossHealth)
public void FieldBossWaveStart()
public void FieldBossDamage()
public void TileCheck()
public void BossWaveCountStart()
public IEnumerator WaitForNextWave(UnityAction action) // Now with parameter
```

**Modified Methods:**
```csharp
public IEnumerator GameStartSequence() // Switch-case for game modes
public void Initialize() // Now public, mode-specific
public void IncreaseRelocationCost() // Enhanced
public void WaveEnd() // Boss wave logic for battle mode
public void SetPlayRecordData() // Battle record support
```

#### 4. **Character.cs** (+79 lines)
**New Properties:**
```csharp
public bool IsLockdown { get; set; }
public bool IsInfected { get; set; }
```

**New Methods:**
```csharp
public virtual void Lockdown()
public virtual void ReleaseLockdown()
public void Infect()
public void ReleaseInfect()
public void SetBomb()
public void BombExplosion()
```

**Purpose:** Support for new boss abilities (lockdown, infection, bombs)

#### 5. **Monster.cs** (+191 lines)
**New Properties:**
```csharp
public MonsterType monsterType { get; set; } // Enhanced with property
public bool IsBossAttack { get; set; }
```

**New Methods:**
```csharp
public virtual void FieldBossInitialize(BossData bossData)
public void SetMonsterMove(SkeletonDataAsset animData, int currentArrayIdx, MonsterType Type)
public void SetMonsterMove(SkeletonDataAsset animData, MonsterType Type)
public void SetBossMove() // New overload
public IEnumerator EnterBossSequenceAsync()
public void EnterBossWave()
```

**Modified Methods:**
```csharp
public void HitDamage() // Boss attack state handling
public void DieMethod() // Monster type-specific death handling
```

#### 6. **MonsterSpawner.cs** (+199 lines)
**New Variables:**
```csharp
public IEnumerator spawnMonster;
public BossMonsterData bossData;
public readonly int[] useMonsterIdList = { 1, 2, 3, 4, 51 };
public Dictionary<int, SkeletonDataAsset> dic_monsterAnimAsset;
```

**New Methods:**
```csharp
public void StartBossWave()
public void SetFieldBossWaveStart(BossData bossData)
public void SetBossMonsterStart()
public void SetCloneMonster(float health, float speed, int cloneId, int arrayIdx)
public void SpawnPlasticMonster(int remainingPlayer, float casterBossHealth)
public IEnumerator SpawnPlasticMonsterSequence(int remainingPlayer, float casterBossHealth)
public WaveData GetCurrentWaveData()
public WaveData GetCurrentWaveData(int waveIdx)
public void ClearMonster()
```

**Purpose:** Boss spawning for multiplayer, clone monsters for other players' views

#### 7. **UIManager.cs** (+202 lines)
**New Variables:**
```csharp
public SummonButton summonButton;
public ButtonComponent button_rank;
public ButtonComponent button_giveUp;
public TextMeshProUGUI text_bossAmount;
public GameObject[] icon_BossActive;
public InGameRankPopup inGameRankPopup;
public BossSelectPopup bossSelectPopup;
public FieldBossRewardPopup fieldBossRewardPopup;
public BattleResultPopup battleResultPopup;
public BattleResultTablePopup battleResultTablePopup;
public BattleResultPromotePopup battleResultPromotePopup;
public Image[] image_BossCoolTime;
public int bossAmount;
public IngameStatusMessage ingameStatusMessage;
public BossWaveItem BossWaveItem;
public ObscuredFloat bossCoolTimeBuffValue = 0;
public IEnumerator BossCoolTimeCo;
```

**New Properties:**
```csharp
public int BOSS_COOLTIME => (int)(ConfigData.BOSS_COOLTIME - ConfigData.BOSS_COOLTIME * bossCoolTimeBuffValue);
```

**New Methods:**
```csharp
public void Initialize(bool isBattleMode) // Modified signature
public void OnClick_RankButton()
public void OnClick_GiveUpButton()
public void TutorialInitialize() // New
```

**Modified Methods:**
```csharp
public void GameOver(int bounsWave) // Battle mode support
public void WaveEndSequence() // Boss button visibility
```

#### 8. **NetworkManager.cs** (+659 lines) ⭐ MAJOR CHANGES
**New API Endpoints:**
```csharp
// Friend System
public static readonly string getRecommendedFriends;
public static readonly string postFriendRequest;
public static readonly string getListFriend;
public static readonly string postSendEnergy;
public static readonly string delDeleteFriend;
public static readonly string getSearchFriend;
public static readonly string getFriendProfile;
public static readonly string putHandleFriendRequest;

// Battle System
public static readonly string postReadyForBattle;
public static readonly string postStartBattle;
public static readonly string postCreateBattleRecord;
public static readonly string postEndBattle;
public static readonly string postSurrenderBattle;
public static readonly string getRankTierConfig;
public static readonly string getMyBattleLeaderboard;
public static readonly string getBattleLeaderboard;
public static readonly string getSummaryBattle;
public static readonly string putHandleFriendlyBattleInvite;
public static readonly string postInviteFriendToBattle;
public static readonly string getBattleConfig;
```

**New Variables:**
```csharp
public Stack<BattleRecord> disposedBattleRecords = new();
```

**New Methods (Friend System):**
```csharp
public async UniTask GetRecommendedFriends(UnityAction<ReqRecommendedFriendsData> onSuccess, UnityAction<string> onFailed)
public async UniTask GetListFriends(UnityAction<ReqListFriendsData> onSuccess, UnityAction<string> onFailed)
public async UniTask SendFriendRequest(string friendId, UnityAction OnSuccess, UnityAction<string> OnFailed)
public async UniTask SearchFriends(string friendId, UnityAction<ReqSearchFriendsData> onSuccess, UnityAction<string> onFailed)
public async UniTask GetFriendProfile(string friendId, UnityAction<TotalProfileData> onSuccess, UnityAction<string> onFailed)
public async UniTask SendEnergy(string friendId, UnityAction onSuccess, UnityAction<string> onFailed)
public async UniTask HandleFriendRequest(UserInbox userInbox, FriendRequestStatus friendRequestStatus, UnityAction onSuccess, UnityAction<string> onFailed)
public async UniTask DeleteFriend(string friendId, UnityAction onSuccess, UnityAction<string> onFailed)
```

**New Methods (Battle System):**
```csharp
public async UniTask ReadyForBattle(ReadyBattlePayload payload, UnityAction<ReadyBattleResponse> onSuccess, UnityAction<string> onFailed)
public async UniTask StartBattle(StartBattlePayload payload, UnityAction<int> onSuccess, UnityAction<string> onFailed)
public async void SendBattleRecord(BattleRecord data)
public async UniTask CreateBattleRecord(BattleRecord data)
public async UniTask EndBattle(EndBattlePayload payload, UnityAction onSuccess, UnityAction onFail)
public async UniTask SurrenderBattle(SurrenderBattlePayload payload, UnityAction onSuccess, UnityAction onFail)
public async UniTask GetRankTierConfig(UnityAction<List<RankTierConfig>> onSuccess)
public async UniTask GetMyBattleLeaderboard(UnityAction<MyBattleLeaderboardInfo> onSuccess, UnityAction<MyBattleLeaderboardInfo> onFail)
public async UniTask GetBattleLeaderboard(UnityAction<GetBattleLeaderboardResponse> onSuccess, UnityAction onFail)
public async UniTask GetBattleSummary(int playId, UnityAction<GetBattleSummaryResponse> onSuccess, UnityAction onFail)
public async UniTask InviteFriendToBattle(string friendId, string roomName, string roomPassword, UnityAction OnSuccess)
public async UniTask HandleFriendlyBattleInvite(UserInbox userInbox, FriendRequestStatus status, UnityAction<FriendlyBattleInviteInfo> onSuccess, UnityAction<string> onFailed)
public async UniTask GetBattleConfig(UnityAction<BattleConfig> onSuccess)
```

### Other Manager Scripts (8)

#### 9. **CharacterSpawner.cs** (+68 lines)
**New Methods:**
```csharp
public void SummonFixedCharacter(CharacterIndex characterIndex, int starGrade)
public void SetTotalSpawnData()
public void SpawnDataClear()
```

#### 10. **DataManager.cs** (+32 lines)
**New Variables:**
```csharp
public Dictionary<int, BossData> dic_BossData;
```
**New Methods:**
```csharp
public async void LoadBossData()
```

#### 11. **GridManager.cs** (+18 lines)
**New Methods:**
```csharp
public void FieldBossDamage()
```

#### 12. **Glacier.cs** (+14 lines)
**New Methods:**
```csharp
public void FieldBossDamageSequence()
```

#### 13. **ObjectParticle.cs** (+8 lines)
**New Variables:**
```csharp
public bool isBossGo;
```
**Modified Methods:**
```csharp
public void SimplePlay(Transform target) // Boss GO particle support
```

#### 14. **LobbyManager.cs** (+34 lines)
**Modified Methods:**
```csharp
public void OnClick_Play() // Battle mode integration
```

#### 15. **DecButton.cs** (+41 lines)
**New Methods:**
```csharp
public void SetBattleMode(bool isBattle)
```

#### 16. **UserInfoManager.cs** (+1 line)
**New Variables:**
```csharp
public List<RankTierConfig> rankTierConfigs;
```

### UI Screens (2)

#### 17. **HomeScreen.cs** (+92 lines)
**New Methods:**
```csharp
public void ShowModeSelection()
public void OnClick_SingleMode()
public void OnClick_BattleMode()
public IEnumerator ShowTransitionOnly(bool fadeIn)
```

#### 18. **RankingScreen.cs** (+363 lines)
**New Variables:**
```csharp
public GameObject panel_WeeklyRanking;
public GameObject panel_BattleRanking;
public List<BattleRankItem> battleRankItems;
```

**New Methods:**
```csharp
public async void LoadBattleLeaderboard()
public void ShowBattleRankingTab()
public void ShowWeeklyRankingTab()
```

### UI Popup Scripts (5)

#### 19. **PopupTemplate.cs** (+2 lines)
**New Methods:**
```csharp
public virtual void OnShow()
public virtual void OnHide()
```

#### 20. **RewardGuidePopup.cs** (+16 lines)
**Modified:**
- Battle mode reward information added

#### 21. **SettingPopup.cs** (+18 lines)
**New Variables:**
```csharp
public Toggle toggle_ShowNetworkStats;
```
**New Methods:**
```csharp
public void OnToggle_NetworkStats(bool value)
```

#### 22. **SystemNoticePopup.cs** (+8 lines)
**Modified:**
- Enhanced error message handling

#### 23. **UserProfileDetailPopup.cs** (+70 lines)
**New Variables:**
```csharp
public ButtonComponent button_SendFriendRequest;
public ButtonComponent button_DeleteFriend;
public ButtonComponent button_InviteToBattle;
```

**New Methods:**
```csharp
public void ShowFriendProfile(string friendId)
public void OnClick_SendFriendRequest()
public void OnClick_DeleteFriend()
public void OnClick_InviteToBattle()
```

### UI Item Scripts (5)

#### 24. **InboxItem.cs** (+206 lines)
**New Methods:**
```csharp
public void HandleFriendRequest(bool accept)
public void HandleBattleInvite(bool accept)
```

#### 25. **MyRank.cs** (+51 lines)
**New Variables:**
```csharp
public Image image_TierIcon;
public TextMeshProUGUI text_TierName;
public TextMeshProUGUI text_Score;
```

**New Methods:**
```csharp
public void SetBattleRank(MyBattleLeaderboardInfo rankInfo)
```

#### 26. **RankInfoItem.cs** (+25 lines)
**New Methods:**
```csharp
public void SetBattleRankData(BattleRankData data)
```

#### 27. **RankInfoWeeklyItem.cs** (+1 line)
**Minor update**

#### 28. **LobbyTutorialManager.cs** (+3 lines)
**Minor update**

### Utility Scripts (1)

#### 29. **Calculator.cs** (+62 lines)
**New Methods:**
```csharp
public static int GetIndependentTrial(float[] probabilities)
{
    // Probability-based random selection for boss rewards
    float random = Random.Range(0f, 1f);
    float cumulative = 0f;

    for (int i = 0; i < probabilities.Length; i++)
    {
        cumulative += probabilities[i];
        if (random <= cumulative)
            return i;
    }

    return probabilities.Length - 1;
}
```

### Networking Data (1)

#### 30. **NetworkData.cs** (+264 lines)
**New Classes:**
```csharp
public class ReqRecommendedFriendsData
{
    public List<FriendData> friends;
}

public class ReqListFriendsData
{
    public List<FriendData> friends;
}

public class ReqSearchFriendsData
{
    public FriendData friend;
}

public class ReadyBattleResponse
{
    public int sessionId;
    public string roomName;
}

public class GetBattleLeaderboardResponse
{
    public List<BattleRankData> rankings;
}

public class GetBattleSummaryResponse
{
    public int rank;
    public int wave;
    public int kills;
    public int score;
}

public class BattleRankData
{
    public int rank;
    public string userId;
    public string nickname;
    public int score;
    public string tierName;
    public int tierLevel;
}
```

### Boss Monster Scripts (2)

#### 31. **CarBossMonster.cs** (+10 lines)
**New Variables:**
```csharp
public float monCloneHealth;
public int monCloneId;
```

**Modified Methods:**
```csharp
public override void Abillity() // Clone monster spawning
```

#### 32. **NormalBossMonster.cs** (+11 lines)
**New Methods:**
```csharp
public override void FieldBossInitialize(BossData bossData)
```

---

# 🎬 ANIMATIONS - Detailed Breakdown

## ✨ NEW ANIMATIONS (35 files)

### Battle UI Animations (21)
1. **BossSelectEntry.anim**
   - Boss selection popup entry animation

2. **BossWaitingEntry.anim**
   - Boss waiting state animation

3. **IngameRankEntry.anim**
   - In-game rank popup entry

4. **IngameRewardCardSelect.anim**
   - Boss reward card selection animation

5. **NetBossEntry.anim**
   - Network boss UI entry

6. **NetBossFill.anim**
   - Boss health bar fill animation

7. **SlectIngameRewardOpen.anim**
   - Reward selection opening

8. **SlectIngameRewardClose.anim**
   - Reward selection closing

9. **StatusMessageEntry.anim**
   - Status message banner animation

10. **loading.anim**
    - Loading/matchmaking animation

### PvP Result Animations (11)
11. **PvPResult1.anim**
    - Victory animation variant 1

12. **PvpResult2.anim**
    - Victory animation variant 2

13. **PvpJewelFail.anim**
    - Rank down jewel animation

14. **PvpJewelSucceed.anim**
    - Rank up jewel animation

15. **PvpResult3_Blank.anim**
    - Result screen blank state

16. **PvpResult3_Enter.anim**
    - Result screen entry

17. **PvpResult3_Idle.anim**
    - Result screen idle

18. **PvpResult3_OpenPromo.anim**
    - Promotion reveal opening

19. **PvpResult3_OpenPromoIdle.anim**
    - Promotion idle state

20. **PvpResult3_PromoEnter.anim**
    - Promotion entry animation

21. **PvpResult3_PromoFail.anim**
    - Demotion animation

22. **PvpResult3_PromoIdle.anim**
    - Promotion idle loop

23. **PvpResult3_PromoSucces.anim**
    - Promotion success

24. **PvpResult3_PromoSuccesEnter.anim**
    - Promotion success entry

25. **PvpResult3_TierDown.anim**
    - Tier demotion animation

26. **PvpResult3_TierLvUp.anim**
    - Tier level up animation

### Animator Controllers (3)
27. **IngameRewarCard.controller**
    - Boss reward card animator

28. **NetBoss.controller**
    - Network boss UI animator

29. **Popup - PvpResult3.controller**
    - PvP result popup animator

### Spine Skeleton Data (6)
30-32. **Penguin_Normal_01 Skeleton Data**
    - Penguin_Normal_01.atlas.txt
    - Penguin_Normal_01.skel_Atlas.asset
    - Penguin_Normal_01_Atlas.asset
    - Penguin_Normal_01_SkeletonData.asset

33-35. **Tier_Ani Skeleton Data**
    - Tier_Ani.atlas.txt
    - Tier_Ani_Atlas.asset
    - Tier_Ani_SkeletonData.asset

---

## 🔧 MODIFIED ANIMATIONS (1 file)

### 1. **Image.controller**
- Enhanced with new animation states for battle UI

---

# 📄 TEXT & DATA ASSETS - Detailed Breakdown

## ✨ NEW ASSETS (10 files)

### Boss Data Assets (7)
1. **Boomber.asset**
   - Boomber boss configuration

2. **Emberon.asset**
   - Emberon boss configuration (not yet implemented)

3. **Locky.asset**
   - Locky boss configuration

4. **Parasite.asset**
   - Parasite boss configuration

5. **Smoker.asset**
   - Smoker boss configuration

6. **Sotty.asset**
   - Sotty boss configuration

7. **Trush.asset**
   - Trush boss configuration

### JSON Data Files (3)
8. **FieldBossRewardGroupData.json**
   - Boss reward groups configuration

9. **FieldBossRewardGroupData1.json**
   - Additional reward group

10. **testWaveData.json**
    - Battle mode wave data for testing

---

## 🔧 MODIFIED ASSETS (Multiple files)

### UI Data (1)
1. **UIData.asset**
   - Added battle mode UI references
   - New popup references
   - Button sprites

### Scriptable Objects (1)
2. **PopupScriptableObject.asset**
   - Added new popup references:
     - BattleResult
     - BattleResultPromote
     - BattleResultTable
     - BossSelect
     - FieldBossReward
     - Friend
     - InGameRank
     - MatchMaking
     - StartUpBattleMode
     - StartUpSingleMode

### Localization (3)
3. **UI_Text_en-US.asset**
   - Added ~150 new text entries for:
     - Battle mode UI
     - Friend system
     - Boss selection
     - Surrender feature
     - Rank/tier names

4. **UI_Text_ko-KR.asset**
   - Korean translations for all new features

5. **UI_Text Shared Data.asset**
   - Shared localization keys

### Font Assets (5)
6. **GROBOLD SDF.asset**
7. **MN Namphrik Long Ruea SDF.asset**
8. **Maplestory Bold SDF.asset**
9. **Mitr-Medium SDF.asset**
10. **Pretendard-ExtraBold SDF.asset**
    - All updated with new glyphs for battle mode text

### Asset Client List (1)
11. **AssetListData.json**
    - Added new asset references:
      - Boss prefabs
      - Effect prefabs
      - UI prefabs
      - Network prefabs

### Addressables Groups (7)
12. **Default Local Group.asset**
13. **GameData.asset**
14. **GameResource.asset**
15. **Localization-Assets-Shared.asset**
16. **Localization-Locales.asset**
17. **Localization-String-Tables-English (United States) (en-US).asset**
18. **Localization-String-Tables-Korean (South Korea) (ko-KR).asset**
    - All updated with new addressable references

### Photon Fusion Assets (5)
19. **PhotonAppSettings.asset** (NEW)
    - Photon Fusion network settings
    - App ID configuration

20. **build_info.txt** (NEW)
    - Fusion SDK build information

21. **package.json** (NEW)
    - Fusion package metadata

22. **release_history.txt** (NEW)
    - Fusion SDK changelog

23. **changes-library.txt** (NEW)
    - PhotonLibs changelog

---

# 🔑 KEY FUNCTIONAL CHANGES SUMMARY

## Game Modes
**Before:** Single mode only
**After:** 3 modes
- `GameMode.SINGLE` - Solo gameplay
- `GameMode.BATTLE` - PvP multiplayer (2-3 players)
- `GameMode.TUTORIAL` - Guided experience

## Boss System
**Before:** 2 bosses (Car, Normal)
**After:** 6 bosses
- CarBossMonster (enhanced with clones)
- NormalBossMonster (enhanced)
- **BoomberBossMonster** (NEW - bomb mechanics)
- **SottyBossMonster** (NEW - pollution)
- **LockyBossMonster** (NEW - lockdown)
- **ParasiteBossMonster** (NEW - infection)

## Character Abilities
**New Mechanics:**
- Lockdown (disabled by Locky boss)
- Infection (debuffed by Parasite boss)
- Bomb target (set by Boomber boss)

## Buff System
**New Buffs:**
- Attack speed global buff
- Attack range buff
- Range-based attack multipliers (short/long range)
- Mission reward multiplier
- Boss reward multiplier
- Monster gem reward bonus
- Upgrade cost reduction
- Relocation cost discount
- Summon cost reduction

## Surrender System
**Conditions:**
- Available from wave 5+ in battle mode
- Triggers game over with penalty
- ELO rating adjustment
- Proper cleanup of multiplayer session

## Friend System
**Features:**
- Add/remove friends
- Send energy to friends
- View friend profiles
- Search for friends
- Recommended friends
- Friend battle invites

## Networking Infrastructure
**Photon Fusion Integration:**
- Session creation/joining
- Host migration
- RPC communication
- Player state synchronization
- Real-time leaderboards
- Matchmaking system

---

# 📋 DEPENDENCY CHANGES

## New Package: Photon Fusion
**Added Files:** ~100+ Photon Fusion SDK files
- Runtime scripts
- Editor tools
- Statistics/debugging tools
- Utilities

## Modified Packages
1. **com.singularitygroup.hotreload**
   - Updated for compatibility

2. **Unity Packages (manifest.json)**
   - Added Photon Fusion dependency

---

# 🎯 TESTING NOTES

## New Test Data
1. **testWaveData.json** - Battle mode wave testing
2. **FieldBossRewardGroupData.json** - Reward testing

## Debug Features
- Boss spawn hotkey (B key) in editor
- Monster clear hotkey (P key) in editor
- Network statistics toggle in settings

---

*End of Detailed Changelog*

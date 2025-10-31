using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Framework.UI;
using Framework.GameData.Defense;
using UnityEngine.SceneManagement;
using Framework.Util;
using System.Linq;
using Framework.Game.Defense;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

namespace Framework.Network
{
    /// <summary>
    /// Handles network connection logic for matchmaking, session management,
    /// and gameplay state synchronization using Photon Fusion.
    /// Refactored to properly use Fusion 1.1.0 built-in Host Migration.
    /// </summary>
    public class NetworkConnect : SimulationBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkConnect Instance;

        [Header("Network State")]
        public int metaScore;
        public bool isHost => runner != null && runner.IsServer;
        public NetworkRunner runner;
        public Dictionary<int, NetworkBattleData> dic_PlayerData = new(); // player index -> battle data
        public NetworkBattleStatus networkBattleStatus;
        public int playerIdx;
        public string nickname;
        public int minPlayerCount = 2;
        public int maxPlayerCount = 3;
        public bool isFriendlyMatch;
        public string roomName = "";
        public string roomPassword = "";
        public int playId;
        public string roomUuid;
        public int sessionId;
        public string userId;
        public float avgElo = 0;
        bool isHostLoadedSceneBattle = false;
        bool firstTimeInitializeCheck = false;

        [Header("References")]
        public GameObject networkObjectPrefab;
        public NetworkGameManager networkGameManager;

        public MyBattleLeaderboardInfo myCurrentRank;

        // Host migration token storage
        private HostMigrationToken _hostMigrationToken;

        // Ensures countdown RPC triggers only once per session
        private bool _countdownStarted;

        // Flag to prevent player sync during host migration on all clients
        private bool _isHostMigrating;

        public bool IsCurrentHost()
        {
            // Always check runner's server status for accurate host detection
            // This is critical for host migration scenarios
            if (runner != null && runner.IsServer)
                return true;

            return false;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("[NetworkConnect] Singleton Instance created.");
            }
            Application.runInBackground = true;
        }

        /// <summary>
        /// Connects to the shared lobby to list/join/create sessions.
        /// </summary>
        public async void ConnectToLobby(bool isFriendlyMatch, string roomName = "", string roomPassword = "")
        {
            this.isFriendlyMatch = isFriendlyMatch;
            this.roomName = roomName;
            this.roomPassword = roomPassword;

            // reset countdown flag on a fresh lobby connection
            _countdownStarted = false;

            Debug.Log($"[NetworkConnect] Connecting to lobby... ~ Is Friendly Match: {isFriendlyMatch}");

            if (runner == null)
            {
                GameObject goRunner = new GameObject("NetworkRunner_Lobby");
                goRunner.transform.SetParent(transform); // Parent to NetworkConnect for organization
                runner = goRunner.AddComponent<NetworkRunner>();
                Debug.Log("[NetworkConnect] NetworkRunner component added.");
            }

            // CRITICAL: Add callbacks BEFORE joining the lobby
            runner.AddCallbacks(this);
            Debug.Log("[NetworkConnect] Callbacks registered for lobby connection.");

            await Task.Delay(Random.Range(0, 1000));
            await NetworkManager.Instance.GetMyBattleLeaderboard((myRank) => myCurrentRank = myRank, (myRank) => myCurrentRank = myRank);

            Debug.Log("[NetworkConnect] Attempting to join session lobby...");
            await runner.JoinSessionLobby(SessionLobby.Shared);
            Debug.Log($"[NetworkConnect] Connected to session lobby. Runner state: {runner.State}");

            // Session list will be automatically updated by Photon after joining the lobby
            Debug.Log("[NetworkConnect] Waiting for session list update from Photon...");
        }

        /// <summary>
        /// Joins an existing session by name.
        /// </summary>
        public async void JoinSession(string sessionName)
        {
            Debug.Log($"[NetworkConnect] Joining session: {sessionName}");

            if (runner == null)
            {
                GameObject goRunner = new GameObject("NetworkRunner_Join");
                goRunner.transform.SetParent(transform);
                runner = goRunner.AddComponent<NetworkRunner>();
            }

            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = Fusion.GameMode.AutoHostOrClient,
                SessionName = sessionName
            });

            if (result.Ok)
            {
                // AutoHostOrClient may make us host even if we tried to join
                Debug.Log($"[NetworkConnect] Successfully joined session. isHost: {isHost}");
                roomUuid = runner.SessionInfo.Name;
                networkBattleStatus = NetworkBattleStatus.LOBBY;
                await NetworkManager.Instance.ReadyForBattle(new ReadyBattlePayload(roomUuid, UserInfoManager.Instance.userId), (response) =>
                {
                    sessionId = response.sessionId;
                    dic_PlayerData[playerIdx].sessionId = sessionId;
                }, null);
            }
            else
            {
                Debug.LogError($"[NetworkConnect] Failed to join session: {result.ShutdownReason}");
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
                });
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        /// <summary>
        /// Joins an existing friendly session by name & password.
        /// </summary>
        public async void JoinFriendlySession(string sessionName)
        {
            Debug.Log($"[NetworkConnect] Joining friendly session: {sessionName}");

            if (runner == null)
            {
                GameObject goRunner = new GameObject("NetworkRunner_Join");
                goRunner.transform.SetParent(transform);
                runner = goRunner.AddComponent<NetworkRunner>();
            }

            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = Fusion.GameMode.AutoHostOrClient,
                SessionName = sessionName,
            });

            if (result.Ok)
            {
                // AutoHostOrClient may make us host even if we tried to join
                Debug.Log($"[NetworkConnect] Successfully joined friendly session. isHost: {isHost}");
                roomUuid = runner.SessionInfo.Name;
                networkBattleStatus = NetworkBattleStatus.LOBBY;
            }
            else
            {
                Debug.LogError($"[NetworkConnect] Failed to join session: {result.ShutdownReason}");
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
                });
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        /// <summary>
        /// Creates a new session as host with session properties.
        /// </summary>
        public async void CreateSession()
        {
            Debug.Log("[NetworkConnect] Creating new session as host...");

            var customProps = new Dictionary<string, SessionProperty>
            {
                ["averageScore"] = metaScore,
                ["averageRate"] = 40,
                ["isPlaying"] = false,
            };

            if (runner == null)
            {
                GameObject goRunner = new GameObject("NetworkRunner_Join");
                goRunner.transform.SetParent(transform);
                runner = goRunner.AddComponent<NetworkRunner>();
            }

            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = Fusion.GameMode.AutoHostOrClient,
                PlayerCount = maxPlayerCount,
                SessionProperties = customProps
            });

            if (result.Ok)
            {
                Debug.Log("[NetworkConnect] Session created successfully.");
                roomUuid = runner.SessionInfo.Name;
                networkBattleStatus = NetworkBattleStatus.LOBBY;
                await NetworkManager.Instance.ReadyForBattle(new ReadyBattlePayload(roomUuid, UserInfoManager.Instance.userId), (response) =>
                {
                    sessionId = response.sessionId;
                }, null);
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").UpdateUserInfo();
            }
            else
            {
                Debug.LogError($"[NetworkConnect] Failed to create session: {result.ShutdownReason}");
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        /// <summary>
        /// Creates a new session with friend.
        /// </summary>
        public async void CreateFriendlySession()
        {
            Debug.Log("[NetworkConnect] Creating new session as host...");

            var customProps = new Dictionary<string, SessionProperty>
            {
                ["averageScore"] = metaScore,
                ["averageRate"] = 40,
                ["password"] = UserInfoManager.Instance.userId,
                ["isPlaying"] = false,
            };

            if (runner == null)
            {
                GameObject goRunner = new GameObject("NetworkRunner_Join");
                goRunner.transform.SetParent(transform);
                runner = goRunner.AddComponent<NetworkRunner>();
            }

            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = Fusion.GameMode.AutoHostOrClient,
                SessionName = UserInfoManager.Instance.userId,
                PlayerCount = maxPlayerCount,
                SessionProperties = customProps
            });

            if (result.Ok)
            {
                Debug.Log("[NetworkConnect] Friendly Session created successfully.");
                roomUuid = runner.SessionInfo.Name;
                roomName = runner.SessionInfo.Name;
                roomPassword = UserInfoManager.Instance.userId;
                networkBattleStatus = NetworkBattleStatus.LOBBY;
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").UpdateUserInfo();
            }
            else
            {
                Debug.LogError($"[NetworkConnect] Failed to create session: {result.ShutdownReason}");
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        [Rpc]
        public static async void Rpc_LoadGameScene(NetworkRunner runner, string idx)
        {
            Debug.Log($"[NetworkConnect] Rpc_LoadGameScene triggered by Player {idx}. " +
                      $"LocalPlayer={runner.LocalPlayer.AsIndex}");

            if (!Instance.isFriendlyMatch)
            {
                StartBattlePayload payload = new StartBattlePayload(Instance.userId, Instance.roomUuid, Instance.nickname, UserSlotManager.Instance.focusIdx, Instance.sessionId);
                await NetworkManager.Instance.StartBattle(payload, (playId) =>
                {
                    Instance.playId = playId;
                    Instance.dic_PlayerData[Instance.playerIdx].playId = playId;
                }, null);
            }

            Instance.StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(true, Instance.GameStartSequence));
        }

        public void GameStartSequence()
        {
            Debug.Log("[NetworkConnect] Running GameStartSequence...");

            if (isHost && !isHostLoadedSceneBattle)
            {
                isHostLoadedSceneBattle = true;
                Debug.Log("[NetworkConnect] Host is loading battle scene.");
                StopAllCoroutines();
                runner.LoadScene(SceneRef.FromIndex(3), LoadSceneMode.Single);
            }
        }

        public IEnumerator GameStart()
        {
            Debug.Log("[NetworkConnect] GameStart coroutine started. Waiting 0s...");

            var customProps = new Dictionary<string, SessionProperty>
            {
                ["isPlaying"] = true,
            };
            runner.SessionInfo.IsOpen = false;
            runner.SessionInfo.UpdateCustomProperties(customProps);
            yield return new WaitForSeconds(0f);
            Rpc_LoadGameScene(runner, runner.LocalPlayer.AsIndex.ToString());
        }

        public PlayerRef GetPlayerRef(int idx)
        {
            foreach (var item in runner.ActivePlayers)
            {
                if (item.AsIndex == idx)
                    return item;
            }
            Debug.LogWarning($"[NetworkConnect] PlayerRef for idx {idx} not found.");
            return default;
        }

        // -------------------------------------------------------------------
        // INetworkRunnerCallbacks Implementation
        // -------------------------------------------------------------------

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"[NetworkConnect] ===== OnPlayerJoined called on Player {playerIdx} =====");
            Debug.Log($"[NetworkConnect] Joining player index: {player.AsIndex}");
            Debug.Log($"[NetworkConnect] Is local player: {player.AsIndex == runner.LocalPlayer.AsIndex}");
            Debug.Log($"[NetworkConnect] Current dic_PlayerData.Count: {dic_PlayerData.Count}");
            Debug.Log($"[NetworkConnect] Current players: [{string.Join(", ", dic_PlayerData.Keys)}]");

            if (player.AsIndex == runner.LocalPlayer.AsIndex)
            {
                Debug.Log("[NetworkConnect] This is the local player joining.");

                // Create and send player data
                NetworkBattleData data = new()
                {
                    profileId = UserInfoManager.Instance.userState.equippedProfileId,
                    isHost = isHost,
                    nickname = UserInfoManager.Instance.nickname,
                    playerIdx = runner.LocalPlayer.AsIndex,
                    decList = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds,
                    isGameOver = false,
                    isInitialize = false,
                    userId = UserInfoManager.Instance.userId,
                    rankTier = DataManager.Instance.GetRankTierConfig(myCurrentRank.finalRank).description,
                    elo = myCurrentRank.elo,
                    hasShownSummary = false,
                };

                this.playerIdx = data.playerIdx;
                this.nickname = data.nickname;
                this.userId = data.userId;

                Debug.Log($"[NetworkConnect] Sending Rpc_JoinGame for local player {player.AsIndex}");
                Rpc_JoinGame(runner, JsonUtility.ToJson(data));
            }
            else
            {
                Debug.Log($"[NetworkConnect] Syncing player data for remote player {player.AsIndex}.");

                // When another player joins/reconnects, sync ALL existing players to them
                if (networkBattleStatus == NetworkBattleStatus.INGAME && isHost)
                {
                    Debug.Log($"[NetworkConnect] INGAME: Syncing {dic_PlayerData.Count} players to reconnecting player {player.AsIndex}");
                    foreach (var value in dic_PlayerData.Values)
                    {
                        Debug.Log($"[NetworkConnect] Syncing Player {value.playerIdx} data to Player {player.AsIndex}");
                        Rpc_JoinGame(this.runner, player, JsonUtility.ToJson(value));
                    }
                }
                else if (networkBattleStatus == NetworkBattleStatus.LOBBY)
                {
                    Debug.Log($"[NetworkConnect] LOBBY: Syncing {dic_PlayerData.Count} players to new player {player.AsIndex}");
                    foreach (var value in dic_PlayerData.Values)
                    {
                        if (value.playerIdx != player.AsIndex)
                        {
                            Debug.Log($"[NetworkConnect] Syncing Player {value.playerIdx} data to Player {player.AsIndex}");
                            Rpc_JoinGame(this.runner, player, JsonUtility.ToJson(value));
                        }
                    }
                }
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"[NetworkConnect] OnPlayerLeft called on Player {playerIdx}: Player {player.AsIndex} left");
            Debug.Log($"[NetworkConnect] dic_PlayerData count before removal: {dic_PlayerData.Count}");
            Debug.Log($"[NetworkConnect] Players: {string.Join(", ", dic_PlayerData.Keys)}");

            // Check if the leaving player was the host
            // IMPORTANT: Before OnPlayerLeft is called, Photon has already migrated the host
            // So we need to check the player data, not runner.IsServer
            bool wasHost = false;
            if (dic_PlayerData.ContainsKey(player.AsIndex))
            {
                wasHost = dic_PlayerData[player.AsIndex].isHost;
                Debug.Log($"[NetworkConnect] Leaving player was host (from playerData): {wasHost}");
            }
            else
            {
                Debug.LogWarning($"[NetworkConnect] Player {player.AsIndex} not found in dic_PlayerData!");
            }

            // Alternative: if OnPlayerLeft is called BEFORE migration, this would be true
            // But Photon Fusion calls OnPlayerLeft AFTER migration completes
            // So the current host is already the NEW host, not the one leaving
            Debug.Log($"[NetworkConnect] Current runner.IsServer: {runner.IsServer}, Local isHost: {isHost}");

            // Handle lobby vs in-game logic
            NetworkBattleData data = null;
            if (dic_PlayerData.ContainsKey(player.AsIndex))
            {
                data = dic_PlayerData[player.AsIndex];
            }

            switch (networkBattleStatus)
            {
                case NetworkBattleStatus.LOBBY:
                    Debug.Log("[NetworkConnect] Updating lobby UI after player left.");

                    // LOBBY: Remove ALL leaving players immediately (including host)
                    // No special host handling needed - just remove and continue
                    if (dic_PlayerData.ContainsKey(player.AsIndex))
                    {
                        dic_PlayerData.Remove(player.AsIndex);
                        Debug.Log($"[NetworkConnect] Removed player {player.AsIndex} from lobby (wasHost: {wasHost})");
                    }

                    PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").UpdateUserInfo();

                    // Recalculate timers based on new player count
                    if (Instance.dic_PlayerData.Count >= Instance.minPlayerCount)
                    {
                        // Still have enough players: RESTART countdown to 30s
                        if (Instance.ICountMatchingTimeOut != null)
                            Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                        if (Instance.ICountTimeStart != null)
                            Instance.StopCoroutine(Instance.ICountTimeStart);
                        Instance.ICountTimeStart = Instance.StartCoroutine(Instance.CountTimeStart());
                        Debug.Log("[NetworkConnect] Player left but still >= minPlayerCount, restarting countdown");
                    }
                    else
                    {
                        // Below minimum: stop countdown, start timeout timer
                        if (Instance.ICountTimeStart != null)
                            Instance.StopCoroutine(Instance.ICountTimeStart);
                        if (Instance.ICountMatchingTimeOut != null)
                            Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                        Instance.ICountMatchingTimeOut = Instance.StartCoroutine(Instance.CountMatchingTimeOut());
                        Debug.Log("[NetworkConnect] Player left: below minPlayerCount, starting timeout timer");
                    }
                    break;
                case NetworkBattleStatus.INGAME:
                    if (data != null)
                    {
                        // In battle: mark as abnormal exit and keep data for summary
                        data.isAbnormalExit = true;
                        data.isInitialize = true;
                        data.isGameOver = true;
                        if (networkGameManager != null)
                        {
                            networkGameManager.Rpc_GameOver(player.AsIndex, data.waveCount, true);
                        }
                        else
                        {
                            Debug.LogWarning("[NetworkConnect] NetworkGameManager is null in OnPlayerLeft, cannot send GameOver RPC");
                        }
                    }
                    break;
            }

            Instance.PushSnapShot();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"[NetworkConnect] Scene load completed, spawning player object. {runner == null} - {networkObjectPrefab == null}");
            GameManager.Instance.gameMode = Game.Defense.GameMode.BATTLE;

            // Only host (state authority) spawns player objects.
            if (isHost)
            {
                NetworkObject networkObject = runner.Spawn(networkObjectPrefab);
                runner.SetPlayerObject(runner.LocalPlayer, networkObject);
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            networkBattleStatus = NetworkBattleStatus.INGAME;
            Debug.Log("[NetworkConnect] Scene load started, entering INGAME state.");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log($"[NetworkConnect] OnSessionListUpdated FIRED! Found {sessionList.Count} sessions. IsFriendlyMatch: {isFriendlyMatch}");

            if (isFriendlyMatch)
            {
                Debug.Log($"[NetworkConnect] Processing friendly match sessions. RoomName: {roomName}, RoomPassword: {roomPassword}");

                if (sessionList.Count == 0)
                {
                    Debug.Log("[NetworkConnect] No sessions found, creating new one.");
                    CreateFriendlySession();
                    return;
                }

                foreach (var session in sessionList)
                {
                    if (!session.IsOpen || session.MaxPlayers == session.PlayerCount)
                        continue;
                    session.Properties.TryGetValue("password", out var pw);
                    session.Properties.TryGetValue("isPlaying", out var isPlaying);
                    if (session.Name == roomName && roomPassword == pw && roomName == session.Name && !isPlaying)
                    {
                        Debug.Log($"[NetworkConnect] Joining available friendly session: {session.Name}");
                        JoinFriendlySession(session.Name);
                        return;
                    }
                }

                Debug.Log("[NetworkConnect] No suitable session found, creating new one.");
                CreateFriendlySession();
            }
            else
            {
                Debug.Log($"[NetworkConnect] Processing regular matchmaking sessions.");

                if (sessionList.Count == 0)
                {
                    Debug.Log("[NetworkConnect] No sessions found, creating new one.");
                    CreateSession();
                    return;
                }

                foreach (var session in sessionList)
                {
                    session.Properties.TryGetValue("isPlaying", out var isPlaying);
                    session.Properties.TryGetValue("password", out var pw);
                    if (!session.IsOpen || session.MaxPlayers == session.PlayerCount || isPlaying || pw != null)
                        continue;

                    Debug.Log($"[NetworkConnect] Joining available session: {session.Name}");
                    JoinSession(session.Name);
                    return;
                }

                Debug.Log("[NetworkConnect] No suitable session found, creating new one.");
                CreateSession();
            }
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.LogWarning($"[NetworkConnect] Runner shutdown. Reason={shutdownReason}");
            switch (shutdownReason)
            {
                case ShutdownReason.Ok:
                    Debug.Log("[NetworkConnect] Normal shutdown.");
                    break;

                case ShutdownReason.OperationCanceled:
                    Debug.LogWarning("[NetworkConnect] Disconnected by Operation Canceled");
                    OnAbnormalShutdown();
                    break;

                case ShutdownReason.DisconnectedByPluginLogic:
                    Debug.LogError("[NetworkConnect] Disconnected by plugin logic.");
                    OnAbnormalShutdown();
                    break;

                case ShutdownReason.Error:
                    Debug.LogError("[NetworkConnect] Disconnected due to an error.");
                    OnAbnormalShutdown();
                    break;

                default:
                    Debug.LogError("[NetworkConnect] Shutdown: " + shutdownReason);
                    break;
            }
        }

        void OnAbnormalShutdown()
        {
            if (networkBattleStatus == NetworkBattleStatus.INGAME && !GameManager.Instance.isGameOver)
            {
                GameManager.Instance.GameOver();
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    ShutDown();
                    GameManager.Instance.objectPoolManager.AllClear();
                    SceneLoadManager.Instance.SwitchingScene(2);
                });
            }
            else if (networkBattleStatus == NetworkBattleStatus.LOBBY)
            {
                StopAllCoroutines();
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
                });
            }
            else
            {
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    ShutDown();
                    GameManager.Instance.objectPoolManager.AllClear();
                    SceneLoadManager.Instance.SwitchingScene(2);
                });
            }
        }

        public void InitializeCheck(int idx, string json)
        {
            if (firstTimeInitializeCheck)
                return;
            Debug.Log($"[NetworkConnect] InitializeCheck called for player {idx}.");
            NetworkBattleData data = JsonUtility.FromJson<NetworkBattleData>(json);
            if (data != null)
            {
                dic_PlayerData[idx].isInitialize = true;
                dic_PlayerData[idx].sessionId = data.sessionId;
                dic_PlayerData[idx].playId = data.playId;
            }

            bool allReady = dic_PlayerData.Values.All(p => p.isInitialize || p.isGameOver || p.isAbnormalExit);
            if (allReady)
            {
                Debug.Log("[NetworkConnect] All players ready. Closing session and starting countdown.");
                firstTimeInitializeCheck = true;
                runner.SessionInfo.IsOpen = false;

                StartCoroutine(InitializeCheckDone());
            }
        }

        IEnumerator InitializeCheckDone()
        {
            GameManager.Instance.anim_Transition.SetTrigger("TransitionOut");

            yield return new WaitForSeconds(1f);

            GameManager.Instance.anim_CloudSequence.Rewind();
            GameManager.Instance.anim_CloudSequence.Play();

            GameManager.Instance.CountStart();
        }

        public void FieldBossRewardCheck(int idx)
        {
            Debug.Log($"[NetworkConnect] Fieldboss reward called for player {idx}.");
            dic_PlayerData[idx].selectedFieldBossReward = true;

            bool allReady = dic_PlayerData.Values.All(p => p.selectedFieldBossReward || p.isGameOver || p.isAbnormalExit);
            if (allReady)
            {
                Debug.Log("[NetworkConnect] All players selected reward");

                foreach (var val in dic_PlayerData.Values)
                {
                    val.selectedFieldBossReward = false;
                }

                GameManager.Instance.CountStart();
            }
        }

        public void OnDestroy()
        {
            Debug.Log("[NetworkConnect] Destroyed. Clearing instance and cleaning up runner.");

            // Clean up the runner GameObject if it exists
            if (runner != null && runner.gameObject != null)
            {
                Destroy(runner.gameObject);
                runner = null;
            }

            _countdownStarted = false;
            Instance = null;
        }

        // -------------------------------------------------------------------
        // Empty Callbacks (kept for completeness, just added logs)
        // -------------------------------------------------------------------
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log($"[NetworkConnect] ===== OnConnectedToServer called on Player {playerIdx} =====");
            Debug.Log($"[NetworkConnect] Runner state: {runner.State}");
            Debug.Log($"[NetworkConnect] Is host: {runner.IsServer}");

            // After non-chosen clients reconnect to the new host, clear migration flag
            if (_isHostMigrating && !runner.IsServer)
            {
                Debug.Log($"[NetworkConnect] Non-chosen client reconnected to new host, clearing migration flag");
                _isHostMigrating = false;
            }
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) => Debug.LogError($"[NetworkConnect] Connection failed: {reason}");
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) => Debug.Log("[NetworkConnect] ConnectRequest received.");
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) => Debug.Log("[NetworkConnect] Custom auth response received.");
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log($"[NetworkConnect] ===== OnDisconnectedFromServer called on Player {playerIdx} ~ Reason {reason} =====");
            Debug.Log($"[NetworkConnect] Reason: {reason}");
            Debug.Log($"[NetworkConnect] Status: {networkBattleStatus}");

            // Reset migration flag
            _isHostMigrating = false;

            switch (networkBattleStatus)
            {
                case NetworkBattleStatus.LOBBY:
                    Debug.Log("[NetworkConnect] Disconnected during LOBBY - resetting and restarting matchmaking");

                    // Stop all timers
                    if (ICountTimeStart != null)
                        StopCoroutine(ICountTimeStart);
                    if (ICountMatchingTimeOut != null)
                        StopCoroutine(ICountMatchingTimeOut);

                    // Clean up runner
                    if (runner != null && runner.gameObject != null)
                    {
                        Destroy(runner.gameObject);
                        this.runner = null;
                    }

                    // Reset state
                    dic_PlayerData.Clear();
                    _countdownStarted = false;

                    // Restart matchmaking
                    Debug.Log("[NetworkConnect] disconnect");
                    OnAbnormalShutdown();
                    break;

                case NetworkBattleStatus.INGAME:
                    if (reason == NetDisconnectReason.Timeout)
                    {
                        Debug.Log("[NetworkConnect] Disconnected during BATTLE - Time Out");
                        OnAbnormalShutdown();
                    }
                    else
                    {
                        Debug.Log("[NetworkConnect] Disconnected during BATTLE - handling abnormal shutdown");
                        OnAbnormalShutdown();
                    }
                    break;
            }
        }

        /// <summary>
        /// Called on ALL clients when host disconnects.
        /// LOBBY: Simply reset and create/join a new session - NO MIGRATION.
        /// INGAME: Use Fusion's built-in host migration with deterministic host selection.
        /// </summary>
        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"[NetworkConnect] ===== OnHostMigration called on Player {playerIdx} =====");
            Debug.Log($"[NetworkConnect] Current status: {networkBattleStatus}");
            Debug.Log($"[NetworkConnect] Active players: [{string.Join(", ", runner.ActivePlayers.Select(p => p.AsIndex))}]");

            // LOBBY: Don't migrate - just reset and create/join a new session
            if (networkBattleStatus == NetworkBattleStatus.LOBBY)
            {
                Debug.Log("[NetworkConnect] LOBBY: Host left - resetting and creating/joining new session");
                Debug.Log("[NetworkConnect] NO HOST MIGRATION in lobby - fresh start");

                // Stop all timers immediately
                StopAllCoroutines();
                ICountTimeStart = null;
                ICountMatchingTimeOut = null;

                // Shutdown the runner cleanly
                Debug.Log("[NetworkConnect] Shutting down runner");
                await runner.Shutdown(shutdownReason: ShutdownReason.Ok);

                // Clean up the old runner GameObject
                if (runner != null && runner.gameObject != null)
                {
                    Destroy(runner.gameObject);
                    this.runner = null;
                }

                // Reset all state variables
                dic_PlayerData.Clear();
                _countdownStarted = false;
                _isHostMigrating = false;
                playerIdx = 0;

                // Restart matchmaking from scratch - will create or join a new session
                Debug.Log("[NetworkConnect] Restarting matchmaking - creating/joining new session");
                ConnectToLobby(isFriendlyMatch);
                var popup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                popup.UpdateUserInfo();
                return;
            }

            // INGAME: Perform host migration
            Debug.Log("[NetworkConnect] INGAME: Starting host migration process");

            // Set migration flag to prevent OnPlayerJoined from syncing during migration
            _isHostMigrating = true;

            // Find the old host index BEFORE clearing dic_PlayerData
            int oldHostIdx = -1;
            foreach (var kvp in dic_PlayerData)
            {
                if (kvp.Value.isHost)
                {
                    oldHostIdx = kvp.Key;
                    Debug.Log($"[NetworkConnect] Found old host: Player {oldHostIdx}");
                    break;
                }
            }

            // Deterministic host selection: Choose client with lowest PlayerRef index
            // EXCLUDING the old host who is leaving
            var sortedPlayers = runner.ActivePlayers
                .Where(p => p.AsIndex != oldHostIdx)
                .OrderBy(p => p.AsIndex)
                .ToList();

            if (sortedPlayers.Count == 0)
            {
                Debug.LogError("[NetworkConnect] No remaining players to become host!");
                return;
            }

            var chosenHost = sortedPlayers.First();
            bool isChosenHost = chosenHost == runner.LocalPlayer;

            Debug.Log($"[NetworkConnect] Old host: Player {oldHostIdx}");
            Debug.Log($"[NetworkConnect] Chosen new host: Player {chosenHost.AsIndex}");
            Debug.Log($"[NetworkConnect] Am I the chosen host? {isChosenHost}");

            if (true)
            {
                // Store the migration token
                _hostMigrationToken = hostMigrationToken;

                // Store current state for restoration
                var currentStatus = networkBattleStatus;

                // Shutdown the current Runner
                Debug.Log("[NetworkConnect] Shutting down current runner for host migration");
                await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

                // Destroy the old runner and create a new one
                Debug.Log("[NetworkConnect] Destroying old runner and creating new one");
                if (runner != null)
                {
                    Destroy(runner.gameObject);
                    runner = null;
                }

                GameObject goRunner = new GameObject("NetworkRunner_Migration");
                goRunner.transform.SetParent(transform);
                var newRunner = goRunner.AddComponent<NetworkRunner>();

                // Configure the new runner
                newRunner.ProvideInput = true;
                newRunner.AddCallbacks(this);

                // Start the new Runner using the HostMigrationToken
                Debug.Log("[NetworkConnect] Starting new runner with host migration token");
                StartGameResult result = await newRunner.StartGame(new StartGameArgs()
                {
                    GameMode = isChosenHost ? Fusion.GameMode.Host : Fusion.GameMode.Client,
                    HostMigrationToken = hostMigrationToken,
                    HostMigrationResume = isChosenHost ? OnHostMigrationResume : null,
                });

                if (result.Ok == false)
                {
                    Debug.LogError($"[NetworkConnect] Host migration failed 1");
                    await Task.Delay(500);
                    StartGameResult result1 = await newRunner.StartGame(new StartGameArgs()
                    {
                        GameMode = isChosenHost ? Fusion.GameMode.Host : Fusion.GameMode.Client,
                        HostMigrationToken = hostMigrationToken,
                        HostMigrationResume = isChosenHost ? OnHostMigrationResume : null,
                    });
                    if (result1.Ok == false)
                        HandleMigrationFailure(currentStatus);
                }
                else
                {
                    Debug.Log($"[NetworkConnect] Host migration StartGame succeeded");
                }
            }
            else
            {
                Debug.Log($"[NetworkConnect] I am NOT the chosen host (Player {runner.LocalPlayer.AsIndex}), waiting for new host to call StartGame");
                // Non-chosen clients just wait for reconnection
                // Their OnReconnectToServer will fire when the new host establishes the session
            }
        }

        /// <summary>
        /// Called by Fusion ONLY on the new host after StartGame(token) completes.
        /// Only called for INGAME migrations - LOBBY never migrates.
        /// </summary>
        private void OnHostMigrationResume(NetworkRunner runner)
        {
            Debug.Log($"[NetworkConnect] ===== OnHostMigrationResume called on Player {playerIdx} =====");
            Debug.Log($"[NetworkConnect] Current status: {networkBattleStatus}");

            // Update the runner reference to the new one
            this.runner = runner;

            // Update host status based on the new runner
            Debug.Log($"[NetworkConnect] I am now the new host: {isHost}");

            // Reset countdown flag on migration to allow fresh start
            _countdownStarted = false;

            // This should ONLY be called for INGAME status
            if (networkBattleStatus == NetworkBattleStatus.LOBBY)
            {
                Debug.LogError("[NetworkConnect] OnHostMigrationResume called during LOBBY - this should NOT happen!");
                return;
            }

            if (networkBattleStatus == NetworkBattleStatus.INGAME)
            {
                Debug.Log($"[NetworkConnect] INGAME migration resume on Player {playerIdx}");

                // Restore network objects from the migration snapshot
                var resumeNetworkObjects = runner.GetResumeSnapshotNetworkObjects();
                Debug.Log($"[NetworkConnect] Restoring {resumeNetworkObjects.Count()} network objects from snapshot");

                foreach (var resumeNO in resumeNetworkObjects)
                {
                    runner.Spawn(resumeNO, onBeforeSpawned: (runner, newNO) =>
                    {
                        newNO.CopyStateFrom(resumeNO);

                        if (newNO.TryGetBehaviour<NetworkGameManager>(out var gameManager))
                        {
                            Debug.Log("[NetworkConnect] Restored NetworkGameManager reference");
                            networkGameManager = gameManager;
                        }
                    });
                }

                // Find old host index
                int oldHostIdx = -1;
                foreach (var kvp in dic_PlayerData)
                {
                    if (kvp.Value.isHost && kvp.Key != playerIdx)
                    {
                        oldHostIdx = kvp.Key;
                        Debug.Log($"[NetworkConnect] Found old host: {oldHostIdx}");
                        break;
                    }
                }

                // Update host flags in dic_PlayerData
                foreach (var playerData in dic_PlayerData.Values)
                {
                    playerData.isHost = false;
                }
                if (dic_PlayerData.ContainsKey(playerIdx))
                {
                    dic_PlayerData[playerIdx].isHost = true;
                }

                // Call GameOver for old host (INGAME only)
                if (oldHostIdx != -1 && dic_PlayerData.ContainsKey(oldHostIdx) && networkGameManager != null)
                {
                    Debug.Log($"[NetworkConnect] Calling GameOver for old host {oldHostIdx}");
                    dic_PlayerData[oldHostIdx].isAbnormalExit = true;
                    dic_PlayerData[oldHostIdx].isInitialize = true;
                    networkGameManager.Rpc_GameOver(oldHostIdx, dic_PlayerData[oldHostIdx].waveCount, true);
                }

                // Update in-game UI after migration
                if (UIManager.Instance != null && UIManager.Instance.inGameRankPopup != null)
                {
                    UIManager.Instance.inGameRankPopup.SortPlayerData();
                    Debug.Log("[NetworkConnect] Updated in-game rank UI after migration");
                }
            }

            // Clear the migration token and flag
            _hostMigrationToken = null;
            _isHostMigrating = false;
            Debug.Log("[NetworkConnect] Cleared migration token and flag");

            Debug.Log($"[NetworkConnect] OnHostMigrationResume completed on Player {playerIdx}");
        }

        /// <summary>
        /// Handles migration failure based on the current battle status
        /// </summary>
        private void HandleMigrationFailure(NetworkBattleStatus status)
        {
            Debug.LogError($"[NetworkConnect] Host migration failed for status: {status}");

            switch (status)
            {
                case NetworkBattleStatus.LOBBY:
                    Debug.Log("[NetworkConnect] Migration failed in LOBBY - returning to main menu");
                    // Migration failed during matchmaking - return to main menu
                    HandleMatchmakingMigrationFailure();
                    break;

                case NetworkBattleStatus.INGAME:
                    Debug.Log("[NetworkConnect] Migration failed in INGAME - ending battle");
                    // End the battle and return to main menu
                    if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
                    {
                        GameManager.Instance.GameOver();
                    }
                    OnAbnormalShutdown();
                    break;
            }
        }

        /// <summary>
        /// Handles migration failure during matchmaking - returns to main menu
        /// </summary>
        private void HandleMatchmakingMigrationFailure()
        {
            Debug.Log("[NetworkConnect] Host migration failed during matchmaking - returning to main menu");

            // Clean up the failed runner
            if (runner != null)
            {
                runner.Shutdown();
                Destroy(runner.gameObject);
                runner = null;
            }

            // Reset state
            dic_PlayerData.Clear();
            networkBattleStatus = NetworkBattleStatus.LOBBY;

            // Show error and return to main menu
            var systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            if (systemNoticePopup != null)
            {
                string message = "Connection lost - returning to main menu";
                try
                {
                    message = LanguageManager.Instance.GetStringData("UI_Connection_Lost_Matchmaking");
                }
                catch
                {
                    Debug.LogWarning("[NetworkConnect] Language key 'UI_Connection_Lost_Matchmaking' not found, using fallback");
                }

                systemNoticePopup.SetNoticeText(message,
                    delegate
                    {
                        // Close matchmaking popup and return to main menu
                        var matchmakingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                        if (matchmakingPopup != null)
                        {
                            matchmakingPopup.Shutdown();
                        }
                        SceneLoadManager.Instance.SwitchingScene(2);
                    });
            }
            else
            {
                // Fallback: directly return to main menu
                var matchmakingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                if (matchmakingPopup != null)
                {
                    matchmakingPopup.Shutdown();
                }
                SceneLoadManager.Instance.SwitchingScene(2);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj) => Debug.Log("[NetworkConnect] OnBeforeSpawned event.");
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) => Debug.Log("[NetworkConnect] Simulation message received.");

        // -------------------------------------------------------------------
        // RPCs
        // -------------------------------------------------------------------
        [Rpc]
        public static void Rpc_SetHost(NetworkRunner runner, int idx)
        {
            Instance.dic_PlayerData[idx].isHost = true;
            Debug.Log($"[NetworkConnect] Rpc_SetHost: Player {idx} is new Host.");
        }


        [Rpc]
        public static void Rpc_JoinGame(NetworkRunner runner, string data)
        {
            Debug.Log($"[NetworkConnect] ===== Rpc_JoinGame (broadcast) received on Player {Instance.playerIdx} =====");
            Debug.Log($"[NetworkConnect] _isHostMigrating = {Instance._isHostMigrating}");
            Debug.Log($"[NetworkConnect] Data: {data}");
            Debug.Log($"[NetworkConnect] dic_PlayerData.Count BEFORE: {Instance.dic_PlayerData.Count}");
            Debug.Log($"[NetworkConnect] Players BEFORE: [{string.Join(", ", Instance.dic_PlayerData.Keys)}]");

            NetworkBattleData battleData = JsonUtility.FromJson<NetworkBattleData>(data);

            if (!Instance.dic_PlayerData.ContainsKey(battleData.playerIdx))
            {
                Instance.dic_PlayerData.Add(battleData.playerIdx, battleData);
                Debug.Log($"[NetworkConnect] ADDED Player {battleData.playerIdx} to dic_PlayerData");
            }
            else
            {
                Instance.dic_PlayerData[battleData.playerIdx] = battleData;
                Debug.Log($"[NetworkConnect] UPDATED Player {battleData.playerIdx} in dic_PlayerData");
            }

            Debug.Log($"[NetworkConnect] dic_PlayerData.Count AFTER: {Instance.dic_PlayerData.Count}");
            Debug.Log($"[NetworkConnect] Players AFTER: [{string.Join(", ", Instance.dic_PlayerData.Keys)}]");

            foreach (var item in Instance.dic_PlayerData.Values)
                item.isInitialize = false;

            if (Instance.networkBattleStatus == NetworkBattleStatus.LOBBY)
            {

                if (Instance.dic_PlayerData.Count >= Instance.minPlayerCount)
                {
                    if (Instance.ICountMatchingTimeOut != null)
                        Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                    if (Instance.ICountTimeStart != null)
                        Instance.StopCoroutine(Instance.ICountTimeStart);
                    Instance.ICountTimeStart = Instance.StartCoroutine(Instance.CountTimeStart());
                }
                else
                {
                    if (Instance.ICountTimeStart != null)
                        Instance.StopCoroutine(Instance.ICountTimeStart);
                    if (Instance.ICountMatchingTimeOut != null)
                        Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                    Instance.ICountMatchingTimeOut = Instance.StartCoroutine(Instance.CountMatchingTimeOut());
                }

                var popup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                popup.UpdateUserInfo();
            }

            Instance.PushSnapShot();
        }

        [Rpc]
        public static void Rpc_JoinGame(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, string data)
        {
            Debug.Log($"[NetworkConnect] Rpc_JoinGame (targeted). \n{data}");
            NetworkBattleData battleData = JsonUtility.FromJson<NetworkBattleData>(data);
            if (data == null)
                return;
            if (!Instance.dic_PlayerData.ContainsKey(battleData.playerIdx))
                Instance.dic_PlayerData.Add(battleData.playerIdx, battleData);
            else
                Instance.dic_PlayerData[battleData.playerIdx] = battleData;

            foreach (var item in Instance.dic_PlayerData.Values)
                item.isInitialize = false;

            if (Instance.networkBattleStatus == NetworkBattleStatus.LOBBY)
            {

                if (Instance.dic_PlayerData.Count >= Instance.minPlayerCount)
                {
                    if (Instance.ICountMatchingTimeOut != null)
                        Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                    if (Instance.ICountTimeStart != null)
                        Instance.StopCoroutine(Instance.ICountTimeStart);
                    Instance.ICountTimeStart = Instance.StartCoroutine(Instance.CountTimeStart());
                }
                else
                {
                    if (Instance.ICountTimeStart != null)
                        Instance.StopCoroutine(Instance.ICountTimeStart);
                    if (Instance.ICountMatchingTimeOut != null)
                        Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                    Instance.ICountMatchingTimeOut = Instance.StartCoroutine(Instance.CountMatchingTimeOut());
                }

                var popup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                popup.UpdateUserInfo();
            }

            Instance.PushSnapShot();
        }

        Coroutine ICountMatchingTimeOut;
        Coroutine ICountTimeStart;
        IEnumerator CountTimeStart()
        {
            Debug.Log($"[NetworkConnect] ===== CountTimeStart STARTED on Player {playerIdx} =====");
            Debug.Log($"[NetworkConnect] Total player count: {dic_PlayerData.Count}");
            Debug.Log($"[NetworkConnect] Player indices: [{string.Join(", ", dic_PlayerData.Keys)}]");
            float totalElo = 0;
            foreach (var kvp in dic_PlayerData)
            {
                Debug.Log($"[NetworkConnect]   Player {kvp.Key}: {kvp.Value.nickname}, isHost={kvp.Value.isHost}");
                totalElo += kvp.Value.elo;
            }
            avgElo = totalElo / dic_PlayerData.Count;
            Debug.Log($"[NetworkConnect] ============================================ Avg ELO {avgElo}");
            int cd = 30;
            while (cd > 0)
            {
                cd -= 1;
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").text_TimeCount.text = $"{cd / 60:00} : {cd % 60:00}";
                yield return new WaitForSeconds(1);
            }
            Instance.StartCoroutine(Instance.GameStart());
        }
        IEnumerator CountMatchingTimeOut()
        {
            int cd = 30;
            if (isFriendlyMatch)
                cd = 300;
            yield return new WaitForSeconds(cd);
            if (networkBattleStatus != NetworkBattleStatus.LOBBY)
                yield break;
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Battle_Terminated"));
            PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
            StopAllCoroutines();
        }

        [Rpc]
        public static void Rpc_CountStart(NetworkRunner runner)
        {
            if (Instance._countdownStarted)
            {
                Debug.Log("[NetworkConnect] Rpc_CountStart skipped (already started).");
                return;
            }
            Instance._countdownStarted = true;
            Debug.Log("[NetworkConnect] Rpc_CountStart triggered. Starting countdown...");
            GameManager.Instance.CountStart();
        }

        public List<NetworkBattleData> GetSortedDictPlayerData()
        {
            //Sort
            List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            // Separate abnormal exits (with locked ranks) from normal players
            var normalPlayers = data.Where(p => !p.isAbnormalExit).ToList();
            var abnormalExits = data.Where(p => p.isAbnormalExit).ToList();

            // Sort normal players by performance (alive first, then by wave/kills)
            normalPlayers = normalPlayers
                // .OrderBy(p => p.isGameOver)                   // Alive players first (false < true)
                .OrderByDescending(p => p.waveCount)          // Higher wave better
                .ThenByDescending(p => p.monsterBossKilled)   // Then boss kills
                .ThenByDescending(p => p.monsterKilled)       // Then normal kills
                                                              // .ThenBy(p => p.playerIdx)                     // Final tie-breaker: lower playerIdx wins
                .ToList();

            // Sort abnormal exits by their LOCKED rank (assigned at surrender time)
            // Their rank was set when they surrendered = lowest rank among alive at that moment
            abnormalExits = abnormalExits
                .OrderBy(p => p.rank)  // Use the rank assigned at surrender time
                .ToList();

            // Build final sorted list by rank
            var sortedData = new List<NetworkBattleData>();
            int nextNormalIdx = 0;
            int nextAbnormalIdx = 0;
            int currentRank = 1;

            // Merge normal and abnormal players by comparing ranks
            while (nextNormalIdx < normalPlayers.Count || nextAbnormalIdx < abnormalExits.Count)
            {
                // Check if we should place a normal player next
                bool placeNormal = false;

                if (nextNormalIdx < normalPlayers.Count && nextAbnormalIdx >= abnormalExits.Count)
                {
                    // Only normal players left
                    placeNormal = true;
                }
                else if (nextNormalIdx >= normalPlayers.Count && nextAbnormalIdx < abnormalExits.Count)
                {
                    // Only abnormal players left
                    placeNormal = false;
                }
                else
                {
                    // Both available - abnormal player's locked rank tells us where they should go
                    var abnormalPlayer = abnormalExits[nextAbnormalIdx];
                    // If abnormal's locked rank is > current rank, place normal players first
                    placeNormal = (abnormalPlayer.rank > currentRank);
                }

                if (placeNormal)
                {
                    var player = normalPlayers[nextNormalIdx];
                    player.rank = currentRank;
                    sortedData.Add(player);
                    nextNormalIdx++;
                    currentRank++;
                }
                else
                {
                    var player = abnormalExits[nextAbnormalIdx];
                    // Keep their locked rank (don't override)
                    sortedData.Add(player);
                    nextAbnormalIdx++;
                    currentRank = player.rank + 1; // Next rank after this abnormal player
                }
            }

            return sortedData;
        }

        public void ShutDown()
        {
            dic_PlayerData.Clear();
            if (runner != null)
                runner.Shutdown();
            Destroy(gameObject);
        }

        void OnApplicationPause(bool paused)
        {
            if (paused && runner.IsServer)
            {
                PauseSequence();
            }
            else if (!paused)
            {
                OnAbnormalShutdown();
            }
        }
        async void PauseSequence()
        {
            Debug.Log("[Fusion] Host paused — serialize HostMigrationToken before suspension");

            await runner.PushHostMigrationSnapshot();
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
        }

        public void PushSnapShot()
        {
            if (runner && runner.IsServer)
            {
                runner.PushHostMigrationSnapshot();
            }
        }
    }
}

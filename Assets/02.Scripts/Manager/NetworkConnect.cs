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
        public bool isHost;
        public int hostIdx;
        public int metaScore;
        public NetworkRunner runner;
        public Dictionary<int, NetworkBattleData> dic_PlayerData = new(); // player index -> battle data
        public bool isJoin;
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

        [Header("References")]
        public GameObject networkObjectPrefab;
        public NetworkGameManager networkGameManager;

        public MyBattleLeaderboardInfo myCurrentRank;

        // Host migration token storage
        private HostMigrationToken _hostMigrationToken;

        // Ensures countdown RPC triggers only once per session
        private static bool _countdownStarted;

        public bool IsCurrentHost()
        {
            // 1. If I was already host, I’m still host
            if (isHost)
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
            isHost = false;
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
                SessionName = sessionName,
            });

            if (result.Ok)
            {
                Debug.Log("[NetworkConnect] Successfully joined session.");
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
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        /// <summary>
        /// Joins an existing friendly session by name & password.
        /// </summary>
        public async void JoinFriendlySession(string sessionName)
        {
            isHost = false;
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
                Debug.Log("[NetworkConnect] Successfully joined session.");
                roomUuid = runner.SessionInfo.Name;
                networkBattleStatus = NetworkBattleStatus.LOBBY;
                // await NetworkManager.Instance.ReadyForBattle(new ReadyBattlePayload(roomUuid, UserInfoManager.Instance.userId), (response) =>
                // {
                //     sessionId = response.sessionId;
                // }, null);
            }
            else
            {
                Debug.LogError($"[NetworkConnect] Failed to join session: {result.ShutdownReason}");
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").Shutdown();
            }
            StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(false));
        }

        /// <summary>
        /// Creates a new session as host with session properties.
        /// </summary>
        public async void CreateSession()
        {
            isHost = true;
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
            isHost = true;
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
                // await NetworkManager.Instance.ReadyForBattle(new ReadyBattlePayload(roomUuid, UserInfoManager.Instance.userId), (response) =>
                // {
                //     sessionId = response.sessionId;
                // }, null);
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

            Instance.StartCoroutine(HomeScreen.Instance.StartGameSequence(Instance.GameStartSequence));
        }

        public void GameStartSequence()
        {
            Debug.Log("[NetworkConnect] Running GameStartSequence...");

            if (isHost)
            {
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
            Debug.Log($"[NetworkConnect] Player joined: {player.AsIndex}");

            if (player.AsIndex == runner.LocalPlayer.AsIndex)
            {
                Debug.Log("[NetworkConnect] This is the local player joining.");

                NetworkBattleData data = new()
                {
                    profileId = UserInfoManager.Instance.userState.equippedProfileId,
                    isHost = isHost,
                    nickname = UserInfoManager.Instance.nickname,
                    playerIdx = runner.LocalPlayer.AsIndex,
                    decList = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds,
                    isGameOver = false,
                    userId = UserInfoManager.Instance.userId,
                    rankTier = DataManager.Instance.GetRankTierConfig(myCurrentRank.finalRank).description,
                };

                this.playerIdx = data.playerIdx;
                this.nickname = data.nickname;
                this.userId = data.userId;

                Rpc_JoinGame(runner, JsonUtility.ToJson(data));
            }
            else
            {
                Debug.Log($"[NetworkConnect] Syncing player data for new player {player.AsIndex}.");
                NetworkBattleData data = new()
                {
                    profileId = UserInfoManager.Instance.userState.equippedProfileId,
                    isHost = isHost,
                    nickname = UserInfoManager.Instance.nickname,
                    playerIdx = runner.LocalPlayer.AsIndex,
                    decList = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds,
                    userId = UserInfoManager.Instance.userId,
                    isGameOver = false,
                    rankTier = DataManager.Instance.GetRankTierConfig(myCurrentRank.finalRank).description,
                };

                foreach (var value in dic_PlayerData.Values)
                {
                    if (value.playerIdx != player.AsIndex)
                        Rpc_JoinGame(this.runner, player, JsonUtility.ToJson(value));
                }
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"[NetworkConnect] Player left: {player.AsIndex}");

            // Check if the leaving player was the host
            bool wasHost = false;
            if (dic_PlayerData.ContainsKey(player.AsIndex))
            {
                wasHost = dic_PlayerData[player.AsIndex].isHost;
                Debug.Log($"[NetworkConnect] Leaving player was host: {wasHost}");
            }

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

                    // Matchmaking (LOBBY): if the host (or any player) leaves, remove their data
                    // Requirement: host data must be cleared when leaving during matchmaking
                    if (dic_PlayerData.ContainsKey(player.AsIndex))
                    {
                        dic_PlayerData.Remove(player.AsIndex);
                        Debug.Log($"[NetworkConnect] Removed player {player.AsIndex} data from lobby (wasHost={wasHost}).");
                    }

                    PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").UpdateUserInfo();

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
                    break;
                case NetworkBattleStatus.INGAME:
                    if (data != null)
                    {
                        // In battle: mark as abnormal exit and keep data for summary
                        data.isAbnormalExit = true;
                        networkGameManager.Rpc_RequestGameOver(player.AsIndex, data.waveCount, true);
                    }
                    break;
            }
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"[NetworkConnect] Scene load completed, spawning player object. {runner == null} - {networkObjectPrefab == null}");
            GameManager.Instance.gameMode = Game.Defense.GameMode.BATTLE;

            // Only host (state authority) spawns player objects.
            if (isHost)
            {
                NetworkObject networkObject = runner.Spawn(networkObjectPrefab);
                runner.SetPlayerObject(Runner.LocalPlayer, networkObject);
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
                    if (session.Name == roomName && roomPassword == pw && !isPlaying)
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
                    if (!session.IsOpen || session.MaxPlayers == session.PlayerCount || isPlaying)
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
                if (runner != null)
                    runner.Shutdown();
                PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeText(LanguageManager.Instance.GetStringData("UI_Unknown_Error"),
                delegate
                {
                    runner.Shutdown();
                    GameManager.Instance.objectPoolManager.AllClear();
                    SceneLoadManager.Instance.SwitchingScene(2);
                });
            }
        }

        public void InitializeCheck(int idx, string json)
        {
            Debug.Log($"[NetworkConnect] InitializeCheck called for player {idx}.");
            NetworkBattleData data = JsonUtility.FromJson<NetworkBattleData>(json);
            dic_PlayerData[idx].isInitialize = true;
            dic_PlayerData[idx].sessionId = data.sessionId;
            dic_PlayerData[idx].playId = data.playId;

            if (isHost)
            {
                bool allReady = dic_PlayerData.Values.All(p => p.isInitialize);
                if (allReady)
                {
                    Debug.Log("[NetworkConnect] All players ready. Closing session and starting countdown.");
                    runner.SessionInfo.IsOpen = false;
                    Rpc_CountStart(runner);
                }
            }
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
        public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("[NetworkConnect] Connected to server.");
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) => Debug.LogError($"[NetworkConnect] Connection failed: {reason}");
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) => Debug.Log("[NetworkConnect] ConnectRequest received.");
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) => Debug.Log("[NetworkConnect] Custom auth response received.");
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) => Debug.LogError($"[NetworkConnect] Disconnected from server: {reason}");

        /// <summary>
        /// Called on clients that are candidates when host disappears.
        /// We store the token and the elected candidate attempts to resume.
        /// </summary>
        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"[NetworkConnect] Host migration initiated - Current status: {networkBattleStatus} - Token: {hostMigrationToken}");

            // Store the migration token for potential retry scenarios
            _hostMigrationToken = hostMigrationToken;

            // Store current state for restoration
            var currentStatus = networkBattleStatus;
            var currentRoomUuid = roomUuid;
            var currentSessionId = sessionId;

            // Step 2.1: Shutdown the current Runner
            Debug.Log("[NetworkConnect] Shutting down current runner for host migration");
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

            // Step 2.2: Destroy the old runner and create a new one
            Debug.Log("[NetworkConnect] Destroying old runner and creating new one for host migration");
            if (runner != null)
            {
                Destroy(runner.gameObject);
                runner = null;
            }

            GameObject goRunner = new GameObject("NetworkRunner_Migration");
            goRunner.transform.SetParent(transform);
            var newRunner = goRunner.AddComponent<NetworkRunner>();

            // Configure the new runner with the same settings as the original
            newRunner.ProvideInput = true;
            newRunner.AddCallbacks(this);

            // Start the new Runner using the HostMigrationToken
            Debug.Log("[NetworkConnect] Starting new runner with host migration token");
            StartGameResult result = await newRunner.StartGame(new StartGameArgs()
            {
                HostMigrationToken = hostMigrationToken,   // contains all necessary info to restart the Runner
                HostMigrationResume = OnHostMigrationResume, // this will be invoked to resume the simulation
            });

            // Check StartGameResult
            if (result.Ok == false)
            {
                Debug.LogError($"[NetworkConnect] Host migration failed: {result.ShutdownReason}");
                // Handle migration failure based on current status
                HandleMigrationFailure(currentStatus);
            }
            else
            {
                Debug.Log($"[NetworkConnect] Host migration started successfully for status: {currentStatus}");
            }
        }

        /// <summary>
        /// Called by Fusion after StartGame(... HostMigrationResume = OnHostMigrationResume) completes.
        /// Reinitialize non-networked state and mark host flags.
        /// </summary>
        private void OnHostMigrationResume(NetworkRunner runner)
        {
            Debug.Log($"[NetworkConnect] Migration resume callback - Host migration completed for status: {networkBattleStatus}");

            // Update the runner reference to the new one
            this.runner = runner;

            // Update host status based on the new runner
            isHost = runner.IsServer;
            Debug.Log($"[NetworkConnect] Host migration completed. IsHost: {isHost}");

            // Update host status in player data for all players
            if (isHost)
            {
                Debug.Log("[NetworkConnect] This client is now the new host - updating player data");
                // Update our own player data to reflect we're now the host
                if (dic_PlayerData.ContainsKey(playerIdx))
                {
                    dic_PlayerData[playerIdx].isHost = true;
                }
            }
            else
            {
                Debug.Log("[NetworkConnect] This client remains a client after migration");
                // Update our own player data to reflect we're not the host
                if (dic_PlayerData.ContainsKey(playerIdx))
                {
                    dic_PlayerData[playerIdx].isHost = false;
                }
            }

            // Clean up any old host data that might still be in the dictionary
            CleanupOldHostData();

            // Restore network objects from the migration snapshot
            var resumeNetworkObjects = runner.GetResumeSnapshotNetworkObjects();
            Debug.Log($"[NetworkConnect] Restoring {resumeNetworkObjects.Count()} network objects from migration snapshot");

            foreach (var resumeNO in resumeNetworkObjects)
            {
                Debug.Log($"[NetworkConnect] Restoring network object: {resumeNO.name} (ID: {resumeNO.Id})");

                // For this game, we primarily have NetworkGameManager objects
                // The NetworkGameManager doesn't use position/rotation, so we spawn it directly
                runner.Spawn(resumeNO, onBeforeSpawned: (runner, newNO) =>
                {
                    Debug.Log($"[NetworkConnect] Spawning migrated object: {newNO.name}");

                    // Copy the complete state from the resume snapshot
                    newNO.CopyStateFrom(resumeNO);

                    // If this is the NetworkGameManager, update our reference
                    if (newNO.TryGetBehaviour<NetworkGameManager>(out var gameManager))
                    {
                        Debug.Log("[NetworkConnect] Restored NetworkGameManager reference");
                        networkGameManager = gameManager;
                    }
                });
            }

            // Restore UI and game state based on current battle status
            RestoreStateAfterMigration();

            Debug.Log("[NetworkConnect] Host migration resume completed successfully");
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
            isHost = false;
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

        /// <summary>
        /// Restores UI and game state after successful host migration
        /// </summary>
        private void RestoreStateAfterMigration()
        {
            Debug.Log($"[NetworkConnect] Restoring state after migration for status: {networkBattleStatus}");

            switch (networkBattleStatus)
            {
                case NetworkBattleStatus.LOBBY:
                    RestoreLobbyState();
                    break;

                case NetworkBattleStatus.INGAME:
                    RestoreIngameState();
                    break;
            }
        }

        /// <summary>
        /// Restores lobby/matchmaking state after migration
        /// </summary>
        private void RestoreLobbyState()
        {
            Debug.Log("[NetworkConnect] Restoring lobby state after migration");

            // Update matchmaking popup with current player data
            var matchmakingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
            if (matchmakingPopup != null)
            {
                matchmakingPopup.UpdateUserInfo();
            }

            // Show brief notification about host migration if we became the new host
            // if (isHost)
            // {
            //     Debug.Log("[NetworkConnect] Showing host migration notification");
            //     ShowHostMigrationNotification();
            // }

            // Restart countdown if we have enough players
            if (dic_PlayerData.Count >= minPlayerCount)
            {
                if (ICountTimeStart != null)
                    StopCoroutine(ICountTimeStart);
                if (ICountMatchingTimeOut != null)
                    StopCoroutine(ICountMatchingTimeOut);
                ICountTimeStart = StartCoroutine(CountTimeStart());
            }
            else
            {
                if (ICountTimeStart != null)
                    StopCoroutine(ICountTimeStart);
                if (ICountMatchingTimeOut != null)
                    StopCoroutine(ICountMatchingTimeOut);
                ICountMatchingTimeOut = StartCoroutine(CountMatchingTimeOut());
            }
        }

        /// <summary>
        /// Cleans up old host data after migration
        /// </summary>
        private void CleanupOldHostData()
        {
            Debug.Log("[NetworkConnect] Cleaning up old host data after migration");

            // Find and remove any player data where isHost is true but the player is no longer connected
            var keysToRemove = new List<int>();

            foreach (var kvp in dic_PlayerData)
            {
                var playerIdx = kvp.Key;
                var playerData = kvp.Value;

                // If this player data says they're host but we're now the host, remove it
                if (playerData.isHost && isHost && playerIdx != this.playerIdx)
                {
                    Debug.Log($"[NetworkConnect] Removing old host data for player {playerIdx}");
                    keysToRemove.Add(playerIdx);
                }
            }

            // Remove the old host data
            foreach (var key in keysToRemove)
            {
                switch (networkBattleStatus)
                {
                    case NetworkBattleStatus.LOBBY:
                        dic_PlayerData.Remove(key);
                        break;
                    case NetworkBattleStatus.INGAME:
                        dic_PlayerData[key].isAbnormalExit = true;
                        networkGameManager.Rpc_RequestGameOver(key, dic_PlayerData[key].waveCount, true);
                        break;
                }
            }

            Debug.Log($"[NetworkConnect] Cleaned up {keysToRemove.Count} old host entries");
        }

        /// <summary>
        /// Shows a brief notification about host migration
        /// </summary>
        private void ShowHostMigrationNotification()
        {
            var systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            if (systemNoticePopup != null)
            {
                string message = "You are now the host";
                try
                {
                    message = LanguageManager.Instance.GetStringData("UI_You_Are_Now_Host");
                }
                catch
                {
                    Debug.LogWarning("[NetworkConnect] Language key 'UI_You_Are_Now_Host' not found, using fallback");
                }

                // Show a brief notification
                systemNoticePopup.SetNoticeText(message, null);
            }
        }


        /// <summary>
        /// Restores in-game state after migration
        /// </summary>
        private void RestoreIngameState()
        {
            Debug.Log("[NetworkConnect] Restoring in-game state after migration");

            // The game state should be automatically restored through the NetworkGameManager
            // and other network objects that were migrated
            if (networkGameManager != null)
            {
                Debug.Log("[NetworkConnect] NetworkGameManager restored successfully");
            }

            // If we became the new host, we might need to take over certain responsibilities
            if (isHost)
            {
                Debug.Log("[NetworkConnect] New host taking over game management responsibilities");
                // Additional host-specific restoration can be added here if needed
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
            Debug.Log("[NetworkConnect] Rpc_JoinGame (all players).");
            NetworkBattleData battleData = JsonUtility.FromJson<NetworkBattleData>(data);
            if (!Instance.dic_PlayerData.ContainsKey(battleData.playerIdx))
                Instance.dic_PlayerData.Add(battleData.playerIdx, battleData);
            else
                Instance.dic_PlayerData[battleData.playerIdx] = battleData;

            var popup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
            popup.UpdateUserInfo();

            /* if (Instance.dic_PlayerData.Count == runner.SessionInfo.MaxPlayers)
            {
                if (Instance.ICountMatchingTimeOut != null)
                    Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                if (Instance.ICountTimeStart != null)
                    Instance.StopCoroutine(Instance.ICountTimeStart);
                Debug.Log("[NetworkConnect] All players joined, starting game.");
                Instance.StartCoroutine(Instance.GameStart());
            }
            else  */
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

            foreach (var item in Instance.dic_PlayerData.Values)
                item.isInitialize = false;
        }

        [Rpc]
        public static void Rpc_JoinGame(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, string data)
        {
            Debug.Log($"[NetworkConnect] Rpc_JoinGame (targeted). {data}");
            NetworkBattleData battleData = JsonUtility.FromJson<NetworkBattleData>(data);
            if (!Instance.dic_PlayerData.ContainsKey(battleData.playerIdx))
                Instance.dic_PlayerData.Add(battleData.playerIdx, battleData);
            else
                Instance.dic_PlayerData[battleData.playerIdx] = battleData;

            /* if (Instance.dic_PlayerData.Count == runner.SessionInfo.MaxPlayers)
            {
                if (Instance.ICountMatchingTimeOut != null)
                    Instance.StopCoroutine(Instance.ICountMatchingTimeOut);
                if (Instance.ICountTimeStart != null)
                    Instance.StopCoroutine(Instance.ICountTimeStart);
                Debug.Log("[NetworkConnect] All players joined, starting game.");
                Instance.StartCoroutine(Instance.GameStart());
            }
            else  */
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

        Coroutine ICountMatchingTimeOut;
        Coroutine ICountTimeStart;
        IEnumerator CountTimeStart()
        {
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
            if (_countdownStarted)
            {
                Debug.Log("[NetworkConnect] Rpc_CountStart skipped (already started).");
                return;
            }
            _countdownStarted = true;
            Debug.Log("[NetworkConnect] Rpc_CountStart triggered. Starting countdown...");
            GameManager.Instance.CountStart();
        }

        public List<NetworkBattleData> GetSortedDictPlayerData()
        {
            //Sort
            List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            data = data
            // .OrderBy(p => p.isGameOver)              // ✅ put abnormal exits last
            .OrderByDescending(p => p.waveCount)          // ✅ higher wave better
            .ThenByDescending(p => p.monsterBossKilled)  // ✅ then boss kills
            .ThenByDescending(p => p.monsterKilled)      // ✅ then kills
            .ToList();

            for (int i = 0; i < data.Count; i++)
                data[i].rank = i + 1;

            return data;
        }

        public async void ShutDown()
        {
            await runner.Shutdown();
            Destroy(gameObject);
        }
    }
}

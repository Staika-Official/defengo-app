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
        /// Called only by host to check if room is full, then start the game.
        /// </summary>
        public void CheckPlayerCount()
        {
            Debug.Log($"[NetworkConnect] Checking player count... Host={isHost}, " +
                      $"MaxPlayers={runner?.SessionInfo.MaxPlayers}, Current={runner?.SessionInfo.PlayerCount}");

            if (isHost && runner.SessionInfo.MaxPlayers == runner.SessionInfo.PlayerCount)
            {
                Debug.Log("[NetworkConnect] All players joined, starting game...");
                StartCoroutine(GameStart());
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

            Debug.Log($"[NetworkConnect] Connecting to lobby... ~ Is Friendly Match: {isFriendlyMatch}");

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
                Debug.Log("[NetworkConnect] NetworkRunner component added.");
            }
            await Task.Delay(Random.Range(0, 1000));
            await NetworkManager.Instance.GetMyBattleLeaderboard((myRank) => myCurrentRank = myRank, (myRank) => myCurrentRank = myRank);
            await runner.JoinSessionLobby(SessionLobby.Shared);
            Debug.Log("[NetworkConnect] Connected to session lobby.");
        }

        /// <summary>
        /// Joins an existing session by name.
        /// </summary>
        public async void JoinSession(string sessionName)
        {
            isHost = false;
            Debug.Log($"[NetworkConnect] Joining session: {sessionName}");

            if (runner == null)
                runner = gameObject.AddComponent<NetworkRunner>();

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
                runner = gameObject.AddComponent<NetworkRunner>();

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
            };

            if (runner == null)
                runner = gameObject.AddComponent<NetworkRunner>();

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
            };

            if (runner == null)
                runner = gameObject.AddComponent<NetworkRunner>();

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
            Debug.Log("[NetworkConnect] GameStart coroutine started. Waiting 4s...");
            yield return new WaitForSeconds(0.5f);
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

            // NOTE: removed manual host reassignment here to avoid conflict with Fusion Host Migration.
            // Let Fusion fire OnHostMigration and elect the migration candidate.

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
                    if (dic_PlayerData.ContainsKey(player.AsIndex))
                        dic_PlayerData.Remove(player.AsIndex);

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
            if (isFriendlyMatch)
            {

                Debug.Log($"[NetworkConnect] Session list updated, found {sessionList.Count} sessions.");

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
                    if (session.Name == roomName && roomPassword == pw)
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
                Debug.Log($"[NetworkConnect] Session list updated, found {sessionList.Count} sessions.");

                if (sessionList.Count == 0)
                {
                    Debug.Log("[NetworkConnect] No sessions found, creating new one.");
                    CreateSession();
                    return;
                }

                foreach (var session in sessionList)
                {
                    if (!session.IsOpen || session.MaxPlayers == session.PlayerCount)
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
            Debug.Log("[NetworkConnect] Destroyed. Clearing instance.");
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
            Debug.Log("[NetworkConnect] Host migration event.");

            // store token for potential resume
            _hostMigrationToken = hostMigrationToken;

            // pick candidate deterministically: smallest PlayerRef.AsInt (lowest index)
            var ordered = runner.ActivePlayers.OrderBy(p => p.AsIndex).ToList();
            if (ordered.Count == 0)
            {
                Debug.LogWarning("[NetworkConnect] No active players to become candidate for migration.");
                return;
            }

            var candidate = ordered.First();
            if (runner.LocalPlayer == candidate)
            {
                Debug.Log("[NetworkConnect] I am the chosen candidate to resume host. Attempting resume...");

                var result = await runner.StartGame(new StartGameArgs()
                {
                    GameMode = Fusion.GameMode.Host,
                    SessionName = runner.SessionInfo.Name,
                    HostMigrationToken = hostMigrationToken,
                    HostMigrationResume = OnHostMigrationResume,
                });

                if (result.Ok)
                    Debug.Log("[NetworkConnect] Migration resumed and StartGame returned Ok.");
                else
                    Debug.LogError("[NetworkConnect] Migration resume failed: " + result.ShutdownReason);
            }
            else
            {
                Debug.Log("[NetworkConnect] Not the chosen candidate. Waiting for migration to complete on other client.");
            }
        }

        /// <summary>
        /// Called by Fusion after StartGame(... HostMigrationResume = OnHostMigrationResume) completes.
        /// Reinitialize non-networked state and mark host flags.
        /// </summary>
        private void OnHostMigrationResume(NetworkRunner runner)
        {
            Debug.Log("[NetworkConnect] Migration resume callback");

            // If we are server now, mark ourselves as host and inform others via RPC
            isHost = runner.IsServer;
            if (isHost)
            {
                hostIdx = runner.LocalPlayer.AsIndex;
                Debug.Log($"[NetworkConnect] I am new Host after migration. LocalIndex={hostIdx}");

                // mark local player's dic entry if it exists
                if (dic_PlayerData.ContainsKey(runner.LocalPlayer.AsIndex))
                    dic_PlayerData[runner.LocalPlayer.AsIndex].isHost = true;

                // Inform others
                try
                {
                    Rpc_SetHost(runner, runner.LocalPlayer.AsIndex);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("[NetworkConnect] Rpc_SetHost failed in OnHostMigrationResume: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("[NetworkConnect] Migration resume but this client is not server.");
            }

            // Reinitialize UI/managers as needed
            try
            {
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking")?.UpdateUserInfo();
            }
            catch { /* ignore if UI not present */ }
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
    }
}

using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Framework.Network;
using System.Linq;
using Framework.Util;
using Framework.UI;

namespace Framework.Game.Defense
{
    public class NetworkGameManager : NetworkBehaviour, IAfterSpawned
    {
        public int rewardGroupIndex;

        public override void Spawned()
        {
            // Assign reference on ALL peers, not just host
            NetworkConnect.Instance.networkGameManager = this;

            if (NetworkConnect.Instance.IsCurrentHost())
            {
                Debug.Log($"{gameObject.name} HasStateAuthority (Host)");
            }
            else
            {
                Debug.Log($"{gameObject.name} Spawned on Client");
            }
        }

        #region Client → Host Requests

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestInitializeComplete(int playerIdx, string json)
        {
            Debug.Log($"Request initialize");
            if (NetworkConnect.Instance.IsCurrentHost())
                RpcInitializeComplete(playerIdx, json);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestCountStart()
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                RpcCountStart();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_ReachBossWave(roundId, nickname, rewardGroupIndex, bossIdx);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestSpawnBoss(string nickname, float bossHealth)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_SpawnBoss(nickname, bossHealth);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestWaveComplete(int playerId, int roundId, int monsterKilled, int monsterBossKilled)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_WaveComplete(playerId, roundId, monsterKilled);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestGameOver(int playerId, int roundId, bool isAbnormal)
        {
            Debug.Log($"{gameObject.name} {NetworkConnect.Instance.dic_PlayerData[playerId].nickname} id: {playerId} request game over \n ");
            if (NetworkConnect.Instance.IsCurrentHost())
            {
                Rpc_GameOver(playerId, roundId, isAbnormal);
                Rpc_SelectdFieldBossReward(playerId);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestSelectFieldBossReward(int playerId)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_SelectdFieldBossReward(playerId);
        }

        #endregion

        #region Host → All Broadcasts

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcInitializeComplete(int playerIdx, string json)
        {
            Debug.Log($"{gameObject.name} {playerIdx} is Initialize Complete \n ");
            NetworkConnect.Instance.InitializeCheck(playerIdx, json);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcCountStart()
        {
            GameManager.Instance.CountStart();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SummaryBattle()
        {
            if (NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary)
                return;
            var data = NetworkConnect.Instance.GetSortedDictPlayerData();

            var myData = data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx);
            int myActualRank = data.FindIndex(x => x.playerIdx == NetworkConnect.Instance.playerIdx) + 1; // Position in sorted list = actual rank
            Debug.Log($"[NetworkGameManager] My rank from data: {myData.rank}, My actual rank from position: {myActualRank}, My wave: {NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount}");

            // Update the rank in dic_PlayerData to match the actual rank from sorted list
            // This ensures BattleResultTablePopup shows the same rank
            NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].rank = myActualRank;
            NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary = true;

            UIManager.Instance.battleResultPopup.SetResultInfo(myActualRank,
            NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount, GameManager.Instance.monsterSpawner.killedMonsterCount);
            NetworkConnect.Instance.PushSnapShot();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_ReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)
        {
            Debug.Log($"{gameObject.name} Boss Wave Start");
            if (!NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].isGameOver)
            {
                this.rewardGroupIndex = rewardGroupIndex;
                GameManager.Instance.BossWaveSeqeunce(roundId, nickname, bossIdx);
                UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
                UIManager.Instance.ingameStatusMessage.SetMessage($"{roundId} Boss Wave!!", nickname);
            }
            NetworkConnect.Instance.PushSnapShot();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SpawnBoss(string nickname, float bossHealth)
        {
            Debug.Log($"{gameObject.name} Spawn Boss");
            if (nickname != NetworkConnect.Instance.nickname)
            {
                int gameOverCount = NetworkConnect.Instance.dic_PlayerData.Values.Count(p => p.isGameOver);
                GameManager.Instance.SpawnPlasticMonster(NetworkConnect.Instance.dic_PlayerData.Count - gameOverCount, bossHealth);
            }
            NetworkConnect.Instance.PushSnapShot();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_WaveComplete(int playerId, int roundId, int monsterKilled = 0, int monsterBossKilled = 0)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            if (!NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver)
                NetworkConnect.Instance.dic_PlayerData[playerId].waveCount = roundId;
            if (monsterKilled != -1)
                NetworkConnect.Instance.dic_PlayerData[playerId].monsterKilled += monsterKilled;
            if (monsterBossKilled != -1)
                NetworkConnect.Instance.dic_PlayerData[playerId].monsterBossKilled += monsterBossKilled;
            UIManager.Instance.inGameRankPopup.SortPlayerData();
            Debug.Log($"{gameObject.name} Rpc Player Id : {playerId}");
            Debug.Log($"{gameObject.name} Rpc nickname : {nickname}");
            Debug.Log($"{gameObject.name} Rpc Round Id : {roundId}");

            int gameOverCount = NetworkConnect.Instance.dic_PlayerData.Values.Count(p => p.isGameOver);
            int remain = NetworkConnect.Instance.dic_PlayerData.Count - gameOverCount;

            // Check if only one player remains alive
            if (remain == 1 && !NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].isGameOver)
            {
                var data = NetworkConnect.Instance.GetSortedDictPlayerData();
                int myActualRank = data.FindIndex(x => x.playerIdx == NetworkConnect.Instance.playerIdx) + 1;

                if (myActualRank == 1)
                {
                    // Then trigger GameOver to clean up and notify other clients
                    GameManager.Instance.GameOver();

                    // Show result for the last survivor (rank 1)
                    Rpc_SummaryBattle();
                }
            }

            // Also show finalized results for all OTHER dead players
            // Since the last survivor completed a wave, all dead players' ranks are now finalized
            CheckAndShowFinalizedResults();
            NetworkConnect.Instance.PushSnapShot();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public async void Rpc_GameOver(int playerId, int roundId, bool isAbnormal)
        {
            Debug.Log($"{gameObject.name} {NetworkConnect.Instance.dic_PlayerData[playerId].nickname} id: {playerId} game over at round {roundId} \n ");
            if (NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver)
                return;

            int maxCount = NetworkConnect.Instance.dic_PlayerData.Count;
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver = true;
            NetworkConnect.Instance.dic_PlayerData[playerId].isAbnormalExit = isAbnormal;
            NetworkConnect.Instance.dic_PlayerData[playerId].selectedFieldBossReward = true;
            NetworkConnect.Instance.FieldBossRewardCheck(playerId);

            // For abnormal exits (surrender/disconnect), assign the lowest rank among currently alive players
            if (isAbnormal)
            {
                // Count how many players are currently NOT game over (still alive, EXCLUDING the surrendering player)
                int aliveCount = NetworkConnect.Instance.dic_PlayerData.Values.Count(p => !p.isGameOver);
                // Assign rank = aliveCount + 1 (lowest rank among ALL alive players INCLUDING the surrendering player)
                // Example: 2 others alive + 1 surrendering = rank 3
                NetworkConnect.Instance.dic_PlayerData[playerId].rank = aliveCount + 1;
                Debug.Log($"[NetworkGameManager] Player {playerId} surrendered, assigned rank {aliveCount + 1} (lowest among {aliveCount + 1} alive players before surrender)");

                NetworkConnect.Instance.InitializeCheck(playerId, JsonUtility.ToJson(NetworkConnect.Instance.dic_PlayerData[playerId]));
            }

            if (NetworkConnect.Instance.IsCurrentHost() && isAbnormal)
            {
                SurrenderBattlePayload payload = new SurrenderBattlePayload()
                {
                    sessionId = NetworkConnect.Instance.sessionId,
                    leavePlayId = NetworkConnect.Instance.dic_PlayerData[playerId].playId,
                    leaveUserId = NetworkConnect.Instance.dic_PlayerData[playerId].userId
                };
                await NetworkManager.Instance.SurrenderBattle(payload, null, async () =>
                {
                    await NetworkManager.Instance.SurrenderBattle(payload, null, null);
                });
            }
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"GameOver !!", nickname);

            var data = NetworkConnect.Instance.GetSortedDictPlayerData();

            int gameOverCount = NetworkConnect.Instance.dic_PlayerData.Values.Count(p => p.isGameOver);
            int remain = NetworkConnect.Instance.dic_PlayerData.Count - gameOverCount;

            Debug.Log($"{gameObject.name} {nickname} is GameOver ~ remain player: {remain} (total: {NetworkConnect.Instance.dic_PlayerData.Count} / died: {gameOverCount}) \n data : {JsonUtility.ToJson(data)}");

            // Check if we should show results for ANY player whose rank is now finalized
            CheckAndShowFinalizedResults();
            NetworkConnect.Instance.PushSnapShot();
        }

        /// <summary>
        /// Checks all dead players to see if their rank is finalized and shows results if ready.
        /// A player's rank is finalized when no alive players have worse performance.
        /// </summary>
        private void CheckAndShowFinalizedResults()
        {
            var data = NetworkConnect.Instance.GetSortedDictPlayerData();

            // Find all dead players who haven't shown summary yet
            var deadPlayers = NetworkConnect.Instance.dic_PlayerData.Values
                .Where(p => p.isGameOver)
                .ToList();

            foreach (var deadPlayer in deadPlayers)
            {
                // Check if this dead player's rank is finalized
                bool rankFinalized = IsRankFinalized(deadPlayer, data);

                if (rankFinalized && deadPlayer.playerIdx == NetworkConnect.Instance.playerIdx
                && !NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary)
                {
                    // Show result for local player
                    int myActualRank = data.FindIndex(x => x.playerIdx == NetworkConnect.Instance.playerIdx) + 1;

                    NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].rank = myActualRank;
                    NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary = true;

                    Debug.Log($"[NetworkGameManager] Showing finalized result for local player - Rank: {myActualRank}");

                    UIManager.Instance.battleResultPopup.SetResultInfo(myActualRank,
                        NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount,
                        GameManager.Instance.monsterSpawner.killedMonsterCount);
                }
            }
        }

        /// <summary>
        /// Determines if a dead player's rank is finalized.
        /// Rank is finalized when all alive players have better or equal performance.
        /// </summary>
        private bool IsRankFinalized(NetworkBattleData deadPlayer, List<NetworkBattleData> sortedData)
        {
            int deadIdx = sortedData.FindIndex(x => x.playerIdx == deadPlayer.playerIdx);
            for (int i = deadIdx; i < sortedData.Count; i++)
            {
                if (!sortedData[i].isGameOver)
                    return false;
            }
            return true;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SelectdFieldBossReward(int playerId)
        {
            NetworkConnect.Instance.FieldBossRewardCheck(playerId);
        }

        #endregion

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }

        public void AfterSpawned()
        {
            Debug.Log($"{gameObject.name} After Spawned");
        }
    }
}

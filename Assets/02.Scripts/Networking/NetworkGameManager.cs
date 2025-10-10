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
        public int gameOverPlayerCount = 0;
        public int rank = 0;

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
                Rpc_GameOver(playerId, roundId, isAbnormal);
            Rpc_RequestSelectFieldBossReward(playerId);
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
        public void RpcSummaryBattle()
        {
            // Don't show summary if already shown (early summary)
            if (NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary)
            {
                Debug.Log("[NetworkGameManager] Summary already shown, skipping RpcSummaryBattle");
                return;
            }

            var data = NetworkConnect.Instance.GetSortedDictPlayerData();

            NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary = true;
            UIManager.Instance.battleResultPopup.SetResultInfo(data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx).rank,
            NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount, GameManager.Instance.monsterSpawner.killedMonsterCount);
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
                Rpc_WaveComplete(NetworkConnect.Instance.playerIdx, roundId);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SpawnBoss(string nickname, float bossHealth)
        {
            Debug.Log($"{gameObject.name} Spawn Boss");
            if (nickname != NetworkConnect.Instance.nickname)
                GameManager.Instance.SpawnPlasticMonster(NetworkConnect.Instance.dic_PlayerData.Count - gameOverPlayerCount, bossHealth);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_WaveComplete(int playerId, int roundId, int monsterKilled = -1, int monsterBossKilled = -1)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].waveCount = roundId;
            if (monsterKilled != -1)
                NetworkConnect.Instance.dic_PlayerData[playerId].monsterKilled += monsterKilled;
            if (monsterBossKilled != -1)
                NetworkConnect.Instance.dic_PlayerData[playerId].monsterBossKilled += monsterBossKilled;
            UIManager.Instance.inGameRankPopup.SortPlayerData();
            Debug.Log($"{gameObject.name} Rpc Player Id : {playerId}");
            Debug.Log($"{gameObject.name} Rpc nickname : {nickname}");
            Debug.Log($"{gameObject.name} Rpc Round Id : {roundId}");

            int remain = NetworkConnect.Instance.dic_PlayerData.Count - gameOverPlayerCount;
            if (remain == 1)
            {
                var data = NetworkConnect.Instance.GetSortedDictPlayerData();
                var my = data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx);
                if (!my.isGameOver && my.rank == 1)
                {
                    GameManager.Instance.GameOver();
                    if (!NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].isGameOver)
                    {
                        var dat = NetworkConnect.Instance.GetSortedDictPlayerData();

                        UIManager.Instance.battleResultPopup.SetResultInfo(dat.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx).rank,
                        NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount, GameManager.Instance.monsterSpawner.killedMonsterCount);
                        NetworkConnect.Instance.ShutDown();
                    }
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public async void Rpc_GameOver(int playerId, int roundId, bool isAbnormal)
        {
            Debug.Log($"{gameObject.name} {NetworkConnect.Instance.dic_PlayerData[playerId].nickname} id: {playerId} game over \n ");
            if (NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver)
                return;

            int maxCount = NetworkConnect.Instance.dic_PlayerData.Count;
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver = true;
            NetworkConnect.Instance.dic_PlayerData[playerId].isAbnormalExit = isAbnormal;
            gameOverPlayerCount += 1;

            if (NetworkConnect.Instance.IsCurrentHost() && isAbnormal)
            {
                SurrenderBattlePayload payload = new SurrenderBattlePayload()
                {
                    sessionId = NetworkConnect.Instance.dic_PlayerData[playerId].sessionId,
                    leavePlayId = NetworkConnect.Instance.dic_PlayerData[playerId].playId,
                    leaveUserId = NetworkConnect.Instance.dic_PlayerData[playerId].userId
                };
                await NetworkManager.Instance.SurrenderBattle(payload, null, null);
            }
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"GameOver !!", nickname);

            var data = NetworkConnect.Instance.GetSortedDictPlayerData();

            int remain = NetworkConnect.Instance.dic_PlayerData.Count - gameOverPlayerCount;

            Debug.Log($"{gameObject.name} {nickname} is GameOver ~ remain player: {remain} (total: {NetworkConnect.Instance.dic_PlayerData.Count} / died: {gameOverPlayerCount}) \n data : {JsonUtility.ToJson(data)}");

            // Check if local player can show summary early
            var myData = data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx);
            if (myData != null && myData.isGameOver && !NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary)
            {
                // Check if my rank is confirmed (can't be beaten by alive players)
                bool rankConfirmed = CheckIfRankConfirmed(myData, data);

                if (rankConfirmed)
                {
                    Debug.Log($"[NetworkGameManager] Player {myData.playerIdx} rank confirmed at {myData.rank}, showing early summary");
                    NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary = true;
                    UIManager.Instance.battleResultPopup.SetResultInfo(myData.rank,
                        NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount,
                        GameManager.Instance.monsterSpawner.killedMonsterCount);
                }
            }

            if (remain == 1)
            {
                var my = data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx);
                if (!my.isGameOver && my.rank == 1)
                {
                    GameManager.Instance.GameOver();
                    if (!NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].isGameOver)
                    {
                        var dat = NetworkConnect.Instance.GetSortedDictPlayerData();

                        NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].hasShownSummary = true;
                        UIManager.Instance.battleResultPopup.SetResultInfo(dat.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx).rank,
                        NetworkConnect.Instance.dic_PlayerData[NetworkConnect.Instance.playerIdx].waveCount, GameManager.Instance.monsterSpawner.killedMonsterCount);
                        NetworkConnect.Instance.ShutDown();
                    }
                }
            }

            if (gameOverPlayerCount == maxCount || remain == 0)
                RpcSummaryBattle();
        }

        /// <summary>
        /// Check if a dead player's rank is confirmed (can't change anymore)
        /// Rank is confirmed when even the WORST alive player has better stats than me
        /// This means all alive players are ahead of me, so my position among dead players is locked
        /// </summary>
        private bool CheckIfRankConfirmed(NetworkBattleData myData, List<NetworkBattleData> sortedData)
        {
            if (!myData.isGameOver)
                return false;

            // Get all alive players
            var alivePlayers = sortedData.Where(p => !p.isGameOver && !p.isAbnormalExit).ToList();

            if (alivePlayers.Count == 0)
                return true; // All others are dead, rank is confirmed

            // Find the WORST alive player (last in sorted list of alive players)
            var worstAlive = alivePlayers.Last();

            // My rank is confirmed if even the worst alive player has better stats than me
            // This means ALL alive players are currently ahead of me
            // So my final rank position is locked (can't get better or worse)
            // Compare: wave count (most important), then boss kills, then normal kills

            if (worstAlive.waveCount > myData.waveCount)
            {
                // Even worst alive player is ahead in waves, rank confirmed
                return true;
            }
            else if (worstAlive.waveCount == myData.waveCount)
            {
                // Same wave, check boss kills
                if (worstAlive.monsterBossKilled > myData.monsterBossKilled)
                    return true;
                else if (worstAlive.monsterBossKilled == myData.monsterBossKilled)
                {
                    // Same boss kills, check normal kills
                    if (worstAlive.monsterKilled > myData.monsterKilled)
                        return true;
                }
            }

            // At least one alive player is behind me, so they could die and affect my rank
            return false;
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

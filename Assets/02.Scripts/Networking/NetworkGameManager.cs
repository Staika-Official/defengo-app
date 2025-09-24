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
        [Networked]
        public int Wave { get; private set; }

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
        public void Rpc_RequestInitializeComplete(int playerIdx)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                RpcInitializeComplete(playerIdx);
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
        public void Rpc_RequestWaveComplete(int playerId, int roundId, int monsterKilled)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_WaveComplete(playerId, roundId, monsterKilled);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_RequestGameOver(int playerId, int roundId)
        {
            if (NetworkConnect.Instance.IsCurrentHost())
                Rpc_GameOver(playerId, roundId);
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
        public void RpcInitializeComplete(int playerIdx)
        {
            Debug.Log($"{gameObject.name} {playerIdx} is Initialize Complete");
            NetworkConnect.Instance.InitializeCheck(playerIdx);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcCountStart()
        {
            GameManager.Instance.CountStart();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_ReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)
        {
            Debug.Log($"{gameObject.name} Boss Wave Start");
            this.rewardGroupIndex = rewardGroupIndex;
            GameManager.Instance.BossWaveSeqeunce(roundId, nickname, bossIdx);
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"{roundId} Boss Wave!!", nickname);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SpawnBoss(string nickname, float bossHealth)
        {
            Debug.Log($"{gameObject.name} Spawn Boss");
            if (nickname != NetworkConnect.Instance.nickname)
                GameManager.Instance.SpawnPlasticMonster(NetworkConnect.Instance.dic_PlayerData.Count - gameOverPlayerCount, bossHealth);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_WaveComplete(int playerId, int roundId, int monsterKilled)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].waveCount = roundId;
            NetworkConnect.Instance.dic_PlayerData[playerId].monsterKilled = monsterKilled;
            UIManager.Instance.inGameRankPopup.SortPlayerData();
            Debug.Log($"{gameObject.name} Rpc Player Id : {playerId}");
            Debug.Log($"{gameObject.name} Rpc nickname : {nickname}");
            Debug.Log($"{gameObject.name} Rpc Round Id : {roundId}");
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_GameOver(int playerId, int roundId)
        {
            int maxCount = NetworkConnect.Instance.dic_PlayerData.Count;
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver = true;
            NetworkConnect.Instance.dic_PlayerData[playerId].rank = maxCount - gameOverPlayerCount;
            gameOverPlayerCount += 1;

            Debug.Log($"{gameObject.name} {nickname} is GameOver");
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"GameOver !!", nickname);

            if (playerId != NetworkConnect.Instance.playerIdx)
                SetRank();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SelectdFieldBossReward(int playerId)
        {
            NetworkConnect.Instance.FieldBossRewardCheck(playerId);
        }

        #endregion

        public void SetRank()
        {
            int maxCount = NetworkConnect.Instance.dic_PlayerData.Count;
            rank = maxCount - gameOverPlayerCount;

            Debug.Log($"{gameObject.name} maxCount : {maxCount}");
            Debug.Log($"{gameObject.name} gameOverCount : {gameOverPlayerCount}");

            if (rank == 1)
            {
                Debug.Log($"{gameObject.name} Player Win");
                GameManager.Instance.GameOver();
            }
        }

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

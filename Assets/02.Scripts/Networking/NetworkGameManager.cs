using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Framework.Network;
using System.Security.Cryptography;
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
            if (Object.HasStateAuthority)
            {
                Debug.Log($"{gameObject.name} HasStateAuthority");
                NetworkConnect.Instance.networkGameManager = this;
            }
        }
        #region Network
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcInitializeComplete(int playerIdx)
        {
            Debug.Log($"{gameObject.name} {playerIdx} is Initialize Complete");
            NetworkConnect.Instance.InitializeCheck(playerIdx);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcCountStart()
        {
            GameManager.Instance.CountStart();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_ReachBossWave(int roundId, string nickname, int rewardGroupIndex, int bossIdx)
        {
            Debug.Log($"{gameObject.name} Boss Wave Start");
            Debug.Log($"{gameObject.name} Round Id : " + roundId);
            Debug.Log($"{gameObject.name} NickName : " + nickname);
            Debug.Log($"{gameObject.name} Current Game State: " + GameManager.Instance.gameState);
            Debug.Log($"{gameObject.name} reward Group Index : " + rewardGroupIndex);
            this.rewardGroupIndex = rewardGroupIndex;
            GameManager.Instance.BossWaveSeqeunce(roundId, nickname, bossIdx);
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"{roundId} Boss Wave!!", nickname);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_SpawnBoss(string nickname, float bossHealth)
        {
            Debug.Log($"{gameObject.name} Spawn Boss");
            Debug.Log($"{gameObject.name} NickName : " + nickname);
            Debug.Log($"{gameObject.name} Current Game State: " + GameManager.Instance.gameState);
            Debug.Log($"{gameObject.name} reward Group Index : " + rewardGroupIndex);
            if (nickname != NetworkConnect.Instance.nickname)
                GameManager.Instance.SpawnPlasticMonster(NetworkConnect.Instance.dic_PlayerData.Count - gameOverPlayerCount, bossHealth);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_WaveComplete(int playerId, int roundId, int monsterKilled)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            // UIManager.Instance.ingameStatusMessage.SetMessage($"{nickname} : {roundId} Wave!!");
            NetworkConnect.Instance.dic_PlayerData[playerId].waveCount = roundId;
            NetworkConnect.Instance.dic_PlayerData[playerId].monsterKilled = monsterKilled;
            UIManager.Instance.inGameRankPopup.SortPlayerData();
            Debug.Log($"{gameObject.name} Rpc Player Id :" + playerId);
            Debug.Log($"{gameObject.name} Rpc nickname :" + nickname);
            Debug.Log($"{gameObject.name} Rpc Round Id :" + roundId);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
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
            {
                SetRank();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_SelectdFieldBossReward(int playerId)
        {
            NetworkConnect.Instance.FieldBossRewardCheck(playerId);
        }

        public void SetRank()
        {
            int maxCount = NetworkConnect.Instance.dic_PlayerData.Count;
            Debug.Log($"{gameObject.name} maxCount : " + maxCount);
            Debug.Log($"{gameObject.name} gameOverCount : " + gameOverPlayerCount);

            rank = maxCount - gameOverPlayerCount;
            if (maxCount - gameOverPlayerCount == 1)
            {
                Debug.Log($"{gameObject.name} Player Win");
                GameManager.Instance.GameOver();
            }
            // List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            // data = data
            // .OrderByDescending(x => x.waveCount)
            // .ThenByDescending(x => x.monsterKilled)
            // .ToList();

            // for (int idx = 0; idx < data.Count; idx++)
            // {
            //     data[idx].rank = idx + 1;
            // }

            // rank = data.Find(x => x.userId == NetworkConnect.Instance.userId).rank;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }

        public void AfterSpawned()
        {
            Debug.Log($"{gameObject.name} After Spawned");
            // string nickname = NetworkConnect.Instance.dic_PlayerData[Object.StateAuthority.AsIndex].nickname;
        }
        #endregion
    }
}

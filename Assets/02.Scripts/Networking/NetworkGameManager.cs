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

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                NetworkConnect.Instance.networkGameManager = this;
            }
        }
        #region Network
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcInitializeComplete(int playerIdx)
        {
            Debug.Log($"{playerIdx} is Initialize Complete");
            NetworkConnect.Instance.InitializeCheck(playerIdx);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcCountStart()
        {
            GameManager.Instance.CountStart();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_ReachBossWave(int roundId, string nickname, int rewardGroupIndex)
        {
            Debug.Log("Boss Wave Start");
            Debug.Log("Round Id : " + roundId);
            Debug.Log("NickName : " + nickname);
            Debug.Log("Current Game State: " + GameManager.Instance.gameState);
            Debug.Log("reward Group Index : " + rewardGroupIndex);
            this.rewardGroupIndex = rewardGroupIndex;
            GameManager.Instance.BossWaveSeqeunce(roundId, nickname);
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"{roundId} Boss Wave!!", nickname);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_WaveComplete(int playerId, int roundId)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            // UIManager.Instance.ingameStatusMessage.SetMessage($"{nickname} : {roundId} Wave!!");
            NetworkConnect.Instance.dic_PlayerData[playerId].waveCount = roundId;
            UIManager.Instance.inGameRankPopup.SortPlayerData();
            Debug.Log("Rpc Player Id :" + playerId);
            Debug.Log("Rpc nickname :" + nickname);
            Debug.Log("Rpc Round Id :" + roundId);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_GameOver(int playerId, int roundId)
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[playerId].nickname;
            NetworkConnect.Instance.dic_PlayerData[playerId].isGameOver = true;
            Debug.Log($"{nickname} is GameOver");
            UIManager.Instance.ingameStatusMessage.gameObject.SetActive(true);
            UIManager.Instance.ingameStatusMessage.SetMessage($"GameOver !!", nickname);

            if (playerId != NetworkConnect.Instance.playerIdx)
            {
                SetRank();
            }

        }

        public void SetRank()
        {
            int gameOverCount = 0;
            int maxCount = 0;
            foreach (var item in NetworkConnect.Instance.dic_PlayerData)
            {
                maxCount++;
                if (item.Value.isGameOver) gameOverCount++;
            }
            Debug.Log("maxCount : " + maxCount);
            Debug.Log("gameOverCount : " + gameOverCount);

            if (maxCount - gameOverCount == 1)
            {
                Debug.Log("Player Win");
                GameManager.Instance.GameOver();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }

        public void AfterSpawned()
        {
            string nickname = NetworkConnect.Instance.dic_PlayerData[Object.StateAuthority.AsIndex].nickname;
            Debug.Log($"{nickname} object Spawned");
        }
        #endregion
    }
}

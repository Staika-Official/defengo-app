using System;
using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.UI;
using Fusion;
using UnityEngine;

namespace Framework.Network
{
    public class NetworkUserInfo : NetworkBehaviour, IAfterSpawned, IPlayerJoined, IPlayerLeft
    {
        // Start is called before the first frame update
        [Networked]
        [Capacity(6)]
        public NetworkArray<int> DecList { get; }
        [Networked]
        public bool IsHost { get; private set; }

        [Networked]
        public string Nickname { get; private set; }

        [Networked]
        public int ProfileId { get; private set; }

        [Networked]
        public int PlayerIdx { get; private set; }

        //public PlayerRef playerRef;

        public void AfterSpawned()
        {
            Debug.Log($"After Spawned {Nickname} Join Game");

            // MatchMakingPopup matchMakingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
            //matchMakingPopup.dic_PlayerData.Add(Object.InputAuthority, this);
            //NetworkConnect.Instance.dic_PlayerData.Add(Object.InputAuthority, this);
            // matchMakingPopup.UpdateUserInfo();

            // NetworkConnect.Instance.CheckPlayerCount();
            // if(NetworkConnect.Instance.runner.SessionInfo.MaxPlayers == NetworkConnect.Instance.runner.SessionInfo.PlayerCount && IsHost)
            // {
            //     Debug.Log("Max Player");
            //     RPC_GameStart();
            // }
        }

        public override void Spawned()
        {
            Debug.Log("Spawned");

            // if (Object.HasStateAuthority)
            // {
            //     Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();
            //     for (int i = 0; i < slot.slotCharacterIds.Length; i++)
            //     {
            //         DecList.Set(i, slot.slotCharacterIds[i]);
            //     }
            //     IsHost = NetworkConnect.Instance.isHost;
            //     ProfileId = UserInfoManager.Instance.userState.equippedProfileId;
            //     Nickname = UserInfoManager.Instance.nickname;
            //this.playerRef = playerRef;
            // }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Debug.Log("Despawned");
            // MatchMakingPopup matchMakingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
            //NetworkConnect.Instance.dic_PlayerData.Remove(Object.InputAuthority);
            // matchMakingPopup.UpdateUserInfo();
        }

        public void PlayerJoined(PlayerRef player)
        {

        }

        public void PlayerLeft(PlayerRef player)
        {

        }
    }
}

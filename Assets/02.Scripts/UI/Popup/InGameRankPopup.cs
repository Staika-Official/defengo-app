using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Network;
using System.Linq;
using System;

namespace Framework.UI
{
    public class InGameRankPopup : PopupTemplate
    {
        public InGameRankItem[] inGameRankItems;
        public override void ActivePopup()
        {
            PopUpSequence(true);
            SortPlayerData();
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                PopUpSequence(false);
            });

            List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            for (int idx = 0; idx < inGameRankItems.Length; idx++)
            {
                inGameRankItems[idx].LockInitialize();
                // if(idx < data.Count)
                // {
                //     inGameRankItems[idx].Initialize(data[idx]);
                // }
                // else
                // {
                //     inGameRankItems[idx].LockInitialize();
                // }
            }
        }

        public void SortPlayerData()
        {
            List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            data = data
            .OrderByDescending(x => x.waveCount)
            .ThenByDescending(x => x.monsterKilled)
            .ToList();

            for (int idx = 0; idx < data.Count; idx++)
            {
                data[idx].rank = idx + 1;
                inGameRankItems[idx].Initialize(data[idx]);
                inGameRankItems[idx].SetWaveCount(data[idx].waveCount);
            }
        }
    }
}
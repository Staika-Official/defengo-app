using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Game.Defense;
using Framework.Network;

namespace Framework.UI
{
    public class BattleResultPopup : PopupTemplate
    {
        public TextMeshProUGUI text_rank;
        public TextMeshProUGUI text_wave;
        public TextMeshProUGUI text_monsterKilled;
        public GameObject winObj;

        public Dictionary<int, NetworkBattleData> dic_PlayerData = null;
        public int playerIdx;
        public MyBattleLeaderboardInfo currentRank;
        public bool isFriendlyMatch;

        public void SetResultInfo(int rank, int wave, int monsterKilled)
        {
            text_rank.text = $"{rank}{GetRankSuffix(rank)}";
            text_wave.text = $"{wave}";
            text_monsterKilled.text = $"{monsterKilled}";
            winObj.SetActive(rank == 1);
            this.dic_PlayerData = new(NetworkConnect.Instance.dic_PlayerData);
            this.playerIdx = NetworkConnect.Instance.playerIdx;
            currentRank = NetworkConnect.Instance.myCurrentRank;
            isFriendlyMatch = NetworkConnect.Instance.isFriendlyMatch;

            ActivePopup();
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            UIManager.Instance.battleResultTablePopup.ActivePopup();
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }

        string GetRankSuffix(int rank)
        {
            switch (rank)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        public List<NetworkBattleData> GetSortedDictPlayerData()
        {
            // Use the authoritative sorting logic from NetworkConnect to avoid code duplication
            return NetworkConnect.Instance.GetSortedDictPlayerData();
        }
    }
}
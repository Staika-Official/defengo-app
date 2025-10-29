using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            // Create a DEEP COPY of dic_PlayerData to avoid shared references
            // This ensures the snapshot is not affected by later modifications
            this.dic_PlayerData = new Dictionary<int, NetworkBattleData>();
            foreach (var kvp in NetworkConnect.Instance.dic_PlayerData)
            {
                var original = kvp.Value;
                var copy = new NetworkBattleData
                {
                    playerIdx = original.playerIdx,
                    nickname = original.nickname,
                    decList = original.decList != null ? (int[])original.decList.Clone() : null,
                    profileId = original.profileId,
                    isHost = original.isHost,
                    isInitialize = original.isInitialize,
                    tier = original.tier,
                    elo = original.elo,
                    waveCount = original.waveCount,
                    isGameOver = original.isGameOver,
                    isAbnormalExit = original.isAbnormalExit,
                    rank = original.rank,
                    userId = original.userId,
                    playId = original.playId,
                    sessionId = original.sessionId,
                    monsterKilled = original.monsterKilled,
                    monsterBossKilled = original.monsterBossKilled,
                    selectedFieldBossReward = original.selectedFieldBossReward,
                    rankTier = original.rankTier,
                    hasShownSummary = original.hasShownSummary
                };
                this.dic_PlayerData[kvp.Key] = copy;
            }

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
            // Use the snapshot that was captured when SetResultInfo was called
            // This ensures BattleResultTablePopup shows the same data as BattleResultPopup
            if (this.dic_PlayerData == null)
            {
                // Fallback to live data if no snapshot available
                return NetworkConnect.Instance.GetSortedDictPlayerData();
            }

            // Simply return the snapshot data sorted by the rank that was already assigned
            // DO NOT modify the rank field - it was already set correctly before the popup was shown
            List<NetworkBattleData> data = dic_PlayerData.Values.ToList();

            // Sort by the rank that was already assigned (not by performance)
            // This preserves the exact ranking that was shown in BattleResultPopup
            return data.OrderBy(p => p.rank == 0 ? int.MaxValue : p.rank).ToList();
        }
    }
}
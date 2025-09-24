using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI
{
    public class StartUpSingleModePopup : PopupTemplate
    {
        public ButtonComponent button_Play;
        public TextMeshProUGUI text_Rank;
        public TextMeshProUGUI text_Week;

        public override void ActivePopup()
        {
            PopUpSequence(true);
            LoadRankData();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });
            button_Play.onPointerUp += () =>
            {
                button_Play.onPointerUp = null;
                SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                HomeScreen.Instance.RequestStartGame();
            };
        }

        public async void LoadRankData()
        {
            await NetworkManager.Instance.GetLeaderBoardBestWaves(LeaderBoardType.WAVE_WEEKLY, 0,
            (json) =>
            {
                LeaderBoardRankInfoList data = JsonUtility.FromJson<LeaderBoardRankInfoList>(json);
                text_Week.text = $"Week {data.roundId}";
            }, null);

            await NetworkManager.Instance.GetMyLeaderBoardRankInfo(LeaderBoardType.WAVE_WEEKLY, 0,
            (json) =>
            {
                MyLeaderBoardRankData myRankData = JsonUtility.FromJson<MyLeaderBoardRankData>(json);
                text_Rank.text = $"{myRankData.rank}";
            }, null);
        }
    }
}

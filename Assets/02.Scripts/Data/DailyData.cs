using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Sound;
using System;

namespace Framework.UI
{
    public class DailyData : MonoBehaviour
    {
        public int dayValue;
        public TextMeshProUGUI text_Day;
        public TextMeshProUGUI text_RewardValue;
        public Button button_Daily;
        public string dateInfo;
        public GameObject today;
        public GameObject selected_day;
        

        public void Initialize()
        {
            button_Daily.onClick.AddListener(() => OnClick_DailyData());
            //DateTime currentDate = RankingScreen.Instance.popUp_Calendar.currentDateTime;

            //DateTime day = new(currentDate.Year, currentDate.Month, dayValue);
            //dateInfo = day.ToString("yyyy-MM-dd");
        }

        public void OnClick_DailyData()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            //DateTime currentDate = RankingScreen.Instance.popUp_Calendar.currentDateTime;

            //DateTime day = new(currentDate.Year, currentDate.Month, dayValue);
            //string now = day.ToString("yyyy-MM-dd");
            //RankingScreen.Instance.GetRankingData(dateInfo);
        }

        public void SetDailyData(string day, int rewardValue)
        {
            text_Day.text = day;
            if(rewardValue <= 0)
            {
                text_RewardValue.text = "";
            }
            else
            {
                text_RewardValue.text = $"<color=#0660B8>+{rewardValue}</color>";
            }
        }
    }
}

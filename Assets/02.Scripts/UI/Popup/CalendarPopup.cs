using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Framework.Sound;
using Framework.Network;
using SimpleJSON;

namespace Framework.UI
{
    public class CalendarPopup : PopupTemplate
    {
        public int monthIdx;
        public DailyData[] dailyDatas;
        public TextMeshProUGUI text_CurrentMonth;
        public DateTime currentDateTime;
        public string todayInfo;

        public Button button_PrevMonth;
        public Button button_NextMonth;

        public async void SetCalendarInfo(DateTime dateTime, bool isPrevOrNext)
        {
            string value = dateTime.ToString("yyyy-MM");

            string data = await NetworkManager.Instance.GetUserMonthlyTikHistory(value);
            JSONNode tikInfo = JSONNode.Parse(data);

            DateTime firstDay = dateTime.AddDays(1 - dateTime.Day);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
            int day = (int)firstDay.DayOfWeek;
            int monthLength = lastDay.Day;

            text_CurrentMonth.text = $"{firstDay.Year} / {firstDay.Month}";

            for (int i = 0; i < dailyDatas.Length; i++)
            {
                dailyDatas[i].text_Day.text = "";
                dailyDatas[i].text_RewardValue.text = "";

                dailyDatas[i].today.SetActive(false);
                dailyDatas[i].selected_day.SetActive(false);
            }

            int date = 1;
            for (int i = day; i < monthLength + day; i++)
            {
                DateTime temp = new(dateTime.Year, dateTime.Month, date);
                dailyDatas[i].dayValue = date;
                dailyDatas[i].dateInfo = temp.ToString("yyyy-MM-dd");
                dailyDatas[i].text_Day.text = date.ToString();
                date++;

                bool isTodayActive = todayInfo == dailyDatas[i].dateInfo;
                if (isTodayActive)
                {
                    dailyDatas[i].today.SetActive(isTodayActive);
                }

                if (!isPrevOrNext)
                {
                    bool isSelectedDayActive = dateTime.ToString("yyyy-MM-dd") == dailyDatas[i].dateInfo;
                    if (isSelectedDayActive)
                    {
                        dailyDatas[i].selected_day.SetActive(isSelectedDayActive && !isTodayActive);
                    }
                }
            }

            for (int i = 0; i < tikInfo["dailyByMonth"].Count; i++)
            {
                for (int j = day; j < monthLength + day; j++)
                {
                    if(dailyDatas[j].dateInfo == tikInfo["dailyByMonth"][i]["aggregatedDate"])
                    {
                        dailyDatas[j].text_RewardValue.text = tikInfo["dailyByMonth"][i]["rewardTik"];
                        break;
                    }
                }
            }
        }

        public void Success(string json)
        {

        }

        public void SetPrevMonthInfo(int month)
        {
            currentDateTime = currentDateTime.AddMonths(month);
            SetCalendarInfo(currentDateTime, true);
        }

        public override void ActivePopup()
        {
            SetCalendarInfo(DateTime.UtcNow, false);
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            currentDateTime = DateTime.Now;

            todayInfo = DateTime.Now.ToString("yyyy-MM-dd");

            SetCalendarInfo(currentDateTime, false);

            for (int i = 0; i < dailyDatas.Length; i++)
            {
                dailyDatas[i].Initialize();
            }

            button_PrevMonth.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                SetPrevMonthInfo(-1);
            });

            button_NextMonth.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                SetPrevMonthInfo(1);
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Util;
using System;
    
namespace Framework.UI
{
    public class GuidePopup : PopupTemplate
    {
        public TextMeshProUGUI text_Description;

        public override void ActivePopup()
        {
            string desc = LanguageManager.Instance.GetStringData("UI_Reward_Guide_Desc");

            DateTime date = DateTime.Parse("2023-03-21T00:00:00.886816Z");

            //string date = "2023-03-21T00:00:00.886816Z";

            string time = date.ToString("HH:mm");

            string tempDesc = string.Format(desc, time);

            text_Description.text = tempDesc;

            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }
    }
}
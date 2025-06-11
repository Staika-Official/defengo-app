using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using TMPro;
using System;
using UnityEngine.Events;

namespace Framework.UI
{
    public class DailyRewardPopup : PopupTemplate
    {
        public TextMeshProUGUI text_Description;
        public TextMeshProUGUI text_RewardTaika;
        //public ParticleSystem attractor;
        public int taika;
        public int wholeTaikaValue;

        private UnityAction OnClosePopup;

        public void Initialize(Transaction transaction, UnityAction OnClosePopup = null)
        {
            wholeTaikaValue = (int)UserInfoManager.Instance.userTikWalletHistory.balance.amount;
            taika = (int)transaction.amount;

            //LobbyManager.Instance.text_Gem.text = $"{wholeTaikaValue - taika}";

            text_RewardTaika.text = $"<sprite=1>{taika}";

            ActivePopup();
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
            OnClosePopup?.Invoke();
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                OnClick_Close();
            });
        }

        public void OnClick_Close()
        {
            DateTime date = DateTime.Now;

            string dateInfo = date.ToString();

            SecurePlayerPrefs.SetString("prevRewardDate", dateInfo);
            AttractorManager.Instance.SetAttractor("tik", button_Close.transform);
            PopUpSequence(false);
        }
    }
}

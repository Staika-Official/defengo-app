using System.Collections;
using System.Collections.Generic;
using Framework.Network;
using UnityEngine;
using UnityEngine.UI;
using Framework.Util;
using TMPro;
using UnityEngine.Events;
using System;

namespace Framework.UI
{
    public class SystemNoticePopup : PopupTemplate
    {
        public TextMeshProUGUI text_NoticeMessage;

        public GameObject m_OneButton;
        public GameObject m_TwoButton;

        public Button m_BtnYes;
        public Button m_BtnNo;

        private UnityAction mCallbackYes;

        public void SetNoticeMessage(string notice)
        {
            text_NoticeMessage.text = LanguageManager.Instance.GetStringData(notice);
            mCallbackYes = null;

            m_OneButton.SetActive(true);
            m_TwoButton.SetActive(false);

            ActivePopup();
        }

        public void SetNoticeText(string text)
        {
            text_NoticeMessage.text = text;
            mCallbackYes = null;

            m_OneButton.SetActive(true);
            m_TwoButton.SetActive(false);

            ActivePopup();
        }

        public void SetYesNoPopupMessage(string notice, UnityAction callback = null)
        {
            if (LanguageManager.Instance.Cotains(notice))
                text_NoticeMessage.text = LanguageManager.Instance.GetStringData(notice);
            else
                text_NoticeMessage.text = notice;

            mCallbackYes = callback;

            m_OneButton.SetActive(false);
            m_TwoButton.SetActive(true);

            ActivePopup();
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public void InActivePopupLogout(bool isLogout)
        {
            if (isLogout)
            {
                button_Close.onClick.RemoveAllListeners();
                _ = NetworkManager.Instance.Logout();
            }
            else
            {
                InActivePopup();
            }
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });

            m_BtnYes.onClick.AddListener(() =>
            {
                mCallbackYes?.Invoke();
                InActivePopup();
            });

            m_BtnNo.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }
    }
}
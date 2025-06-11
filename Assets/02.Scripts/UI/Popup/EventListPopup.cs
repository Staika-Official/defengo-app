using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;
using DG.Tweening;
using Framework.Network;
using System;

namespace Framework.UI
{
    public class EventListPopup : PopupTemplate
    {
        public EventItem eventItem;
        public RectTransform listRect;
        public RectTransform scrollRect;
        
        public override void ActivePopup()
        {
            PopUpSequence(true);
            
            //이벤트 레드닷 제거
            //HomeScreen.Instance.eventNotice.SetActive(false);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            _ = NetworkManager.Instance.GetEventPopupList(Success, Failed);

            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }

        public void Success(EventPopupList data)
        {
            float sizeY = (data.popups.Length * 335) + ((data.popups.Length - 1) * 12);
            scrollRect.sizeDelta = new Vector2(940, sizeY);

            for (int i = 0; i < data.popups.Length; i++)
            {
                EventItem obj = Instantiate(eventItem);
                obj.transform.SetParent(listRect);
                obj.transform.localScale = Vector3.one;
                obj.Initialize(data.popups[i]);
            }

            // if (data.popups.Length != SecurePlayerPrefs.GetInt("EventNotice"))
            // {
            //     SecurePlayerPrefs.SetInt("EventNotice", data.popups.Length);
            //     HomeScreen.Instance.eventNotice.SetActive(true);
            // }
            // else
            // {
            //     HomeScreen.Instance.eventNotice.SetActive(false);
            // }

        }

        public void Failed()
        {

        }
    }
}
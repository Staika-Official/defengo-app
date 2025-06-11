using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;
using DG.Tweening;
using Framework.Network;
using System;
using System.IO;
using System.IO.Enumeration;

namespace Framework.UI
{
    public class EventItem : MonoBehaviour
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Desc;
        public RawImage image_BackGround;
        public Button button_Confirm;
        public Button button_Movement;
 public async void Initialize(EventPopup data)
        {
#if UNITY_IOS && !UNITY_EDITOR
            string dataPath = $"{Application.persistentDataPath}";
#elif UNITY_ANDROID && !UNITY_EDITOR
            string dataPath = Application.persistentDataPath;
#elif UNITY_EDITOR
            string dataPath = Application.dataPath;
#endif


            bool isKorean = LanguageManager.Instance.GetLanguageKey() == "ko";

            if (isKorean)
            {
                text_Title.text = data.titleKo;
                text_Desc.text = data.contentKo;
            }
            else
            {
                text_Title.text = data.titleEn;
                text_Desc.text = data.contentEn;
            }
#if UNITY_ANDROID || UNITY_EDITOR
            if (File.Exists(dataPath + data.id.ToString() + ".png"))
            {
                byte[] file = File.ReadAllBytes(dataPath + data.id + ".png");
                Texture2D texture2D = new(2, 2, TextureFormat.ASTC_6x6, false);
                texture2D.LoadImage(file);

                image_BackGround.texture = texture2D;
            }
            else
            {
                Texture2D texture2D = await NetworkManager.Instance.GetImageFromS3(data.imageUrl);

                byte[] bytes = texture2D.EncodeToPNG();
                string fileName = data.imageUrl.Replace('/', ' ');

                File.WriteAllBytes(dataPath + data.id + ".png", bytes);
                image_BackGround.texture = texture2D;
            }
#elif UNITY_IOS && !UNITY_EDITOR
                Texture2D texture2D = await NetworkManager.Instance.GetImageFromS3(data.imageUrl);
                image_BackGround.texture = texture2D;
#endif

            EventPopupType eventPopupType = (EventPopupType)Enum.Parse(typeof(EventPopupType), data.type);

            switch (eventPopupType)
            {
                case EventPopupType.NONE:
                    button_Confirm.gameObject.SetActive(false);
                    button_Movement.gameObject.SetActive(true);
                    break;
                case EventPopupType.NOTICE_WEBVIEW:
                    button_Confirm.gameObject.SetActive(true);
                    button_Movement.gameObject.SetActive(false);

                    button_Confirm.onClick.AddListener(() =>
                    {
                        string key = isKorean ? data.linkUrlKo : data.linkUrlEn;
                        Application.OpenURL(key);
                    });

                    break;
                case EventPopupType.STORE:
                    button_Confirm.gameObject.SetActive(false);
                    button_Movement.gameObject.SetActive(true);

                    button_Movement.onClick.AddListener(() =>
                    {
                        PopupManager.Instance.GetPopUp<EventListPopup>("eventList").PopUpSequence(false);
                        PopupManager.Instance.GetPopUp<LimitedShopPopup>("limitedShop").ActivePopup();
                    });
                    break;
                case EventPopupType.EVENT:
                    button_Confirm.gameObject.SetActive(false);
                    button_Movement.gameObject.SetActive(true);
                    break;
                case EventPopupType.NOTICE_EXTERNAL:
                    button_Confirm.gameObject.SetActive(true);
                    button_Movement.gameObject.SetActive(false);

                    button_Confirm.onClick.AddListener(() =>
                    {
                        string key = isKorean ? data.linkUrlKo : data.linkUrlEn;
                        Application.OpenURL(key);
                    });
                    break;
                case EventPopupType.BATTLE_PASS:
                    button_Confirm.gameObject.SetActive(true);
                    button_Movement.gameObject.SetActive(false);

                    button_Confirm.onClick.AddListener(()=>
                    {
                        PopupManager.Instance.GetPopUp<EventListPopup>("eventList").PopUpSequence(false);
                        PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass").ActivePopup();
                    });
                    break;
            }
        }
    }
}

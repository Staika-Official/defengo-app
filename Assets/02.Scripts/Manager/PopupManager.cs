using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public enum Popup
    {
        SETTING,
        PROFILE,
        ENERGY,
        TERMS,
        WITHDRAWAL,
        INVENTORY,
        DECCHANGE,
        CALENDER,
        SHOP,
        WALLETHISTORY,
        EDITNICKNAME,
        EARN
    }

    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance;
        public PopupScriptableObject popupData;

        public RectTransform focusRect;
        public RectTransform unfocusRect;

        [Header("ToastPopup")]
        public TransferAlert toastTransferAlert;
        public GameObject toastSwapAlert;

        private SerializableDictionary<string, PopupTemplate> dic_Active_Popup = new SerializableDictionary<string, PopupTemplate>();

        public static RectTransform GetPopupRect(bool isFocus)
        {
            RectTransform rect = isFocus ? Instance.focusRect : Instance.unfocusRect;

            return rect;
        }

        private void Start()
        {
            if (Instance is null)
            {
                Instance = this;
            }

            Initiailize();
        }

        public T GetPopUp<T>(string name) where T : PopupTemplate
        {
            T popup = null;
            if (dic_Active_Popup.ContainsKey(name))
                popup = dic_Active_Popup[name] as T;
            else
            {
                PopupTemplate obj = Instantiate(popupData.dic_Popup[name]);
                InitializePopup(obj);
                dic_Active_Popup.Add(new SerializableDictionary<string, PopupTemplate>.Pair(name, obj));
                popup = obj as T;
            }

            return popup;
        }

        public void Initiailize()
        {
            // foreach (var item in popupData.dic_Popup)
            // {
            //     InitializePopup(popupData.dic_Popup[item.Key]);
            // }
        }

        public void InitializePopup(PopupTemplate popup)
        {
            if (!popup.gameObject.activeSelf)
            {
                popup.gameObject.SetActive(true);
            }

            popup.PopUpSequence(false);
            RectTransform rect = popup.transform as RectTransform;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.one;

            popup.Initialize();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}

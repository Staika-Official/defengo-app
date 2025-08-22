using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Network;
using System;
using Framework.Util;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class InboxItem : MonoBehaviour
    {
        public InboxType inboxType;
        public InboxStatus inboxStatus;
        public int id;
        public int inboxId;
        public int quantity;
        public string title;
        public string expiredDateText;
        public string value;

        public GameObject itemInfo;
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_ExpiredDateInfo;
        public TextMeshProUGUI text_Quantity;
        public DateTime expiredDate;

        public Image image_Icon;
        public ButtonComponent button_Read;
        public ButtonComponent button_GetItem;
        public ButtonComponent button_InActiveRead;
        public ButtonComponent button_Accept;
        public ButtonComponent button_Reject;

        public void Initialize(UserInbox data)
        {
            inboxStatus = (InboxStatus)Enum.Parse(typeof(InboxStatus), data.status);
            expiredDate = DateTime.Parse(data.expiredDate);
            inboxId = data.inboxId;
            id = data.id;
            value = data.value;
            quantity = data.quantity;

            int[] temp = Calculator.GetExpiredTime(expiredDate);

            switch (LanguageManager.Instance.language)
            {
                case Language.EN:
                    title = data.titleEn;
                    expiredDateText = string.Format(LanguageManager.Instance.GetStringData("UI_Inbox_Expiration_Date"), temp[0], temp[1], temp[2]);
                    break;
                case Language.KO:
                    title = data.titleKo;
                    expiredDateText = string.Format(LanguageManager.Instance.GetStringData("UI_Inbox_Expiration_Date"), temp[0], temp[1], temp[2]);
                    break;
                default:
                    break;
            }

            inboxType = (InboxType)Enum.Parse(typeof(InboxType), data.inboxType);

            switch (inboxType)
            {
                case InboxType.MAIL:
                    text_Quantity.gameObject.SetActive(false);
                    itemInfo.SetActive(false);

                    bool isActive = inboxStatus == InboxStatus.READ;

                    button_InActiveRead.gameObject.SetActive(isActive);
                    button_Read.gameObject.SetActive(!isActive);

                    break;
                case InboxType.RANDOM_BOX:
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon[value];

                    text_Quantity.gameObject.SetActive(true);

                    text_Quantity.text = $"x{quantity}";

                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.ENERGY:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Energy"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.TIK:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Tik"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.STIK:
                    itemInfo.SetActive(true);
                    text_Quantity.gameObject.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Stik"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.GEM:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Gem"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.GO:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Go"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.PROFILE:
                    break;
                case InboxType.STIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.PTIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.TIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.PTIK:
                    itemInfo.SetActive(true);
                    text_Quantity.gameObject.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Gem"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.HATCHING_ORB:
                    itemInfo.SetActive(true);
                    //부화석 타입별 스프라이트가 달라서 value값으로 넣어줌
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon[value];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.quantity.ToString();
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);
                    break;
                case InboxType.FRIEND_REQUEST:
                    button_Accept.gameObject.SetActive(true);
                    button_Reject.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }


            text_ExpiredDateInfo.text = expiredDateText;
            text_Title.text = title;

            //bool isTextActive = quantity != 0;

            //text_Quantity.gameObject.SetActive(isTextActive);
            //text_Quantity.text = quantity.ToString();

            button_Read.onPointerUp = () =>
            {
                OnClick_Read();
                button_Read.onPointerUp = null;
            };

            button_GetItem.onPointerUp = () =>
            {
                OnClick_GetItem();
                button_GetItem.onPointerUp = null;
            };

            button_InActiveRead.onPointerUp = () =>
            {
                OnClick_InactiveRead();
                button_InActiveRead.onPointerUp = null;
            };

            button_Accept.onPointerUp = () =>
            {

            };

            button_Reject.onPointerUp = () =>
            {

            };
            
        }

        public void OnClick_InactiveRead()
        {
            _ = NetworkManager.Instance.GetUserInboxDetail(id, inboxId, SuccessRead, Failed);
        }

        public void OnClick_Read()
        {
            //Debug.Log("OnClick_Read");
            _ = NetworkManager.Instance.GetUserInboxDetail(id, inboxId, SuccessRead, Failed);
        }

        public void OnClick_GetItem()
        {
            button_GetItem.button.interactable = false;
            _ = NetworkManager.Instance.GetUserInboxDetail(id, inboxId, SuccessRead, Failed);
        }

        public void Failed()
        {
            Debug.Log("Failed");
            button_GetItem.button.interactable = false;
            PopupManager.Instance.GetPopUp<InboxPopup>("inbox").InActivePopup();
            PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeMessage("SYSTEM_ERROR");
        }

        public void SuccessRead(string data)
        {
            button_GetItem.button.interactable = false;
            InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");

            switch (inboxType)
            {
                case InboxType.MAIL:
                    popup.GetMail(title, data);
                    break;
                case InboxType.RANDOM_BOX:
                    popup.GetRandomBox(data, value);
                    break;
                case InboxType.ENERGY:
                    popup.GetAsset(data);
                    break;
                case InboxType.TIK:
                    popup.GetAsset(data);
                    break;
                case InboxType.STIK:
                    popup.GetAsset(data);
                    break;
                case InboxType.GEM:
                    popup.GetAsset(data);
                    break;
                case InboxType.GO:
                    popup.GetAsset(data);
                    break;
                case InboxType.PROFILE:
                    break;
                case InboxType.STIK_RANDOMBOX:
                    popup.GetEventRandomBox(data);
                    break;
                case InboxType.PTIK_RANDOMBOX:
                    break;
                case InboxType.TIK_RANDOMBOX:
                    break;
                case InboxType.PTIK:
                    popup.GetAsset(data);
                    break;
                case InboxType.HATCHING_ORB:
                    //포스트맨에서 부화석 리스폰스 형식이 DetailInbox로 되어있어서 에셋함수를 호출
                    popup.GetAsset(data);
                    break;
                default:
                    break;
            }
        }
    }
}
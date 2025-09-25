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
using Newtonsoft.Json;

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
        public TextMeshProUGUI text_battleInvite;
        public TextMeshProUGUI text_ExpiredDateInfo;
        public TextMeshProUGUI text_Quantity;
        public DateTime expiredDate;

        public Image image_Icon;
        public ButtonComponent button_Read;
        public ButtonComponent button_GetItem;
        public ButtonComponent button_InActiveRead;
        public ButtonComponent button_Accept;
        public ButtonComponent button_Reject;
        public ButtonComponent button_GoFriendlyBattle;
        public ButtonComponent button_RejectFriendlyBattle;
        public Image image_userProfile;

        public void Initialize(UserInbox data)
        {
            Debug.Log($"Inbox data: {JsonConvert.SerializeObject(data)}");
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

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);

                    break;
                case InboxType.RANDOM_BOX:
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon[value];

                    text_Quantity.gameObject.SetActive(true);

                    text_Quantity.text = $"x{quantity}";

                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.ENERGY:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Energy"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.TIK:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Tik"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.STIK:
                    itemInfo.SetActive(true);
                    text_Quantity.gameObject.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Stik"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.GEM:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Gem"];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.GO:
                    itemInfo.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Go"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.PROFILE:
                    break;
                case InboxType.STIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.PTIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.TIK_RANDOMBOX:
                    text_Quantity.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["EventChest"];
                    itemInfo.SetActive(true);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.PTIK:
                    itemInfo.SetActive(true);
                    text_Quantity.gameObject.SetActive(true);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon["Gem"];
                    text_Quantity.text = data.value;
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.HATCHING_ORB:
                    itemInfo.SetActive(true);
                    //부화석 타입별 스프라이트가 달라서 value값으로 넣어줌
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_InboxItemIcon[value];
                    text_Quantity.gameObject.SetActive(true);
                    text_Quantity.text = data.quantity.ToString();
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(true);

                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.FRIEND_REQUEST:
                    button_Accept.gameObject.SetActive(true);
                    button_Reject.gameObject.SetActive(true);

                    text_Quantity.gameObject.SetActive(false);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(false);
                    button_GoFriendlyBattle.gameObject.SetActive(false);
                    button_RejectFriendlyBattle.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(true);
                    image_userProfile.gameObject.SetActive(false);
                    text_battleInvite.gameObject.SetActive(false);
                    break;
                case InboxType.BATTLE_FRIEND_REQUEST:
                    button_GoFriendlyBattle.gameObject.SetActive(true);
                    button_RejectFriendlyBattle.gameObject.SetActive(true);

                    text_Quantity.gameObject.SetActive(false);
                    button_Read.gameObject.SetActive(false);
                    button_GetItem.gameObject.SetActive(false);
                    button_Accept.gameObject.SetActive(false);
                    button_Reject.gameObject.SetActive(false);

                    text_Title.gameObject.SetActive(false);
                    image_userProfile.gameObject.SetActive(true);
                    text_battleInvite.gameObject.SetActive(true);

                    BattleInviteData battleRequestData = JsonConvert.DeserializeObject<BattleInviteData>(data.titleEn);
                    text_battleInvite.text = battleRequestData.content;
                    UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[battleRequestData.profileImage];
                    image_userProfile.sprite = userProfileData.sprite_image;

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

            button_Accept.onPointerUp = async () =>
            {
                await NetworkManager.Instance.HandleFriendRequest(data, FriendRequestStatus.ACCEPTED, () =>
                {
                    InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
                    popup.SetInboxData(true);
                }, null);
            };

            button_Reject.onPointerUp = async () =>
            {
                await NetworkManager.Instance.HandleFriendRequest(data, FriendRequestStatus.REJECTED, () =>
                {
                    InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
                    popup.SetInboxData(true);
                }, null);
            };

            button_GoFriendlyBattle.onPointerUp = async () =>
            {
                Debug.Log(JsonConvert.SerializeObject(data));
                await NetworkManager.Instance.HandleFriendlyBattleInvite(data, FriendRequestStatus.ACCEPTED, (info) =>
                {
                    StartCoroutine(HomeScreen.Instance.ShowTransitionOnly(true, () =>
                    {
                        NetworkConnect networkConnect = Instantiate(HomeScreen.Instance.networkPrefab).GetComponent<NetworkConnect>();
                        networkConnect.ConnectToLobby(true, info.roomName, info.roomPassword);
                        MatchMakingPopup matchingPopup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                        matchingPopup.ActivePopup();
                        DontDestroyOnLoad(networkConnect);
                        PopupManager.Instance.GetPopUp<InboxPopup>("inbox").InActivePopup();
                    }));
                }, null);
            };

            button_RejectFriendlyBattle.onPointerUp = async () =>
            {
                await NetworkManager.Instance.HandleFriendlyBattleInvite(data, FriendRequestStatus.REJECTED, (info) =>
                {
                    InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
                    popup.SetInboxData(true);
                }, null);
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
                case InboxType.FRIEND_REQUEST:
                    popup.SetInboxData(true);
                    break;
                case InboxType.BATTLE_FRIEND_REQUEST:
                    popup.SetInboxData(true);
                    break;
                default:
                    break;
            }
        }
    }
}
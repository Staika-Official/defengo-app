using System.Collections;
using System.Collections.Generic;
using Framework.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Sound;
using System;
using Framework.UI;
using Newtonsoft.Json;
using Framework.Util;

public class FriendItem : MonoBehaviour
{
    public Image image_userProfile;
    public TMP_Text text_userName;
    public Button button_sendEnergy;
    public Button button_sendFriendRequest;
    public Button button_checkProfile;

    public FriendData friendData;
    public bool isFriend;
    public bool pendingFromUser;
    public bool pendingFromFriend;

    void Awake()
    {
        button_sendEnergy.onClick.AddListener(async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendEnergy(friendData.userId, SuccessSendEnergy, FailedSendEnergy);
        });
        button_sendFriendRequest.onClick.AddListener(async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendFriendRequest(friendData.userId, SuccesSendFriendRequest, FailedSendFriendRequest);
        });
        button_checkProfile.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            var userProfilePopup = PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail");
            userProfilePopup.ActivePopup();
            userProfilePopup.ShowOtherProfile(this, friendData.equippedProfileId, isFriend);
        });
    }

    public void SetFriendData(ReqSearchFriendsData data)
    {
        friendData = data.friendData;
        isFriend = data.isFriend;
        UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[friendData.equippedProfileId];
        image_userProfile.sprite = userProfileData.sprite_image;
        text_userName.text = friendData.nickname;
        pendingFromFriend = data.pendingFromFriend;
        pendingFromUser = data.pendingFromCaller;

        button_sendEnergy.gameObject.SetActive(isFriend);
        button_sendEnergy.interactable = isFriend && DateTime.UtcNow > friendData.blockSendEnergyDate;
        button_sendEnergy.transform.GetChild(0).GetComponent<Image>().color = button_sendEnergy.interactable ? Color.white : Color.gray;
        button_sendFriendRequest.gameObject.SetActive(!isFriend && !data.pendingFromCaller && !data.pendingFromFriend);
    }

    public void SetFriendData(FriendData data, bool _isFriend)
    {
        friendData = data;
        isFriend = _isFriend;
        UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[friendData.equippedProfileId];
        image_userProfile.sprite = userProfileData.sprite_image;
        text_userName.text = friendData.nickname;

        button_sendEnergy.interactable = isFriend && DateTime.UtcNow > friendData.blockSendEnergyDate;
        button_sendEnergy.transform.GetChild(0).GetComponent<Image>().color = button_sendEnergy.interactable ? Color.white : Color.gray;
        button_sendFriendRequest.gameObject.SetActive(!isFriend);
    }

    void SuccessSendEnergy()
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Success"));
        button_sendEnergy.interactable = false;
        button_sendEnergy.transform.GetChild(0).GetComponent<Image>().color = button_sendEnergy.interactable ? Color.white : Color.gray;
    }
    void FailedSendEnergy(string energy)
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Failed"));
    }

    void SuccesSendFriendRequest()
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Success"));
        gameObject.SetActive(false);
    }
    void FailedSendFriendRequest(string error)
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Failed"));
    }
}

public enum FriendItemStatus
{
    RECOMMEND,
    FRIEND
}
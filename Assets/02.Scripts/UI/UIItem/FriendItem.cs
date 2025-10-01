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
using Framework.Util;

public class FriendItem : MonoBehaviour
{
    public Image image_userProfile;
    public TMP_Text text_userName;
    public ButtonComponent button_sendEnergy;
    public ButtonComponent button_sendFriendRequest;
    public ButtonComponent button_inviteFriendlyBattle;
    public Button button_checkProfile;

    public FriendData friendData;
    public bool isFriend;
    public bool isBattleInvite;
    public bool pendingFromUser;
    public bool pendingFromFriend;

    void Awake()
    {
        button_sendEnergy.onPointerUp += async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendEnergy(friendData.userId, SuccessSendEnergy, FailedSendEnergy);
        };
        button_sendFriendRequest.onPointerUp += async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendFriendRequest(friendData.userId, SuccesSendFriendRequest, FailedSendFriendRequest);
        };
        button_checkProfile.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            var userProfilePopup = PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail");
            userProfilePopup.ActivePopup();
            userProfilePopup.ShowOtherProfile(this, friendData.equippedProfileId, isFriend);
        });
        button_inviteFriendlyBattle.onPointerUp += async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.InviteFriendToBattle(friendData.userId, NetworkConnect.Instance.roomName, NetworkConnect.Instance.roomPassword, () =>
            {
                button_inviteFriendlyBattle.SetInterectible(false);
            });
        };
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
        button_sendEnergy.SetInterectible(isFriend && DateTime.UtcNow > friendData.blockSendEnergyDate);
        button_sendFriendRequest.gameObject.SetActive(!isFriend && !data.pendingFromCaller && !data.pendingFromFriend);
        button_inviteFriendlyBattle.gameObject.SetActive(false);
        button_inviteFriendlyBattle.SetInterectible(button_inviteFriendlyBattle.gameObject.activeSelf);
    }

    public void SetFriendData(FriendData data, bool _isFriend, bool _isBattleInvite)
    {
        friendData = data;
        isFriend = _isFriend;
        isBattleInvite = _isBattleInvite;
        UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[friendData.equippedProfileId];
        image_userProfile.sprite = userProfileData.sprite_image;
        text_userName.text = friendData.nickname;

        button_sendEnergy.SetInterectible(isFriend && DateTime.UtcNow > friendData.blockSendEnergyDate);
        button_sendFriendRequest.gameObject.SetActive(!isFriend);
        button_inviteFriendlyBattle.gameObject.SetActive(isFriend && isBattleInvite);
        button_inviteFriendlyBattle.SetInterectible(button_inviteFriendlyBattle.gameObject.activeSelf);
    }

    void SuccessSendEnergy()
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Success"));
        button_sendEnergy.SetInterectible(false);
        PopupManager.Instance.GetPopUp<FriendPopup>("friend").OnSendEnergy(friendData.userId);
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
        PopupManager.Instance.GetPopUp<FriendPopup>("friend").OnSendFriendRequest(friendData.userId);
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
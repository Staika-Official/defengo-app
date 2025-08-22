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

public class FriendItem : MonoBehaviour
{
    public Image image_userProfile;
    public TMP_Text text_userName;
    public Button button_sendEnergy;
    public Button button_sendFriendRequest;
    public Button button_checkProfile;

    FriendData friendData;

    void Awake()
    {
        button_sendEnergy?.onClick.AddListener(async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendEnergy(friendData.userId, SuccessSendEnergy, FailedSendEnergy);
        });
        button_sendFriendRequest?.onClick.AddListener(async () =>
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            await NetworkManager.Instance.SendFriendRequest(friendData.userId, SuccesSendFriendRequest, FailedSendFriendRequest);
        });
        button_checkProfile.onClick.AddListener(() =>
        {
            var userProfilePopup = PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail");
            userProfilePopup.ActivePopup();
            userProfilePopup.ShowOtherProfile(friendData.userId, friendData.equippedProfileId);
        });
    }

    public void SetFriendData(FriendData data, bool isFriend)
    {
        friendData = data;

        UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[friendData.equippedProfileId];
        image_userProfile.sprite = userProfileData.sprite_image;
        text_userName.text = friendData.nickname;

        button_sendEnergy?.gameObject.SetActive(isFriend && DateTime.Now > friendData.blockSendEnergyDate);
        button_sendFriendRequest?.gameObject.SetActive(!isFriend);
    }

    void SuccessSendEnergy()
    {
        button_sendEnergy.gameObject.SetActive(false);
    }
    void FailedSendEnergy(string energy)
    {

    }

    void SuccesSendFriendRequest()
    {
        gameObject.SetActive(false);
    }
    void FailedSendFriendRequest(string error)
    {

    }
}

public enum FriendItemStatus
{
    RECOMMEND,
    FRIEND
}
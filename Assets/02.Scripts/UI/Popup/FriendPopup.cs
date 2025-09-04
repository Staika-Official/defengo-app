using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Network;
using System;
using DG.Tweening;
using Newtonsoft.Json;
using Framework.Util;

namespace Framework.UI
{
    public class FriendPopup : PopupTemplate
    {
        List<FriendData> recommendedFriendDatas = new List<FriendData>();
        List<FriendData> friendDatas = new List<FriendData>();

        public TMP_InputField input_UID;
        public Button button_Search;
        public FriendItem searchFriendItem;

        public GameObject goListFriend;
        public ScrollRect scrollFriends;
        public FriendItem friendItem;

        public GameObject goRecommendedFriend;
        public Button button_RefreshRecommend;
        public Transform recommendParent;
        public FriendItem recommendFriendItem;

        public override void ActivePopup()
        {
            PopUpSequence(true);

            LoadData();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            input_UID.onValueChanged.AddListener((string txt) =>
            {
                button_Search.interactable = !string.IsNullOrEmpty(txt);
                if (string.IsNullOrEmpty(txt))
                    searchFriendItem.transform.parent.gameObject.SetActive(false);
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            button_Search.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClickButtonSearch();
            });

            button_RefreshRecommend.onClick.AddListener(async () =>
            {
                foreach (Transform child in recommendParent)
                {
                    Destroy(child.gameObject);
                }
                await NetworkManager.Instance.GetRecommendedFriends(SuccessGetRecommendedFriends, FailedGetRecommendedFriends);
            });
        }

        async void LoadData()
        {
            foreach (Transform child in recommendParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in scrollFriends.content)
            {
                Destroy(child.gameObject);
            }
            await NetworkManager.Instance.GetListFriends(SuccessGetListFriend, FailedGetListFriend);
            await NetworkManager.Instance.GetRecommendedFriends(SuccessGetRecommendedFriends, FailedGetRecommendedFriends);
        }

        async void OnClickButtonSearch()
        {
            await NetworkManager.Instance.SearchFriends(input_UID.text, (ReqSearchFriendsData data) =>
            {
                searchFriendItem.transform.parent.gameObject.SetActive(true);
                searchFriendItem.gameObject.SetActive(true);
                searchFriendItem.SetFriendData(data);
            }, (string err) =>
            {
                SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Invalid_UID"));
            });
        }

        void SuccessGetListFriend(ReqListFriendsData data)
        {
            friendDatas = data.friends;

            goListFriend.gameObject.SetActive(friendDatas.Count > 0);
            goRecommendedFriend.gameObject.SetActive(friendDatas.Count == 0);

            foreach (var friend in friendDatas)
            {
                FriendItem itm = Instantiate(friendItem, scrollFriends.content);
                itm.gameObject.SetActive(true);
                itm.SetFriendData(friend, true);
            }
        }
        void FailedGetListFriend(string error)
        {
            Debug.Log($"Error List Friend: {error}");
        }
        void SuccessGetRecommendedFriends(ReqRecommendedFriendsData data)
        {
            recommendedFriendDatas = data.players;

            foreach (var recommend in recommendedFriendDatas)
            {
                FriendItem itm = Instantiate(recommendFriendItem, recommendParent);
                itm.gameObject.SetActive(true);
                itm.SetFriendData(recommend, false);
            }
        }
        void FailedGetRecommendedFriends(string error)
        {
            Debug.Log($"Error Recommended Friends: {error}");
        }
    }
}

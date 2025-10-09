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
        public TextMeshProUGUI text_FriendCount;
        public GameObject goNoFriend;

        public GameObject goRecommendedFriend;
        public Button button_RefreshRecommend;
        public Transform recommendParent;
        public FriendItem recommendFriendItem;

        public List<FriendItem> listFriendItems = new();
        public List<FriendItem> listRecommendedFriendItems = new();

        bool isBattleInvite = false;

        public override void ActivePopup()
        {
            PopUpSequence(true);

            input_UID.text = "";
        }

        public void Show(bool isBattleInvite)
        {
            this.isBattleInvite = isBattleInvite;
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
                listRecommendedFriendItems.Clear();
                await NetworkManager.Instance.GetRecommendedFriends(SuccessGetRecommendedFriends, FailedGetRecommendedFriends);
            });
        }

        async void LoadData()
        {
            foreach (Transform child in scrollFriends.content)
            {
                Destroy(child.gameObject);
            }
            listFriendItems.Clear();
            foreach (Transform child in recommendParent)
            {
                Destroy(child.gameObject);
            }
            listRecommendedFriendItems.Clear();
            await NetworkManager.Instance.GetListFriends(SuccessGetListFriend, FailedGetListFriend);
            if (!isBattleInvite)
                await NetworkManager.Instance.GetRecommendedFriends(SuccessGetRecommendedFriends, FailedGetRecommendedFriends);
        }

        async void OnClickButtonSearch()
        {
            if (!isBattleInvite)
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
            else
            {
                var friend = friendDatas.Find(x => x.userId == input_UID.text);
                if (friend != null)
                {
                    searchFriendItem.transform.parent.gameObject.SetActive(true);
                    searchFriendItem.gameObject.SetActive(true);
                    searchFriendItem.SetFriendData(friend, true, isBattleInvite);
                }
            }
        }

        void SuccessGetListFriend(ReqListFriendsData data)
        {
            friendDatas = data.friends;

            // goListFriend.gameObject.SetActive(friendDatas.Count > 0);
            goNoFriend.SetActive(friendDatas.Count == 0);
            goRecommendedFriend.gameObject.SetActive(!isBattleInvite);

            for (int i = 0; i < friendDatas.Count; i++)
            {
                if (i < listFriendItems.Count)
                {
                    listFriendItems[i].gameObject.SetActive(true);
                    listFriendItems[i].SetFriendData(friendDatas[i], true, isBattleInvite);
                }
                else
                {
                    FriendItem itm = Instantiate(friendItem, scrollFriends.content);
                    itm.gameObject.SetActive(true);
                    itm.SetFriendData(friendDatas[i], true, isBattleInvite);
                    listFriendItems.Add(itm);
                }
            }
            text_FriendCount.text = $"{listFriendItems.Count}/30";
        }
        void FailedGetListFriend(string error)
        {
            Debug.Log($"Error List Friend: {error}");
        }
        void SuccessGetRecommendedFriends(ReqRecommendedFriendsData data)
        {
            recommendedFriendDatas = data.players;

            for (int i = 0; i < recommendedFriendDatas.Count; i++)
            {
                if (i < listRecommendedFriendItems.Count)
                {
                    listRecommendedFriendItems[i].gameObject.SetActive(true);
                    listRecommendedFriendItems[i].SetFriendData(recommendedFriendDatas[i], true, isBattleInvite);
                }
                else
                {
                    FriendItem itm = Instantiate(recommendFriendItem, recommendParent);
                    itm.gameObject.SetActive(true);
                    itm.SetFriendData(recommendedFriendDatas[i], false, isBattleInvite);
                    listRecommendedFriendItems.Add(itm);
                }
            }
        }
        void FailedGetRecommendedFriends(string error)
        {
            Debug.Log($"Error Recommended Friends: {error}");
        }

        public void OnSendEnergy(string userId)
        {
            var itm = listFriendItems.Find(x => x.friendData.userId == userId);
            if (itm != null)
                itm.button_sendEnergy.SetInterectible(false);
        }
        public void OnSendFriendRequest(string userId)
        {
            var itm = listRecommendedFriendItems.Find(x => x.friendData.userId == userId);
            if (itm != null)
            {
                listRecommendedFriendItems.Remove(itm);
                Destroy(itm.gameObject);
            }
        }
        public void OnDeleteFriend(string userId)
        {
            var itm = listFriendItems.Find(x => x.friendData.userId == userId);
            if (itm != null)
            {
                listFriendItems.Remove(itm);
                Destroy(itm.gameObject);
            }
            text_FriendCount.text = $"{listFriendItems.Count}/30";
        }
    }
}

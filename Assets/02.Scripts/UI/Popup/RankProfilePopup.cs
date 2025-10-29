using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using System;

namespace Framework.UI
{
    public class RankProfilePopup : PopupTemplate
    {
        public Image image_Limited;
        public Image image_CharacterPortrait;
        public Image image_backGround;

        public ButtonComponent button_AddFriend;
        public ButtonComponent button_DeleteFriend;

        public TextMeshProUGUI text_userNickname;
        public TextMeshProUGUI text_bestScore;
        public TextMeshProUGUI text_bestBossLevel;
        public TextMeshProUGUI text_playCount;
        public TextMeshProUGUI text_bestWave;

        public TextMeshProUGUI text_rankType;

        public TextMeshProUGUI text_gold;
        public TextMeshProUGUI text_silver;
        public TextMeshProUGUI text_bronze;

        public Button button_switching;
        public LeaderBoardType focusType;
        public Dictionary<LeaderBoardType, ResponseProfileData> dic_profileData = new();

        public CharacterCard[] characterCards;
        List<FriendData> friends = new List<FriendData>();
        string userId;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => InActivePopup());


            button_AddFriend.button.onClick.AddListener(async () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                if (friends.Count >= 30)
                {
                    SystemNoticePopup popupNotice = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    popupNotice.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Friend_Max_Number"));
                }
                else
                {
                    await NetworkManager.Instance.SendFriendRequest(userId, () =>
                    {
                        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Friend_Request_Success"));
                        button_AddFriend.gameObject.SetActive(false);
                    }, (err) =>
                    {
                        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                        popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Friend_Request_Max_Number"));
                    });
                }
            });

            button_DeleteFriend.button.onClick.AddListener(async () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                await NetworkManager.Instance.DeleteFriend(userId, () =>
                {
                    button_DeleteFriend.gameObject.SetActive(false);
                    button_AddFriend.gameObject.SetActive(true);
                }, null);
            });
        }

        public string RankInfo(int rank)
        {
            string info = rank == 0 ? "-" : rank.ToString("N0");
            return info;
        }

        public void SetProfileData(ResponseProfileData data)
        {
            string nickName = data.nickname;

            text_userNickname.text = data.nickname;

            string key = data.leaderBoardType == "WAVE_WEEKLY" ? "UI_WeeklyStat" : (data.leaderBoardType == "WAVE_DAILY" ? "UI_DailyStat" : "UI_LeagueStat");

            string stat = LanguageManager.Instance.GetStringData(key);
            text_rankType.text = stat;

            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.equippedProfileId];

            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    image_Limited.gameObject.SetActive(false);
                    image_CharacterPortrait.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(true);
                    image_CharacterPortrait.sprite = userProfileData.sprite_image;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                    break;

                case ProfileType.Skin:
                    image_Limited.gameObject.SetActive(false);
                    image_CharacterPortrait.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(true);
                    image_CharacterPortrait.sprite = userProfileData.sprite_image;

                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                    break;

                default:
                    image_CharacterPortrait.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(false);
                    image_Limited.gameObject.SetActive(true);

                    image_Limited.sprite = userProfileData.sprite_image;
                    break;
            }

            if (data.ranking == null)
                data.ranking = new Podium();

            if (data.ranking.first > 0)
            {
                text_userNickname.text = $"{nickName}";
                text_userNickname.rectTransform.localPosition = new Vector3(10, 0, 0);
            }
            else
            {
                text_userNickname.text = nickName;
                text_userNickname.rectTransform.localPosition = Vector3.zero;
            }

            text_gold.text = RankInfo(data.ranking.first);
            text_silver.text = RankInfo(data.ranking.second);
            text_bronze.text = RankInfo(data.ranking.third);

            text_playCount.text = RankInfo(data.playCount);
            text_bestWave.text = RankInfo(data.bestWave);
            text_bestBossLevel.text = data.bestBossLevel == 0 ? "-" : $"Lv.{data.bestBossLevel}";
            text_bestScore.text = RankInfo(data.bestScore);

            if (data.bestCharacter == null) data.bestCharacter = new Network.CharacterInfo[0];
            for (int i = 0; i < data.bestCharacter.Length; i++)
            {
                CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.bestCharacter[i].characterId];
                characterCards[i].Initialize(characterData);
                characterCards[i].text_Level.text = $"Lv.{data.bestCharacter[i].classLevel}";
            }

            ActivePopup();
        }

        public async void FriendCheck(string userId)
        {
            if (UserInfoManager.Instance.userId == userId)
            {
                button_AddFriend.gameObject.SetActive(false);
                button_DeleteFriend.gameObject.SetActive(false);
                return;
            }
            this.userId = userId;
            await NetworkManager.Instance.GetListFriends((data) => friends = data.friends, null);
            await NetworkManager.Instance.SearchFriends(userId, (ReqSearchFriendsData data) =>
                {
                    button_AddFriend.gameObject.SetActive(!data.isFriend && !data.pendingFromFriend && friends.Count < 30);
                    button_DeleteFriend.gameObject.SetActive(data.isFriend);
                }, (string err) =>
                {
                    SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Invalid_UID"));
                });
        }

        #region 여러가지 프로필 보여줘야하는 상황일 때 주석해제
        // todo 위클리 데일리 둘다 보여줘야하는 상황일 때 주석해지
        // public void OnClick_ProfileType()
        // {
        //     focusType = focusType switch
        //     {
        //         LeaderBoardType.SCORE_DAILY => LeaderBoardType.WAVE_WEEKLY,
        //         LeaderBoardType.WAVE_WEEKLY => LeaderBoardType.SCORE_DAILY,
        //         _ => LeaderBoardType.SCORE_DAILY
        //     };
        //     ResponseProfileData data = dic_profileData[focusType];
        //
        //     SetProfileData(data);
        // }
        //
        // public void Success(TotalProfileData data)
        // {
        //     for (int i = 0; i < data.userProfile.Length; i++)
        //     {
        //         LeaderBoardType leaderBoardType = (LeaderBoardType)Enum.Parse(typeof(LeaderBoardType), data.userProfile[i].leaderBoardType);
        //         dic_profileData.Add(leaderBoardType, data.userProfile[i]);
        //     }
        //
        //     SetProfileData(dic_profileData[LeaderBoardType.SCORE_DAILY]);
        // }
        #endregion
    }
}

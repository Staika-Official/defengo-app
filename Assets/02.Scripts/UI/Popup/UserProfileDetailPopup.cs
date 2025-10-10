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
using UnityEngine.Localization.Components;
using Framework.Util;

namespace Framework.UI
{
    public class UserProfileDetailPopup : PopupTemplate
    {
        public GameObject objectInfo;
        //public GameObject objectModify;
        public GameObject objectFriendlyCharacter;
        public GameObject objectNullFriendlyCharacter;

        public Image image_SeletedProfile;
        public Image image_backGround;
        public Image image_limited;

        public LocalizeStringEvent text_Title;

        //public Transform ts_ProfileImg;
        //public TextMeshProUGUI text_UserNickname;
        //public Button button_EditNickName;
        //public Button button_Confirm;
        public Button button_editProfileDetail;
        public ButtonComponent button_AddFriend;
        public ButtonComponent button_DeleteFriend;
        public int focusProfileIdx;
        //public List<ProfileImageItem> profileImageItems = new();
        public CharacterCard[] characterCards;

        public TextMeshProUGUI text_nickName;
        public TextMeshProUGUI text_gold;
        public TextMeshProUGUI text_silver;
        public TextMeshProUGUI text_bronze;

        public TextMeshProUGUI text_stat;

        public TextMeshProUGUI text_playCount;
        public TextMeshProUGUI text_bestWave;
        public TextMeshProUGUI text_bestScore;
        public TextMeshProUGUI text_bestBossLevel;
        public bool isGoldMedal;

        public Button button_switching;
        public LeaderBoardType focusType;
        public Dictionary<LeaderBoardType, ResponseProfileData> dic_profileData = new();

        FriendItem friendItem;

        public override void ActivePopup()
        {
            //SetProfile();
            PopUpSequence(true);
        }

        // public async void SetProfile()
        // {
        //     await NetworkManager.Instance.GetUserProfileList(SetOwnedProfile, Failed);
        //     SwitchProperty(true);
        //     PopUpSequence(true);
        // }

        // public void SetOwnedProfile(GetUserProfileList data)
        // {
        //     int[] userProfileIds = data.userProfileIds;

        //     foreach (var temp in DataManager.Instance.dic_userProfileData)
        //     {
        //         DataManager.Instance.dic_userProfileData[temp.Key].isOwned = false;
        //     }

        //     for (int i = 0; i < userProfileIds.Length; i++)
        //     {
        //         int tempIdx = userProfileIds[i];
        //         DataManager.Instance.dic_userProfileData[tempIdx].isOwned = true;
        //     }

        //     List<int> tempList = new();

        //     foreach (var temp in DataManager.Instance.dic_userProfileData)
        //     {
        //         tempList.Add(temp.Key);
        //     }

        //     tempList.Sort();

        //     for (int i = 0; i < tempList.Count; i++)
        //     {
        //         UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[tempList[i]];
        //         profileImageItems[i].gameObject.SetActive(true);
        //         profileImageItems[i].SelectedProfile(tempList[i] == focusProfileIdx);
        //         profileImageItems[i].Initiailize(tempList[i], userProfileData.isOwned);
        //     }
        // }

        public override void InActivePopup()
        {

        }

        public string RankInfo(int rank)
        {
            string info = rank == 0 ? "-" : rank.ToString("N0");
            return info;
        }

        public void OnClick_ProfileType()
        {
            //todo 2.9.0이전 버전에는 위클리 데일리 스왑이있었는데 현재는 사라짐
            //todo 위클리만 유지
            // focusType = focusType switch
            // {
            //     LeaderBoardType.SCORE_DAILY => LeaderBoardType.WAVE_WEEKLY,
            //     LeaderBoardType.WAVE_WEEKLY => LeaderBoardType.SCORE_DAILY,
            //     _ => LeaderBoardType.SCORE_DAILY
            // };
            //
            // string key = focusType switch
            // {
            //     LeaderBoardType.SCORE_DAILY => "UI_DailyStat",
            //     LeaderBoardType.WAVE_WEEKLY => "UI_WeeklyStat",
            //     _ => "UI_DailyStat"
            // };


            //todo OnlyWeeKly;
            string key = "UI_WeeklyStat";
            focusType = LeaderBoardType.WAVE_WEEKLY;

            text_stat.text = Util.LanguageManager.Instance.GetStringData(key);
            ResponseProfileData data = dic_profileData[focusType];
            SetProfileData(data);
        }

        public void Success(TotalProfileData data)
        {
            dic_profileData = new Dictionary<LeaderBoardType, ResponseProfileData>();
            //todo Daily삭제되면 타입 줄이기
            for (int i = 0; i < data.userProfile.Length; i++)
            {
                //todo 대전모드를 대비해서 일단 임시용 코드 (이전에 데일리가 있어서 이런로직이였음)
                if (data.userProfile[i].leaderBoardType.Contains("WAVE_WEEKLY"))
                {
                    LeaderBoardType leaderBoardType =
                        (LeaderBoardType)Enum.Parse(typeof(LeaderBoardType), data.userProfile[i].leaderBoardType);
                    dic_profileData.Add(leaderBoardType, data.userProfile[i]);
                }
            }

            //todo OnlyWeekly
            focusType = LeaderBoardType.WAVE_WEEKLY;
            string key = "UI_WeeklyStat";

            // string key = focusType switch
            // {
            //     LeaderBoardType.SCORE_DAILY => "UI_DailyStat",
            //     LeaderBoardType.WAVE_WEEKLY => "UI_WeeklyStat",
            //     _ => "UI_WeeklyStat"
            // };

            text_stat.text = Util.LanguageManager.Instance.GetStringData(key);
            SetProfileData(dic_profileData[LeaderBoardType.WAVE_WEEKLY]);

            //todo 데일리 리더보드 삭제
            // SetProfileData(dic_profileData[LeaderBoardType.SCORE_DAILY]);
            // focusType = LeaderBoardType.SCORE_DAILY;
        }

        public void SetProfileData(ResponseProfileData data)
        {
            string nickName = data.nickname;
            if (data.ranking.first > 0)
            {
                //isGoldMedal = true;
                text_nickName.text = $"{nickName}";
                text_nickName.rectTransform.localPosition = new Vector3(10, 0, 0);
            }
            else
            {
                text_nickName.text = nickName;
                text_nickName.rectTransform.localPosition = Vector3.zero;
            }
            //text_UserNickname.text = nickName;

            text_gold.text = RankInfo(data.ranking.first);
            text_silver.text = RankInfo(data.ranking.second);
            text_bronze.text = RankInfo(data.ranking.third);

            text_playCount.text = RankInfo(data.playCount);
            text_bestWave.text = RankInfo(data.bestWave);
            text_bestBossLevel.text = data.bestBossLevel == 0 ? "-" : $"Lv.{data.bestBossLevel}";
            text_bestScore.text = RankInfo(data.bestScore);

            if (data.bestCharacter == null || data.bestCharacter.Length == 0)
            {
                objectFriendlyCharacter.SetActive(false);
                objectNullFriendlyCharacter.SetActive(true);
            }
            else
            {
                objectFriendlyCharacter.SetActive(true);
                objectNullFriendlyCharacter.SetActive(false);

                for (int i = 0; i < characterCards.Length; i++)
                {
                    CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.bestCharacter[i].characterId];
                    characterCards[i].Initialize(characterData);
                    characterCards[i].text_Level.text = $"Lv.{data.bestCharacter[i].classLevel}";
                }
            }
        }

        public void Failed()
        {
            Debug.Log("Set profile Failed");
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });

            button_editProfileDetail.onClick.AddListener(() =>
            {
                if (friendItem == null)
                    OnClick_EditDetailProfile();
            });

            button_AddFriend.button.onClick.AddListener(async () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                await NetworkManager.Instance.SendFriendRequest(friendItem.friendData.userId, () =>
                {
                    SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Success"));
                    button_AddFriend.gameObject.SetActive(false);
                    PopupManager.Instance.GetPopUp<FriendPopup>("friend").OnSendFriendRequest(friendItem.friendData.userId);
                }, null);
            });

            button_DeleteFriend.button.onClick.AddListener(async () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                await NetworkManager.Instance.DeleteFriend(friendItem.friendData.userId, () =>
                {
                    button_DeleteFriend.gameObject.SetActive(false);
                    button_AddFriend.gameObject.SetActive(true);
                    PopupManager.Instance.GetPopUp<FriendPopup>("friend").OnDeleteFriend(friendItem.friendData.userId);
                }, null);
            });

            //todo Daily삭제되면 주석처리 할것 / 해당버튼은 데일리 <-> 위클리 변경하는 버튼
            // button_switching.onClick.AddListener(() =>
            // {
            //     OnClick_ProfileType();
            // });
            //todo Daily삭제되면 주석처리 해제 할것
            button_switching.gameObject.SetActive(false);


        }

        public void ShowMyProfile()
        {
            text_Title.SetEntry("UI_Profile");
            friendItem = null;
            button_AddFriend.gameObject.SetActive(false);
            // text_Title.RefreshString();
            int[] userProfileIds = UserInfoManager.Instance.userProfileImageIds;
            focusProfileIdx = UserInfoManager.Instance.userState.equippedProfileId;
            UserProfileData data = DataManager.Instance.dic_userProfileData[focusProfileIdx];

            SetProfileImage(data);

            _ = NetworkManager.Instance.GetTotalUserProfileDetail(UserInfoManager.Instance.userId, Success, Failed);
        }
        public void ShowOtherProfile(FriendItem _friendItem, int otherEquippedProfileId, bool isFriend)
        {
            friendItem = _friendItem;
            text_Title.SetEntry("UI_Friend");
            button_AddFriend.gameObject.SetActive(!isFriend && !friendItem.pendingFromUser);
            button_DeleteFriend.gameObject.SetActive(isFriend);
            // text_Title.RefreshString();
            UserProfileData data = DataManager.Instance.dic_userProfileData[otherEquippedProfileId];
            SetProfileImage(data);

            _ = NetworkManager.Instance.GetFriendProfile(friendItem.friendData.userId, Success, null);
        }


        public void SetNickname(string nickname)
        {
            text_nickName.text = isGoldMedal ? $"{nickname}<sprite=9>" : nickname;
            //text_UserNickname.text = nickname;
        }

        public void OnClick_EditNickname()
        {
            EditNickNamePopup editNickNamePopup = PopupManager.Instance.GetPopUp<EditNickNamePopup>("editNickname");
            editNickNamePopup.ActivePopup();
            editNickNamePopup.PopUpSequence(true);
        }

        public void OnClick_EditDetailProfile()
        {
            //SwitchProperty(false);
            ProfilePopup profilePopup = PopupManager.Instance.GetPopUp<ProfilePopup>("profile");
            profilePopup.ActivePopup();
        }

        public void OnClick_Confirm()
        {
            //SwitchProperty(true);
        }

        public void SwitchProperty(bool isActive)
        {
            objectInfo.SetActive(isActive);
        }

        public void SetProfileImage(UserProfileData userProfileData)
        {
            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    {
                        Color color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                        image_limited.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_SeletedProfile.gameObject.SetActive(true);

                        image_backGround.color = color;
                        image_SeletedProfile.sprite = userProfileData.sprite_image;
                    }
                    break;

                case ProfileType.Skin:
                    {
                        SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                        Color color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                        image_limited.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_SeletedProfile.gameObject.SetActive(true);

                        image_backGround.color = color;
                        image_SeletedProfile.sprite = userProfileData.sprite_image;
                    }
                    break;

                default:
                    {
                        image_backGround.gameObject.SetActive(false);

                        image_SeletedProfile.gameObject.SetActive(false);

                        image_limited.gameObject.SetActive(true);
                        image_limited.sprite = userProfileData.sprite_image;
                    }
                    break;
            }
        }
    }
}

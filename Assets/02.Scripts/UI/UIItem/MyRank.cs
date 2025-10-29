using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.Network;
using Newtonsoft.Json;

namespace Framework.UI
{
    public class MyRank : MonoBehaviour
    {
        [SerializeField] private Image image_Profile;
        [SerializeField] private Image image_limitedProfile;
        [SerializeField] private Image image_backGround;
        [SerializeField] private TextMeshProUGUI text_Rank;
        [SerializeField] private TextMeshProUGUI text_RankTier;
        [SerializeField] private TextMeshProUGUI text_Go;
        [SerializeField] private TextMeshProUGUI text_NickName;
        [SerializeField] private GameObject go_LP;
        // public GameObject goIconObject;
        // public GameObject swapIconObject;
        [SerializeField] private Button button_myRank;

        private void Start()
        {
            button_myRank.onClick.AddListener(() =>
            {
                RankingScreen.Instance.RankDetail(UserInfoManager.Instance.userId);
            });
        }

        public void SetMyRank(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                transform.DOLocalMoveY(0, 0.05f);
            }
            else
            {
                MyLeaderBoardRankData myRankData = JsonUtility.FromJson<MyLeaderBoardRankData>(json);

                UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[myRankData.profileId];

                switch (userProfileData.profileType)
                {
                    case ProfileType.Character:
                        image_Profile.gameObject.SetActive(true);
                        image_limitedProfile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_Profile.sprite = userProfileData.sprite_image;

                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                        break;

                    case ProfileType.Skin:
                        image_Profile.gameObject.SetActive(true);
                        image_limitedProfile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_Profile.sprite = userProfileData.sprite_image;

                        SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                        break;

                    default:
                        image_Profile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(false);
                        image_limitedProfile.gameObject.SetActive(true);
                        image_limitedProfile.sprite = userProfileData.sprite_image;
                        break;
                }

                text_Rank.text = $"{myRankData.rank}";
                text_NickName.text = myRankData.nickname;
                string value = $"<sprite=12>{myRankData.bestWave}";

                text_Go.text = value;
                transform.DOLocalMoveY(233, 0.05f);
                go_LP.SetActive(false);
                text_RankTier.gameObject.SetActive(false);
            }
        }
        public void SetMyRank(MyBattleLeaderboardInfo myRankData)
        {
            Debug.Log($"Set My Rank {JsonConvert.SerializeObject(myRankData)}");
            if (myRankData == null)
            {
                Debug.Log($"Set My Rank 1");
                transform.DOLocalMoveY(0, 0.05f);
            }
            else
            {
                Debug.Log($"Set My Rank 2");
                UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[UserInfoManager.Instance.userState.equippedProfileId];

                switch (userProfileData.profileType)
                {
                    case ProfileType.Character:
                        image_Profile.gameObject.SetActive(true);
                        image_limitedProfile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_Profile.sprite = userProfileData.sprite_image;

                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                        break;

                    case ProfileType.Skin:
                        image_Profile.gameObject.SetActive(true);
                        image_limitedProfile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(true);
                        image_Profile.sprite = userProfileData.sprite_image;

                        SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                        break;

                    default:
                        image_Profile.gameObject.SetActive(false);
                        image_backGround.gameObject.SetActive(false);
                        image_limitedProfile.gameObject.SetActive(true);
                        image_limitedProfile.sprite = userProfileData.sprite_image;
                        break;
                }
                var find = RankingScreen.Instance.activeRankinfoItems.Find(x => x.userId == myRankData.userId);
                text_Rank.text = find != null ? find.text_Rank.text : "99+";
                text_NickName.text = UserInfoManager.Instance.nickname;

                string value = $"{(int)myRankData.totalLp}";
                if ((int)DataManager.Instance.GetRankTierConfig(myRankData.finalRank).tierType < (int)RankTierType.RANKTIER_5)
                    value = $"{(int)myRankData.totalLp % 100}";
                else
                    value = $"{(int)myRankData.totalLp}";
                text_RankTier.text = $"{DataManager.Instance.GetRankTierConfig(myRankData.finalRank).description}";
                text_RankTier.gameObject.SetActive(true);
                go_LP.SetActive(true);

                text_Go.text = value;
                transform.DOLocalMoveY(233, 0.05f);
            }
        }

        // public void SetDailyMyRank(string json)
        // {
        //     if (string.IsNullOrEmpty(json))
        //     {
        //         transform.DOLocalMoveY(0, 0.05f);
        //     }
        //     else
        //     {
        //         UserRankInfo userRankInfo = JsonUtility.FromJson<UserRankInfo>(json);

        //         UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[userRankInfo.profileId];

        //         switch (userProfileData.profileType)
        //         {
        //             case ProfileType.Character:
        //                 image_Profile.gameObject.SetActive(true);
        //                 image_limitedProfile.gameObject.SetActive(false);
        //                 image_backGround.gameObject.SetActive(true);
        //                 image_Profile.sprite = userProfileData.sprite_image;

        //                 image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
        //                 break;

        //             case ProfileType.Skin:
        //                 image_Profile.gameObject.SetActive(true);
        //                 image_limitedProfile.gameObject.SetActive(false);
        //                 image_backGround.gameObject.SetActive(true);
        //                 image_Profile.sprite = userProfileData.sprite_image;

        //                 SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
        //                 image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
        //                 break;

        //             default:
        //                 image_Profile.gameObject.SetActive(false);
        //                 image_backGround.gameObject.SetActive(false);
        //                 image_limitedProfile.gameObject.SetActive(true);
        //                 image_limitedProfile.sprite = userProfileData.sprite_image;
        //                 break;
        //         }

        //         text_Rank.text = $"{userRankInfo.rank}";
        //         text_NickName.text = userRankInfo.nickname;

        //         // DateTime utcNow = DateTime.UtcNow.Date;
        //         // DateTime dataDate = DateTime.Parse(userRankInfo.aggregatedDate);

        //         // int compare = DateTime.Compare(utcNow, dataDate);

        //         // bool isPrevious = compare > 0;
        //         // string value = isPrevious ? userRankInfo.rewardTik.ToString("N0", new CultureInfo("en-US")) : userRankInfo.obtainedGo.ToString("N0", new CultureInfo("en-US"));
        //         // string symbolKey = isPrevious ? "<sprite=13>" : "<sprite=0>";
        //         // text_Go.text = $"{symbolKey}{value}";
        //         string value = $"<sprite=12>{userRankInfo.bestWave}";

        //         text_Go.text = value;

        //         transform.DOLocalMoveY(233, 0.05f);
        //     }
        // }

        public void Hide()
        {
            transform.DOLocalMoveY(0, 0.05f);
        }
    }
}
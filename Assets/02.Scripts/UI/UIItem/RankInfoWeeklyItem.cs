using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class RankInfoWeeklyItem : MonoBehaviour
    {
        [SerializeField] private Image image_RankBadge;
        [SerializeField] private Image image_Profile;
        [SerializeField] private Image image_limitedProfile;
        [SerializeField] private TextMeshProUGUI text_Nickname;
        [SerializeField] private TextMeshProUGUI text_GoValue;
        [SerializeField] private Button button_ViewDetail;

        private int userId;

        private void Start()
        {
            button_ViewDetail.onClick.AddListener(() => OnClick_ViewDetail());
        }
        // WeeklyRankInfo
        public void Initialize(LeaderBoardRankInfo data, int rank)
        {
            userId = data.userId;
            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];
            
            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_Profile.gameObject.SetActive(true);

                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                case ProfileType.Skin:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_Profile.gameObject.SetActive(true);

                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                default:
                    image_limitedProfile.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(false);

                    image_limitedProfile.sprite = userProfileData.sprite_image;
                    break;
            }

            string[] temp = data.nickname.Split('@');
            text_Nickname.text = temp[0];

            bool isMine = UserInfoManager.Instance.userId == data.userId.ToString();
            text_GoValue.text = $"<sprite=12>{data.bestWave}";
            SetCellColorInfo(rank, isMine);
        }
        // DailyRankInfo
        // public void Initialize(RankInfo data, int rank, bool isPrevious)
        // {
        //     userId = data.userId;

        //     UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];
        //     //CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.profileId];

        //     switch (userProfileData.profileType)
        //     {
        //         case ProfileType.Character:
        //             image_limitedProfile.gameObject.SetActive(false);
        //             image_backGround.gameObject.SetActive(true);
        //             image_Profile.gameObject.SetActive(true);

        //             image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
        //             image_Profile.sprite = userProfileData.sprite_image;
        //             break;

        //         case ProfileType.Skin:
        //             image_limitedProfile.gameObject.SetActive(false);
        //             image_backGround.gameObject.SetActive(true);
        //             image_Profile.gameObject.SetActive(true);

        //             SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
        //             image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;

        //             image_Profile.sprite = userProfileData.sprite_image;
        //             break;

        //         default:
        //             image_limitedProfile.gameObject.SetActive(true);
        //             image_backGround.gameObject.SetActive(false);
        //             image_Profile.gameObject.SetActive(false);

        //             image_limitedProfile.sprite = userProfileData.sprite_image;
        //             break;
        //     }

        //     string[] temp = data.nickname.Split('@');
        //     text_Nickname.text = temp[0];

        //     string valueInfo = isPrevious ? data.rewardTik.ToString("N0", new CultureInfo("en-US")) : data.obtainedGo.ToString("N0", new CultureInfo("en-US"));

        //     text_GoValue.text = isPrevious ? $"<sprite=13>{valueInfo}" : $"<sprite=0>{valueInfo}";

        //     bool isMine = UserInfoManager.Instance.userId == data.userId.ToString();
        //     SetCellColorInfo(rank, isMine);
        // }

        public void OnClick_ViewDetail()
        {
            Debug.Log("OnClick Detail");

            RankingScreen.Instance.RankDetail(userId.ToString());
        }

        public void SetCellColorInfo(int rank, bool isMine)
        {
            bool isPodium = rank <= 3;

            image_RankBadge.gameObject.SetActive(isPodium);
            
            if (isPodium)
            {
                RankingBoxProperty data = DataManager.Instance.uiPropertyData.dic_RankingBoxProperties[(RankingType)rank];
                image_RankBadge.sprite = data.sprite_Icon;
            }
        }
    }
}

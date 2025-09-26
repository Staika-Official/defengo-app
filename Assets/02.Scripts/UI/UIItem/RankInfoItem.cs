using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using UnityEngine.UI.Extensions;
using System;
using System.Globalization;

namespace Framework.UI
{
    [System.Serializable]
    public class LayerGroup
    {
        public Transform group;
        public Image[] layers;
    }

    public class RankInfoItem : MonoBehaviour
    {
        public Image image_RankBadge;
        public Image image_Profile;
        public Image image_limitedProfile;
        public Image image_backGround;
        public TextMeshProUGUI text_Rank;
        public TextMeshProUGUI text_Nickname;
        public TextMeshProUGUI text_GoValue;
        public GameObject taikaIcon;
        public GameObject goIcon;

        public Button button_ViewDetail;
        
        [SerializeField] private GameObject go_Profile;
        [SerializeField] private Image image_Rank;
        [SerializeField] private List<Sprite> sprite_Ranks;

        public int userId;

        public LayerGroup[] layerGroups;

        public Gradient2 gradient;

        private void Start()
        {
            button_ViewDetail.onClick.AddListener(() => OnClick_ViewDetail());
        }

        public void Initialize(LeaderBoardRankInfo data, int rank)
        {
            userId = data.userId;
            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];
            //CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.profileId];

            image_Rank.gameObject.SetActive(false);
            go_Profile.SetActive(true);

            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                case ProfileType.Skin:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;

                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                default:
                    image_limitedProfile.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(false);
                    image_Profile.gameObject.SetActive(false);

                    image_limitedProfile.sprite = userProfileData.sprite_image;
                    break;
            }

            text_Rank.text = $"{rank}";
            text_Rank.color = Color.white;
            string[] temp = data.nickname.Split('@');
            text_Nickname.text = temp[0];

            bool isMine = UserInfoManager.Instance.userId == data.userId.ToString();
            text_GoValue.text = $"<sprite=12>{data.bestWave}";
            SetCellColorInfo(rank, isMine);
        }
        public void Initialize(BattleLeaderboardInfo data, int rank)
        {
            userId = data.userId;
            //CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.profileId];

            image_Rank.gameObject.SetActive(true);
            image_Rank.sprite = sprite_Ranks[(int)DataManager.Instance.GetRankTierConfig(data.finalRank).tierType];
            go_Profile.SetActive(false);

            text_Rank.text = $"{rank}";
            text_Rank.color = Color.white;
            string[] temp = data.nickname.Split('@');
            text_Nickname.text = temp[0];

            bool isMine = UserInfoManager.Instance.userId == data.userId.ToString();
            text_GoValue.text = $"{(int)data.lp} lp";
            SetCellColorInfo(rank, isMine);
        }

        public void Initialize(RankInfo data, int rank, bool isPrevious)
        {
            userId = data.userId;

            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];
            //CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.profileId];

            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                case ProfileType.Skin:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;

                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                default:
                    image_limitedProfile.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(false);
                    image_Profile.gameObject.SetActive(false);

                    image_limitedProfile.sprite = userProfileData.sprite_image;
                    break;
            }

            text_Rank.text = $"{rank}";
            text_Rank.color = Color.white;
            string[] temp = data.nickname.Split('@');
            text_Nickname.text = temp[0];

            string valueInfo = isPrevious ? data.rewardTik.ToString("N0", new CultureInfo("en-US")) : data.obtainedGo.ToString("N0", new CultureInfo("en-US"));

            text_GoValue.text = isPrevious ? $"<sprite=13>{valueInfo}" : $"<sprite=0>{valueInfo}";

            bool isMine = UserInfoManager.Instance.userId == data.userId.ToString();
            SetCellColorInfo(rank, isMine);
        }

        public void OnClick_ViewDetail()
        {
            Debug.Log("OnClick Detail");

            RankingScreen.Instance.RankDetail(userId.ToString());
        }

        public void SetCellColorInfo(int rank, bool isMine)
        {
            for (int i = 0; i < layerGroups.Length; i++)
            {
                layerGroups[i].group.gameObject.SetActive(false);
            }

            bool isPodium = rank <= 3;

            image_RankBadge.gameObject.SetActive(isPodium);
            gradient.enabled = isPodium;
            text_Rank.gameObject.SetActive(!isPodium);
            if (rank <= 3)
            {
                RankingBoxProperty data = DataManager.Instance.uiPropertyData.dic_RankingBoxProperties[(RankingType)rank];

                LayerGroup layerGroup = layerGroups[0];
                layerGroup.group.gameObject.SetActive(true);
                image_RankBadge.sprite = data.sprite_Icon;
                for (int i = 0; i < layerGroup.layers.Length; i++)
                {
                    layerGroup.layers[i].color = data.color_Layer[i];
                }
                gradient.EffectGradient = new UnityEngine.Gradient()
                {
                    colorKeys = new GradientColorKey[]
                    {
                        new GradientColorKey(data.gradientColorKeys[0], 0), new GradientColorKey(data.gradientColorKeys[1], 1)
                    }
                };
            }
            else
            {
                text_Rank.text = $"{rank}";
                RankingType rankingType = isMine ? RankingType.MYRANK : RankingType.COMMON;

                LayerGroup layerGroup = layerGroups[1];
                layerGroup.group.gameObject.SetActive(true);
                RankingBoxProperty data = DataManager.Instance.uiPropertyData.dic_RankingBoxProperties[rankingType];
                for (int i = 0; i < layerGroup.layers.Length; i++)
                {
                    layerGroup.layers[i].color = data.color_Layer[i];
                }
            }
        }
    }
}

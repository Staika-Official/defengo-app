using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class RandomReward : MonoBehaviour
    {
        private enum rewardType
        {
            Card,
            Item,
            Skin,
            Profile

        }


        [Header("Character")]
        public GameObject object_CharacterIcon;
        public Image image_Frame;
        public Image image_Shadow;
        public Image image_UpperShadow;
        public Image image_CharacterPortrait;


        [Header("Item")]
        public GameObject object_ItemIcon;
        public Image image_RewardIcon;

        [Header("Skin")]
        public Image image_SkinPortrait;

        [Header("Profile")]
        public GameObject object_ProfileIcon;
        public Image image_ProfilePortrait;
        public Image image_ProfileBackground;

        [Header("Common")]
        public GameObject newCharacter;
        public GameObject epicEffect;

        public TextMeshProUGUI text_ItemAmount;

        public Animator animator;




        public UserItem userItemData { get; private set; }

        public void Initialize(CharacterData data, int amount)
        {
            userItemData = null;

            SetFrameInner(rewardType.Card);

            CharacterCardInfo cardInfo = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade];
            text_ItemAmount.text = $"x{amount}";
            image_Frame.sprite = cardInfo.sprite_Frame;
            image_CharacterPortrait.sprite = data.sprite_ChracterPortrait;
            image_Shadow.color = cardInfo.color_Shadow;
            image_UpperShadow.color = cardInfo.disableShadowColor;
            newCharacter.SetActive(false);

            bool isEpicGrade = (int)data.characterGrade >= 6;
            epicEffect.SetActive(isEpicGrade);

            if (data.isNew)
            {
                newCharacter.SetActive(true);
                data.isNew = false;
            }
            animator.SetTrigger("CardEnter");
        }

        public void Initialize(UserItem data)
        {
            userItemData = data;

            SetFrameInner(rewardType.Item);

            image_RewardIcon.sprite = DataManager.Instance.uiPropertyData.dic_RandomRewardIcon[data.item];

            text_ItemAmount.text = $"x{data.quantity}";

            //특정 등급이상에서만 켜주고싶은데 어찌해야할지 못정함 준영대리님한테 여쭤봐야함
            //bool isEpicGrade = data.item.Contains("EPIC") || data.item.Contains("LEGEND");

            epicEffect.SetActive(true);
            newCharacter.SetActive(false);
        }

        public void InitializeSkin(int skin_id)
        {
            userItemData = null;

            SetFrameInner(rewardType.Skin);

            image_SkinPortrait.sprite = DataManager.Instance.CharacterResourceData.dic_Character[skin_id].SkinPortrait;

            text_ItemAmount.text = "";

            epicEffect.SetActive(false);
            newCharacter.SetActive(false);
        }

        public void InitializeProfile(int profile_id)
        {
            userItemData = null;

            SetFrameInner(rewardType.Profile);
            UserProfileData profile_data = DataManager.Instance.dic_userProfileData[profile_id];
            image_ProfilePortrait.sprite = profile_data.sprite_image;

            switch (profile_data.profileType)
            {
                case ProfileType.Character:
                    image_ProfilePortrait.sprite = profile_data.sprite_image;
                    image_ProfileBackground.gameObject.SetActive(true);
                    image_ProfileBackground.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[profile_data.characterGrade].color_BackGround;
                    break;

                case ProfileType.Skin:
                    image_ProfilePortrait.sprite = profile_data.sprite_image;
                    image_ProfileBackground.gameObject.SetActive(true);
                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == profile_data.profileIndex).skin_grade_type;
                    image_ProfileBackground.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                    break;

                default:
                    image_ProfilePortrait.sprite = profile_data.sprite_image;
                    image_ProfileBackground.gameObject.SetActive(false);
                    break;
            }


            text_ItemAmount.text = "";

            epicEffect.SetActive(false);
            newCharacter.SetActive(false);
        }

        private void SetFrameInner(rewardType type)
        {
            object_CharacterIcon.SetActive(type == rewardType.Card);
            object_ItemIcon.SetActive(type == rewardType.Item);
            image_SkinPortrait.gameObject.SetActive(type == rewardType.Skin);
            object_ProfileIcon.SetActive(type == rewardType.Profile);
        }
    }
}

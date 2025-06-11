using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;

namespace Framework.UI
{
    public class ProfileImageItem : MonoBehaviour
    {
        public int profileIdx;
        public Button button_Select;
        public Image image_limited;
        public Image image_ProfileImage;
        public Image image_Frame;
        public Image image_backGround;
        public GameObject image_select;
        public GameObject equipBadge;

        private void Start()
        {
            button_Select.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_ImageSelect();
            });
        }

        public void Initiailize(int idx)
        {
            profileIdx = idx;
            UserProfileData data = DataManager.Instance.dic_userProfileData[idx];

            switch (data.profileType)
            {
                case ProfileType.Character:
                    image_limited.gameObject.SetActive(false);
                    image_ProfileImage.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(true);

                    image_ProfileImage.sprite = data.sprite_image;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_BackGround;
                    break;

                case ProfileType.Skin:
                    image_limited.gameObject.SetActive(false);
                    image_ProfileImage.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(true);

                    image_ProfileImage.sprite = data.sprite_image;
                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == data.profileIndex).skin_grade_type;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                    break;

                default:
                    image_limited.gameObject.SetActive(true);
                    image_ProfileImage.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(false);

                    image_limited.sprite = data.sprite_image;
                    break;
            }
        }

        public void Initiailize(int idx, bool isOwned)
        {
            profileIdx = idx;
            UserProfileData data = DataManager.Instance.dic_userProfileData[idx];

            if (data.profileType == ProfileType.Character || data.profileType == ProfileType.Skin)
            {
                image_limited.gameObject.SetActive(false);
                image_ProfileImage.gameObject.SetActive(true);
                image_backGround.gameObject.SetActive(true);

                image_ProfileImage.sprite = data.sprite_image;
                if (isOwned)
                {
                    if (data.profileType == ProfileType.Character)
                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_BackGround;
                    else
                    {
                        SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == data.profileIndex).skin_grade_type;
                        image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;
                    }

                    image_Frame.color = Color.black;
                    image_ProfileImage.color = Color.white;
                }
                else
                {
                    image_backGround.color = DataManager.Instance.uiPropertyData.TxtColor["maskingFrame_Profile_Disable_Color"];
                    image_Frame.color = DataManager.Instance.uiPropertyData.TxtColor["ImgFrame_Profile_Disable_Color"];
                    image_ProfileImage.color = DataManager.Instance.uiPropertyData.TxtColor["Img_Profile_Disable_Color"];
                }
            }
            else
            {
                image_limited.gameObject.SetActive(true);
                image_ProfileImage.gameObject.SetActive(false);
                image_backGround.gameObject.SetActive(false);

                image_limited.sprite = data.sprite_image;
                if (isOwned)
                {
                    image_limited.color = Color.white;
                }
                else
                {
                    Color limitedColor = DataManager.Instance.uiPropertyData.TxtColor["LimitedProfile_Disable_Color"];
                    image_limited.color = limitedColor;
                }
            }

            //button_Select.interactable = isOwned;

        }

        public void SelectedProfile(bool isSelected)
        {
            image_select.SetActive(isSelected);

        }

        public void EquipProfile(bool isEquip)
        {
            image_select.SetActive(isEquip);
            equipBadge.SetActive(isEquip);
        }

        public void OnClick_ImageSelect()
        {
            // Debug.Log("Onclick Profile");
            ProfilePopup profilePopup = PopupManager.Instance.GetPopUp<ProfilePopup>("profile");
            profilePopup.SetProfileImages(profileIdx);
            //_ = NetworkManager.Instance.UpdateUserProfile(profileIdx, PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail").SetProfileImages);
        }
    }
}

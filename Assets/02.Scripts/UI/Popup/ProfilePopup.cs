using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Sound;
using DG.Tweening;
using Framework.Network;
using Framework.Util;

namespace Framework.UI
{
    public class ProfilePopup : PopupTemplate
    {
        public Image image_limited;
        public Image image_SeletedProfile;
        public Image image_backGround;
        public TextMeshProUGUI text_UserNickname;
        public Button button_EditNickName;
        public Button button_Confirm;
        public GameObject button_Unconfirm;
        public GameObject ts_ProfileImg;
        public TextMeshProUGUI text_GetInfo;
        public int focusProfileIdx;
        public ProfileImageItem profileImageItemSrc;
        public Transform profileParent;

        private List<ProfileImageItem> profileImageItems = new();

        public override void ActivePopup()
        {
            if (uiSequences != null)
            {
                uiSequences.Open_Popup();
            }

            focusProfileIdx = UserInfoManager.Instance.userState.equippedProfileId;
            UserProfileData data = DataManager.Instance.dic_userProfileData[focusProfileIdx];

            SetProfileImages(focusProfileIdx);

            PopUpSequence(true);
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_EditNickName.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_EditNickname();
            });
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });

            button_Confirm.onClick.AddListener(() =>
            {
                ConfirmProfileImage();
                PopUpSequence(false);
            });

            text_UserNickname.text = UserInfoManager.Instance.nickname;
            int[] userProfileIds = UserInfoManager.Instance.userProfileImageIds;
            focusProfileIdx = UserInfoManager.Instance.userState.equippedProfileId;

            // CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)focusProfileIdx];
            // image_SeletedProfile.sprite = data.sprite_ChracterPortrait;
            // image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_BackGround;
            UserProfileData data = DataManager.Instance.dic_userProfileData[focusProfileIdx];

            SetProfileImage(data);

            for (int i = 0; i < userProfileIds.Length; i++)
            {
                int tempIdx = userProfileIds[i];
                DataManager.Instance.dic_userProfileData[tempIdx].isOwned = true;
            }

            List<int> tempList = new();

            profileImageItemSrc.gameObject.SetActive(true);
            foreach (var temp in DataManager.Instance.dic_userProfileData)
            {
                tempList.Add(temp.Key);
                ProfileImageItem item = Instantiate<ProfileImageItem>(profileImageItemSrc, profileParent);
                profileImageItems.Add(item);
            }
            profileImageItemSrc.gameObject.SetActive(false);

            tempList.Sort();


            for (int i = 0; i < tempList.Count; i++)
            {
                UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[tempList[i]];
                profileImageItems[i].gameObject.SetActive(true);
                profileImageItems[i].EquipProfile(tempList[i] == focusProfileIdx);
                profileImageItems[i].Initiailize(tempList[i], userProfileData.isOwned);
            }
        }

        public void SetNickname(string nickname)
        {
            text_UserNickname.text = nickname;
        }

        public void OnClick_EditNickname()
        {
            EditNickNamePopup editNickNamePopup = PopupManager.Instance.GetPopUp<EditNickNamePopup>("editNickName");
            editNickNamePopup.ActivePopup();
            editNickNamePopup.PopUpSequence(true);
        }

        public void SetProfileImages(int focusProfileIdx)
        {
            this.focusProfileIdx = focusProfileIdx;
            UserProfileData data = DataManager.Instance.dic_userProfileData[focusProfileIdx];
            if (data.isOwned)
                SetProfileImage(data);

            button_Confirm.gameObject.SetActive(data.isOwned);
            button_Unconfirm.SetActive(!data.isOwned);

            for (int i = 0; i < profileImageItems.Count; i++)
            {
                bool isFocus = focusProfileIdx == (int)profileImageItems[i].profileIdx;
                if (data.isOwned)
                    profileImageItems[i].EquipProfile(isFocus);
                else
                    profileImageItems[i].SelectedProfile(isFocus);
            }

            if (data.isOwned)
            {
                ts_ProfileImg.transform.DOScale(1.6f, 0f).SetAutoKill(true);
                ts_ProfileImg.transform.DOScale(1.8f, 0.1f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo).SetAutoKill(true);
            }

            text_GetInfo.text = LanguageManager.Instance.GetStringData(string.Format("UI_Profile_{0}", focusProfileIdx));
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

        public void ConfirmProfileImage()
        {
            _ = NetworkManager.Instance.UpdateUserProfile(focusProfileIdx, SetGlobalProfileImage, FailUpdateUserProfile);
        }

        public void SetGlobalProfileImage(int focusProfileIdx)
        {
            UserProfileData data = DataManager.Instance.dic_userProfileData[focusProfileIdx];
            SetProfileImage(data);
            PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail").SetProfileImage(data);
            LobbyManager.Instance.SetProfileImage(data);
        }

        private void FailUpdateUserProfile(long errorCode)
        {
            if (errorCode == 500)
            {
                SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                popup.SetNoticeMessage("NOT_OWNED_PROFILE");
            }

        }
    }
}


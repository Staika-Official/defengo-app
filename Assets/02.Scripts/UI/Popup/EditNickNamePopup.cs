using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;


namespace Framework.UI
{
    public class EditNickNamePopup : PopupTemplate
    {
        public string editNickname;
        public TMP_InputField inputField_EditNickname;
        public TextMeshProUGUI text_Status;

        public Button button_TikConfim;
        public Button button_Confirm;
        public Button button_Validation;

        public bool isPossibleChangeNickname = false;

        public string warnColor = "#FF122A";

        public GameObject confirm_Message;
        //public GameObject confirm_Taika;

        public GameObject taikaobject;
        public GameObject freeObject;

        public GameObject activeObject;
        public GameObject inactiveObject;

        public bool isFirst = false;

        public override void ActivePopup()
        {
            inputField_EditNickname.text = LanguageManager.Instance.GetStringData("UI_NicknameDesc");
            text_Status.text = "";

            if(UserInfoManager.Instance.userNicknameInfo.changeableCount <= 0)
            {
                taikaobject.SetActive(true);
                freeObject.SetActive(false);

                bool isActive = UserInfoManager.Instance.gemValue >= 100;

                activeObject.SetActive(isActive);
                inactiveObject.SetActive(!isActive);
                button_Confirm.interactable = false;
                button_TikConfim.interactable = true;
                isFirst = false;
            }
            else
            {
                freeObject.SetActive(true);
                taikaobject.SetActive(false);

                button_Confirm.interactable = true;
                button_TikConfim.interactable = false;
                isFirst = true;
            }
        }

        public override void InActivePopup()
        {
                
        }

        public override void Initialize()
        {
            inputField_EditNickname.characterLimit = 10;

            isPossibleChangeNickname = false;
            button_Confirm.interactable = isPossibleChangeNickname;

            button_Confirm.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Confirm();
            });

            button_TikConfim.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Confirm();
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });

            inputField_EditNickname.onValueChanged.AddListener((value) =>
            {
                text_Status.text = "";
                isPossibleChangeNickname = false;
                button_Confirm.interactable = isPossibleChangeNickname;
            });

            button_Validation.onClick.AddListener(() =>
            {
                OnClick_Validation();
            });
        }

        public void OnFailedValid(string errorMessage)
        {
            text_Status.text = LanguageManager.Instance.GetStringData(errorMessage);
        }

        public void OnClick_Validation()
        {
            string nickName = inputField_EditNickname.text;

            if(nickName == "D1892105!#") { // 개발자모드
                PopUpSequence(false);
                PopupManager.Instance.GetPopUp<EditConfigPopup>("editConfig").ActivePopup();
                return;
            }

            if (nickName.Length < ConfigData.NICKNAME_MINIMUM_LIMIT)
            {
                inputField_EditNickname.text = "";
                string text = string.Format(LanguageManager.Instance.GetStringData("UI_NicknameMinimum"), ConfigData.NICKNAME_MINIMUM_LIMIT.ToString());
                text_Status.text = text;
                return;
            }

            _ = NetworkManager.Instance.GetUserNicknameValid(nickName, OnSuccessValid, OnFailedValid);
        }

        public void OnSuccessValid()
        {
            text_Status.text = LanguageManager.Instance.GetStringData("UI_NicknameSucceess");
            isPossibleChangeNickname = true;

            if(isFirst)
            {
                button_Confirm.interactable = isPossibleChangeNickname;
            }
            else
            {
                button_TikConfim.interactable = isPossibleChangeNickname && UserInfoManager.Instance.gemValue >= 100;
            }
        }

        public void OnClick_Confirm()
        {
            //Debug.Log("Onclick _ NicknameEdit Confirm");
            string nickName = inputField_EditNickname.text;

            UserGameNickname data = new()
            {
                userId = int.Parse(UserInfoManager.Instance.userId),
                nickname = nickName
            };

            string jsonData = JsonUtility.ToJson(data);

            _ = NetworkManager.Instance.EditUserNickname(jsonData, OnCompleteEdit);
        }

        public void SuccessEditNickname(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }

        public void OnCompleteEdit(bool isSuccess)
        {
            if (isSuccess)
            {
                PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail").SetNickname(UserInfoManager.Instance.userNicknameInfo.nickname);
                PopupManager.Instance.GetPopUp<ProfilePopup>("profile").SetNickname(UserInfoManager.Instance.userNicknameInfo.nickname);
                LobbyManager.Instance.SetNickName(UserInfoManager.Instance.userNicknameInfo.nickname);
                PopUpSequence(false);
                _ = NetworkManager.Instance.GetUserWalletInfo(SuccessEditNickname, "PTIK");
                _ = NetworkManager.Instance.GetUserWalletHistory("PTIK");
            }
            else
            {
                text_Status.text = LanguageManager.Instance.GetStringData("UI_NicknameImpossible");
            }
        }
    }
}

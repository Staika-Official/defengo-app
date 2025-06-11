using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Sound;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Google;
using Framework.GameData.Defense;
using Framework.Login;

namespace Framework.UI
{
    public class WithdrawalPopup : PopupTemplate
    {
        public TMP_InputField inputField_reason;
        public Button button_Terminate;

        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });

            button_Terminate.onClick.AddListener(() =>
            {
                OnClick_Terminate();
            });
        }

        public void OnClick_Terminate()
        {
            string reason = inputField_reason.text;

            TerminateAccountInfo data = new(int.Parse(UserInfoManager.Instance.userId), "reason");

            _ = NetworkManager.Instance.TerminationAccountV2(data, OnSuccessTerminate, OnFailedTerminate);
        }

        public void OnSuccessTerminate()
        {
            if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
            {
                GoogleSignIn.DefaultInstance.SignOut();
                //GoogleSignInManager.Instance.InitConfig();
            }
        }

        public void OnFailedTerminate()
        {

        }
    }
}

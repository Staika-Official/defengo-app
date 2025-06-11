using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using System.Globalization;
using Framework.Network;

namespace Framework.UI
{
    public class InAppPurchasePopup : PopupTemplate
    {
        public TextMeshProUGUI text_Amount;
        //public ParticleSystem attractor;

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
                OnClick_Confirm();
            });
        }

        public void SuccessInAppPurchase(int gemValue)
        {
            text_Amount.text = "<sprite=2>" + gemValue.ToString("N0", new CultureInfo("en-US"));
            PopUpSequence(true);
        }

        public void GetUserGemInfo()
        {
            _ = NetworkManager.Instance.GetUserWalletInfo(SetGemValueSequence, "PTIK");
        }

        public void SetGemValueSequence(int gemValue)
        {
            int temp = UserInfoManager.Instance.gemValue;
            UserInfoManager.Instance.gemValue = gemValue;
            LobbyManager.Instance.ChangeValueGem(gemValue, temp);
        }

        public void SetGemValue(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }

        public void OnClick_Confirm()
        {
            LobbyManager.Instance.GetUserWallet();
            //_ = NetworkManager.Instance.GetUserWalletInfo(SetGemValueSequence, "PTIK");
            //int wholeTaika = UserInfoManager.Instance.userTikWalletHistory.balance.amount;
            //int currentTaika = wholeTaika - taikaValue;
            //LobbyManager.Instance.AttractorAction(wholeTaika, currentTaika);
            AttractorManager.Instance.SetAttractor("gem", button_Close.transform, GetUserGemInfo);
            PopUpSequence(false);
        }
    }
}
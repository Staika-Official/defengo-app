using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Sound;

public class EnergyPopup : PopupTemplate
{
    public Button button_Taika;
    public Button button_Ad;
    public TextMeshProUGUI text_CoolTime;
    public TextMeshProUGUI text_Amount;
    public TextMeshProUGUI text_Price;
    public string itemCode;
    public float price;
    
    public IEnumerator tempSequence;

    public override void ActivePopup()
    {
        Debug.Log("Active Popup Energy");
        _ = NetworkManager.Instance.UserAdValidInfo("ENERGY", OnSuccessGetAdmobInfo, FailedNotEnoughCurrency);
        PopUpSequence(true);
    }

    public override void InActivePopup()
    {
        if (tempSequence != null)
        {
            StopCoroutine(tempSequence);
            tempSequence = null;
        }
        PopUpSequence(false);
    }

    public override void Initialize()
    {
        button_Close.onClick.AddListener(() =>
        {
            InActivePopup();
        });
        button_Taika.onClick.AddListener(() => OnClick_Buy());
        button_Ad.onClick.AddListener(() => OnClick_Ad());

        //StoreItem storeItem = DataManager.Instance.storeItemDTO.storeItems[0];

        for (int i = 0; i < DataManager.Instance.storeItemDTO.storeItems.Length; i++)
        {
            StoreItem storeItem = DataManager.Instance.storeItemDTO.storeItems[i];

            if (storeItem.code == "ENERGY_1")
            {
                text_Amount.text = $"<sprite=7>x{storeItem.receivedItemAmount}";
                text_Price.text = $"<sprite=2>{storeItem.price}";
                price = storeItem.price;
                itemCode = storeItem.code;
            }
        }

    }
    public void OnSuccessGetAdmobInfo(UserAdValidInfo userAdInfo)
    {
        button_Ad.interactable = userAdInfo.isAvailable;

        if (userAdInfo.isAvailable)
        {
            text_CoolTime.gameObject.SetActive(false);
        }
        else
        {
            text_CoolTime.gameObject.SetActive(true);
            if (tempSequence != null)
            {
                StopCoroutine(tempSequence);
            }

            tempSequence = CountDown(userAdInfo.remainCoolTime);
            StartCoroutine(tempSequence);
        }
        // if (userAdInfo.isAvailable)
        // {
        //     inactiveObject.SetActive(false);
        //     activeObject.SetActive(true);
        //     LobbyNavigator.Instance.Notification(MenuType.SHOP, true);
        //     notice.SetActive(true);
        // }
        // else
        // {
        //     notice.SetActive(false);
        //     inactiveObject.SetActive(true);
        //     LobbyNavigator.Instance.Notification(MenuType.SHOP, false);
        //     activeObject.SetActive(false);

        //     if (tempSequence != null)
        //     {
        //         StopCoroutine(tempSequence);
        //     }

        //     tempSequence = SetAdmobTimer(data.remainCoolTime);
        //     StartCoroutine(tempSequence);
        // }
    }


    public void OnSuccessWatchAd(UserAdInfo userAdInfo)
    {
        button_Ad.interactable = userAdInfo.isAvailable;

        if (userAdInfo.isAvailable)
        {
            text_CoolTime.gameObject.SetActive(false);
        }
        else
        {
            text_CoolTime.gameObject.SetActive(true);
            if (tempSequence != null)
            {
                StopCoroutine(tempSequence);
            }

            tempSequence = CountDown(userAdInfo.remainCoolTime);
            StartCoroutine(tempSequence);
        }
    }

    public IEnumerator CountDown(int remainCoolTime)
    {
        int temp = remainCoolTime;

        for (int i = 0; i < remainCoolTime; i++)
        {
            int minute = temp / 60;
            int seconds = temp % 60;

            text_CoolTime.text = minute.ToString("00") + ":" + seconds.ToString("00");

            yield return new WaitForSeconds(1f);
            temp--;
        }
        _ = NetworkManager.Instance.UserAdValidInfo("ENERGY", OnSuccessGetAdmobInfo, FailedNotEnoughCurrency);
        //_ = NetworkManager.Instance.UserAdValid(OnSuccessWatchAd, Failed);
    }

    public void Success(ResRewardTemplate data)
    {
        //d_gem_energy
        Framework.Util.AdjustInitializer.TrackEvent("z5265e");

        LobbyManager.Instance.GetEnergyValue();
        NetworkManager.Instance.GetUserWalletHistories();
        GetUserWallet();

        PopupManager.Instance.GetPopUp<ShopPopup>("shop").SetEarnPopup("ENERGY_1");
        PopUpSequence(false);
    }

    public void FailedNotEnoughCurrency()
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
        popup.SetNoticeMessage("UI_Not_Enough_Currency");
    }
    
    public void Failed(string errorData)
    {
        SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

        try
        {
            ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

            switch (error.errorCode)
            {
                case "INSUFFICIENT_BALANCE":
                    popup.SetNoticeMessage("UI_Not_Enough_Currency");
                    break;
                case "TOO_MANY_REQUESTS":
                    popup.SetNoticeMessage("MSG_TOO_MANY_REQUESTS");
                    break;
                default:
                    popup.SetNoticeMessage("UI_Not_Enough_Currency");
                    break;
            }
        }
        catch
        {
            popup.SetNoticeMessage("UI_Network_Unstable");
        }
    }

    public void GetUserWallet()
    {
        _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
    }

    public void SuccessWallet(int gem)
    {
        UserInfoManager.Instance.gemValue = gem;
        LobbyManager.Instance.text_Gem.text = $"{gem}";
    }

    public void OnClick_Ad()
    {
        AdmobManager.onSuccessWatchAd = () =>
        {
            string itemKey = "";
            //_ = NetworkManager.Instance.UserAdHistory("ENERGY_AD_1", LobbyManager.Instance.GetEnergyValue);
            foreach (var item in ShopScreen.Instance.dic_Item.Values)
            {
                if (item.code == "ENERGY_1" && item.storeType == StoreType.NORMAL)
                {
                    itemKey = item.itemKey;
                }
            }
            
            //_ = NetworkManager.Instance.RegistUserAdValid("ENERGY_AD_1", "energy", ShopScreen.Instance.dic_Item[itemCode].SetItemStatus, FailedUserADVaild);
            _ = NetworkManager.Instance.RegistUserAdValid("ENERGY_AD_1", "energy", ShopScreen.Instance.dic_Item[itemKey].SetItemStatus, FailedNotEnoughCurrency);
            PopupManager.Instance.GetPopUp<ShopPopup>("shop").SetEarnPopup();
            InActivePopup();
            LobbyManager.Instance.GetEnergyValue();

            //D_ad_energy
            Framework.Util.AdjustInitializer.TrackEvent("j6asom");
        };

        AdmobManager.Instance.ShowRewardedAd();
    }

    public void OnClick_Buy()
    {
        SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
     
        if (price > UserInfoManager.Instance.gemValue)
        {
            FailedNotEnoughCurrency();
            return;
        }
        
        RequestBuyItem data = new()
        {
            clientId = "DEFENGO",
            code = itemCode,
            userId = UserInfoManager.Instance.userId,
            repurchase = false
        };

        _ = NetworkManager.Instance.RequestBuyItem(data, Success, Failed);
    }
}

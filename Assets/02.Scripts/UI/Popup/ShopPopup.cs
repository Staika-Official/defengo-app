using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using System;

namespace Framework.UI
{
    public class ShopPopup : PopupTemplate
    {
        public Image image_energyIcon;
        public Image image_otherIcon;
        public TextMeshProUGUI text_Price;
        public TextMeshProUGUI text_Amount;
        public TextMeshProUGUI text_ItemName;
        public TextMeshProUGUI text_Timer;
        public TextMeshProUGUI text_description;
        public MoneyType moneyType;
        public StorePackageType storePackageType;
        public StoreType storeType;

        public Button button_Buy;
        public Button button_Ad;
        public string itemCode;
        public float price;
        public bool isItemAvailable;
        public string itemKey;
        
        public GameObject tikObject;
        public GameObject adObject;
        public GameObject activeObject;
        public GameObject inactiveObject;

        public ShopItem object_ForcusItem;
        
        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => OnClick_Close());
            button_Buy.onClick.AddListener(() => OnClick_BuyItem());
            button_Ad.onClick.AddListener(() =>
            {
                OnClick_BuyItem();
            });
        }

        public void OnClick_Close()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            PopUpSequence(false);
        }

        public void OnClick_BuyItem()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);

            if (storeType == StoreType.SPECIAL && !isItemAvailable)
            {
                Failed_Limited();
                return;
            }

            switch (moneyType)
            {
                case MoneyType.TAIKA:
                    double tikPrice= double.Parse(UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString);
                    if(price > tikPrice)
                    {
                        Failed_Enough();
                        return;
                    }

                    RequestBuyItem tikData = new()
                    {
                        clientId = "DEFENGO",
                        code = itemCode,
                        userId = UserInfoManager.Instance.userId,
                        repurchase = false
                    };

                    _ = NetworkManager.Instance.RequestBuyItem(tikData, Success, Failed);
                    break;
                case MoneyType.STIK:
                    double stikPrice= double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                    if(price > stikPrice)
                    {
                        Failed_Enough();
                        return;
                    }

                    RequestBuyItem stikData = new()
                    {
                        clientId = "DEFENGO",
                        code = itemCode,
                        userId = UserInfoManager.Instance.userId,
                        repurchase = false
                    };

                    _ = NetworkManager.Instance.RequestBuyItem(stikData, Success, Failed);
                    break;
                case MoneyType.ADMOB:
                    AdmobManager.onSuccessWatchAd = () =>
                    {
                        //_ = NetworkManager.Instance.RegistUserAdValid("ENERGY_AD_1", "energy", ShopScreen.Instance.dic_Item[itemCode].SetItemStatus, Failed_Enough);
                        _ = NetworkManager.Instance.RegistUserAdValid("ENERGY_AD_1", "energy", ShopScreen.Instance.dic_Item[itemKey].SetItemStatus, Failed_Enough);
                        SetEarnPopup();
                        AdjustInitializer.TrackEvent("j6asom");
                        PopUpSequence(false);
                    };
                    
                    //ads test
                    //AdmobManager.Instance.AdsRewardedAd();
                    //admob
                    AdmobManager.Instance.ShowRewardedAd();

                    break;
                case MoneyType.GEM:
                    if (price > UserInfoManager.Instance.gemValue)
                    {
                        Failed_Enough();
                        return;
                    }

                    RequestBuyItem reqData = new()
                    {
                        clientId = "DEFENGO",
                        code = itemCode,
                        userId = UserInfoManager.Instance.userId,
                        repurchase = false
                    };
                    _ = NetworkManager.Instance.RequestBuyItem(reqData, Success, Failed);
                    break;
                default:
                    break;
            }
        }

        public void SetEarnPopup(string itemCode)
        {
            EarnPopup earnPopup = PopupManager.Instance.GetPopUp<EarnPopup>("earn");

            earnPopup.SetAmountValue(text_Amount.text, itemCode, DataManager.Instance.uiPropertyData.sprite_Energy, StorePackageType.ENERGY);
            earnPopup.PopUpSequence(true);
        }

        public void SetEarnPopup()
        {
            EarnPopup earnPopup = PopupManager.Instance.GetPopUp<EarnPopup>("earn");

            earnPopup.SetAmountValue(text_Amount.text, itemCode, image_energyIcon.sprite, storePackageType);
            earnPopup.PopUpSequence(true);
        }
        
        public void OnFailedWatchAd()
        {

        }

        public void Success(ResRewardTemplate data)
        {
            Sprite sprite;
            string adjustKey;
            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    sprite = image_energyIcon.sprite;
                    //D_gem_energy
                    adjustKey = "z5265e";
                    break;
                case StorePackageType.RANDOM_BOX:
                    sprite = image_otherIcon.sprite;
                    //D_gem_randombox
                    adjustKey = "mvdjf0";
                    break;
                case StorePackageType.GEM:
                    //D_gem_exchange
                    sprite = image_otherIcon.sprite;
                    adjustKey = "eqg1e1";
                    break;
                default:
                    sprite = image_otherIcon.sprite;
                    adjustKey = "";
                    break;
            }

            AdjustInitializer.TrackEvent(adjustKey);

            EarnPopup earnPopup = PopupManager.Instance.GetPopUp<EarnPopup>("earn");
            NetworkManager.Instance.GetUserWalletHistories();

            earnPopup.SetAmountValue(text_Amount.text, itemCode, sprite, storePackageType);
            earnPopup.PopUpSequence(true);
            PopUpSequence(false);
            
            _ = NetworkManager.Instance.GetUserWalletHistory(SuccessTikWallet, SuccessStikWallet);
            
            if (storeType == StoreType.SPECIAL)
            {
                _ = NetworkManager.Instance.GetSpecialStore(SuccessSepcialInit);
            }
        }
        public void SuccessSepcialInit(SpecialStoreDTO specialStoreDTO)
        {
            ShopScreen.Instance.SpecialStoreItemInit(specialStoreDTO);
        }
        
        public void SuccessStikWallet()
        {
            double stikBalance = Double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
            LobbyManager.Instance.text_Stik.text = $"{stikBalance:0.####}";
        }
        
        public void SuccessTikWallet()
        {
            LobbyManager.Instance.text_Taika.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;
        }


        public void Failed_Enough()
        {
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Enough_Currency");
        }

        public void Failed_Limited()
        {
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("MSG_limited_Count");
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
                    case "EXCEED_LIMITED_QUANTITY":
                        popup.SetNoticeMessage("MSG_limited_Count");
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

        public void SetShopItemInfo(ShopItem item, StorePackageType storePackageType)
        {
            itemKey = item.itemKey;
            itemCode = item.code;
            price = item.price;
            storeType = item.storeType;
            isItemAvailable = item.isAvailable;

            if (storePackageType == StorePackageType.ENERGY)
            {
                image_energyIcon.gameObject.SetActive(true);
                image_otherIcon.gameObject.SetActive(false);
                image_energyIcon.sprite = item.image_Icon.sprite;
                text_ItemName.text = LanguageManager.Instance.GetStringData("UI_Purchase_Check");
                text_description.text = LanguageManager.Instance.GetStringData("UI_Energy_Store_Desc");
            }
            else if (storePackageType == StorePackageType.ITEM)
            {
                if(itemCode == "ROULETTE_TICKET")
                {
                    image_energyIcon.gameObject.SetActive(false);
                    image_otherIcon.gameObject.SetActive(true);
                    image_otherIcon.sprite = item.image_Icon.sprite;
                    text_ItemName.text = LanguageManager.Instance.GetStringData("UI_Gem_Exchange_Check");
                    text_description.text = LanguageManager.Instance.GetStringData("UI_RouletteTicket_Exchange_Desc");
                }
            }
            else
            {
                image_energyIcon.gameObject.SetActive(false);
                image_otherIcon.gameObject.SetActive(true);
                image_otherIcon.sprite = item.image_Icon.sprite;
                text_ItemName.text = LanguageManager.Instance.GetStringData("UI_Gem_Exchange_Check");
                text_description.text = LanguageManager.Instance.GetStringData("UI_Gem_Exchange_Desc");
            }
            this.moneyType = item.moneyType;
            this.storePackageType = storePackageType;
            switch (moneyType)
            {
                case MoneyType.TAIKA:
                    tikObject.SetActive(true);
                    adObject.SetActive(false);
                    text_Price.text = $"{item.text_Price.text}";
                    break;
                case MoneyType.STIK:
                    tikObject.SetActive(true);
                    adObject.SetActive(false);
                    text_Price.text = $"{item.text_Price.text}";
                    break;
                case MoneyType.ADMOB:
                    text_Price.text = "<sprite=6>Free";
                    adObject.SetActive(true);
                    tikObject.SetActive(false);

                    bool isActive = item.isAvailable;
                    activeObject.SetActive(isActive);
                    inactiveObject.SetActive(!isActive);
                    button_Ad.interactable = isActive;
                    if (!isActive)
                    {
                        StartCoroutine(SetAdmobTimer(item.remainTime));
                    }

                    break;
                case MoneyType.GEM:
                    tikObject.SetActive(true);
                    adObject.SetActive(false);
                    text_Price.text = $"{item.text_Price.text}";
                    break;
                default:
                    break;
            } 

            text_Amount.text = item.text_Amount.text;
        }

        public IEnumerator SetAdmobTimer(int timer)
        {
            int remainTime = timer;

            for (int i = 0; i < timer; i++)
            {
                text_Timer.text = $"{remainTime / 60:00} : {remainTime % 60:00}";
                yield return new WaitForSeconds(1f);
                remainTime--;
            }

            button_Ad.interactable = true;
            inactiveObject.SetActive(false);
            activeObject.SetActive(true);
        }

        public void SetTimer(int timer)
        {
            Debug.Log(timer);
            if(timer > 0)
            {
                text_Timer.text = $"{timer / 60:00} : {timer % 60:00}";
            }
            else
            {
                activeObject.SetActive(true);
                inactiveObject.SetActive(false);
            }
        }

    }
}

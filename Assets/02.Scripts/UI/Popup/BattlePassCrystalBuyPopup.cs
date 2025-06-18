using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using Framework.Network;
using Framework.GameData.Defense;
using UnityEngine.Purchasing;
using Framework.Sound;

namespace Framework.UI
{
    public class BattlePassCrystalBuyPopup : PopupTemplate
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Info;
        public TextMeshProUGUI text_InApp;
        public TextMeshProUGUI text_Levelup;

        public Button button_InApp;

        public List<CrystalItem> cristalItems;
        public Dictionary<int, int> dic_CrystalData = new();

        public UserBattlePassLevel userData;

        public string productId;

        public readonly int defalutLevelUp = 10;
        public int maxLevel;
        
        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            //지정해준 크리스탈 값이 고정값이라 이렇게 해둬도 무관
            dic_CrystalData.Add(5, 10);
            dic_CrystalData.Add(30, 50);
            dic_CrystalData.Add(70, 100);

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            button_InApp.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InAppPayMent();
            });
        }

        public void SetPopup(UserBattlePassLevel data, int maxLevel)
        {
            //크리스탈 초기화
            int count = 0;
            
            this.maxLevel = maxLevel;
            
            foreach (var crystal in dic_CrystalData)
            {
                //키와 벨류값이 매칭되어야함으로 이렇게 해줌
                cristalItems[count].Initialize(crystal.Key, crystal.Value);
                ++count;
            }

            userData = data;

            text_Title.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Cristal_Exchange");
            text_Info.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Cristal_Exchange_Desc");
            text_Levelup.text = $"+{defalutLevelUp}";

            //크리스탈 인앱 결제 아이디 셋
            //인앱 가격을 셋해주고 결제 할때 본인의 key값을 이용해서 인앱상품을 결제하기위함
            productId = IAPManager.Instance.iapPassCrystalKey;
            Product product = IAPManager.Instance.GetProductInfo(productId);
            if(product == null)
            {
                Debug.LogError("product is null");
                return;
            }
            string price = product.metadata.localizedPrice.ToString();
            string priceInfo = product.metadata.isoCurrencyCode + " " + price;
            text_InApp.text = priceInfo;

            ActivePopup();
        }

        public void GemPayMent(int gemAmount, int crystal)
        {
            //함수 의도 : 젬결제 클릭 시 호출되는 함수이며 서버에 요청을 하기위한 함수

            //유저 젬 갯수가 부족한 경우
            if (gemAmount > UserInfoManager.Instance.gemValue)
            {
                Failed_Gem("UI_Not_Enough_Currency");
                return;
            }

            BattlePassBuyExp data = new()
            {
                payType = "GEM",
                battlePassId = UserInfoManager.Instance.userPassId,
                price = gemAmount
            };

            _ = NetworkManager.Instance.RequestPassBuyExp(data, Sucess_Gem, Failed_Gem);
        }

        public void InAppPayMent()
        {
            //함수 의도 : 인앱 결제 클릭 시 호출되는 함수이며 서버에 요청을 하기위한 함수

            //인앱 결제
            LobbyManager.Instance.PurchasePopup(true);

            IAPManager.Instance.iapBuyType = IapBuyType.BATTLEPASS_EXP;
            
            //결제 성공 시 불리는 콜백 함수
            NetworkManager.Instance.onSuccessPassExpInAppPurchase = () =>
            {
                Debug.Log("Crystal InappPaySucess");

                LobbyManager.Instance.PurchasePopup(false);

                RequestInAppCrystal();
            };
            
            //결제 시작
            IAPManager.Instance.PurchaseProduct(productId);
        }

        public void RequestInAppCrystal()
        {
            BattlePassBuyExp data = new()
            {
                payType = "INAPP",
                battlePassId = UserInfoManager.Instance.userPassId,
                price = 0f
            };
                
                
            // //결제 성공하면 경험치 구매를 해줘야함
            _ = NetworkManager.Instance.RequestPassBuyExp(data, Sucess_InApp, Failed_InApp);
        }

        public void Sucess_Gem(BattlePassUserInfoDetailData data)
        {
            //데이터 셋
            BattlePassPopup PassPopup = PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass");
            PassPopup.SetBattlePassContent(data);

            //유저 레벨이 만랩이면 창 닫아줌
            if (data.userLevel.level == maxLevel)
            {
                InActivePopup();
            }

            //구매 성공하면 다시 보유중인 젬 셋
            GetUserWallet();
        }

        public void Failed_Gem(string errorCode)
        {
            //실패 팝업 창
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage(errorCode);
        }

        public void Sucess_InApp(BattlePassUserInfoDetailData data)
        {
            //인앱 결제 성공 시 데이터 셋
            PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass").SetBattlePassContent(data);
            //유저 레벨이 만랩이면 창 닫아줌
            if (data.userLevel.level == maxLevel)
            {
                InActivePopup();
            }
        }

        public void Failed_InApp(string errorCode)
        {
            //실패 팝업 창
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("NOT_FOUND_IAP_PURCHASE");
        }

        public void GetUserWallet()
        {
            //보유 젬 다시 들고옴
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        public void SuccessWallet(int gem)
        {
            //젬 셋팅
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Network;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Util;
using System;
using UnityEngine.Purchasing;
using Framework.Sound;

namespace Framework.UI
{
    public class BattlePassPremiumPopup : PopupTemplate
    {
        public List<BattlePassPremiumActivateItem> premiumItem;
        public Queue<BattlePassPremiumActivateItem> queuePremiumItem = new();   

        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_explanation;
        public TextMeshProUGUI text_SessionDate;
        public TextMeshProUGUI text_Info;
        public TextMeshProUGUI text_InApp;

        public GameObject stikObject;
        //소지하고있는 틱
        public TextMeshProUGUI text_HoldingSTik;
        //결제하려는 틱
        public TextMeshProUGUI text_PayMentSTik;

        //인앱 결제버튼
        public Button button_InApp;
        //틱 결제 버튼
        public Button button_PayMentTik;

        public string productId;

        public float stickPrice;

        public RectTransform viewPortRect;


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
            button_InApp.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InAppPayMent();
            });

            button_PayMentTik.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                StikPayMent();
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });
        }

        public void PremiumBuyPopupActive(BattlePassUserInfoDetailData data)
        {
            //풀링 기법 사용해서 보상 생성 및 초기화
            SetPremiumItem(data.premiumRewards);

            string toDate = DateTime.Parse(data.toDate).ToString("yyyy.MM.dd");
            string fromDate = DateTime.Parse(data.fromDate).ToString("yyyy.MM.dd");

            //제목
            text_Title.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Active");
            //설명1
            text_explanation.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Activate_Desc");
            //기간
            text_SessionDate.text = string.Format(LanguageManager.Instance.GetStringData("UI_BattlePass_Period"), fromDate, toDate);
            //설명2
            text_Info.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Purchase_Desc");

            //프리미엄 패스 결제 아이디 셋
            productId = IAPManager.Instance.iapPassPremiumKey;
            Product product = IAPManager.Instance.GetProductInfo(productId);
            string price = product.metadata.localizedPrice.ToString();
            string priceInfo = product.metadata.isoCurrencyCode + " " + price;
            text_InApp.text = priceInfo;

            //인앱 결제 버튼 활성화
            button_InApp.gameObject.SetActive(true);
            button_InApp.interactable = true;

            //stik관련 오브젝트 초기화
            Init_Stik(data.price);

            //팝업 활성화
            ActivePopup();
        }

        public void Init_Stik(float price)
        {
            //스틱 가격
            stickPrice = price;

            //프리미엄 결제 스틱 가격
            text_PayMentSTik.text = $"<sprite=14>{price}";
            //보유 스틱
            //text_HoldingSTik.text = UserInfoManager.Instance.GetUserStikAmount();
            text_HoldingSTik.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;

            //production에서 지역 별로 true false해줘야함
            if (UserInfoManager.Instance.IsBlockRegion)
            {
                button_PayMentTik.gameObject.SetActive(false);
                stikObject.gameObject.SetActive(false);
            }
            else
            {
                button_PayMentTik.gameObject.SetActive(true);
                stikObject.gameObject.SetActive(true);
            }
        }

        public void InAppPayMent()
        {
            //버튼 클릭 중복 방지
            //button_InApp.interactable = false;
            //로딩 바 액티브
            LobbyManager.Instance.PurchasePopup(true);
 
            IAPManager.Instance.iapBuyType = IapBuyType.BATTLEPASS_PREMIUM;

            //결제 성공 시 불러주는 콜백 함수
            NetworkManager.Instance.onSuccessPassPremiumInAppPurchase = () =>
            {
                Debug.Log("Success inapp premium popup");

                //로딩 바 인액티브
                LobbyManager.Instance.PurchasePopup(false);

                //영수증 나오면 프리미엄 패스 구매 리퀘스트
                 RequestInAppPremium();
            };
            
            //결제 시작
            IAPManager.Instance.PurchaseProduct(productId);
        }

        public void RequestInAppPremium()
        {
            //리퀘스트 데이터
            BattlePassBuyExp data = new()
            {
                payType = "INAPP",
                battlePassId = UserInfoManager.Instance.userPassId,
                price = 0f
            };
            
            //영수증 나오면 프리미엄 패스 구매 리퀘스트
            _ = NetworkManager.Instance.RequestPassBuyPremium(data ,Success_InApp, Failed_InApp);
        }
        

        public void StikPayMent()
        {
            //버튼 클릭 중복 방지
            button_PayMentTik.interactable = false;
            double stikPrice= double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
            
            //결제하려는 스틱보다 보유중인 스틱이 더 작으면 실패를 띄워줘야함
            if (stickPrice > stikPrice)
            {
                Failed_Stik();
                return;
            }

            BattlePassBuyExp data = new()
            {
                payType = "STIK",
                battlePassId = UserInfoManager.Instance.userPassId,
                price = stickPrice
            };

            //스틱 구매 리퀘스트
            _ = NetworkManager.Instance.RequestPassBuyPremium(data, Success_Stik, Failed_Stik);
        }

        //인앱 결제 콜백함수
        public void Success_InApp(BattlePassUserInfoDetailData data)
        {
            //인앱 결제 성공하면 프리미엄 패스 팝업창 닫아줘야함
            InActivePopup();
            PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass").SetBattlePassContent(data);
        }

        public void Failed_InApp()
        {
            //인앱 결제 실패 시 다시 결제할 수 있도록 버튼 활성화
            button_InApp.interactable = true;

            //실패 팝업
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("NOT_FOUND_IAP_PURCHASE");
        }

        //stick결제 콜백함수
        public void Success_Stik(BattlePassUserInfoDetailData data)
        {
            //결제 성공 시 팝업 창 닫기
            InActivePopup();

            //데이터 셋 
            PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass").SetBattlePassContent(data);

            //stik 가격 다시 셋 해야함
            GetWalletStik();
        }

        public void Failed_Stik()
        {
            //실패하면 다시 결제 할 수 있도록 버튼 활성화
            button_PayMentTik.interactable = true;

            //실패 팝업
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Enough_Currency");
        }

        public async void GetWalletStik()
        {
            //프리미엄 패스를 스틱으로 구매하면 스틱 가격을 다시 셋해줘야함
            //여기서 스틱 소비처 추가되면 다시 작업해줘야함 현재는 임시용으로 해서 이정도로만 셋
            UserWalletHistory userStikWallethistory = await NetworkManager.Instance.SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"), SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;
            text_HoldingSTik.text = userStikWallethistory.balance.uiAmountString;
        }

        public void SetPremiumItem(BattlePassPremiumItem[] items)
        {
            RefreshReward();

            for (int i = 0; i < items.Length; ++i)
            {
                BattlePassPremiumActivateItem obj = GetItem();
                obj.gameObject.SetActive(true);
                

                obj.Initialize(items[i]);
            }

            queuePremiumItem.Clear();
        }

        public void RefreshReward()
        {
            for (int i = 0; i < premiumItem.Count; ++i)
            {
                premiumItem[i].gameObject.SetActive(false);
                queuePremiumItem.Enqueue(premiumItem[i]);
            }
        }

        public BattlePassPremiumActivateItem GetItem()
        {
            if (queuePremiumItem.Count == 0)
            {
                BattlePassPremiumActivateItem item = Instantiate(premiumItem[0].gameObject).GetComponent<BattlePassPremiumActivateItem>();
                item.transform.SetParent(viewPortRect);
                item.transform.localScale = premiumItem[0].transform.localScale;
                premiumItem.Add(item);
                return item;
            }
            else
            {
                return queuePremiumItem.Dequeue();
            }
        }


    }
}

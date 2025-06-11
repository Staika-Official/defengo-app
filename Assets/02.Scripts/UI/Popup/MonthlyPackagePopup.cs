using System;
using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.UI;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class MonthlyPackagePopup : PopupTemplate
    {
        public TextMeshProUGUI text_Gem;
        public GameObject packageItem;

        public List<MonthlyItem> packageItems = new();
        public Queue<MonthlyItem> queue_packageItems = new();
        public Dictionary<MonthlyPackageType, MonthlyItem> dic_packageItems = new();
            
        public MonthlyPackageType packageItemType;

        public RectTransform rect_Parent;
        public RectTransform rect_BackGround;
        public RectTransform rect_GemAttractor;
        
        public ScrollRect scrollRect; 
        
        public readonly int maxItem = 2;
        
        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => OnClick_Close());
        }

        public override void ActivePopup()
        {
            //Test
            //MonthlyPackageDTO packageTest = JsonUtility.FromJson<MonthlyPackageDTO>(testData.text);
            //SetMontlyPackageInfo(packageTest);
        }
        
        public override void InActivePopup()
        {
            PopUpSequence(false);
        }
        
        //데이터 초기화 이후 팝업 활성화 시켜주기위해 async계얄 함수 만듬
        public async void AsyncActivePopup()
        {
            //처음에만 셋해주고 이후 패키지를 구매하면 동적으로 변경시켜줌
            //젬 셋
            text_Gem.text = $"{UserInfoManager.Instance.gemValue}";
            
            //서버에서 받아온 데이터기반으로 초기화 진행
            await NetworkManager.Instance.GetUserMonthlyPackage(SetMonthlyPackageInfo, Failed);
            
            //레드닷 제거
            HomeScreen.Instance.monthlyNoticeObject.SetActive(false);
            
            //팝업창 활성화
            PopUpSequence(true);
        }
        
        public void OnClick_Close()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            InActivePopup();
        }

        //초기화 함수
         public void SetMonthlyPackageInfo(MonthlyPackageDTO data)
         {
             //패키지 갯수를 알아야지 새로운 패키지가 들어왔는지 확인 가능
             int packageCount = 0;

             //오브젝트 풀링 준비용 함수
             RefreshPackageItem();
             
             //패키지 상품 제작
             for (int i = 0; i < data.specialStore.Length; ++i)
             {
                 MonthlyItem item = GetMonthlyItem();
                 item.gameObject.SetActive(true);
                 item.Initialize(data.specialStore[i], GetProductId(data.specialStore[i].storeCategoryType));
                 
                 //프리팹에 환불정책 설명이 자식으로 붙어있음으로 맨마지막 프리팹에 환불정책텍스트를 액티브를 켜주기위한 함수
                 SetRefundActivate(i, data.specialStore.Length - 1, item);
                 
                 //만들어진 패키지를 관리하기위한 딕셔너리
                 dic_packageItems.Add(item.packageType, item);
                 ++packageCount;
             }
             
             //새로운 상품이 들어오면 홈스크린에서 월정액패키지 아이콘 notice켜짐
             if (packageCount != SecurePlayerPrefs.GetInt("MonthlyPackage"))
             {
                SecurePlayerPrefs.SetInt("MonthlyPackage", packageCount);
                 HomeScreen.Instance.monthlyNoticeObject.SetActive(true);
             }
             
             queue_packageItems.Clear();
             
             //렉트 사이즈 조절을 해서 화면크기를 맞춰줌
             SetRectSize();
             SetScrollRect();
         }

         //생성된 패키지 중 마지막 패키지에 환불정책 텍스트를 액티브해주기위한 함수
         public void SetRefundActivate(int curIdx, int maxIdx, MonthlyItem item)
         {
             if (curIdx == maxIdx)
             {
                 item.object_TextRefund.SetActive(true);
             }
             else
             {
                 item.object_TextRefund.SetActive(false);
             }
         }
         
         //해당 패키지의 상품 아이디를 얻어오기위한 함수
         public string GetProductId(string packageType)
         {
             MonthlyPackageType type = (MonthlyPackageType)Enum.Parse(typeof(MonthlyPackageType),packageType);
             return IAPManager.Instance.GetMonthlyProductId((int)type, false);
         }
         
         
         //렉트 사이즈 조절을위한 함수 (패키지 갯수에따라 렉트크기가 달라져야함)
         public void SetRectSize()
         {
             float sizeDeltaY = 0f;
             
             foreach (var item in dic_packageItems.Values)
             {
                 sizeDeltaY += item.rect_BackGround.sizeDelta.y;
                 rect_BackGround.sizeDelta = new Vector2(rect_BackGround.sizeDelta.x, sizeDeltaY);
             }
         }

         public void SetScrollRect()
         {
             if (dic_packageItems.Count > maxItem)
             {
                 scrollRect.vertical = true;
             }
             else
             {
                 scrollRect.vertical = false;
             }
         }
         
         //풀링을 준비하는 함수
         public void RefreshPackageItem()
         {
             for (int i = 0; i < packageItems.Count; ++i)
             {
                 packageItems[i].gameObject.SetActive(false);
                 queue_packageItems.Enqueue(packageItems[i]);
             }
             
             dic_packageItems.Clear();
         }

         //풀링을 이용해서 아이템을 꺼내오는 함수
         public MonthlyItem GetMonthlyItem()
         {
             if (queue_packageItems.Count == 0)
             {
                 GameObject obj = Instantiate(packageItem);
                 MonthlyItem item = obj.GetComponent<MonthlyItem>();
                 
                 obj.transform.SetParent(rect_Parent);
                 item.transform.localScale = Vector3.one;
                 
                 packageItems.Add(item);
                 return item;
             }
             else
             {
                 return queue_packageItems.Dequeue();
             }
         }
         
         
         //인앱 아이템 결제를 진행하며 진행이 완료되면 서버에 리퀘스트를 날릴 준비를하는 함수
         public void PurchaseMonthlyPackage(string productId,MonthlyPackageType type, int packageId, string buyType)
         {
             //타입을 이용해서 인앱결제 성공 이후 서버에 리퀘스트 success시 패키지 타입별로 리워드를 지급해주기위한 변수 
             packageItemType = type;
             
             LobbyManager.Instance.PurchasePopup(true);

             IAPManager.Instance.iapBuyType = IapBuyType.MONTHLY_GEM;
             
             //인앱결제 성공 시 호출해주는 이벤트 함수
             NetworkManager.Instance.onSuccessMonthlyPackagePurchase = () =>
             {
                 LobbyManager.Instance.PurchasePopup(false);

                 RequestInAppPurchaseMonthlyPackage(packageId, buyType);
             };
             
             //인앱결제 시작
             IAPManager.Instance.PurchaseProduct(productId);
         }

         public void RequestInAppPurchaseMonthlyPackage(int packageId, string buyType)
         {
             RequestMonthlyPackage data = new()
             {
                 storeId = packageId,
                 type = buyType
             };

             _ = NetworkManager.Instance.BuyUserMonthlyPackage(data, Success, Failed);
         }
        
         // public void TestPurchaseMonthlyPackage(string productId,MonthlyPackageType type, int packageId, string buyType)
         // {
         //     packageItemType = type;
         //     
         //     RequestMonthlyPackage data = new()
         //     {
         //         storeId = packageId,
         //         type = buyType
         //     };
         //
         //     _ = NetworkManager.Instance.BuyUserMonthlyPackage(data, Success, Failed);
         // }

         //리워드 버튼 클릭 시 호출해주는 함수
         public void MonthlyRewardReceive(MonthlyPackageType type, int packageId, string buyType)
         {
             packageItemType = type;
             
             Debug.Log("Success Monthly Reward");
             
             RequestInAppPurchaseMonthlyPackage(packageId, buyType);
         }

         
         //성공하면 패키지별로 리워드 지급 
         public void Success(MonthlyPackageDTO data)
         {
             SetMonthlyPackageInfo(data);
             
             switch (packageItemType)
             {
                 case MonthlyPackageType.GEM_PACKAGE:
                     AttractorManager.Instance.SetAttractor("packageGem", dic_packageItems[packageItemType].rect_Icon, OnCompleteGetGem, rect_GemAttractor);
                     break;
             }
         }

         //팝업활성화 없이 초기화를 해주는 함수
         //핸드폰 백그라운드를 왔다 들어왔을 때 시간값을 다시 셋해주거나 레드닷설정을 위해 함수 정의
         public void GetPackage()
         {
             _ = NetworkManager.Instance.GetUserMonthlyPackage(SetMonthlyPackageInfo, Failed);
         }
         
         //어트랙터 성공 시 호출해주는 함수 젬을 최신화 시켴줌
         public void OnCompleteGetGem()
         {
             _ = NetworkManager.Instance.GetUserWalletInfo(SetGemValueSequence, "PTIK");
         }

         public void SetGemValueSequence(int gemValue)
         {
             int preGemValue = UserInfoManager.Instance.gemValue;
             UserInfoManager.Instance.gemValue = gemValue;
             
             //로비 젬 표시 변경
             LobbyManager.Instance.text_Gem.text = $"{gemValue}";
             
             //월정액 패키치 젬 표시 변경
             StartCoroutine(LobbyManager.Instance.ChangeValueSequenceAsync(gemValue, preGemValue, text_Gem));
         }

         
         //실패 시 에러메세지 처리
         public void Failed(string errorData)
         {
             foreach (var item in dic_packageItems.Values)
             {
                 item.SetButtonInteractable(true);
             }
             
             SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

             try
             {
                 ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

                 switch (error.status)
                 {
                     case 400:
                         popup.SetNoticeMessage("MSG_Not_Enough_Cool_Time");
                         break;
                     case 422:
                         popup.SetNoticeMessage("MSG_Not_Having_Package");
                         break;
                     default:
                         popup.SetNoticeMessage("UI_Network_Unstable");
                         break;
                 }
             }
             catch
             {
                 popup.SetNoticeMessage("UI_Network_Unstable");
             }
             
             InActivePopup();
         }
         
         //핸드폰 어플리케이션 백그라운드 진입하면 호출해주는 함수
         //시간값 최신화를 위해
         void OnApplicationFocus(bool focus)
         {
             if (focus)
             {
                 GetPackage();
             }
         }
    }
}


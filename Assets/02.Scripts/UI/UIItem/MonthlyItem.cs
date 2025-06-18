using System;
using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System.Globalization;
using Framework.Sound;

namespace Framework.UI
{
    public class MonthlyItem : MonoBehaviour
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Info;
        public TextMeshProUGUI text_MonthlyTimer;
        public TextMeshProUGUI text_ButtonTimer;
        public TextMeshProUGUI text_ButtonRecive;
        public TextMeshProUGUI text_PurchasePrice;
        
        public TextMeshProUGUI text_FirstRewardInfo;
        public TextMeshProUGUI text_RewardInfo;
        
        public TextMeshProUGUI text_FirstRewardCount;
        public TextMeshProUGUI text_RewardCount;

        public Image image_Icon;
        public RectTransform rect_Icon;
        
        public Button button_Recive;
        public Button button_Purchase;

        public int totalRemainCoolTime;
        public int rewardRemainCoolTime;

        public int packageId;
        public string productId;

        public GameObject object_Recive;
        public GameObject object_Purchase;
        public GameObject object_Timer;
        public GameObject object_TotalTimer;
        public GameObject object_TextRefund;
        
        public IEnumerator totalRemainCoroutine;
        public IEnumerator rewardRemainCoroutine;

        public RectTransform rect_BackGround;
        
        public MonthlyPackageType packageType;
        
        public void Start()
        {
            button_Recive.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                RewardReceive();
            });
            
            button_Purchase.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                MonthlyPackageBuy();
            });
        }
        
        public void Initialize(MonthlyPackageItem packageItem, string productId)    
        {
            packageType = (MonthlyPackageType)Enum.Parse(typeof(MonthlyPackageType),packageItem.storeCategoryType);
            
            packageId = packageItem.storeId;
            this.productId = productId;
            
            this.totalRemainCoolTime = packageItem.totalRemainCoolTime;
            this.rewardRemainCoolTime = packageItem.rewardRemainCoolTime;
            
            image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_MonthlyPackageSprite[packageType];
            text_ButtonRecive.text = LanguageManager.Instance.GetStringData("UI_MonthlyGemPack_BonusButton");
            
            Product product = IAPManager.Instance.GetProductInfo(productId);
            if(product == null)
            {
                Debug.LogError("product is null");
                return;
            }
            string price = product.metadata.localizedPrice.ToString();
            string priceInfo = product.metadata.isoCurrencyCode + " " + price;
            text_PurchasePrice.text = priceInfo;
            
            text_FirstRewardInfo.text = LanguageManager.Instance.GetStringData("UI_Purchase_Immediate");
            text_RewardInfo.text = string.Format(LanguageManager.Instance.GetStringData("UI_Purchase_Cycle"), ConfigData.MONTHLY_PACKAGEREWARD_RATE);
            
            MonthlyTextInit();
            TimerInitialize();
        }

        
        //패키지 별로 텍스트가 달라져야하는 부분을 셋 해주는 함수
        public void MonthlyTextInit()
        {
            switch (packageType)
            {
                case MonthlyPackageType.GEM_PACKAGE:
                    text_Title.text = LanguageManager.Instance.GetStringData("UI_MonthlyGemPack");
                    text_Info.text = LanguageManager.Instance.GetStringData("UI_MonthlyGemPack_Slogan");                  
                    text_FirstRewardCount.text = "<size=48><sprite=2></size=>x 3480";
                    text_RewardCount.text = "<size=48><sprite=2></size=>x 160";
                    break;
                default:
                    break;
            }
        }
        
        
        //타이머 초기화 함수
        public void TimerInitialize()
        {
            if (totalRemainCoroutine != null)
            {
                StopCoroutine(totalRemainCoroutine);
                totalRemainCoroutine = null;
            }
            if (rewardRemainCoroutine != null)
            {
                StopCoroutine(rewardRemainCoroutine);
                totalRemainCoroutine = null;
            }

            SetButtonInteractable(true);
            
            //0이아니라면 구매를했다는 것
            bool isPackageActive = totalRemainCoolTime != 0;

            object_TotalTimer.SetActive(isPackageActive);
            object_Purchase.SetActive(!isPackageActive);
            
            if (isPackageActive)
            {
                //구매를 한 상태 보상을 받을수 있는 총 시간 표시
                totalRemainCoroutine = TotalPackageCoolTime(totalRemainCoolTime,TotalRewardEndTime);
                StartCoroutine(totalRemainCoroutine);
                
                //패키지를 구매하고 시간이 0이라면 리워드를 받을 수 있다는것
                var isReawrdActive = rewardRemainCoolTime == 0;
                
                if (isReawrdActive)
                {   
                    //보상을 받을 수 있음으로 쿨타임 종료
                    if (rewardRemainCoroutine != null)
                    {
                        StopCoroutine(rewardRemainCoroutine);
                    } 
                }
                else
                {
                    //이미 보상을 받았으면 쿨타임 시작
                    rewardRemainCoroutine = RewardPackageCoolTime(rewardRemainCoolTime, RewardEndTime);
                    StartCoroutine(rewardRemainCoroutine);
                }
                
                //구매를 한 상태이므로
                //보상을 받을수 있는지 없는지에대한 버튼 처리 및 레드닷 표시
                HomeScreen.Instance.monthlyNoticeObject.SetActive(isReawrdActive);
                
                //보상을 받을수있는지 없는지에 대한 여부로 오브젝트 활성화 및 비활성화
                object_Recive.SetActive(isReawrdActive);
                object_Timer.gameObject.SetActive(!isReawrdActive);
            }
            else
            {
                if (totalRemainCoroutine != null)
                {
                    StopCoroutine(totalRemainCoroutine);
                    totalRemainCoroutine = null;
                }
                if (rewardRemainCoroutine != null)
                {
                    StopCoroutine(rewardRemainCoroutine);
                    rewardRemainCoroutine = null;
                } 
                
                //구매하지않았음으로
                //받기버튼, 타이머버튼 비활성화
                object_Recive.SetActive(false);
                object_Timer.gameObject.SetActive(false);
            }
        }

        
        //패키지 유지 시간이 다되면 호출해주는 함수
        public void TotalRewardEndTime()
        {
            object_TotalTimer.SetActive(false);
            object_Recive.SetActive(false);
            object_Purchase.SetActive(true);
            object_Timer.gameObject.SetActive(false);
            
            if (rewardRemainCoroutine != null)
            {
                StopCoroutine(rewardRemainCoroutine);
            }
            
            HomeScreen.Instance.monthlyNoticeObject.SetActive(false);
        }

        public void SetButtonInteractable(bool isInteractable)
        {
            button_Recive.interactable = isInteractable;
            button_Purchase.interactable = isInteractable;
        }
        
        
        //리워드를 받을수있는 대기시간이 다되면 호출해주는 함수
        public void RewardEndTime()
        {
            object_Recive.SetActive(true);
            object_Purchase.SetActive(false);
            object_Timer.gameObject.SetActive(false);
            
            HomeScreen.Instance.monthlyNoticeObject.SetActive(true);
        }  
        
        
        //받기 버튼 클릭 시 호출해주는 함수
        public void RewardReceive()
        {
            button_Recive.interactable = false;
            MonthlyPackagePopup packagePopup = PopupManager.Instance.GetPopUp<MonthlyPackagePopup>("monthlyPackage");
            packagePopup.MonthlyRewardReceive(packageType, packageId, "REWARD");
        }

        //패키지 구매 버튼 클릭 시 호출해주는 함수
        public void MonthlyPackageBuy()
        {
            button_Purchase.interactable = false;
            MonthlyPackagePopup packagePopup = PopupManager.Instance.GetPopUp<MonthlyPackagePopup>("monthlyPackage");
            
            //Test
            //packagePopup.TestPurchaseMonthlyPackage(productId, packageType, packageId, "BUY");
            
            //NonTest
            packagePopup.PurchaseMonthlyPackage(productId, packageType, packageId, "BUY");
        }
  
        //패키지 유지 시간 보여주는 코루틴 함수
        public IEnumerator TotalPackageCoolTime(int timer, UnityAction success)
        {
            string totalCoolTime = LanguageManager.Instance.GetStringData("UI_MonthlyGemPack_DurationTimer");
            
            while (timer > 0)
            {
                text_MonthlyTimer.text = string.Format(totalCoolTime,PackageCalcTime(timer));
                yield return new WaitForSeconds(1f);
                --timer;
            }
            success?.Invoke();
        }

        //리워드 받기 쿨타임 시간 보여주는 코루틴 함수
        public IEnumerator RewardPackageCoolTime(int timer, UnityAction success)
        {
            while (timer > 0)
            {
                text_ButtonTimer.text = PackageCalcTime(timer);
                yield return new WaitForSeconds(1f);
                --timer;
            }
            success?.Invoke();
        }
        public string PackageCalcTime(int timer)
        {
            int hours = timer / 3600;
            int minutes = (timer % 3600) / 60;
            int seconds = timer % 60;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
    }
}
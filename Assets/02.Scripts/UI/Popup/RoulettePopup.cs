using System.Collections.Generic;
using DG.Tweening;
using Framework.Game.Defense;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI
{
    public class RoulettePopup : PopupTemplate
    {
        [SerializeField] private Animator animator;
        //소지하고있는 틱
        [SerializeField] private GameObject stikInfoObj;
        [SerializeField] private TextMeshProUGUI text_HoldingSTik;
        [SerializeField] private RectTransform image_Wheel;
        [SerializeField] private List<TextMeshProUGUI> text_BounsScoreList;
        [SerializeField] private ButtonComponent button_free;
        [SerializeField] private ButtonComponent button_ticket;
        [SerializeField] private ButtonComponent button_stik;
        [SerializeField] private ButtonComponent button_ad;
        [SerializeField] private ButtonComponent button_exit;
        [SerializeField] private TextMeshProUGUI text_ticketCount;
        [SerializeField] private TextMeshProUGUI text_stikCount;
        [SerializeField] private TextMeshProUGUI text_adCount;

        // 연출
        [SerializeField] private GameObject overGlow;
        [SerializeField] private GameObject lightingGlow;
        [SerializeField] private List<Image> lightingGlowList;
        [SerializeField] private GameObject confetti;
        [SerializeField] private Image image_select;

        private bool isFreeUse;
        private int adMaxCount;
        private int adAvailableCount;
        private double stikUseCount;
        // 결과 값
        private int rewardWave = 0;
        private List<int> bounsWaveList = new();

        public void SetRoulette(ReqRouletteGroupData data)
        {
            isFreeUse = data.freeRoulette;
            adMaxCount = data.maxAd;
            adAvailableCount = data.availableAd;
            stikUseCount = data.price;
            if(UserInfoManager.Instance.userItemDic.ContainsKey(UserItemType.ROULETTE_TICKET))
                UserInfoManager.Instance.userItemDic[UserItemType.ROULETTE_TICKET] = data.rouletteTicket;
            
            rewardWave = 0;
            confetti.SetActive(false);
            image_select.gameObject.SetActive(false);
            overGlow.SetActive(false);
            lightingGlow.SetActive(false);

            text_HoldingSTik.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;

            bounsWaveList.Clear();
            bounsWaveList.AddRange(DataManager.Instance.RouletteTableData[data.rouletteGroupId.ToString()]);

            for (int i = 0; i < bounsWaveList.Count && i < text_BounsScoreList.Count; i++)
            {
                text_BounsScoreList[i].text = $"+{bounsWaveList[i]}";
            }

            SetButton();

            ActivePopup();
        }

        // 왼쪽 버튼 : 무료 -> 룰렛티켓 -> STIK(차단국가 비활성화)
        // 오른쪽 버튼 : 광고 (최대2번) -> 비활성화
        private void SetButton()
        {
            button_free.gameObject.SetActive(false);
            button_ticket.gameObject.SetActive(false);
            button_stik.gameObject.SetActive(false);
            stikInfoObj.SetActive(false);

            if (!isFreeUse)
            {
                button_free.gameObject.SetActive(true);
            }
            else if (UserInfoManager.Instance.userItemDic[UserItemType.ROULETTE_TICKET] > 0)
            {
                button_ticket.gameObject.SetActive(true);
                button_ticket.SetInterectible(true);
                text_ticketCount.text = $"<sprite=29>{UserInfoManager.Instance.userItemDic[UserItemType.ROULETTE_TICKET]}/1";
            }
            else if (UserInfoManager.Instance.IsBlockRegion)
            {
                button_ticket.gameObject.SetActive(true);
                button_ticket.SetInterectible(false);
                text_ticketCount.text = $"<sprite=29>{UserInfoManager.Instance.userItemDic[UserItemType.ROULETTE_TICKET]}/1";
            }
            else
            {
                double stikPrice = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                bool canUseTik = stikUseCount > stikPrice;

                stikInfoObj.SetActive(true);
                button_stik.gameObject.SetActive(true);
                button_stik.SetInterectible(!canUseTik);
                text_stikCount.text = $"<sprite=14>{stikUseCount}";
            }

            button_ad.SetInterectible(adAvailableCount > 0);
            text_adCount.text = $"<sprite=6>{adAvailableCount}/{adMaxCount}";
            button_exit.SetInterectible(true);
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);

            //애니메이션 시작
            animator.Rebind();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_free.button.onClick.AddListener(() =>
            {
                button_exit.SetInterectible(false);
                button_free.SetInterectible(false);

                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Free();
            });

            button_ticket.button.onClick.AddListener(() =>
            {
                button_exit.SetInterectible(false);
                button_ticket.SetInterectible(false);

                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Ticket();
            });

            button_stik.button.onClick.AddListener(() =>
            {
                button_exit.SetInterectible(false);
                button_stik.SetInterectible(false, false);

                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Stik();
            });
            
            button_ad.button.onClick.AddListener(() =>
            {
                button_exit.SetInterectible(false);
                button_ad.SetInterectible(false, false);
                
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Ad();
            });

            button_exit.button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
                GameManager.Instance.GameFinishedSend(rewardWave);
            });
        }

        private void OnClick_Free()
        {
            if (isFreeUse)
                return;
            
            _ = NetworkManager.Instance.PostRouletteReward(GameManager.Instance.playId, RoulettePaidType.FREE, (data) =>
            {
                isFreeUse = true;
                // 결과 룰렛 실행 data.result
                rewardWave = data.result;
                SetRouletteRotation();
                
                // D_free_roulette
                AdjustInitializer.TrackEvent("y6167w");
            }, (error) =>
            {
                // 코인부족 및 실패
                ErrorPopup();
            });
        }

        private void OnClick_Ticket()
        {
            if (!isFreeUse)
                return;

            _ = NetworkManager.Instance.PostRouletteReward(GameManager.Instance.playId, RoulettePaidType.TICKET, (data) =>
            {
                UserInfoManager.Instance.userItemDic[UserItemType.ROULETTE_TICKET]--;

                // 결과 룰렛 실행 data.result
                rewardWave = data.result;
                SetRouletteRotation();

                // D_ticket_roulette
                AdjustInitializer.TrackEvent("yb7me5");
            }, (error) =>
            {
                // 코인부족 및 실패
                ErrorPopup();
            });
        }

        private void OnClick_Stik()
        {
            if (!isFreeUse)
                return;

            _ = NetworkManager.Instance.PostRouletteReward(GameManager.Instance.playId, RoulettePaidType.STIK, (data) =>
            {
                // 결과 룰렛 실행 data.result
                rewardWave = data.result;

                GetWalletStik(() =>
                {
                    SetRouletteRotation();
                });
                
                // D_stik_roulette
                AdjustInitializer.TrackEvent("vyy01z");
            }, (error) =>
            {
                // 코인부족 및 실패
                ErrorPopup();
            });
        }

        private void OnClick_Ad()
        {
            if (adAvailableCount <= 0)
                return;

            AdmobManager.onSuccessWatchAd = () =>
            {
                _ = NetworkManager.Instance.PostRouletteReward(GameManager.Instance.playId, RoulettePaidType.AD, (data) =>
                {
                    adAvailableCount--;
                    // 결과 룰렛 실행 data.result
                    rewardWave = data.result;
                    SetRouletteRotation();

                    // D_ad_roulette
                    AdjustInitializer.TrackEvent("y5b1tf");
                }, (error) =>
                {
                    // 코인부족 및 실패
                    ErrorPopup();
                });
            };

            AdmobManager.Instance.ShowRewardedAd();
        }

        private void ErrorPopup()
        {
            // UI 메시지 추가 가능
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("MSG_ROULETTE_FAILED");
            SetButton();
        }

        private void SetRouletteRotation()
        {
            int index = bounsWaveList.IndexOf(rewardWave);
            if (index < 0)
            {
                Debug.Log("Roulette Reward Index Error!!!!!!!!!!!!!!!!!");
                return;
            }

            overGlow.SetActive(true);
            lightingGlow.SetActive(true);
            lightingGlowList.ForEach(i => i.color = Color.white);
            confetti.SetActive(false);
            image_select.gameObject.SetActive(false);

            image_Wheel.localEulerAngles = new Vector3(0, 0, 0);

            SoundManager.Instance.PlaySound(SoundKey.SF_ROULETTE_TURN);
            float onesAngle = 360f / text_BounsScoreList.Count;
            float targetAngle = 1440f - (onesAngle * index);
            image_Wheel.DORotate(new Vector3(0f, 0f, targetAngle), 1.5f, RotateMode.FastBeyond360)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            SetButton();
                            image_select.gameObject.SetActive(true);

                            if (rewardWave >= 70) // 70점이상
                            {
                                SoundManager.Instance.PlaySound(SoundKey.SF_ROULETTE_HIGHSCORE);
                                confetti.SetActive(true);

                                if (ColorUtility.TryParseHtmlString("#FEDF74", out Color targetColor))
                                {
                                    image_select.color = targetColor;
                                    lightingGlowList.ForEach(i => i.color = targetColor);
                                }
                            }
                            else
                            {
                                image_select.color = Color.white;
                                overGlow.SetActive(false);
                                lightingGlow.SetActive(false);
                            }
                        });
        }

        private async void GetWalletStik(UnityAction action)
        {
            UserWalletHistory userStikWallethistory = await NetworkManager.Instance.SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"), SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;
            text_HoldingSTik.text = userStikWallethistory.balance.uiAmountString;

            action?.Invoke();
        }
    }
}
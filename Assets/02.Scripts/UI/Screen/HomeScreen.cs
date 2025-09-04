using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Network;
using Framework.Sound;
using System;
using System.Globalization;
using com.adjust.sdk;


namespace Framework.UI
{
    public class HomeScreen : ScreenTemplate
    {
        public static HomeScreen Instance;
        public ButtonComponent button_Play;
        public ButtonComponent button_Friend;
        public ButtonComponent button_Inbox;
        public ButtonComponent button_Guide;
        public ButtonComponent button_Limited;
        public ButtonComponent button_BattlePass;
        public ButtonComponent button_MonthlyPack;
        public ButtonComponent button_Quest;

        public ButtonComponent button_Event;
        public TextMeshProUGUI text_Period;
        public TextMeshProUGUI text_BattlePassPeriod;
        public TextMeshProUGUI text_LimitedPeriod;
        public GameObject eventNotice;

        public DiscordEvent discordEvent;

        public GameObject postObject;
        public GameObject postNoticeObject;
        public GameObject monthlyNoticeObject;
        public GameObject passNoticeObject;
        public GameObject eventNoticeObject;

        public TextMeshProUGUI text_TodayScore;
        public GameObject questNoticeObject;

        public Animation anim_Transition;

        public int focusDecIdx;
        public UIDec uiDec;
        public bool isDecDataReady = false;

        private void Start()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            button_Play.onPointerUp = () =>
            {
                button_Play.onPointerUp = null;
                SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                RequestStartGame();
            };

            button_Friend.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<FriendPopup>("friend").ActivePopup();
            };

            button_Inbox.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<InboxPopup>("inbox").ActivePopup();
            };

            button_Guide.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<GuidePopup>("guide").ActivePopup();
            };

            button_Event.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<EventListPopup>("eventList").ActivePopup();
            };

            button_Limited.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<LimitedShopPopup>("limitedShop").ActivePopup();
            };

            button_BattlePass.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass").ActivePopup();
            };

            button_MonthlyPack.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<MonthlyPackagePopup>("monthlyPackage").AsyncActivePopup();
            };

            button_Quest.onPointerUp = () =>
            {
                PopupManager.Instance.GetPopUp<QuestPopup>("quest").ActivePopup();
            };

            DecListInit();
            NoticeInitCheck();

            DateTime utcNow = DateTime.UtcNow;
            string date = utcNow.ToString("yyyy-MM-dd");
            
            bool isLimitedPeriod = DataManager.Instance.limitedStoreItem != null;

            button_Limited.gameObject.SetActive(isLimitedPeriod);
            
            if (isLimitedPeriod)
            {
                DateTime period = DateTime.Parse(DataManager.Instance.limitedStoreItem.toDate);
                string textPeriod = period.ToString("MM.dd");
                text_LimitedPeriod.text = $"~ {textPeriod}";
            }

            bool isPassPeriod = UserInfoManager.Instance.userPassInfo.isActivated;
            if (isPassPeriod)
            {
                DateTime passPeriod = DateTime.Parse(UserInfoManager.Instance.userPassInfo.toDate);
                string textPassPeriod = passPeriod.ToString("MM.dd");

                text_BattlePassPeriod.text = $"~{textPassPeriod}";
            }

            GetUserGoInfo(date);
            GetUserEventPrize();
        }

        public async void GetUserGemInfo()
        {
            await NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        public void SuccessWallet(int gemValue)
        {
            LobbyManager.Instance.text_Gem.text = gemValue.ToString();
        }

        public async void GetUserGoInfo(string date)
        {
            await NetworkManager.Instance.GetUserDailyRankInfo(date, SetTodayGo);
        }

        public void SetTodayGo(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                text_TodayScore.text = "0";
            }
            else
            {
                UserRankInfo data = JsonUtility.FromJson<UserRankInfo>(json);

                text_TodayScore.text = data.obtainedGo.ToString("N0", new CultureInfo("en-US"));
                //data.obtainedGo.ToString("N0", new CultureInfo("vi-VN"));
            }
        }

        public void OnCompleteGameStart()
        {

            SceneLoadManager.Instance.SwitchingScene(3);
        }

        public void ReleasePlayButton(bool isInterectable)
        {
            button_Play.SetInterectible(isInterectable);
        }

        public void GameStart()
        {
            StartCoroutine(StartGameSequence());
        }

        public IEnumerator StartGameSequence()
        {
            anim_Transition.gameObject.SetActive(true);
            anim_Transition.Play();

            float length = anim_Transition["TransitionEntry"].length;

            yield return new WaitForSeconds(length);

            //yield return new WaitForSeconds(0.5f);
            SceneLoadManager.Instance.SwitchingScene(3);

            SceneLoadManager.onCompleteLoadScene = () =>
            {
                // D_game_start
                if (!UserInfoManager.Instance.userState.finishedTutorial)
                {
                    AdjustInitializer.TrackEvent("qds6zs");
                }
                SoundManager.Instance.PlaySound(SoundKey.BGM_INGAME);
            };
        }

        public async void RequestStartGame()
        {
            //PopupManager.Instance.GetPopUp<EnergyPopup>("energy").ActivePopup();

            ReleasePlayButton(false);
            if (UserInfoManager.Instance.userState.energyAmount < 10)
            {
                ReleasePlayButton(true);
                PopupManager.Instance.GetPopUp<EnergyPopup>("energy").ActivePopup();

                //button_Play.onPointerUp = null;없이 한번만 셋해주면 문제없는 로직이지만
                //핸드폰이 느리면 한프레임에 2번 호출이 될 가능성 우려 방어 코드 작성
                button_Play.onPointerUp = () =>
                {
                    button_Play.onPointerUp = null;
                    SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                    RequestStartGame();
                };
            }
            else
            {
                //게임 끝나고 다시 홈으로 오면 다시 이니셜라이즈 해주기 때문에 문제X
                //button_Play.onPointerUp 다시 작성해줄 필요X
                UserInfoManager.OnCompleteGameStart = () =>
                {
                    if (!UserInfoManager.Instance.userState.finishedTutorial)
                    {
                        button_Play.transform.SetParent(transform);
                        LobbyTutorialManager.Instance.LobbyTutorialFailed();
                    }

                    GameStart();
                };

                if (!UserInfoManager.Instance.userState.finishedTutorial)
                {
                    await NetworkManager.Instance.GetRewardRule();
                }
                else
                {
                    await NetworkManager.Instance.RegistUserPlay(FailedGameStart, FailedGameReStart);
                }
            }
        }

        public void FailedGameStart(string data)
        {
            //button_Play.onPointerUp = null;없이 한번만 셋해주면 문제없는 로직이지만
            //핸드폰이 느리면 한프레임에 2번 호출이 될 가능성 우려 방어 코드 작성
            button_Play.onPointerUp = () =>
            {
                button_Play.onPointerUp = null;
                SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                RequestStartGame();
            };

            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                button_Play.transform.SetParent(transform);
                LobbyTutorialManager.Instance.LobbyTutorialFailed();
            }

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeText(data);
            ReleasePlayButton(true);
        }

        public void DecListInit()
        {
            isDecDataReady = true;
            uiDec.Initialize(menuType);
            uiDec.SetUserDec(UserSlotManager.Instance.GetFocusIdx());
        }

        public void FailedGameReStart(string data)
        {
            button_Play.onPointerUp = () =>
            {
                button_Play.onPointerUp = null;
                SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                RequestStartGame();
            };


            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                button_Play.transform.SetParent(transform);
                LobbyTutorialManager.Instance.LobbyTutorialFailed();
            }

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeText(data);

            popup.button_Close.onClick.AddListener(() =>
            {
                popup.InActivePopupLogout(true);
            });


            ReleasePlayButton(true);
        }

        public override void ActiveScreen()
        {
            //Todo : Dec Setting
            if (isDecDataReady)
            {
                DecListInit();
            }

            NoticeActiveCheck();

            postObject.SetActive(true);
            discordEvent.SetEventInfo(UserInfoManager.Instance.discordConnect);
            button_Limited.gameObject.SetActive(DataManager.Instance.limitedStoreItem != null);
            discordEvent.gameObject.SetActive(true);
            button_Event.gameObject.SetActive(true);
            button_BattlePass.gameObject.SetActive(UserInfoManager.Instance.userPassInfo.isActivated);
            button_MonthlyPack.gameObject.SetActive(true);
            button_Friend.gameObject.SetActive(true);
            button_Quest.gameObject.SetActive(true);

            //button_Limited.gameObject.SetActive(true);
        }

        public override void InactiveScreen()
        {
            //Todo : Screen Off
            postObject.SetActive(false);
            discordEvent.gameObject.SetActive(false);
            discordEvent.gemIcon.SetActive(false);
            button_Event.gameObject.SetActive(false);
            button_Limited.gameObject.SetActive(false);
            button_BattlePass.gameObject.SetActive(false);
            button_MonthlyPack.gameObject.SetActive(false);
            button_Quest.gameObject.SetActive(false);
            button_Friend.gameObject.SetActive(false);
        }

        public void GetUserEventPrize()
        {
            _ = NetworkManager.Instance.GetUserEventPrize(SuccessEventPrize, FailedEventPrize);
        }

        public void SuccessEventPrize(UserEventPrize data)
        {
            EventPrize eventPrize = (EventPrize)Enum.Parse(typeof(EventPrize), data.eventPrize);

            switch (eventPrize)
            {
                case EventPrize.NONE:
                    break;
                case EventPrize.PRIZE:
                    EventRewardPopup popup = PopupManager.Instance.GetPopUp<EventRewardPopup>("eventReward");
                    popup.ActivePopup();
                    break;
                case EventPrize.PRIZE_COMPLETED:
                    break;
                default:
                    break;
            }
        }

        //첫 홈 화면 진입시에만 레드닷 초기화
        public void NoticeInitCheck()
        {
            //battlepass
            // BattlePassPopup passPopup = PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass");
            // passPopup.SetBattlePassNotice();
            //
            // //monthly
            // MonthlyPackagePopup monthlyPopup = PopupManager.Instance.GetPopUp<MonthlyPackagePopup>("monthlyPackage");
            // monthlyPopup.GetPackage();
        }

        //홈화면이 액티브 될때마다 레드닷 초기화
        public void NoticeActiveCheck()
        {
            //inbox
            InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
            popup.SetInboxData(false);

            bool isNewObject = popup.activeInboxItem.Count != 0;
            postNoticeObject.SetActive(isNewObject);

            //todo 퀘스트 레드닷
            QuestPopup questPopup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            questPopup.NonActivePopupNoticeCheck();

            //monthly
            // MonthlyPackagePopup monthlyPopup = PopupManager.Instance.GetPopUp<MonthlyPackagePopup>("monthlyPackage");
            // monthlyPopup.GetPackage();

            //battlePass 해줄 필요없음 내부적으로 팝업을 열고 닫을 때 처리해주기 때문
            //홈화면을 계속클릭해서 확인해줄 필요x

            //shop 내부적으로 Init할때 한번 호출해줌 지속적으로 호출할 필요X
        }


        public void FailedEventPrize()
        {

        }

        public void OnApplicationFocus(bool focus)
        {
            if (focus)
            {

            }
            else
            {

            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Network;
using Framework.Sound;
using TMPro;
using UnityEngine.UI;
using System;
using Framework.GameData.Defense;
using Framework.Util;
using UnityEngine.Events;
using AssetKits.ParticleImage;

namespace Framework.UI
{
    [Serializable]
    public class QuestGroupSizeInfo
    {
        public RectTransform rect_BackGround;
        public RectTransform rect_Group;
        public int groupAmount;
        public QuestType state;
    }

    public class QuestPopup : PopupTemplate
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Timer;

        public TextMeshProUGUI[] text_bottomDailyQuestTitle;
        public TextMeshProUGUI[] text_bottomAchivementTitle;

        public CanvasGroup canvasGroup_Daily;
        public CanvasGroup canvasGroup_Achievement;

        public List<QuestItem> list_QuestItems = new();
        public Queue<QuestItem> queue_QuestItems = new();

        public List<CompleteMissionItem> list_CompleteItmes = new();
        public Queue<CompleteMissionItem> queue_CompleteItems = new();

        public Slider slider_CompleteGuage;

        public RectTransform rect_CompleteParent;
        public RectTransform rect_QuestParent;
        public RectTransform rect_AchievementParent;
        public RectTransform rect_attractorTarget;


        public Button[] button_Daily;
        public Button[] button_Achievement;

        public GameObject[] object_DailyNotice;
        public GameObject[] object_AchievementNotice;

        public QuestType prePageState;
        public QuestType currentPageState;

        public IEnumerator DailyQuestTimerCoroutine;

        public int userDailyQuestExp;

        public bool isQuestNotice;
        public bool isAchievementNotice;

        //기획상 절때 변하지 않는값이라 지정됨
        public readonly int userDailyQuestMaxExp = 100;
        public override void ActivePopup()
        {
            prePageState = QuestType.NONE;
            currentPageState = QuestType.DAILY;
            PageChange(currentPageState, true);
            NonActivePopupNoticeCheck(QuestType.ACHIEVEMENT);
        }

        public override void InActivePopup()
        {
            prePageState = QuestType.NONE;
            currentPageState = QuestType.DAILY;
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            for (int i = 0; i < button_Daily.Length; ++i)
            {
                button_Daily[i].onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                    PageChange(QuestType.DAILY);
                });

                text_bottomDailyQuestTitle[i].text = LanguageManager.Instance.GetStringData("UI_Daily_Quest");
            }

            for (int j = 0; j < button_Achievement.Length; ++j)
            {
                button_Achievement[j].onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                    PageChange(QuestType.ACHIEVEMENT);
                });

                text_bottomAchivementTitle[j].text = LanguageManager.Instance.GetStringData("UI_Achievement");
            }
        }

        public IEnumerator DailyQuestTimeOut(UnityAction<QuestType, bool> success)
        {
            //todo region 확인
            DateTime tomorrowMidnight = DateTime.UtcNow.Date.AddDays(1);
            TimeSpan time = tomorrowMidnight - DateTime.UtcNow;

            // Debug.Log(DateTime.UtcNow);
            // Debug.Log(time.TotalSeconds);
            // Debug.Log($"utc 남은시간:{time.Hours}:{time.Minutes}:{time.Seconds}");

            int seconds = (int)time.TotalSeconds;
            //int seconds = 10;

            while (seconds > 0)
            {
                text_Timer.text = $"<sprite=23>{CalcTimer(seconds)}";
                yield return new WaitForSeconds(1f);
                --seconds;
            }
            success?.Invoke(QuestType.DAILY, false);
        }

        public string CalcTimer(int timer)
        {
            int hours = timer / 3600;
            int minutes = (timer % 3600) / 60;
            int seconds = timer % 60;

            string stringTime = LanguageManager.Instance.GetStringData("UI_Daily_Quest_Limit");

            return string.Format(stringTime, hours, minutes, seconds);
        }

        public void PageSequence(CanvasGroup canvasGroup, bool isShow)
        {
            float alpha = isShow ? 1 : 0;

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }

        public async void GetQuestData(bool isActivate)
        {
            text_Title.text = LanguageManager.Instance.GetStringData("UI_Daily_Quest");

            //NonTest
            await NetworkManager.Instance.GetUserDailyQuestDetail(SetDailyQuestDetail, Failed);

            PageSequence(canvasGroup_Daily, true);
            PageSequence(canvasGroup_Achievement, false);

            //test
            // SetCompleteInfo(testData.text);
            // SetQuestInfo(testData.text);
            // DailyQuestTimerCoroutine = DailyQuestTimeOut(PageChange);
            // StartCoroutine(DailyQuestTimerCoroutine);

            if (isActivate)
            {
                PopUpSequence(true);
            }
        }

        public async void GetAchievementData(bool isActivate)
        {
            text_Title.text = LanguageManager.Instance.GetStringData("UI_Achievement");

            //NonTest
            await NetworkManager.Instance.GetUserAchievementDetail(SetAchievementDetail, Failed);

            PageSequence(canvasGroup_Daily, false);
            PageSequence(canvasGroup_Achievement, true);

            //test
            //SetAchievementInfo("test");

            if (isActivate)
            {
                PopUpSequence(true);
            }
        }

        public void SetDailyQuestDetail(string json)
        {
            bool isCompleteNotice;
            bool isQuestNotice;
            bool isTotalNotice;

            isCompleteNotice = SetCompleteInfo(json);
            isQuestNotice = SetQuestInfo(json);

            isTotalNotice = isCompleteNotice || isQuestNotice;

            BottomNoticeActivate(QuestType.DAILY, isTotalNotice);

            ActionInitialize();
            DailyQuestTimerCoroutine = DailyQuestTimeOut(PageChange);
            StartCoroutine(DailyQuestTimerCoroutine);
        }

        public void SetAchievementDetail(string json)
        {
            SetAchievementInfo(json);
        }

        public bool SetCompleteInfo(string json)
        {
            DailyQuestData data = JsonUtility.FromJson<DailyQuestData>(json);
            UserDailyQuests[] completeDatas = data.userDailyQuests;
            DailyQuestReward completeRewardData = data.dailyQuestReward;

            userDailyQuestExp = SumCompleteMissionReward(completeDatas);

            //기획자 의도로 인한 하드코딩 가능한 변수
            //컴플리트 박스의 경험치 추가 및 변경은 앞으로도없다라고 하심
            int defalutBoxExp = 20;

            slider_CompleteGuage.value = (float)userDailyQuestExp / userDailyQuestMaxExp;

            RefreshCompleteItem();

            bool isNotice = false;

            //컴플리트 박스 5개 확정 하드코딩 가능
            for (int i = 0; i < 5; ++i)
            {
                int boxExp = (i + 1) * defalutBoxExp;
                bool isValidate = GetCompleteMissionRewardCheck(i, completeRewardData);
                CompleteMissionItem item = GetCompleteMissionItem();
                item.transform.SetParent(rect_CompleteParent);
                item.gameObject.SetActive(true);

                item.Initialize(isValidate, boxExp, userDailyQuestExp);

                if (!isNotice)
                {
                    isNotice = GetCompleteMissionRewarded(isValidate, boxExp, userDailyQuestExp);
                }
            }

            Queue_CompleteMissionClear();

            return isNotice;
        }

        public int SumCompleteMissionReward(UserDailyQuests[] data)
        {
            int sumAmount = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                QuestRewardStateType type = (QuestRewardStateType)Enum.Parse(typeof(QuestRewardStateType), data[i].rewardStatus);

                if (type == QuestRewardStateType.REWARDED)
                {
                    sumAmount += data[i].rewardAmount;
                }
            }

            return sumAmount;
        }

        public bool GetCompleteMissionRewardCheck(int index, DailyQuestReward data)
        {
            bool isRewarded = index switch
            {
                0 => data.firstReward,
                1 => data.secondReward,
                2 => data.thirdReward,
                3 => data.fourthReward,
                4 => data.fifthReward,
                _ => false
            };

            return isRewarded;
        }

        public bool SetQuestInfo(string json, bool isReward = false)
        {
            RefreshQuestItem();

            DailyQuestData data = JsonUtility.FromJson<DailyQuestData>(json);

            bool isNotice = false;

            for (int i = 0; i < data.userDailyQuests.Length; ++i)
            {
                QuestItem item = GetQuestItem();
                item.transform.SetParent(rect_QuestParent, false);
                item.gameObject.SetActive(true);
                item.transform.SetSiblingIndex(i);

                item.Initialize(data.userDailyQuests[i], QuestType.DAILY, isReward);

                if (!isNotice && data.userDailyQuests[i].rewardStatus.Contains("AVAILABLE"))
                {
                    isNotice = true;
                }
            }

            Queue_Clear();

            return isNotice;
        }

        public void SetAchievementInfo(string json, bool isReward = false)
        {
            RefreshQuestItem();

            //Non Test
            AchievementData data = JsonUtility.FromJson<AchievementData>(json);

            //Test
            //AchievementData data = JsonUtility.FromJson<AchievementData>(testData.text);

            bool isNotice = false;

            for (int i = 0; i < data.userAchievements.Length; ++i)
            {
                QuestItem item = GetQuestItem();
                item.transform.SetParent(rect_AchievementParent, false);
                item.gameObject.SetActive(true);
                item.transform.SetSiblingIndex(i);

                item.Initialize(data.userAchievements[i], QuestType.ACHIEVEMENT, isReward);

                if (!isNotice && data.userAchievements[i].rewardStatus.Contains("AVAILABLE"))
                {
                    isNotice = true;
                }
            }

            BottomNoticeActivate(QuestType.ACHIEVEMENT, isNotice);
            Queue_Clear();
        }

        public void PageChange(QuestType state, bool isPopupActivate = false)
        {
            if (state == prePageState) return;

            prePageState = state;

            ActionInitialize();

            switch (state)
            {
                case QuestType.DAILY:
                    GetQuestData(isPopupActivate);
                    break;
                case QuestType.ACHIEVEMENT:
                    //todo 주석 해제해줘야함 500끝나면
                    GetAchievementData(isPopupActivate);
                    break;
                //주간퀘스트가 혹시나 만약에 추가되면 여기다가작업
                default:
                    break;
            }
        }

        public void Failed(string errorData)
        {
            InActivePopup();

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

            try
            {
                ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

                switch (error.errorCode)
                {
                    case "TOO_MANY_REQUESTS":
                        //짧은 시간 내에 일정 기준의 연속적인 호출
                        popup.SetNoticeMessage("MSG_TOO_MANY_REQUESTS");
                        break;
                    default:
                        //네트워크 환경 불안정
                        popup.SetNoticeMessage("UI_Network_Unstable");
                        break;
                }
            }
            catch
            {
                popup.SetNoticeMessage("UI_Network_Unstable");
            }

        }

        public void RefreshQuestItem()
        {
            for (int i = 0; i < list_QuestItems.Count; ++i)
            {
                list_QuestItems[i].gameObject.SetActive(false);
                queue_QuestItems.Enqueue(list_QuestItems[i]);
            }
        }

        public void RefreshCompleteItem()
        {
            for (int i = 0; i < list_CompleteItmes.Count; ++i)
            {
                list_CompleteItmes[i].gameObject.SetActive(false);
                queue_CompleteItems.Enqueue(list_CompleteItmes[i]);
            }
        }

        public void Queue_CompleteMissionClear()
        {
            queue_CompleteItems.Clear();
        }

        public void Queue_Clear()
        {
            queue_QuestItems.Clear();
        }

        public CompleteMissionItem GetCompleteMissionItem()
        {
            if (queue_CompleteItems.Count == 0)
            {
                GameObject obj = Instantiate(list_CompleteItmes[0].gameObject);
                CompleteMissionItem item = obj.GetComponent<CompleteMissionItem>();
                list_CompleteItmes.Add(item);
                return item;
            }
            else
            {
                return queue_CompleteItems.Dequeue();
            }
        }

        public QuestItem GetQuestItem()
        {
            if (queue_QuestItems.Count == 0)
            {
                GameObject obj = Instantiate(list_QuestItems[0].gameObject);
                QuestItem item = obj.GetComponent<QuestItem>();
                list_QuestItems.Add(item);
                return item;
            }
            else
            {
                return queue_QuestItems.Dequeue();
            }
        }

        public void SetQuestItemButtonInteractable(bool isInteractable)
        {
            for (int i = 0; i < list_QuestItems.Count; ++i)
            {
                if (list_QuestItems[i].rewardState == QuestRewardStateType.AVAILABLE)
                {
                    list_QuestItems[i].button_Reward.SetInterectible(isInteractable);
                }
            }
        }

        public void QuestReward(string json, QuestItem item)
        {
            bool isCompleteNotice;
            bool isQuestNotice;
            bool isTotalNotice;

            isCompleteNotice = SetCompleteInfo(json);
            isQuestNotice = SetQuestInfo(json, true);

            isTotalNotice = isCompleteNotice || isQuestNotice;

            BottomNoticeActivate(QuestType.DAILY, isTotalNotice);

            DailyQuestTimerCoroutine = DailyQuestTimeOut(PageChange);
            StartCoroutine(DailyQuestTimerCoroutine);

            AttractorManager.Instance.SetAttractor_StartAction_LateInteractable("quest", item.rect_Star, GetDailyQuestExp, rect_attractorTarget);
        }

        public void AchievementReward(string json, QuestItem item)
        {
            GetAchievementReward(item);
            SetAchievementInfo(json, true);
        }

        public void GetAchievementReward(QuestItem item)
        {
            //각 타입별로 최신화 시켜줘야함
            switch (item.rewardType)
            {
                case RandomRewardType.ENERGY:
                    AttractorManager.Instance.SetAttractor_StartAction_LateInteractable("achievementEnergy", item.rect_Star, GetEnergy);
                    break;
                case RandomRewardType.GEM:
                    AttractorManager.Instance.SetAttractor_StartAction_LateInteractable("achievementGem", item.rect_Star, GetGemWallet);
                    break;
                case RandomRewardType.HATCHING_ORB:
                    AttractorManager.Instance.SetHatchingAttractor_StartAction_LateInteractable("achievementHatching", item.rewardType.ToString(), item.rect_Star, GetHatching);
                    break;
                default:
                    break;
            }
        }

        public void GetDailyQuestExp()
        {
            SetQuestItemButtonInteractable(true);
        }

        public void GetEnergy()
        {
            SetQuestItemButtonInteractable(true);

            LobbyManager.Instance.GetEnergyValue();
        }

        public void GetHatching()
        {
            SetQuestItemButtonInteractable(true);

            _ = NetworkManager.Instance.GetHatchingStone();
        }

        public void GetGemWallet()
        {
            SetQuestItemButtonInteractable(true);

            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessGem, "PTIK");
        }

        public void SuccessGem(int gemValue)
        {
            int preGemValue = UserInfoManager.Instance.gemValue;
            UserInfoManager.Instance.gemValue = gemValue;

            //로비 젬 표시 변경
            LobbyManager.Instance.text_Gem.text = $"{gemValue}";

            StartCoroutine(LobbyManager.Instance.ChangeValueSequenceAsync(gemValue, preGemValue, LobbyManager.Instance.text_Gem));
        }

        public void ActionInitialize()
        {
            if (null != DailyQuestTimerCoroutine)
            {
                StopCoroutine(DailyQuestTimerCoroutine);
                DailyQuestTimerCoroutine = null;
            }
        }

        public void HomeNoticeActive()
        {
            bool isNoticeActive = false;
            isNoticeActive = isQuestNotice || isAchievementNotice;
            HomeScreen.Instance.questNoticeObject.SetActive(isNoticeActive);
        }

        public async void NonActivePopupNoticeCheck()
        {
            await NetworkManager.Instance.GetUserDailyQuestDetail(GetDailyQuestNoticeCheck, Failed);
            await NetworkManager.Instance.GetUserAchievementDetail(GetAchievementNoticeCheck, Failed);

            HomeNoticeActive();
        }

        public async void NonActivePopupNoticeCheck(QuestType state)
        {
            switch (state)
            {
                case QuestType.DAILY:
                    await NetworkManager.Instance.GetUserDailyQuestDetail(GetDailyQuestNoticeCheck, Failed);
                    break;
                case QuestType.ACHIEVEMENT:
                    await NetworkManager.Instance.GetUserAchievementDetail(GetAchievementNoticeCheck, Failed);
                    break;
                default:
                    break;
            }
        }

        public void GetDailyQuestNoticeCheck(string json)
        {
            DailyQuestData data = JsonUtility.FromJson<DailyQuestData>(json);
            DailyQuestReward completeRewardData = data.dailyQuestReward;

            bool isNotice = false;
            int defalutBoxExp = 20;
            int userExp = SumCompleteMissionReward(data.userDailyQuests);

            for (int i = 0; i < 5; ++i)
            {
                int boxExp = (i + 1) * defalutBoxExp;
                bool isValidate = GetCompleteMissionRewardCheck(i, completeRewardData);
                isNotice = GetCompleteMissionRewarded(isValidate, boxExp, userExp);
                if (isNotice) break;
            }

            //컴플리트 미션 보상이 활성화된게 없으면 조건성립
            if (!isNotice)
            {
                for (int i = 0; i < data.userDailyQuests.Length; ++i)
                {
                    if (data.userDailyQuests[i].rewardStatus.Contains("AVAILABLE"))
                    {
                        isNotice = true;
                        break;
                    }
                }
            }

            BottomNoticeActivate(QuestType.DAILY, isNotice);
        }

        public void GetAchievementNoticeCheck(string json)
        {
            AchievementData data = JsonUtility.FromJson<AchievementData>(json);

            bool isNotice = false;

            for (int i = 0; i < data.userAchievements.Length; ++i)
            {
                if (data.userAchievements[i].rewardStatus.Contains("AVAILABLE"))
                {
                    isNotice = true;
                    break;
                }
            }

            BottomNoticeActivate(QuestType.ACHIEVEMENT, isNotice);
        }

        public void BottomNoticeActivate(QuestType state, bool isActive)
        {
            //유니티 인스펙터 창에는 해당 페이지 마다
            //버튼이 따로 있어서 업적 부분의 notice와 일일퀘스트부분의 notice를 함께담아둠
            //또한 업적도 일일퀘스트 부분의 notice와 업적의 업적notice를 담아두었음

            switch (state)
            {
                case QuestType.DAILY:
                    for (int i = 0; i < object_DailyNotice.Length; ++i)
                    {
                        object_DailyNotice[i].SetActive(isActive);
                    }
                    isQuestNotice = isActive;
                    break;
                case QuestType.ACHIEVEMENT:
                    for (int i = 0; i < object_AchievementNotice.Length; ++i)
                    {
                        object_AchievementNotice[i].SetActive(isActive);
                    }
                    isAchievementNotice = isActive;
                    break;
                default:
                    break;
            }

            HomeNoticeActive();
        }

        public bool GetCompleteMissionRewarded(bool isReward, int boxExp, int userExp)
        {
            bool isCompleteMissionAvailable;

            if (boxExp <= userExp)
            {
                isCompleteMissionAvailable = !isReward;
            }
            else
            {
                isCompleteMissionAvailable = isReward;
            }

            return isCompleteMissionAvailable;
        }


    }
}


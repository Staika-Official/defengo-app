using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Network;
using System;
using TMPro;
using System.Globalization;
using Framework.Util;
using Framework.GameData.Defense;
using System.Collections;

namespace Framework.UI
{
    public class RankingScreen : ScreenTemplate
    {
        public static RankingScreen Instance;

        [SerializeField] private GameObject noRank;
        [SerializeField] private GameObject devideLine;
        [SerializeField] private GameObject weeklyPodium;
        [SerializeField] private RankInfoWeeklyItem[] rankLeaderBoardInfoItems;
        [SerializeField] private Animator animator;
        [SerializeField] private RectTransform scrollRect;
        [SerializeField] private RectTransform viewPortRect;
        [SerializeField] private GameObject rankItem;
        //public Sprite[] badgeSprites;
        [SerializeField] private Transform inactive;
        [SerializeField] private TextMeshProUGUI text_Today;
        [SerializeField] private TextMeshProUGUI text_weekly;
        [SerializeField] private TextMeshProUGUI text_WeeklyNum;
        [SerializeField] private Button button_Tip;
        public Button button_weeklyTip;
        [SerializeField] private MyRank myRank;
        [SerializeField] private int dailyLastRoundId = 0;
        [SerializeField] private int lastRoundId = 0;
        [SerializeField] private int focusRoundId;

        [SerializeField] private GameObject guideNotification;

        [SerializeField] private GameObject dailyObject;
        [SerializeField] private GameObject weeklyObject;

        [SerializeField] private CustomToggle toggle;

        [SerializeField] private TextMeshProUGUI text_DailyRewardPool;
        [SerializeField] private TextMeshProUGUI text_NoRanking;

        [SerializeField] private Button[] buttons_Date;
        [SerializeField] private Button[] buttons_weeks;

        [SerializeField] private LeaderBoardType focusLeaderBoardType = LeaderBoardType.WAVE_DAILY;

        [SerializeField] private List<RankInfoItem> activeRankinfoItems = new();
        [SerializeField] private Queue<RankInfoItem> rankInfoItems = new();

        private void Start()
        {
            Instance = this;
        }

        public override void ActiveScreen()
        {
            //todo (true)-Daily, (false)-Weekly
            if (UserInfoManager.Instance.userState.finishedTutorial == false)
            {
                focusLeaderBoardType = LeaderBoardType.WAVE_WEEKLY;
                toggle.SetOnOff(true);
            }
                
            TogglePage(focusLeaderBoardType == LeaderBoardType.WAVE_DAILY);
            //TogglePage(false);
            
            focusRoundId = focusLeaderBoardType == LeaderBoardType.WAVE_DAILY ? dailyLastRoundId : lastRoundId;
            animator.Rebind();
            animator.SetTrigger("_GoEntry");

            guideNotification.SetActive(SecurePlayerPrefs.GetInt("guide") != 2);
        }

        public override void InactiveScreen()
        {
            for (int i = 0; i < activeRankinfoItems.Count; i++)
            {
                activeRankinfoItems[i].transform.SetParent(inactive);
                activeRankinfoItems[i].gameObject.SetActive(false);
                rankInfoItems.Enqueue(activeRankinfoItems[i]);
            }

            activeRankinfoItems.Clear();
            myRank.Hide();
        }

        public void RankDetail(string userId)
        {
            RankProfilePopup popup = PopupManager.Instance.GetPopUp<RankProfilePopup>("rankProfile");
            _ = NetworkManager.Instance.GetUserProfileDetail(focusLeaderBoardType, userId, focusRoundId, popup.SetProfileData, FailedDetail);
        }

        public void FailedDetail()
        {

        }

        public void OnClick(bool isPrev)
        {
            if (focusLeaderBoardType == LeaderBoardType.WAVE_DAILY)
                foreach (var button in buttons_Date)
                    button.interactable = false;
            else
                foreach (var button in buttons_weeks)
                    button.interactable = false;
            
            for (int i = 0; i < activeRankinfoItems.Count; i++)
            {
                activeRankinfoItems[i].transform.SetParent(inactive);
                activeRankinfoItems[i].gameObject.SetActive(false);
                rankInfoItems.Enqueue(activeRankinfoItems[i]);
            }

            activeRankinfoItems.Clear();

            focusRoundId += isPrev ? -1 : 1;

            GetLeaderBoardBestWavesData(focusRoundId);
        }
        
        public void TogglePage(bool isLeftOn)
        {
            if (isLeftOn)
            {
                //todo DailyRanking
                focusRoundId = dailyLastRoundId;
                focusLeaderBoardType = LeaderBoardType.WAVE_DAILY;
                dailyObject.SetActive(true);
                weeklyObject.SetActive(false);

                animator.Rebind();
                animator.SetTrigger("_GoEntry");
                string now = DateTime.UtcNow.ToString("yyyy-MM-dd");

                for (int i = 0; i < activeRankinfoItems.Count; i++)
                {
                    activeRankinfoItems[i].transform.SetParent(inactive);
                    activeRankinfoItems[i].gameObject.SetActive(false);
                    rankInfoItems.Enqueue(activeRankinfoItems[i]);
                }
                
                GetLeaderBoardBestWavesData(focusRoundId);
            }
            else
            {
                focusRoundId = lastRoundId;
                focusLeaderBoardType = LeaderBoardType.WAVE_WEEKLY;

                //todo DailyRanking관련 이미지 오브젝트
                dailyObject.SetActive(false);

                weeklyObject.SetActive(true);

                for (int i = 0; i < activeRankinfoItems.Count; i++)
                {
                    activeRankinfoItems[i].transform.SetParent(inactive);
                    activeRankinfoItems[i].gameObject.SetActive(false);
                    rankInfoItems.Enqueue(activeRankinfoItems[i]);
                }

                GetLeaderBoardBestWavesData(focusRoundId);
            }

            activeRankinfoItems.Clear();
        }

        public async void GetLeaderBoardBestWavesData(int roundId)
        {
            await NetworkManager.Instance.GetLeaderBoardBestWaves(focusLeaderBoardType, roundId, SetLeaderBoardRankingData, Failed);
            await NetworkManager.Instance.GetMyLeaderBoardRankInfo(focusLeaderBoardType, roundId, SetMyRankInfo, Failed);
        }
        
        public void SetMyRankInfo(string json)
        {
            myRank.SetMyRank(json);
        }
        
        public void Failed()
        {
            foreach (var button in buttons_Date)
                button.interactable = true;
                
            foreach (var button in buttons_weeks)
                button.interactable = true;
            
            Debug.Log("Failed Get Ranking");
        }

        public RankInfoItem GetRankInfoItem()
        {
            if(rankInfoItems.Count > 0)
            {
                RankInfoItem item = rankInfoItems.Dequeue();
                return item;
            }
            else
            {
                RankInfoItem item = Instantiate(rankItem).GetComponent<RankInfoItem>();
                return item;
            }
        }

        public void SetLeaderBoardRankingData(string json)
        {
            foreach (var item in activeRankinfoItems)
                rankInfoItems.Enqueue(item);

            activeRankinfoItems.Clear();

            bool isDaily = focusLeaderBoardType == LeaderBoardType.WAVE_DAILY;

            LeaderBoardRankInfoList data = JsonUtility.FromJson<LeaderBoardRankInfoList>(json);
            
            Button[] buttons = isDaily ? buttons_Date : buttons_weeks;
            int lastId = 0;
            if (isDaily)
            {
                if (dailyLastRoundId == 0)
                {
                    dailyLastRoundId = data.roundId;
                    focusRoundId = data.roundId;
                }
                lastId = dailyLastRoundId;
            }
            else
            {
                if (lastRoundId == 0)
                {
                    lastRoundId = data.roundId;
                    focusRoundId = data.roundId;
                }
                lastId = lastRoundId;
            }

            buttons[0].gameObject.SetActive(1 < focusRoundId);
            buttons[1].gameObject.SetActive(data.roundId < lastId);

            LeaderBoardRankInfo[] rankInfos = data.waveWeeklyRanking;

            if (isDaily)
            {
                DateTime fromDate = DateTime.Parse(data.fromDate);
                text_Today.text = fromDate.ToString("yyyy.MM.dd");
            }
            else
            {
                DateTime fromDate = DateTime.Parse(data.fromDate);
                string from = fromDate.ToString("yyyy.MM.dd");
                DateTime toDate = DateTime.Parse(data.toDate);
                string to = toDate.ToString("yyyy.MM.dd");
                text_weekly.text = $"{from} ~ {to}";
                text_WeeklyNum.text = $"Week {data.roundId}";
            }

            bool isNoRank = rankInfos.Length == 0;

            if (data.activated)
            {
                noRank.SetActive(isNoRank);
                devideLine.SetActive(!isNoRank);
                weeklyPodium.SetActive(!isDaily && !isNoRank);
                text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Ranking_Empty");
            }
            else
            {
                noRank.SetActive(!data.activated);
                devideLine.SetActive(data.activated);
                weeklyPodium.SetActive(!isDaily && data.activated);
                text_NoRanking.text = LanguageManager.Instance.GetStringData(isDaily ? "UI_Ranking_Empty" : "UI_Weekly_Ready_Desc");
            }

            scrollRect.sizeDelta = new Vector2(0, rankInfos.Length * 152);
            viewPortRect.sizeDelta = new Vector2(0, rankInfos.Length * 152 + 2000);

            foreach (var item in rankLeaderBoardInfoItems)
                item.gameObject.SetActive(false);

            // UI 업데이트를 프레임마다 나누어 수행
            StartCoroutine(UpdateRankUI(rankInfos, isDaily));
        }

        private IEnumerator UpdateRankUI(LeaderBoardRankInfo[] rankInfos, bool isDaily)
        {
            float offsetY = -123f;
            
            // WeeklyRanking 3위 까지 커스텀
            for (int i = 0; i < rankInfos.Length; i++)
            {
                if (!isDaily && i < 3)
                {
                    rankLeaderBoardInfoItems[i].gameObject.SetActive(true);
                    rankLeaderBoardInfoItems[i].Initialize(rankInfos[i], i + 1);
                    offsetY -= 151f;
                }
                else
                {
                    RankInfoItem obj = GetRankInfoItem();
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(scrollRect, false);
                    obj.transform.localPosition = new Vector2(0, offsetY);
                    offsetY -= 151f;

                    activeRankinfoItems.Add(obj);
                    obj.Initialize(rankInfos[i], i + 1);
                }

                if (i == 2)
                {
                    offsetY -= 151f;
                }

                // 10개씩 UI 생성 후 한 프레임 쉬기 (렉 방지)
                if (i % 10 == 0)
                    yield return null;
            }

            // 버튼 활성화
            if (isDaily)
                foreach (var button in buttons_Date)
                    button.interactable = true;
            else
                foreach (var button in buttons_weeks)
                    button.interactable = true;
        }

        public override void Initialize()
        {
            focusRoundId = 0;

            //todo DailyRanking
            buttons_Date[0].onClick.AddListener(() => OnClick(true));
            buttons_Date[1].onClick.AddListener(() => OnClick(false));
            //[0] 추가했음 나중에 지워줘야함
            buttons_Date[0].gameObject.SetActive(false);
            buttons_Date[1].gameObject.SetActive(false);

            //todo WeeklyRanking
            buttons_weeks[0].onClick.AddListener(() => OnClick(true));
            buttons_weeks[1].onClick.AddListener(() => OnClick(false));

            buttons_weeks[1].gameObject.SetActive(false);

            //todo DailyRanking
            button_Tip.onClick.AddListener(() =>
            {
                PopupManager.Instance.GetPopUp<RewardGuidePopup>("rewardGuide").OpenPopup(LeaderBoardType.WAVE_DAILY);
            });

            button_weeklyTip.onClick.AddListener(() =>
            {
                SecurePlayerPrefs.SetInt("guide", 2);
                guideNotification.SetActive(false);
                PopupManager.Instance.GetPopUp<RewardGuidePopup>("rewardGuide").OpenPopup(LeaderBoardType.WAVE_WEEKLY);
            });


            //todo 토글 미사용 코드 재사용전까지 주석처리 사용되면 주석 해제 후 다시 로직 구성
            // toggle.gameObject.SetActive(false);//대전모드 추가로인해 토글 추가되기전까지 액티브 꺼주는 코드 유지
            toggle.Initialize();

            toggle.onCompleteToggle = (isActive) =>
            {
                TogglePage(isActive);
            };
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}

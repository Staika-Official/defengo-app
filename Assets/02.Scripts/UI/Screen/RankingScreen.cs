using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Network;
using TMPro;
using Framework.Util;
using Framework.GameData.Defense;
using System.Collections;
using System;

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
        [SerializeField] private TextMeshProUGUI text_League;
        [SerializeField] private TextMeshProUGUI text_LeagueNum;
        [SerializeField] private TextMeshProUGUI text_Today;
        [SerializeField] private TextMeshProUGUI text_weekly;
        [SerializeField] private TextMeshProUGUI text_WeeklyNum;
        [SerializeField] private Button button_Tip;
        [SerializeField] public Button button_weeklyTip;
        [SerializeField] private Button button_leagueTip;
        [SerializeField] private MyRank myRank;
        [SerializeField] private int dailyLastRoundId = 0;
        [SerializeField] private int weeklyLastRoundId = 0;
        [SerializeField] private int leagueLastRoundId = 0;
        [SerializeField] private int focusRoundId;

        [SerializeField] private GameObject guideNotification;

        [SerializeField] private GameObject leagueObject;
        [SerializeField] private GameObject dailyObject;
        [SerializeField] private GameObject weeklyObject;

        private CustomListButtons toggle;
        [SerializeField] private CustomListButtons toggle1;
        [SerializeField] private CustomListButtons toggle2;

        [SerializeField] private TextMeshProUGUI text_DailyRewardPool;
        [SerializeField] private TextMeshProUGUI text_NoRanking;

        [SerializeField] private Button[] buttons_League;
        [SerializeField] private Button[] buttons_Date;
        [SerializeField] private Button[] buttons_weeks;

        [SerializeField] private LeaderBoardType focusLeaderBoardType = LeaderBoardType.WAVE_WEEKLY;

        [SerializeField] public List<RankInfoItem> activeRankinfoItems = new();
        [SerializeField] private Queue<RankInfoItem> rankInfoItems = new();

        private void Start()
        {
            Instance = this;
        }

        public override void ActiveScreen()
        {
            //todo (true)-Daily, (false)-Weekly
            // if (UserInfoManager.Instance.userState.finishedTutorial == false)
            // {
            //     focusLeaderBoardType = LeaderBoardType.WAVE_WEEKLY;
            //     toggle.SelectTab(1);
            // }

            toggle.SelectTab(1);
            focusRoundId = focusLeaderBoardType switch
            {
                LeaderBoardType.WAVE_DAILY => dailyLastRoundId,
                LeaderBoardType.WAVE_WEEKLY => dailyLastRoundId,
                LeaderBoardType.LEAGUE => dailyLastRoundId,
                _ => dailyLastRoundId
            };
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
            if (focusLeaderBoardType != LeaderBoardType.LEAGUE)
            {
                _ = NetworkManager.Instance.GetUserProfileDetail(focusLeaderBoardType, userId, focusRoundId, (data) =>
                {
                    popup.SetProfileData(data);
                    popup.FriendCheck(userId);
                }, FailedDetail);
            }
            else
            {
                _ = NetworkManager.Instance.GetBattleUserProfileDetail(userId, (data) =>
                {
                    popup.SetProfileData(data);
                    popup.FriendCheck(userId);
                }, FailedDetail);
            }
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

        public void TogglePage(int tab)
        {
            Debug.Log($"{gameObject.name} Select Tab: {tab}");
            switch (tab)
            {
                case 0:
                    focusRoundId = leagueLastRoundId;
                    focusLeaderBoardType = LeaderBoardType.LEAGUE;

                    dailyObject.SetActive(false);
                    weeklyObject.SetActive(false);
                    leagueObject.SetActive(true);

                    for (int i = 0; i < activeRankinfoItems.Count; i++)
                    {
                        activeRankinfoItems[i].transform.SetParent(inactive);
                        activeRankinfoItems[i].gameObject.SetActive(false);
                        rankInfoItems.Enqueue(activeRankinfoItems[i]);
                    }

                    GetLeaderBoardBestWavesData(focusRoundId);
                    break;
                case 1:
                    focusRoundId = weeklyLastRoundId;
                    focusLeaderBoardType = LeaderBoardType.WAVE_WEEKLY;

                    //todo DailyRanking관련 이미지 오브젝트
                    dailyObject.SetActive(false);
                    weeklyObject.SetActive(true);
                    leagueObject.SetActive(false);

                    for (int i = 0; i < activeRankinfoItems.Count; i++)
                    {
                        activeRankinfoItems[i].transform.SetParent(inactive);
                        activeRankinfoItems[i].gameObject.SetActive(false);
                        rankInfoItems.Enqueue(activeRankinfoItems[i]);
                    }

                    GetLeaderBoardBestWavesData(focusRoundId);
                    break;
                case 2:
                    //todo DailyRanking
                    focusRoundId = dailyLastRoundId;
                    focusLeaderBoardType = LeaderBoardType.WAVE_DAILY;

                    dailyObject.SetActive(true);
                    weeklyObject.SetActive(false);
                    leagueObject.SetActive(false);

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
                    break;
                default:

                    break;
            }

            activeRankinfoItems.Clear();
        }

        public async void GetLeaderBoardBestWavesData(int roundId)
        {
            if (focusLeaderBoardType == LeaderBoardType.LEAGUE)
            {
                await NetworkManager.Instance.GetBattleLeaderboard(SetLeaderBoardRankingData, FailedGetBattleLeaderboard);
                await NetworkManager.Instance.GetMyBattleLeaderboard(SetMyRankInfo, FailedGetMyRank);
            }
            else
            {
                await NetworkManager.Instance.GetLeaderBoardBestWaves(focusLeaderBoardType, roundId, SetLeaderBoardRankingData, Failed);
                await NetworkManager.Instance.GetMyLeaderBoardRankInfo(focusLeaderBoardType, roundId, SetMyRankInfo, Failed);
            }
        }

        public void SetMyRankInfo(string json)
        {
            myRank.SetMyRank(json);
        }
        public void SetMyRankInfo(MyBattleLeaderboardInfo leader)
        {
            myRank.SetMyRank(leader);
        }

        void FailedGetMyRank(MyBattleLeaderboardInfo defRank)
        {

        }
        void FailedGetBattleLeaderboard()
        {
            noRank.SetActive(true);
            devideLine.SetActive(false);
            weeklyPodium.SetActive(false);
            text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Ranking_Empty");
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
            if (rankInfoItems.Count > 0)
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

            LeaderBoardRankInfoList data = JsonUtility.FromJson<LeaderBoardRankInfoList>(json);

            Button[] buttons = new Button[] { };
            int lastId = 0;

            switch (focusLeaderBoardType)
            {
                case LeaderBoardType.WAVE_DAILY:
                    buttons = buttons_Date;

                    if (dailyLastRoundId == 0)
                    {
                        dailyLastRoundId = data.roundId;
                        focusRoundId = data.roundId;
                    }
                    lastId = dailyLastRoundId;
                    break;
                case LeaderBoardType.WAVE_WEEKLY:
                    buttons = buttons_weeks;

                    if (weeklyLastRoundId == 0)
                    {
                        weeklyLastRoundId = data.roundId;
                        focusRoundId = data.roundId;
                    }
                    lastId = weeklyLastRoundId;
                    break;
                case LeaderBoardType.LEAGUE:
                    buttons = buttons_League;

                    if (leagueLastRoundId == 0)
                    {
                        leagueLastRoundId = data.roundId;
                        focusRoundId = data.roundId;
                    }
                    lastId = leagueLastRoundId;
                    break;
            }

            buttons[0].gameObject.SetActive(1 < focusRoundId);
            buttons[1].gameObject.SetActive(data.roundId < lastId);

            LeaderBoardRankInfo[] rankInfos = data.waveWeeklyRanking;

            if (focusLeaderBoardType == LeaderBoardType.WAVE_DAILY)
            {
                DateTime fromDate = DateTime.Parse(data.fromDate);
                text_Today.text = fromDate.ToString("yyyy.MM.dd");
            }
            else if (focusLeaderBoardType == LeaderBoardType.WAVE_WEEKLY)
            {
                DateTime fromDate = DateTime.Parse(data.fromDate);
                string from = fromDate.ToString("yyyy.MM.dd");
                DateTime toDate = DateTime.Parse(data.toDate);
                string to = toDate.ToString("yyyy.MM.dd");
                text_weekly.text = $"{from} ~ {to}";
                text_WeeklyNum.text = $"Week {data.roundId}";
            }
            else if (focusLeaderBoardType == LeaderBoardType.LEAGUE)
            {
                DateTime fromDate = DateTime.Parse(data.fromDate);
                string from = fromDate.ToString("yyyy.MM.dd");
                DateTime toDate = DateTime.Parse(data.toDate);
                string to = toDate.ToString("yyyy.MM.dd");
                text_League.text = $"{from} ~ {to}";
                text_LeagueNum.text = $"Week {data.roundId}";
            }

            bool isNoRank = rankInfos.Length == 0;

            if (data.activated)
            {
                noRank.SetActive(isNoRank);
                devideLine.SetActive(!isNoRank);
                weeklyPodium.SetActive(focusLeaderBoardType == LeaderBoardType.WAVE_WEEKLY && !isNoRank);
                text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Ranking_Empty");
            }
            else
            {
                noRank.SetActive(!data.activated);
                devideLine.SetActive(data.activated);
                weeklyPodium.SetActive(focusLeaderBoardType == LeaderBoardType.WAVE_WEEKLY && data.activated);

                if (focusLeaderBoardType == LeaderBoardType.WAVE_DAILY)
                    text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Ranking_Empty");
                else if (focusLeaderBoardType == LeaderBoardType.WAVE_WEEKLY)
                    text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Weekly_Ready_Desc");
                else if (focusLeaderBoardType == LeaderBoardType.LEAGUE)
                    text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_League_Ready_Desc");
            }

            scrollRect.sizeDelta = new Vector2(0, rankInfos.Length * 152);
            viewPortRect.sizeDelta = new Vector2(0, rankInfos.Length * 152 + 2000);

            foreach (var item in rankLeaderBoardInfoItems)
                item.gameObject.SetActive(false);

            // UI 업데이트를 프레임마다 나누어 수행
            StartCoroutine(UpdateRankUI(rankInfos, focusLeaderBoardType));
        }
        public void SetLeaderBoardRankingData(GetBattleLeaderboardResponse data)
        {
            foreach (var item in activeRankinfoItems)
                rankInfoItems.Enqueue(item);

            activeRankinfoItems.Clear();

            Button[] buttons = new Button[] { };
            int lastId = 0;

            buttons = buttons_League;

            if (leagueLastRoundId == 0)
            {
                leagueLastRoundId = 0;
                focusRoundId = 0;
            }
            lastId = leagueLastRoundId;

            buttons[0].gameObject.SetActive(/* 1 < focusRoundId */false);
            buttons[1].gameObject.SetActive(/* data.roundId < lastId */false);

            string from = data.season.fromDate.ToString("yyyy.MM.dd");
            string to = data.season.toDate.ToString("yyyy.MM.dd");
            text_League.text = $"{from} ~ {to}";
            text_LeagueNum.text = $"Season {data.season.id}";

            bool isNoRank = data.leaders.Count == 0;

            if (data.season.activated)
            {
                noRank.SetActive(isNoRank);
                devideLine.SetActive(!isNoRank);
                weeklyPodium.SetActive(false);
                text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_Ranking_Empty");
            }
            else
            {
                noRank.SetActive(!data.season.activated);
                devideLine.SetActive(data.season.activated);
                weeklyPodium.SetActive(false);
                text_NoRanking.text = LanguageManager.Instance.GetStringData("UI_League_Ready_Desc");
            }

            scrollRect.sizeDelta = new Vector2(0, data.leaders.Count * 152);
            viewPortRect.sizeDelta = new Vector2(0, data.leaders.Count * 152 + 2000);

            foreach (var item in rankLeaderBoardInfoItems)
                item.gameObject.SetActive(false);

            // UI 업데이트를 프레임마다 나누어 수행
            StartCoroutine(UpdateRankUI(data.leaders));
        }

        private IEnumerator UpdateRankUI(LeaderBoardRankInfo[] rankInfos, LeaderBoardType type)
        {
            float offsetY = -123f;

            // WeeklyRanking 3위 까지 커스텀
            for (int i = 0; i < rankInfos.Length; i++)
            {
                if (type != LeaderBoardType.WAVE_DAILY && i < 3)
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
            if (type == LeaderBoardType.WAVE_DAILY)
                foreach (var button in buttons_Date)
                    button.interactable = true;
            else if (type == LeaderBoardType.WAVE_WEEKLY)
                foreach (var button in buttons_weeks)
                    button.interactable = true;
            else if (type == LeaderBoardType.LEAGUE)
                foreach (var button in buttons_League)
                    button.interactable = true;
        }
        private IEnumerator UpdateRankUI(List<BattleLeaderboardInfo> leaders)
        {
            float offsetY = -123f;

            // WeeklyRanking 3위 까지 커스텀
            for (int i = 0; i < leaders.Count; i++)
            {
                RankInfoItem obj = GetRankInfoItem();
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(scrollRect, false);
                obj.transform.localPosition = new Vector2(0, offsetY);
                offsetY -= 151f;

                activeRankinfoItems.Add(obj);
                obj.Initialize(leaders[i], i + 1);

                if (i == 2)
                {
                    offsetY -= 151f;
                }

                // 10개씩 UI 생성 후 한 프레임 쉬기 (렉 방지)
                if (i % 10 == 0)
                    yield return null;
            }

            // 버튼 활성화
            foreach (var button in buttons_League)
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

            button_leagueTip.onClick.AddListener(() =>
            {
                PopupManager.Instance.GetPopUp<RewardGuidePopup>("rewardGuide").OpenPopup(LeaderBoardType.LEAGUE);
            });

            toggle = DataManager.Instance.battleConfig.BATTLE_OPEN ? toggle1 : toggle2;
            toggle.gameObject.SetActive(true);
            //todo 토글 미사용 코드 재사용전까지 주석처리 사용되면 주석 해제 후 다시 로직 구성
            // toggle.gameObject.SetActive(false);//대전모드 추가로인해 토글 추가되기전까지 액티브 꺼주는 코드 유지
            toggle.Initialize();

            toggle.onCompleteSelect = (tab) =>
            {
                TogglePage(tab);
            };
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}

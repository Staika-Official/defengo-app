using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework.Network;
using Framework.Util;
using Framework.Sound;
using UnityEngine.Events;
using UniRx.Triggers;
using UniRx;
using System.Net;

namespace Framework.GameData.Defense
{
    public class UserInfoManager : MonoBehaviour
    {
        public static UserInfoManager Instance;

        public int playId;
        public string userId;
        public string nickname;
        public string email;
        public string provider;
        public UserState userState;
        public UserStoreItemList userStoreItemList;
        public UserWalletHistory userTikWalletHistory;
        public UserWalletHistory userStikWalletHistory;
        public DiscordConnect discordConnect;
        public RewardRuleList rewardRuleList;
        public ResponseProfileData profileData;
        public Dictionary<string, EmergeRate> dic_RandomboxEmergeRate = new();
        public UserAdInfo userAdInfo;
        public int[] userProfileImageIds;
        public GetUserGameNickname userNicknameInfo;
        public EventInfo eventInfo;
        public RewardLeaderBoard dailyRewardBoard;
        public RewardLeaderBoard weeklyRewardBoard;
        public Dictionary<UserItemType, int> userItemDic;

        public UserPassInfo userPassInfo;
        public int userPassId;

        public bool isGoogleLogin = false;

        public int currentRemainCooltime = 0;

        public int gemValue = 0;

        public bool isTutorial;
        public TutorialProgressType tutorial_State;

        public bool isSkinPackageOpened = false;

        public Dictionary<string, BoardList> dic_BoardList = new();
        public delegate void OnCompleteRewardRule();
        public static OnCompleteRewardRule OnCompleteGameStart;

        private readonly string[] blockRegincode = { "KR", "CN", "SG" };
        private bool isBlockRegion = false;
        public bool IsBlockRegion { get { return isBlockRegion; } }

        private void Start()
        {
            if (Instance is null)
            {
                Instance = this;
            }

#if UNITY_EDITOR
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.O))
                .Subscribe(_ => SetTutorialTest());
#endif
        }

        public void SetTutorialTest()
        {
            userState.finishedTutorial = !userState.finishedTutorial;
        }

        public RegistUserPlay GetRegistUserPlayData()
        {
            RegistUserPlay data = new()
            {
                nickname = this.nickname,
                userId = int.Parse(this.userId),
                slotNumber = UserSlotManager.Instance.GetFocusIdx()
            };
            return data;
        }

        // public void SetEmergeRate(RandomBoxRate data)
        // {
        //     for (int i = 0; i < data.randomBoxEmergeRate.Length; i++)
        //     {
        //         dic_RandomboxEmergeRate.Add(data.randomBoxEmergeRate[i].code, data.randomBoxEmergeRate[i].emergeRate);
        //     }
        // }

        public void SetEmergeRate(RandomBoxEmergeRate[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                dic_RandomboxEmergeRate.Add(data[i].code, data[i].emergeRate);
            }
        }

        public void SuccessPlayInfo(ResponeUserPlay responeUserPlay)
        {
            playId = responeUserPlay.playId;
            userState.energyAmount = responeUserPlay.energyAmount;
            userState.energyRemainCoolTime = responeUserPlay.energyRemainCoolTime;

            _ = NetworkManager.Instance.GetRewardRule();
        }

        public void SetUserNicknameInfo(GetUserGameNickname data)
        {
            this.userNicknameInfo = data;
            this.nickname = data.nickname;
        }

        public void SuccessRewardRuleList(RewardRuleList rewardRuleList)
        {
            this.rewardRuleList = rewardRuleList;

            OnCompleteGameStart?.Invoke();

            SceneLoadManager.onCompleteLoadScene = () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.BGM_INGAME);
            };
        }

        public int GetPlayId()
        {
            return playId;
        }

        public string GetUserId()
        {
            return userId;
        }

        public void SetUserNickname(string nickname)
        {
            this.nickname = nickname;
        }

        public void SetUserData(UserVM data)
        {
            this.userId = data.id;
            this.nickname = data.nickname;
            this.email = data.email;

            //Debug.Log("@@@@@@@@@@@email : " + email);
            //Debug.Log("Provider in userVm : " + data.provider);
            //this.provider = data.provider;
        }

        public void SetUserStoreItem(UserStoreItemList data)
        {
            userStoreItemList = data;
        }

        public void SetUserWalletHistory(UserWalletHistory data)
        {
            this.userTikWalletHistory = data;
        }
        
        public LeaderBoardRewardList GetDailyRankInfoList(string key)
        {
            for (int i = 0; i < dailyRewardBoard.rewardLeaderBoard.Length; i++)
            {
                if (dailyRewardBoard.rewardLeaderBoard[i].rankType == key)
                {
                    return dailyRewardBoard.rewardLeaderBoard[i];
                }
            }

            return null;
        }

        public LeaderBoardRewardList GetWeeklyRankInfoList(string key)
        {
            for (int i = 0; i < weeklyRewardBoard.rewardLeaderBoard.Length; i++)
            {
                if (weeklyRewardBoard.rewardLeaderBoard[i].rankType == key)
                {
                    return weeklyRewardBoard.rewardLeaderBoard[i];
                }
            }

            return null;
        }

        //todo DailyReward 2.9.0 이후 삭제
        public Transaction GetDailyRewardTransaction()
        {
            for (int i = 0; i < userTikWalletHistory.transactions.Length; i++)
            {
                if (userTikWalletHistory.transactions[i].content == "DAILY_REWARD")
                {
                    return userTikWalletHistory.transactions[i];
                }
            }
            return null;
        }

        public void SetUserState(UserState userState)
        {
            this.userState = userState;
            userProfileImageIds = userState.userProfileIds;
            
            // Block Region Check
            isBlockRegion = !GetIp() && Array.IndexOf(blockRegincode, userState.region) != -1 && NetworkManager.Instance.applicationState == ApplicationState.PRODUCTION;
        }

        private bool GetIp()
        {
            string externalip = new WebClient().DownloadString("https://api.ipify.org");
            Debug.Log(externalip);

            bool isSameIp = externalip == "1.232.94.157";
            return isSameIp;
        }

        public string GetUserStikAmount()
        {
            string value;

            if (userStikWalletHistory.transactions.Length == 0)
            {
                value = "Unknown";
            }
            else
            {
                value = userStikWalletHistory.balance.uiAmountString;
            }

            return value;
        }
    }
}

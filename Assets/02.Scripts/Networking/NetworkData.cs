using System;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using Framework.GameData.Defense;

namespace Framework.Network
{
    public enum AccountStatus
    {
        NORMAL,
        ALREADY_CONNECTED_DEVICE,
        TERMINATION_REQUESTED,
        TERMINATION_COMPLETED,
        JOIN_IN_PROGRESS

    }

    public class SignInGuestDTO
    {
        public string clientId;
        public string deviceId;
        public string fcmToken;
        public string platform;
        public string appVersion;
        public string osVersion;
        public string deviceModel;
    }

    public class SignInSocialDTO
    {
        public string appVersion;
        public string osVersion;
        public string clientId;
        public string deviceId;
        public string fcmToken;
        public bool forceLogin;
        public string deviceModel;
        public string platform;
        public string provider;
        public string token;
    }

    public class GuestIntegration
    {
        public string clientId;
        public string provider;
        public string platform;
        public string token;
    }


    public class GuestIntegrationCheck
    {
        public bool isAvailable;
    }

    public class TokenVM
    {
        public string accessToken;
        public string accountStatus;
        public string refreshToken;
    }

    public class RefreshToken
    {
        public string clientId;
        public string fcmToken;
        public string deviceId;
        public string platform;
        public string deviceModel;
        public string osVersion;
        public string appVersion;
    }

    public class Logout
    {
        public string clientId;
        public string deviceId;
    }

    public class AccountDTO
    {
        public int id;
        public string login;
        public string email;
        public string nickname;
        public string profileImageUrl;
        public string phone;
        public string userCode;
        public string provider;
        public bool activated;
        public string createdDate;
        public string[] authorities;
        public string terminationDate;
    }

    public class UserGameNickname
    {
        public int userId;
        public string nickname;
    }

    [Serializable]
    public class TotalUserData
    {
        public GetUserGameNickname nickname;
        public UserCharacters character;
        public UserSlot slot;
        public StoreItem[] storeItems;
        public RandomBoxEmergeRate[] randomBoxEmergeRate;
        public int eventBoss;
        public LeaderBoardRewardList[] rewardLeaderBoards;
        public DiscordConnect discord;
        public LeaderBoardRewardList[] rewardDailyLeaderBoards;
        public UserItems[] userItems;
        public ClassUpRule[] classUpRule;
    }

    [Serializable]
    public class GetUserGameNickname
    {
        public string clientId;
        public int userId;
        public string nickname;
        public int changeableCount;
    }

    public class GetUserProfileList
    {
        public int equippedProfileId;
        public int[] userProfileIds;
    }

    public class ServerErrorMessage
    {
        public int status;
        public string errorMessage;
        public string errorCode;
    }

    public class ExpireAccessToken
    {
        public int status;
        public string errorMessage;
    }

    public class MemberInfo
    {
        public int id;
        public string email;
        public string nickname;
        public string profileImageUrl;
        public string availableChangeNickname;
        public bool marketingChecked;
        public string provider;
        public string countryCode;
    }

    public class UserVM
    {
        public bool activated;
        public bool alarmEvent;
        public bool alarmTransaction;
        public string[] authorities;
        public string createdDate;
        public string id;
        public string login;
        public string email;
        public bool marketingChecked;
        public string nickname;
        public string phone;
        public string profileImageUrl;
        public string provider;
        public string terminationDate;
        public string userCode;
    }

    [SkipRename]
    public class ConfigurationData
    {
        public int id;
        public string clientId;
        public string configKey;
        public string valueDataType;
        public string value;
        public string description;
        public string createdBy;
        public string createdDate;
        public string lastModifiedBy;
        public string lastModifiedDate;
    }

    public enum EventPopupType
    {
        NOTICE_WEBVIEW,
        NOTICE_EXTERNAL,
        STORE,
        NONE,
        EVENT,
        BATTLE_PASS
    }

    [Serializable]
    public class EventPopup
    {
        public int id;
        public string clientId;
        public string type;
        public string imageUrl;
        public string titleKo;
        public string titleEn;
        public string contentKo;
        public string contentEn;
        public string linkUrlKo;
        public string linkUrlEn;
        public string typeValue;
        public int listOrder;
        public string fromDate;
        public string toDate;
        public bool activated;
        public string createdBy;
        public string createdDate;
        public string lastModifiedBy;
        public string lastModifiedDate;
    }

    [Serializable]
    public class EventPopupList
    {
        public EventPopup[] popups;
    }

    public class UserCharacter
    {
        public string userId;
        public int[] characterIds;
    }

    [Serializable]
    public class UserCharacters
    {
        public int userId;
        public List<CharacterInfo> characters = new List<CharacterInfo>();
        public List<int> skins = new List<int>();
        public List<int> equippedSkins = new List<int>();
    }

    public class UserCharacterInfo
    {
        public CharacterInfo[] userCharacterList;
    }

    [Serializable]
    public class CharacterInfo
    {
        public int characterId;
        public int quantity;
        public int classLevel;
    }

    [Serializable]
    public class TargetVersion
    {
        public string targetVersion;
    }

    [Serializable]
    public class UserState
    {
        public int userId;
        public int equippedProfileId;
        public int[] userProfileIds;
        public int energyAmount;
        public int energyRemainCoolTime;
        public int obtainedGo;
        public int characterSlotCount;
        public bool finishedTutorial;
        public string langKey;
        public string region;
        public bool activated;
        public string userStatus;
    }

    [Serializable]
    public class UserSlot
    {
        public string userId;
        public Slot[] slots;
    }

    [Serializable]
    public class Slot
    {
        public int slotNumber;
        public int[] slotCharacterIds;
    }

    [Serializable]
    public class StoreItemDTO
    {
        public StoreItem[] storeItems;
    }

    [Serializable]
    public class SpecialStoreDTO
    {
        public SpecialStoreItem[] specialStore;
    }

    [Serializable]
    public class RewardRuleList
    {
        public RewardRule[] rewardRules;
    }

    [Serializable]
    public class RewardRule
    {
        public int id;
        public string code;
        public int rewardGo;
    }

    [Serializable]
    public class SpecialStore
    {
        public SpecialStorePackageType type;
        public string fromDate;
        public string toDate;
        public SpecialStoreItem[] items;
        public SpecialStoreSkin skin;

    }

    [Serializable]
    public class StoreItem
    {
        public string itemType;
        public string code;
        public string nameKo;
        public string nameEn;
        public int receivedItemAmount;
        public string moneyType;
        public float price;
        public string noticeType;
        public string description;
        public int userCeiling;
        public int maxCeiling;
    }

    [Serializable]
    public class SpecialStoreItem
    {
        public string rewardType;
        public string code;
        public string nameKo;
        public string nameEn;
        public int receivedItemAmount;
        public MoneyType moneyType;
        public float price;
        public bool isAvailable;
        public int limitedQuantity;
        public int purchasedQuantity;
        public int saleRate;
        public int userCeiling;
        public int maxCeiling;
    }
    
    [Serializable]
    public class LimitedStoreItem
    {
        public string code;
        public string nameKo;
        public string nameEn;
        public string fromDate;
        public string toDate;
        public int receivedItemAmount;
        public string moneyType;
        public float price;
        public int limitedQuantity;
        public int purchasedQuantity;
        public int saleRate;
        public int userCeiling;
        public int maxCeiling;
        public int[] rewardCharacters;
        public bool isAvailable;
    }

    [Serializable]
    public class MonthlyPackageDTO
    {
        public MonthlyPackageItem[] specialStore;
    }

    [Serializable]
    public class MonthlyPackageItem
    {
        public int storeId;
        public string storeCategoryType;
        public int totalRemainCoolTime;
        public int rewardRemainCoolTime;
    }

    [Serializable]
    public class RequestMonthlyPackage
    {
        public int storeId;
        public string type;
    }

    public class UserAdHistory
    {
        public string clientId;
        public int userId;
        public string code;
        public string adId;
    }

    public class UserAdInfo
    {
        public bool isAvailable;
        public int remainCoolTime;
    }

    public enum AdmobType
    {
        ALL = 0,
        ENERGY = 1,
        RANDOM_BOX = 2
    }

    [Serializable]
    public class UserAdValidInfos
    {
        public UserAdValidInfo[] userAds;
    }

    [Serializable]
    public class UserAdValidInfo
    {
        public string code;
        public bool isAvailable;
        public int remainCoolTime;
    }

    public class RegistUserPlay
    {
        public int userId;
        //public int profileId;
        public string nickname;
        public int slotNumber;
    }

    public class ResponeUserPlay
    {
        public int playId;
        public int energyAmount;
        public int energyRemainCoolTime;
    }


    // 2.9.0 이전버전 플레이 레코드 
    // public class PlayRecord
    // {
    //     public int waveNumber;
    //     public int requestGo;
    //     public int killedMonster;
    //     public string completedMissions;
    //     public int bossLevel;
    //     public int playId;
    //     public string characterInfo;
    //     public string monsterInfo;
    // }

    public class PlayRecord
    {
        public int waveNumber;
        public int requestGo;
        public int killedMonster;
        public string completedMissions;
        public int bossLevel;
        public int playId;
        public float attackSpeed;
        public int attackDamage;
        public int upgrade;
        public int monsterHealth;
        public int killedBossMonster;
        public int missionClear;
        public int characterSummon;
        public int characterMerge;
        public int characterUpgrade;
        public int fourStarGrade;
    }

    public class UserStoreItemList
    {
        public UserStoreItem[] userStoreItems;
    }

    public class DailyRewardPool
    {
        public int dailyRewardPool;
    }

    [Serializable]
    public class UserStoreItem
    {
        public int id;
        public string code;
        public string paidMoneyType;
        public int paidAmount;
        public string createdDate;
    }

    [Serializable]
    public class LeaderBoardRankInfoList
    {
        public int roundId;
        public bool activated;
        public string fromDate;
        public string toDate;
        public LeaderBoardRankInfo[] waveWeeklyRanking;
    }

    [Serializable]
    public class LeaderBoardRankInfo
    {
        public string clientId;
        public int userId;
        public int bestWave;
        public string nickname;
        public int profileId;
    }

    [Serializable]
    public class RankInfoList
    {
        public RankInfo[] dailyRanking;
    }

    [Serializable]
    public class TermsHistory
    {
        public string terms;
        public string boardType;
        public int postId;
        public bool activated;
        public string clientId;
    }

    public enum EventPrize
    {
        NONE = 0,
        PRIZE = 1,
        PRIZE_COMPLETED = 2
    }

    public class UserEventPrize
    {
        public string eventPrize;
    }

    public class BoardList
    {
        public int postId;
        public string boardType;
        public string lang;
        public string title;
        public string content;
    }

    [Serializable]
    public class RankInfo
    {
        public int id;
        public string clientId;
        public string aggregatedDate;
        public int userId;
        public int profileId;
        public string nickname;
        public int obtainedGo;
        public int rewardTik;
    }

    public class UserRankInfo
    {
        public int rank;
        public string clientId;
        public string aggregatedDate;
        public int userId;
        public int profileId;
        public string nickname;
        public int obtainedGo;
        public int rewardTik;
    }

    public class RewardHistory
    {
        public RewardInfo[] dailyBymonth;
    }

    public class RewardInfo
    {
        public string clientId;
        public int userId;
        public int obtainedGo;
        public int rewardTik;
        public string aggregatedDate;
        public string nickname;
        public int profileId;
    }

    public class UserTermsHistory
    {
        public string boardType;
        public bool activated;
        public string createdDate;
    }

    public class EventBoxResult
    {
        public string inboxType;
        public string value;
        public int quantity;
        public string contentKo;
        public string contentEn;
    }

    [Serializable]
    public class UserWalletHistory
    {
        public Balance balance;
        public Transaction[] transactions;
    }

    [Serializable]
    public class RequestValidateIAP
    {
        public string receipt;
        public string platform;
        public string productId;
        public string purchaseId;
    }

    [Serializable]
    public class ResponseValidateIAP
    {
        public bool valid;
    }

    [Serializable]
    public class ResponsePendingIAP
    {
        public string appstorePurchaseId;
        public string appProductId;
        public string state;
    }

    [Serializable]
    public class RequestPay
    {
        public string purchaseId;
    }

    [Serializable]
    public class ResponsePay
    {
        public bool payed;
    }

    [Serializable]
    public class Balance
    {
        public string symbol;
        public int decimals;
        public double amount;
        public string uiAmountString;
    }

    [Serializable]
    public class WalletAccount
    {
        public SpendingAccounts[] account;
    }

    [Serializable]
    public class SpendingAccounts
    {
        public double amount;
        public string uiAmountString;
        public string symbol;
        public string name;
        public int decimals;
        public string tokenAddress;
        public string logoUrl;
    }

    [Serializable]
    public class Transaction
    {
        public int transactionId;
        public string type;
        public string title;
        public string content;
        public string symbol;
        public int decimals;
        public double amount;
        public string uiAmountString;
        public string memo;
        public string createdDate;
    }

    [Serializable]
    public class TerminationStatusInfo
    {
        public int status;
        public string errorMessage;
        public string errorCode;
    }

    [Serializable]
    public class TerminateAccountInfo
    {
        public int userId;
        public string reason;
        public string clientId;

        public TerminateAccountInfo()
        {
            this.reason = "회원탈퇴";
            this.clientId = "DEFENGO";
        }

        public TerminateAccountInfo(int userId, string reason)
        {
            this.userId = userId;
            this.reason = reason;
            this.clientId = "DEFENGO";
        }
    }

    [Serializable]
    public class UserId
    {
        public int userId;
    }

    public class UserClassUpResponse
    {
        public string userId;
        public int characterId;
        public int classLevel;
        public int remainQuantity;
    }

    public class RequestBuyItem
    {
        public string userId;
        public string clientId;
        public string code;
        public bool repurchase;

        public void Set(string userId, string clientId, string code, bool repurchase)
        {
            this.userId = userId;
            this.clientId = clientId;
            this.code = code;
            this.repurchase = repurchase;
        }
        public void Clear()
        {
            this.userId = null;
            this.clientId = null;
            this.code = null;
            this.repurchase = false;
        }
    }

    [Serializable]
    public class ResponseRandomBox
    {
        public string type;
        public int[] purchaseList;
    }

    [Serializable]
    public class RandomBoxIndex
    {
        public string type;
        public ResRewardValueData[] rewardList;
    }

    [Serializable]
    public class RandomBoxProperty
    {
        public string value;
        public int amount;
    }

    public class RequestRankDetail
    {
        public int userId;
        public string clientId;
        public string aggregatedDate;
    }

    [Serializable]
    public class ResponseRankDetail
    {
        public string nickname;
        public int profileId;
        public int bestWave;

        public CharacterInfo[] userCharacterList;
        public Podium ranking;
    }

    public enum LeaderBoardType
    {
        WAVE_DAILY,
        WAVE_WEEKLY
    }

    [Serializable]
    public class TotalProfileData
    {
        public ResponseProfileData[] userProfile;
    }

    [Serializable]
    public class ResponseProfileData
    {
        public string leaderBoardType;
        public string nickname;
        public int equippedProfileId;
        public int playCount;
        public int bestWave;
        public int bestScore;
        public int bestBossLevel;
        public Podium ranking;
        public CharacterInfo[] bestCharacter;
    }

    [Serializable]
    public class Podium
    {
        public int first;
        public int second;
        public int third;
    }

    [Serializable]
    public class RandomBoxRate
    {
        public RandomBoxEmergeRate[] randomBoxEmergeRate;
    }

    [Serializable]
    public class RandomBoxEmergeRate
    {
        public string code;
        public EmergeRate emergeRate;
    }

    [Serializable]
    public class EmergeRate
    {
        public double normal;
        public double rare;
        public double unique;
        public double epic;
        public double legend;
    }

    [Serializable]
    public class HatchingItemData
    {
        public UserItem[] userItems;
    }

    [Serializable]
    public class UserItem
    {
        public string item;
        public int quantity;
    }

    [Serializable]
    public class HatchingOpenData
    {
        public string item;
        public string type;
        public HatchReward[] rewardList;
    }

    [Serializable]
    public class HatchReward
    {
        public string value;
        public int amount;
    }

    [Serializable]
    public class UserInboxInfo
    {
        public UserInbox[] userInboxes;
    }

    [Serializable]
    public class EventInfo
    {
        public int eventBoss;
    }

    [Serializable]
    public class UserInbox
    {
        public int id;
        public int inboxId;
        public string clientId;
        public string status;
        public string titleKo;
        public string titleEn;
        public string inboxType;
        public string value;
        public int quantity;
        public string expiredDate;
        public string createdDate;
    }

    public enum InboxType
    {
        MAIL = 0,
        RANDOM_BOX = 1,
        ENERGY = 2,
        TIK = 3,
        STIK = 4,
        GEM = 5,
        GO = 6,
        PROFILE = 7,
        STIK_RANDOMBOX = 8,
        PTIK_RANDOMBOX = 9,
        TIK_RANDOMBOX = 10,
        PTIK = 11,
        HATCHING_ORB = 12
    }

    public enum InboxStatus
    {
        READ,
        UNREAD
    }

    public class DetailInbox
    {
        public string inboxType;
        public string value;
        public int quantity;
        public string contentKo;
        public string contentEn;
    }

    public class RandomBoxInbox
    {
        public string inboxType;
        public string type;
        public int[] rewardList;
    }

    [Serializable]
    public class WeeklyLeaderBoard
    {
        public int roundId;
        public string fromDate;
        public string toDate;
        public WaveWeeklyRanking[] waveWeeklyRanking;
    }

    [Serializable]
    public class WaveWeeklyRanking
    {
        public string clientId;
        public int userId;
        public int bestWave;
        public string nickname;
        public int profileId;
    }

    [Serializable]
    public class LeaderBoardReward
    {
        public string rewardType;
        public string rewardValue;
    }

    [Serializable]
    public class LeaderBoardRewardList
    {
        public string rankType;
        public LeaderBoardReward[] rewards;
    }

    [Serializable]
    public class RewardLeaderBoard
    {
        public LeaderBoardRewardList[] rewardLeaderBoard;
    }

    [Serializable]
    public class LeaderBoardRewardStatusData
    {
        public string finalRank;
        public string rewardStatus;
    }

    [Serializable]
    public class UserItems
    {
        public UserItemType item;
        public int quantity;
    }

    [Serializable]
    public class SwapToken
    {
        public int userId;
        public string title;
        public string fromTokenSymbol;
        public double fromUiAmount;
        public string toTokenSymbol;
        public double toUiAmount;
        public string feeTokenSymbol;
        public double feeUiAmount;
        public double priceKRW;
        public double priceUSD;
    }

    public class AbusingRecord
    {
        public string clientId;
        public int userId;
        public string abusingData;

    }

    [Serializable]
    public class ExchangeToken
    {
        public string type;
        public double uiAmount;
        public double fee;
        public string encodedTransaction;
    }

    [Serializable]
    public class MyLeaderBoardRankData
    {
        public string rank;
        public int userId;
        public int bestWave;
        public string nickname;
        public int profileId;
    }

    [Serializable]
    public class DiscordConnect
    {
        public int userId;
        public bool eventJoined;
        public string linkedUrl;
        public string amount;
    }

    [Serializable]
    public class UserPassInfo
    {
        public bool isActivated;
        public int battlePassId;
        public string toDate;
        public string passImageUrl;
        public int mainDisplayedTitle;
    }

    [Serializable]
    public class BattlePassUserInfoDetailData
    {
        public int battlePassId;
        public string title;
        public string subTitle;
        public string titleEn;
        public string subTitleEn;
        public string bgImageUrl;
        public string passImageUrl;
        public string fromDate;
        public string toDate;
        public float price;
        public UserBattlePassLevel userLevel;
        public BattlePassPremiumItem[] premiumRewards;
        public BattlePassRewards userRewards;
    }

    [Serializable]
    public class UserBattlePassLevel
    {
        public bool passActivated;
        public int level;
        public int userExp;
        public int requiredExp;
    }

    [Serializable]
    public class BattlePassPremiumItem
    {
        public string rewardType;
        public int amount;
        public string displayType;
    }

    [Serializable]
    public class BattlePassRewards
    {
        public BattlePassReward[] normal;
        public BattlePassReward[] premium;
    }

    [Serializable]
    public class BattlePassReward
    {
        public int rewardId;
        public int passLevel;
        public string displayType;
        public string category;
        public string rewardType;
        public int amount;
        public bool isAvailable;
    }

    public class BattlePassBuyExp
    {
        public string payType;
        public int battlePassId;
        public double price;
    }

    public class BattlePassGetReward
    {
        public int battlePassId;
        public int rewardId;
    }

    [Serializable]
    public class BattlePassRewardResponseData
    {
        public string rewardType;
        public int amount;
    }

    [Serializable]
    public class DailyQuestData
    {
        public string aggregatedDate;
        public DailyQuestReward dailyQuestReward;
        public UserDailyQuests[] userDailyQuests;
    }

    [Serializable]
    public class DailyQuestReward
    {
        public bool firstReward;
        public bool secondReward;
        public bool thirdReward;
        public bool fourthReward;
        public bool fifthReward;
    }

    [Serializable]
    public class UserDailyQuests
    {
        public int rewardAmount;
        public string type;
        public int attainAmount;
        public int acquireCount;
        public string rewardStatus;
    }

    [Serializable]
    public class AchievementData
    {
        public UserAchievements[] userAchievements;
    }

    [Serializable]
    public class UserAchievements
    {
        public string reward;
        public int rewardAmount;
        public string type;
        public int attainAmount;
        public int acquireCount;
        public string rewardStatus;
    }

    [Serializable]
    public class ResUserSkinData
    {
        public int skinId;
        public bool equipped;
    }

    [Serializable]
    public class ResUserSkins
    {
        public int userId;
        public int characterId;

        public ResUserSkinData[] skins;
    }

    [Serializable]
    public class SpecialStoreSkin
    {
        public int storeGroupId;
        public MoneyType moneyType;
    }

    [Serializable]
    public class ResRewardTemplate
    {
        public ResRewardData reward;

        private List<RewardData> reward_data_list = new List<RewardData>();

        public List<RewardData> RewardDataList
        {
            get
            {
                if (reward_data_list.Count <= 0)
                {
                    foreach (ResRewardValueData data in reward.CURRENCY)
                        reward_data_list.Add(new RewardData("CURRENCY", data.value, data.amount));

                    foreach (ResRewardValueData data in reward.BOX)
                        reward_data_list.Add(new RewardData("BOX", data.value, data.amount));

                    foreach (ResRewardValueData data in reward.ITEM)
                        reward_data_list.Add(new RewardData("ITEM", data.value, data.amount));

                    foreach (ResRewardValueData data in reward.CHARACTER)
                        reward_data_list.Add(new RewardData("CHARACTER", data.value, data.amount));

                    foreach (ResRewardValueData data in reward.CHARACTER_SKIN)
                        reward_data_list.Add(new RewardData("CHARACTER_SKIN", data.value, data.amount));

                    foreach (ResRewardValueData data in reward.PROFILE)
                        reward_data_list.Add(new RewardData("PROFILE", data.value, data.amount));
                }

                return reward_data_list;
            }
        }
    }

    [Serializable]
    public class ResRewardData
    {
        public List<ResRewardValueData> CURRENCY = new List<ResRewardValueData>();
        public List<ResRewardValueData> BOX = new List<ResRewardValueData>();
        public List<ResRewardValueData> ITEM = new List<ResRewardValueData>();
        public List<ResRewardValueData> CHARACTER = new List<ResRewardValueData>();
        public List<ResRewardValueData> CHARACTER_SKIN = new List<ResRewardValueData>();
        public List<ResRewardValueData> PROFILE = new List<ResRewardValueData>();
    }

    [Serializable]
    public class ResRewardValueData
    {
        public string value;
        public int amount;
    }

    [Serializable]
    public class ReqPityReward
    {
        public int storeGroupId;
        public int rewardStep;
    }

    [Serializable]
    public class ReqPityStepData
    {
        public int storeGroupId;
    }

    [Serializable]
    public class ResPityDatas
    {
        public List<ResPityData> pityDatas;
    }

    [Serializable]
    public class ResPityData
    {
        public int id;
        public int userId;
        public int storeGroupId;
        public int openCount;
    }

    [Serializable]
    public class ResPityStepData
    {
        public int id;
        public int userId;
        public int storeGroupId;
        public int rewardStep;
    }

    [Serializable]
    public class ReqSkinBuyData
    {
        public int characterId;
        public int skinId;
    }

    #region Roulette
    [Serializable]
    public class ReqRouletteGroupData
    {
        public int rouletteGroupId;
        public bool freeRoulette;
        public int maxAd;
        public int availableAd;
        public double price;
        public int rouletteTicket;
    }
    [Serializable]
    public class ResponeRoulettePlayId
    {
        public int playId;
        public string paidType;
    }
    [Serializable]
    public class ReqRouletteRewardData
    {
        public int rouletteGroupId;
        public int result;
    }
    #endregion

    #region Character Class Up
    [Serializable]
    public class ReqCharacterClassUp
    {
        public int characterId;
        public string paidType;
    }
    #endregion
}


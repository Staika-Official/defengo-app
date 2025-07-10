// #define __STAGE__
// #define __PRODUCTION__

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using Framework.GameData.Defense;
using Framework.Login;
using Framework.UI;
using Framework.Util;
using Gpm.Common.ThirdParty.LitJson;
using Newtonsoft.Json;
using SimpleJSON;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Framework.Network
{
    static class Domain
    {
        public static string baseUrl;
    }

    public class Url
    {
        public static readonly string socialLogin = "/uaa/api/sign-in/social";
        public static readonly string guestLogin = "/uaa/api/guest/login";
        public static readonly string userSocialLoginIntegration = "/uaa/api/guest/users/{0}/integration";
        public static readonly string logout = "/uaa/api/sign-out";
        public static readonly string adminLogin = "/uaa/api/sign-in/email";
        public static readonly string account = "/uaa/api/account";
        public static readonly string getToken = "/uaa/api/sign-in/token";
        public static readonly string uaaUsers = "/uaa/api/users/{0}";
        public static readonly string terminationAccount = "/uaa/api/account/termination";
        public static readonly string terminationAccountV2 = "/uaa/api/account/users/{0}/termination";
        public static readonly string restoreAccount = "/uaa/api/account/activation?clientId=DEFENGO";
        public static readonly string restoreAccountV2 = "/uaa/api/account/users/{0}/activation?clientId=DEFENGO";
        public static readonly string getEnergyValue = "/game/api/defengo-user-states/users/{0}/energy";
        public static readonly string storeItem = "/game/api/stores/clients/DEFENGO";
        public static readonly string specialStoreItem = "/game/api/stores/special/users/{0}";
        public static readonly string userState = "/game/api/v2/defengo-user-states/users/{0}/lang/{1}?email={2}&appVersion={3}";
        public static readonly string userSlots = "/game/api/defengo-user-slots/users/{0}";
        public static readonly string buyItem = "/game/api/stores/users/{0}/buy";
        public static readonly string registPlay = "/game/api/defengo-user-plays/users/{0}";
        public static readonly string rewardRuleList = "/game/api/defengo-reward-rules";
        public static readonly string playRecord = "/game/api/v2/defengo-play-records/users/{0}";
        public static readonly string gameFinished = "/game/api/defengo-user-plays/finished/{0}";
        public static readonly string updateUserState = "/game/api/defengo-user-states/users/{0}";
        public static readonly string userStoreItems = "/game/api/user-store-items/clients/DEFENGO/user/{0}";
        public static readonly string dailyLeaderBoard = "/game/api/leader-board-dailies/clients/{0}/dates/{1}/?page=0&size=100";
        public static readonly string LeaderBoardBestWaves = "/game/api/leader-board-best-waves/leader-board-types/{0}?roundId={1}&?page=0&size=100";
        public static readonly string terms_histories = "/member/api/terms-histories/users/{0}";
        public static readonly string getBoardContent = "/board/api/DEFENGO/posts/list?boardTypes={0}&lang={1}";
        public static readonly string getNoticeContent = "/board/api/popups/clients/DEFENGO";
        public static readonly string createWallet = "/wallet-go/api/spending/wallet?clientId={0}";
        public static readonly string createUserWallet = "/wallet-go/api/spending/accounts/users/{0}";
        public static readonly string getuserWallet = "/wallet-go/api/spending/accounts/users/{0}/balances?clientId=DEFENGO";
        public static readonly string getUserTermsHistory = "/member/api/terms-histories/users/{0}/{1}";
        public static readonly string getUserWallet = "/wallet-go/api/spending/balance/{0}?clientId={1}";

        public static readonly string getUserWalletHistory = "/wallet-go/api/spending/transactions/{0}?clientId=DEFENGO&size=100";

        public static readonly string getUserMonthlyTikHistory = "/game/api/leader-board-dailies/clients/{0}/users/{1}/monthly/{2}";

        public static readonly string getUserDailyRankInfo = "/game/api/leader-board-dailies/users/{0}/clients/{1}/dates/{2}";
        public static readonly string getUserNicknameValid = "/game/api/user-nicknames/clients/{0}/nicknames/{1}";
        public static readonly string getUserGameNickname = "/game/api/user-nicknames/clients/{0}/users/{1}";
        public static readonly string editUserNickname = "/game/api/user-nicknames/clients/{0}/users/{1}";
        public static readonly string postUserAdHistory = "/game/api/user-ad-histories";
        public static readonly string getMemberInfo = "/member/api/users/{0}?clientId=DEFENGO";
        public static readonly string validUserSocialLogin = "/uaa/api/guest/users/{0}/integration/check";
        public static readonly string getHatchItem = "/game/api/user-items/clients/DEFENGO/users/{0}/item-types/HATCHING_ORB";
        public static readonly string hatchOpenItem = "/game/api/user-items/clients/DEFENGO/users/{0}/consume";
        public static readonly string userBanCharacter = "/game/api/defengo-characters/ban";
        public static readonly string monthlyPackageData = "/game/api/stores/package/users/{0}";

        public static readonly string getUserDailyQuestInfo = "/game/api/user-daily-quests/users/{0}";
        public static readonly string getDailyQuestAmount = "/game/api/user-daily-quests/users/{0}/types/{1}";
        public static readonly string getDailyCompleteMissionReward = "/game/api/user-daily-quest-rewards/users/{0}/{1}";
        public static readonly string getUserAchievementInfo = "/game/api/user-achievements/users/{0}";
        public static readonly string getUserAchievementReward = "/game/api/user-achievements/users/{0}/types/{1}";

        public static readonly string getUserLimitedShopPityInfo = "/game/api/user-limited-shops/user/{0}";
        public static readonly string userPityStep = "/game/api/rewards/user/{0}";
        public static readonly string userPityRewards = "/game/api/rewards/claim/{0}";
        /// <summary>
        /// 0 : userId
        /// 1 : passId
        /// </summary>
        public static readonly string getUserPassCheck = "/game/api/battle-passes/users/{0}";

        public static readonly string getUserPassCheckDetail = "/game/api/battle-passes/users/{0}/{1}";
        public static readonly string passBuyExp = "/game/api/user-battle-passes/users/{0}/pay";
        public static readonly string passBuyPremium = "/game/api/user-battle-passes/users/{0}/premium/{1}";
        public static readonly string passUserReward = "/game/api/user-reward-battle-passes/users/{0}/reward";

        /// <summary>
        /// 0 : userId
        /// 1 : type
        /// </summary>
        public static readonly string registUserAdHistory = "/game/api/user-ad-histories/clients/DEFENGO/users/{0}/{1}";

        public static readonly string getUserAdValidInfo = "/game/api/v2/user-ad-histories/clients/DEFENGO/users/{0}?adType={1}";
        public static readonly string getUserAdValid = "/game/api/user-ad-histories/clients/{0}/users/{1}";
        public static readonly string getUserCharacterInfo = "/game/api/defengo-user-characters/users/{0}";

        public static readonly string characterClassUp = "/game/api/user-class-ups/users/{0}";

        public static readonly string getUserRankDetatil = "/game/api/leader-board-dailies/clients/DEFENGO/summary/{0}?userId={1}";

        public static readonly string getUserInboxInfo = "/game/api/user-inboxes/clients/DEFENGO/users/{0}";

        //public static string getUserInboxDetail = "/game/api/user-inboxes/clients/DEFENGO/users/{0}/{1}?inboxId={2}";
        public static readonly string getUserInboxDetail = "/game/api/v2/user-inboxes/clients/DEFENGO/users/{0}/{1}?inboxId={2}";
        public static readonly string deleteUserInbox = "/game/api/user-inboxes/clients/DEFENGO/users/{0}";
        public static readonly string validateInAppPurchase = "/wallet-go/api/iap/validate?clientId=DEFENGO";
        public static readonly string paidInAppPurchase = "/wallet-go/api/iap/pay?clientId=DEFENGO";
        public static readonly string getRandomBoxEmergeRate = "/game/api/random-boxes/emerge-rate?clientId=DEFENGO";
        public static readonly string getEventPrize = "/game/api/defengo-user-plays/users/{0}/prize";
        public static readonly string getDailyRewardPool = "/game/api/reward-pools/clients/DEFENGO/{0}";
        public static readonly string getEventBossInfo = "/game/api/defengo-user-plays/users/{0}/prize/boss";

        public static readonly string getTotalUserProfileData = "/game/api/user-profiles/clients/DEFENGO/users/{0}";
        public static readonly string setUserProfileIdx = "/game/api/user-profiles/clients/DEFENGO/users/{0}/profiles/{1}";
        public static readonly string getUserLeaderBoardDetail = "/game/api/leader-board-best-waves/leader-board-types/{0}/detail/{1}?userId={2}";

        // public static readonly string getWeeklyRewardBoard = "/game/api/reward-leader-boards/leader-board-types/WAVE_WEEKLY";
        public static readonly string getTokenProductList = "/wallet-go/api/spending/tokens/swap?action={0}&markets=cmc";
        public static readonly string swapToken = "/wallet-go/api/spending/tokens/swap?clientId=DEFENGO";

        public static readonly string exchangeToken = "/wallet-go/api/on-chains/{0}/tokens/{1}/users/{2}/exchange?clientId=DEFENGO";

        public static readonly string getLeaderBoardRewardData = "/game/api/leader-board-best-waves/users/{0}/leader-board-types/{1}";

        /// <summary>
        /// userId, boardtype, roundId
        /// </summary>
        public static readonly string getMyLeaderBoardRankData = "/game/api/leader-board-best-waves/users/{0}/leader-board-types/{1}/rounds/{2}";

        public static readonly string connectDiscord = "/game/api/discord/users/{0}";
        public static readonly string getEventPopupList = "/game/api/popups/users/{0}";
        public static readonly string getLimitedShopInfo = "/game/api/popups/clients/DEFENGO/store";
        public static readonly string getLimitedShop = "/game/api/stores/limited/users/{0}";
        public static readonly string getUserProfileList = "/game/api/user-profiles/clients/DEFENGO/users/{0}/profiles";
        public static readonly string abusesingRecord = "/game/api/abusing-records";
        public static readonly string initTotalUserData = "/game/api/init/clients/DEFENGO/users/{0}";

        public static readonly string getUserIapPurchaseState = "/wallet-go/api/iap/users/{0}/state?clientId=DEFENGO&purchaseId={1}";

        public static readonly string getUserSkinEquip = "/game/api/user-skins/users/{0}";
        public static readonly string getUserSkinCharacter = "/game/api/user-skins/users/{0}/characters/{1}";
        public static readonly string patchUserSkinChange = "/game/api/user-skins/users/{0}/characters/{1}/skins/{2}";
        public static readonly string GameAPI = "/game/api/";

        public static readonly string getRouletteTable = "/game/api/roulettes";
        public static readonly string getRouletteGroup = "/game/api/user-roulettes/users/{0}/plays/{1}";
        public static readonly string postRouletteReward = "/game/api/user-roulettes/users/{0}";
    }

    public enum ApplicationState
    {
        DEV,
        STAGE,
        PRODUCTION
    }

    public enum SendType
    {
        GET,
        POST,
        PATCH
    }

    public enum TokenType
    {
        ACCESSTOKEN,
        REFRESHTOKEN,
        NULL
    }

    public class NetworkManager : MonoBehaviour
    {
        private enum NetworkType
        {
            POST,
            GET,
            PATCH
        }
        public static NetworkManager Instance;
        public ApplicationState applicationState;
        public UnityAction<string> callback;

        private readonly string authorization = "Authorization";
        private readonly string bearer = "Bearer ";
        private readonly string contentType = "Content-Type";
        private readonly string contentTypeValue = "application/json";

        [SerializeField] private string accessToken;
        [SerializeField] private string refreshToken;

        private delegate void RefreshTokenAsync();

        private static RefreshTokenAsync OnComplete_RefreshToken;
        private static RefreshTokenAsync OnFailed_RefreshToken;

        public UnityAction<string> OnFailedLoadUserData;

        public Stack<PlayRecord> disposedPlayRecords = new();

        private void Awake()
        {
            isGetUserGameData = false;
            if (Instance is null)
            {
                Instance = this;
#if __PRODUCTION__
                applicationState = ApplicationState.PRODUCTION;
                //Debug.unityLogger.logEnabled = false;
#elif __STAGE__
                applicationState = ApplicationState.STAGE;
#endif
            }
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => ClearLocalStorage());
        }
#endif

        public void SetBaseUrl()
        {
            switch (applicationState)
            {
                case ApplicationState.DEV:
                    Domain.baseUrl = "http://api.dev.staika.io:8080/services";
                    Debug.unityLogger.logEnabled = true;
                    break;
                case ApplicationState.STAGE:
                    Domain.baseUrl = "https://api.stage.staika.io/services";
                    Debug.unityLogger.logEnabled = true;
                    break;
                case ApplicationState.PRODUCTION:
                    Domain.baseUrl = "https://api.staika.io/services";
#if !UNITY_EDITOR
                    //Debug.unityLogger.logEnabled = false; // Debug.Log 출력 여부
#endif
                    break;
            }
        }

        #region GetUserData

        bool isGetUserGameData = false;

        public async UniTask GetUserGameData()
        {
            Debug.Log("[Get User Game Data]");
            if (isGetUserGameData) return;
            // Debug.Log("[Get User Game Data]123");
            isGetUserGameData = true;

            string hash = Crypto(Application.version, UserInfoManager.Instance.userId.ToString(),
                SecurePlayerPrefs.GetString("accessToken"));

            UserState userState =
                await SendToServerAsync<UserState>(
                string.Format(Url.userState, UserInfoManager.Instance.userId, LanguageManager.Instance.nationalKey,
                UserInfoManager.Instance.email, Application.version), SendType.GET, TokenType.ACCESSTOKEN, "",
            hash);
            // #endif

            UserInfoManager.Instance.SetUserState(userState);

            MemberInfo memberInfo = await SendToServerAsync<MemberInfo>(
                string.Format(Url.getMemberInfo, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);

            UserInfoManager.Instance.provider = memberInfo.provider;

            if (!userState.activated) return;

            //Total Api
            TotalUserData totalUserData = await SendToServerAsync<TotalUserData>(
                string.Format(Url.initTotalUserData, UserInfoManager.Instance.userId), SendType.GET,
                TokenType.ACCESSTOKEN);

            UserInfoManager.Instance.SetUserNicknameInfo(totalUserData.nickname);
            DataManager.Instance.userCharacters = totalUserData.character;
            UserSlotManager.Instance.UserSlotData(totalUserData.slot);
            DataManager.Instance.storeItemDTO.storeItems = totalUserData.storeItems;
            UserInfoManager.Instance.SetEmergeRate(totalUserData.randomBoxEmergeRate);
            UserInfoManager.Instance.eventInfo.eventBoss = totalUserData.eventBoss;
            UserInfoManager.Instance.dailyRewardBoard.rewardLeaderBoard = totalUserData.rewardDailyLeaderBoards;
            UserInfoManager.Instance.weeklyRewardBoard.rewardLeaderBoard = totalUserData.rewardLeaderBoards;
            UserInfoManager.Instance.discordConnect = totalUserData.discord;
            UserInfoManager.Instance.userItemDic = totalUserData.userItems.ToDictionary(u => u.item, u => u.quantity);
            CharacterClassManager.Instance.Initialize(totalUserData.classUpRule);

            HatchingItemData hatchDatas = await SendToServerAsync<HatchingItemData>(
                string.Format(Url.getHatchItem, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.LoadHatchingStoneItemData(hatchDatas);

            UserPassInfo userPassData = await SendToServerAsync<UserPassInfo>(
                string.Format(Url.getUserPassCheck, UserInfoManager.Instance.userId), SendType.GET,
                TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userPassInfo = userPassData;
            UserInfoManager.Instance.userPassId = userPassData.battlePassId;

            SpecialStoreDTO specialStoreDTO = await SendToServerAsync<SpecialStoreDTO>(string.Format(Url.specialStoreItem, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.specialStoreDTO = specialStoreDTO;

            LimitedStoreItem limitedStoreItem = await SendToServerAsync<LimitedStoreItem>(string.Format(Url.getLimitedShop, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.limitedStoreItem = limitedStoreItem;

            UserWalletHistory userTikWalletHistory =
                await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "TIK"), SendType.GET,
                    TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userTikWalletHistory = userTikWalletHistory;

            UserWalletHistory userStikWallethistory =
                await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"),
                    SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;

            // Roulette
            _ = GetRouletteTable();

            onPendingInAppPurchase?.Invoke();

            AssetManager.Instance.AssetDownload();
        }

        #endregion

        public async UniTask GetHatchingStone()
        {
            HatchingItemData hatchDatas = await SendToServerAsync<HatchingItemData>(
                string.Format(Url.getHatchItem, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.LoadHatchingStoneItemData(hatchDatas);

            //action?.Invoke();
        }

        public async UniTask GetUserState()
        {
            UserState userState = await SendToServerAsync<UserState>(
                           string.Format(Url.userState, UserInfoManager.Instance.userId, LanguageManager.Instance.nationalKey,
                               UserInfoManager.Instance.email, Application.version), SendType.GET, TokenType.ACCESSTOKEN);
            
            UserInfoManager.Instance.SetUserState(userState);
        }

        public async UniTask GetUserWalletHistory()
        {
            UserWalletHistory userTikWalletHistory =
                await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "TIK"), SendType.GET,
                    TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userTikWalletHistory = userTikWalletHistory;

            //Debug.Log(JsonUtility.ToJson(userTikWalletHistory));

            UserWalletHistory userStikWallethistory =
                await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"),
                    SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;
        }

        public async UniTask GetUserWalletHistory(UnityAction tikSuccess, UnityAction stikSuccess)
        {
            UserWalletHistory userTikWalletHistory = await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "TIK"), SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userTikWalletHistory = userTikWalletHistory;

            UserWalletHistory userStikWallethistory = await SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"), SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;

            stikSuccess?.Invoke();
            tikSuccess?.Invoke();
        }

        public async UniTask GuestLoginProcess(string json, bool isAdmin, UnityAction<string> failedAlert)
        {
            string url = Url.guestLogin;
            UnityWebRequest req = new(Domain.baseUrl + url, SendType.POST.ToString());
            req.downloadHandler = new DownloadHandlerBuffer();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);

            req.SetRequestHeader(contentType, contentTypeValue);

            try
            {
                var res = await req.SendWebRequest();
                TokenVM tokenVM = JsonUtility.FromJson<TokenVM>(res.downloadHandler.text);

                AccountStatus accountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), tokenVM.accountStatus);
                SetUserToken(tokenVM);
                Debug.Log(accountStatus);
                switch (accountStatus)
                {
                    case AccountStatus.NORMAL:
                        _ = GetUserResourceByTokenModule(() => { LoginManager.Instance.SetLoginScreen(false); });

                        break;
                    case AccountStatus.ALREADY_CONNECTED_DEVICE:
                        LoginManager.Instance.SetLoginScreen(false);
                        break;
                    case AccountStatus.TERMINATION_REQUESTED:
                        LoginManager.Instance.RestorePopup();
                        break;
                    case AccountStatus.TERMINATION_COMPLETED:
                        break;
                    case AccountStatus.JOIN_IN_PROGRESS:
                        Debug.Log("Login Manager is Login : " + LoginManager.Instance.isLogin);
                        _ = GetUserResourceByTokenModule(() =>
                        {
                            _ = GetBoardContent(() => { LoginManager.Instance.joinPopup.SetJoinPopup(); },
                                () => { });
                        });

                        break;
                    default:
                        break;


                }

                res.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("[Failed Login Process]");
#if !UNITY_EDITOR
                GoogleSignInManager.Instance.OnSignOut();
#endif
                Debug.Log(req.downloadHandler.text);
                ResponseError(req.downloadHandler.text, failedAlert);
            }

            req.Dispose();
        }

        public async UniTask LoginProcess(string json, bool isAdmin, UnityAction<string> failedAlert)
        {
            Debug.Log("[In Login Process]");
            string url = isAdmin ? Url.adminLogin : Url.socialLogin;
            UnityWebRequest req = new(Domain.baseUrl + url, SendType.POST.ToString());
            req.downloadHandler = new DownloadHandlerBuffer();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);

            req.SetRequestHeader(contentType, contentTypeValue);

            try
            {
#if !UNITY_EDITOR
                var res = await req.SendWebRequest();
                TokenVM tokenVM = JsonUtility.FromJson<TokenVM>(res.downloadHandler.text);

                AccountStatus accountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), tokenVM.accountStatus);
                SetUserToken(tokenVM);
                Debug.Log(accountStatus);
                switch (accountStatus)
                {
                    case AccountStatus.NORMAL:
                        _ = GetUserResourceByTokenModule(() =>
                        {
                            LoginManager.Instance.SetLoginScreen(false);    
                        });
                        
                        break;
                    case AccountStatus.ALREADY_CONNECTED_DEVICE:
                        LoginManager.Instance.SetLoginScreen(false);
                        break;
                    case AccountStatus.TERMINATION_REQUESTED:
                        _ = GetUserResourceByTokenModule(() =>
                        {
                            LoginManager.Instance.RestorePopup();
                        });
                        break;
                    case AccountStatus.TERMINATION_COMPLETED:
                        break;
                    case AccountStatus.JOIN_IN_PROGRESS:
                        Debug.Log("Login Manager is Login : " + LoginManager.Instance.isLogin);
                        _ = GetUserResourceByTokenModule(() =>
                            {
                                _ = GetBoardContent(() =>
                                {         
                                    LoginManager.Instance.joinPopup.SetJoinPopup();
                                }, () => { });
                            });

                        break;
                    default:
                        break;

                Debug.Log("asd");
                
               
                }
#elif UNITY_EDITOR
                var res = await req.SendWebRequest();
                TokenVM tokenVm = JsonUtility.FromJson<TokenVM>(res.downloadHandler.text);
                SetUserToken(tokenVm);
                _ = GetUserResourceByTokenModule(() => { LoginManager.Instance.SetLoginScreen(false); });
#endif
                res.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("[Failed Login Process]");
#if !UNITY_EDITOR
                GoogleSignInManager.Instance.OnSignOut();
#endif
                Debug.Log(req.downloadHandler.text);
                ResponseError(req.downloadHandler.text, failedAlert);
            }

            req.Dispose();
        }

        public async UniTask SocialIntegrationCheck(string json, string token, string provider,
            UnityAction<string, string, string> OnSuccess, UnityAction<string> OnFailed)
        {
            string url = string.Format(Url.validUserSocialLogin, UserInfoManager.Instance.userId);
            UnityWebRequest req = new(Domain.baseUrl + url, SendType.POST.ToString());
            req.downloadHandler = new DownloadHandlerBuffer();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);

            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader(contentType, contentTypeValue);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                OnSuccess?.Invoke(res.downloadHandler.text, token, provider);
                res.Dispose();
            }
            catch (Exception)
            {
                Debug.Log("[Failed Social Integration Check]");
                Debug.Log(req.downloadHandler.text);

                if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
                {
                    GoogleSignInManager.Instance.OnSignOut();
                }

                string failed = "MSG_Switching_Accouunt_Fail_2";
                OnFailed?.Invoke(failed);
            }

            req.Dispose();
        }

        public async UniTask SocialIntegrationProcess(string json, UnityAction OnSuccess, UnityAction<string> OnFailed)
        {
            string url = string.Format(Url.userSocialLoginIntegration, UserInfoManager.Instance.userId);
            UnityWebRequest req = new(Domain.baseUrl + url, SendType.POST.ToString());
            req.downloadHandler = new DownloadHandlerBuffer();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader(contentType, contentTypeValue);

            try
            {
                var res = await req.SendWebRequest();
                TokenVM tokenVM = JsonUtility.FromJson<TokenVM>(res.downloadHandler.text);

                SetUserToken(tokenVM);
                _ = GetUserState();
                OnSuccess?.Invoke();
                res.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log("[Failed Social Integration]");
                Debug.Log(e);
                if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
                {
                    GoogleSignInManager.Instance.OnSignOut();
                }

                string failed = req.downloadHandler.text.Contains("ALREADY_INTEGRATION_ACCOUNT")
                    ? "MSG_Switching_Accouunt_Fail_1"
                    : "MSG_Switching_Accouunt_Fail_2";
                OnFailed?.Invoke(failed);
            }

            req.Dispose();
        }


        public async UniTask Logout()
        {
            isGetUserGameData = false;
            UnityWebRequest req = new(Domain.baseUrl + Url.logout, "DELETE");

            Logout data = new()
            {
                clientId = "DEFENGO",
                deviceId = SystemInfo.deviceUniqueIdentifier
            };
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(req.downloadHandler.text);
            }
            catch
            {
                Debug.Log("[Logout Failed]");
                Debug.Log(req.downloadHandler.text);
            }

            SingleToneManager.Instance.SetLogout();
            SecurePlayerPrefs.DeleteKey("accessToken");
            SecurePlayerPrefs.DeleteKey("refreshToken");
            SceneLoadManager.Instance.SwitchingScene(0);
            req.Dispose();
        }


        public async UniTask GoWalletCheck()
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getuserWallet, UserInfoManager.Instance.userId), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                JSONNode json = JSONNode.Parse(res.downloadHandler.text);
                for (int i = 0; i < json.Count; i++)
                {
                    JSONNode value = json[i];
                    SpendingAccounts wallet = new()
                    {
                        amount = value["amount"],
                        uiAmountString = value["uiAmountString"],
                        symbol = value["symbol"],
                        name = value["name"],
                        decimals = value["decimals"],
                        tokenAddress = value["tokenAddress"],
                        logoUrl = value["logoUrl"]
                    };
                }
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
            }
            req.Dispose();
        }


        // api 1.4.4
        public async UniTask GetBoardContent(UnityAction Success, UnityAction Failed)
        {
            UnityWebRequest req =
                new(
                    Domain.baseUrl + string.Format(Url.getBoardContent, "TERMS,PRIVACY,MARKETING",
                        LanguageManager.Instance.nationalKey), "GET");
            //UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getBoardContent, LanguageManager.Instance.nationalKey), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            Debug.Log("[Get Board Content]");
            try
            {
                var res = await req.SendWebRequest();
                JSONNode json = JSONNode.Parse(res.downloadHandler.text);
                for (int i = 0; i < json.Count; i++)
                {
                    JSONNode value = json[i];
                    BoardList boardList = new()
                    {
                        postId = value["id"],
                        boardType = value["boardType"],
                        lang = value["lang"],
                        title = value["title"],
                        content = value["content"]
                    };
                    if(!UserInfoManager.Instance.dic_BoardList.ContainsKey(boardList.boardType))
                    {
                        UserInfoManager.Instance.dic_BoardList.Add(boardList.boardType, boardList);
                    }
                }

                Success?.Invoke();
                //LoginManager.Instance.joinPopup.SetJoinPopup(UserInfoManager.Instance.dic_BoardList);
            }
            catch
            {
                Failed?.Invoke();
            }
        }

        // api 1.4.4
        public async UniTask GetNoticeContent(UnityAction<string> Success)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.getNoticeContent, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");

            try
            {
                var res = await req.SendWebRequest();
                Success?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
            }
        }

        public async UniTask TermsHistory(string json)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.terms_histories, UserInfoManager.Instance.userId), "POST");
            Debug.Log(Domain.baseUrl + string.Format(Url.terms_histories, UserInfoManager.Instance.userId));
            string accessToken = SecurePlayerPrefs.GetString("accessToken");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                LoginManager.Instance.joinPopup.PopUpSequence(false);

                LoginManager.Instance.SetLoginScreen(false);
                //_ = GetUserGameData();
                Debug.Log(res.downloadHandler.text);
                res.Dispose();
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                req.Dispose();
            }
        }

        public async void GetEnergyValue(UnityAction<ResponeUserPlay> success, UnityAction failed)
        {
            await GetEnergyValueAsync(success, failed);
        }

        public async UniTask GetEnergyValueAsync(UnityAction<ResponeUserPlay> Success, UnityAction Failed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getEnergyValue, UserInfoManager.Instance.userId), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                ResponeUserPlay data = JsonUtility.FromJson<ResponeUserPlay>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Failed?.Invoke();
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserProfileList(UnityAction<GetUserProfileList> Success, UnityAction Failed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getUserProfileList, UserInfoManager.Instance.userId), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                GetUserProfileList data = JsonUtility.FromJson<GetUserProfileList>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Failed?.Invoke();
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetDailyRewardPool(string date, UnityAction<DailyRewardPool> Success, UnityAction Failed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getDailyRewardPool, date), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                DailyRewardPool data = JsonUtility.FromJson<DailyRewardPool>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Failed?.Invoke();
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserTermsHistory()
        {
            Debug.Log("Get User Terms History");
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getUserTermsHistory, UserInfoManager.Instance.userId, "DEFENGO"),
                    "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                JSONNode json = JSONNode.Parse(res.downloadHandler.text);

                if (json.Count == 0)
                {
                    LoginManager.Instance.joinPopup.PopUpSequence(true);
                    return;
                }

                bool isAccept = true;

                for (int i = 0; i < json.Count; i++)
                {
                    string data = json[i]["boardType"];
                    switch (data)
                    {
                        case "TERMS":
                            if (!json[i]["activated"])
                            {
                                LoginManager.Instance.joinPopup.PopUpSequence(true);
                                isAccept = false;
                            }

                            break;
                        case "PRIVACY":
                            if (!json[i]["activated"])
                            {
                                LoginManager.Instance.joinPopup.PopUpSequence(true);
                                isAccept = false;
                            }

                            break;
                        case "MARKETING":
                            if (!json[i]["activated"])
                            {
                            }

                            break;
                    }
                }

                if (isAccept)
                {
                    _ = GetUserGameData();
                }

                res.Dispose();
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                if (req.responseCode == 404)
                {
                    LoginManager.Instance.joinPopup.PopUpSequence(true);
                }
            }

            req.Dispose();
        }

        public async UniTask GetUserResourceByTokenModule(UnityAction Success)
        {
            //Debug.Log("Get User Resources By Token Module");
            UnityWebRequest req = new(Domain.baseUrl + Url.account, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                SetUserToken();
                UserVM userVM = GetT<UserVM>(res.downloadHandler.text);

                //Debug.Log(res.downloadHandler.text);
                UserInfoManager.Instance.SetUserData(userVM);

                Success?.Invoke();
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ResponseError(req.downloadHandler.text);
                OnComplete_RefreshToken = () =>
                {
                    // Debug.Log("OnComplete Refresh Token");
                    _ = GetUserResourceByTokenModule(() => { Success?.Invoke(); });
                };
            }

            req.Dispose();
        }

        // public async UniTask GetUserResourceByTokenModuleRestore()
        // {
        //     UnityWebRequest req = new(Domain.baseUrl + Url.account, "GET");
        //     req.downloadHandler = new DownloadHandlerBuffer();
        //     req.SetRequestHeader(contentType, contentTypeValue);
        //     string accessToken = SecurePlayerPrefs.GetString("accessToken");
        //     req.SetRequestHeader(authorization, bearer + accessToken);

        //     try
        //     {
        //         var res = await req.SendWebRequest();
        //         SetUserToken();
        //         UserVM userVM = GetT<UserVM>(res.downloadHandler.text);
        //         UserInfoManager.Instance.SetUserData(userVM);
        //         LoginManager.Instance.RestorePopup();
        //     }
        //     catch
        //     {
        //         Debug.Log(req.downloadHandler.text);
        //     }
        //     req.Dispose();
        // }

        public bool isRefreshSequence = false;

        public async UniTask RefreshToken()
        {
            if (isRefreshSequence) return;
            string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "AOS";
            isRefreshSequence = true;
            UnityWebRequest req = new(Domain.baseUrl + Url.getToken, "POST");
            RefreshToken data = new()
            {
                clientId = "DEFENGO",
                fcmToken = SecurePlayerPrefs.GetString("fcmToken"),
                deviceId = SystemInfo.deviceUniqueIdentifier,
                deviceModel = SystemInfo.deviceModel,
                platform = deviceInfo,
                appVersion = Application.version,
                osVersion = SystemInfo.operatingSystem
            };
            string json = JsonUtility.ToJson(data);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string refreshToken = SecurePlayerPrefs.GetString("refreshToken");
            req.SetRequestHeader(authorization, bearer + refreshToken);

            try
            {
                var res = await req.SendWebRequest();
                TokenVM tokenVM = GetT<TokenVM>(res.downloadHandler.text);
                SetUserToken(tokenVM);
                OnComplete_RefreshToken?.Invoke();
                isRefreshSequence = false;
            }
            catch (Exception)
            {
                // Debug.Log(e);
                // Debug.Log(req.downloadHandler.text);
                // Debug.Log("SceneNum : " + SceneLoadManager.Instance.GetCurrentScene());
                if (SceneLoadManager.Instance.GetCurrentScene() == 1)
                {
                    LoginManager.Instance.SetLoginScreen(true);
                }
                else // if (SceneLoadManager.Instance.GetCurrentScene() == 2 && SceneLoadManager.Instance.GetCurrentScene() == 3)
                {
                    SingleToneManager.Instance.SetLogout();
                    SecurePlayerPrefs.DeleteKey("accessToken");
                    SecurePlayerPrefs.DeleteKey("refreshToken");
                    SceneLoadManager.Instance.SwitchingScene(0);
                }

                isRefreshSequence = false;
            }

            OnComplete_RefreshToken = null;
            req.Dispose();
        }

        public async UniTask<Texture2D> GetImageFromS3(string url)
        {
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            try
            {
                var res = await req.SendWebRequest();
                Texture2D texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
                return texture;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return default;
            }
        }

        #region General

        public async UniTask<T> SendToServerAsync<T>(string url, SendType sendType, TokenType tokenType,
            string json = "", string fingerPrint = null, string iosCheck = null)
        {
            // Debug.Log(url);
            OnComplete_RefreshToken = null;
            UnityWebRequest req = new(Domain.baseUrl + url, sendType.ToString());
            req.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(json))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            req.SetRequestHeader(contentType, contentTypeValue);
            if (!string.IsNullOrEmpty(fingerPrint))
            {
                req.SetRequestHeader("Fingerprint", fingerPrint);
            }

            if (!string.IsNullOrEmpty(iosCheck))
            {
                req.SetRequestHeader("AppSealingCredential", iosCheck);
            }

            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            string refreshToken = SecurePlayerPrefs.GetString("refreshToken");

            switch (tokenType)
            {
                case TokenType.ACCESSTOKEN:
                    req.SetRequestHeader(authorization, bearer + accessToken);
                    break;
                case TokenType.REFRESHTOKEN:
                    req.SetRequestHeader(authorization, bearer + refreshToken);
                    break;
                case TokenType.NULL:
                    break;
            }

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                T result = JsonMapper.ToObject<T>(res.downloadHandler.text);
                req.Dispose();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.Log(url);
                ResponseError(req.downloadHandler.text, OnFailedLoadUserData);
                //OnFailedLoadUserData?.Invoke("UI_Network_Unstable");

                req.Dispose();
                return default;
            }
        }

        #endregion

        public async UniTask RegistUserPlay(UnityAction<string> OnFailed, UnityAction<string> OnFailedGameLogOut)
        {
            Debug.Log("[Regist User Play]");
            if (disposedPlayRecords.Count > 0)
            {
                disposedPlayRecords.Clear();
            }

            RegistUserPlay data = UserInfoManager.Instance.GetRegistUserPlayData();
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.registPlay, data.userId), "POST");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Fingerprint",
                Crypto(Application.version, UserInfoManager.Instance.userId.ToString(),
                    SecurePlayerPrefs.GetString("accessToken")));

            try
            {
                var res = await req.SendWebRequest();
                ResponeUserPlay responeUserPlay = GetT<ResponeUserPlay>(res.downloadHandler.text);
                UserInfoManager.Instance.SuccessPlayInfo(responeUserPlay);
            }
            catch
            {
                // Not Enough Energy
                Debug.Log(req.responseCode.ToString());

                RegisterUserResponseError(req.downloadHandler.text, OnFailed, OnFailedGameLogOut);
                //ResponseError(req.downloadHandler.text, OnFailed);
            }

            req.Dispose();
        }

        public async UniTask CreateUserWallet()
        {
            string url = string.Format(Url.createUserWallet, UserInfoManager.Instance.userId);
            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                // Not Enough Energy
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask CreateWallet()
        {
            UserId data = new()
            {
                userId = int.Parse(UserInfoManager.Instance.userId)
            };

            string url = string.Format(Url.createWallet, "DEFENGO");
            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                // Not Enough Energy
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetRewardRule()
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.rewardRuleList, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                RewardRuleList data = GetT<RewardRuleList>(res.downloadHandler.text);
                UserInfoManager.Instance.SuccessRewardRuleList(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserWalletHistory(string key)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserWalletHistory, key), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                UserWalletHistory data = GetT<UserWalletHistory>(res.downloadHandler.text);
                if (key == "TIK")
                {
                    UserInfoManager.Instance.userTikWalletHistory = data;
                }

                int purchaseTaika = (int)data.transactions[0].amount;
                onSuccessWallet?.Invoke(purchaseTaika);
                onSuccessWallet = null;
                //Success?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserMonthlyPackage(string key)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserWalletHistory, key), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                UserWalletHistory data = GetT<UserWalletHistory>(res.downloadHandler.text);
                if (key == "TIK")
                {
                    UserInfoManager.Instance.userTikWalletHistory = data;
                }
                //int purchaseTaika = (int)data.transactions[0].amount;
                onSuccessMonthlyPackagePurchase?.Invoke();
                onSuccessMonthlyPackagePurchase = null;
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        public async UniTask GetEventPopupList(UnityAction<EventPopupList> onSuccess, UnityAction onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getEventPopupList, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                EventPopupList data = GetT<EventPopupList>(res.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetUserEventPrize(UnityAction<UserEventPrize> onSuccess, UnityAction onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getEventPrize, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                UserEventPrize data = GetT<UserEventPrize>(res.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke();
            }

            req.Dispose();
        }

        public async void GetUserWalletHistories()
        {
            await GetUserWalletHistory("PTIK");
        }

        public async void GetUserMonthlyPackageWalletHistories()
        {
            await GetUserMonthlyPackage("PTIK");
        }

        public async UniTask GetUserPassDetail(UnityAction<BattlePassUserInfoDetailData> onSuccess,
            UnityAction onFailed)
        {
            UnityWebRequest req =
                new(
                    Domain.baseUrl + string.Format(Url.getUserPassCheckDetail, UserInfoManager.Instance.userId,
                        UserInfoManager.Instance.userPassId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                BattlePassUserInfoDetailData data = GetT<BattlePassUserInfoDetailData>(res.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetUserMonthlyPackage(UnityAction<MonthlyPackageDTO> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.monthlyPackageData, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                MonthlyPackageDTO data = GetT<MonthlyPackageDTO>(res.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }

        public async UniTask BuyUserMonthlyPackage(RequestMonthlyPackage data, UnityAction<MonthlyPackageDTO> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.monthlyPackageData, UserInfoManager.Instance.userId), "POST");

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                if (data.type.Contains("BUY"))
                {
                    IAPManager.Instance.ConfirmPendingProduct();
                }

                MonthlyPackageDTO monthlyPackageData = GetT<MonthlyPackageDTO>(res.downloadHandler.text);
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(monthlyPackageData);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }

        public async UniTask GetSpecialStore(UnityAction<SpecialStoreDTO> onSuccess)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.specialStoreItem, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                SpecialStoreDTO data = JsonMapper.ToObject<SpecialStoreDTO>(res.downloadHandler.text);
                DataManager.Instance.specialStoreDTO = data;
                onSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }
            req.Dispose();
        }


        public async UniTask RequestPassBuyExp(BattlePassBuyExp data,
            UnityAction<BattlePassUserInfoDetailData> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.passBuyExp, UserInfoManager.Instance.userId),
                "POST");

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                if (data.payType.Contains("INAPP"))
                {
                    Debug.Log("Success Crystal");
                    IAPManager.Instance.ConfirmPendingProduct();
                }

                BattlePassUserInfoDetailData detailData = GetT<BattlePassUserInfoDetailData>(res.downloadHandler.text);
                onSuccess?.Invoke(detailData);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                ServerErrorMessage error = GetT<ServerErrorMessage>(req.downloadHandler.text);
                onFailed?.Invoke(error.errorCode);
            }

            req.Dispose();
        }

        public async UniTask RequestPassBuyPremium(BattlePassBuyExp data,
            UnityAction<BattlePassUserInfoDetailData> onSuccess, UnityAction onFailed)
        {
            UnityWebRequest req =
                new(
                    Domain.baseUrl + string.Format(Url.passBuyPremium, UserInfoManager.Instance.userId,
                        UserInfoManager.Instance.userPassId), "POST");

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);

                if (data.payType.Contains("INAPP"))
                {
                    IAPManager.Instance.ConfirmPendingProduct();
                }

                BattlePassUserInfoDetailData detailData = GetT<BattlePassUserInfoDetailData>(res.downloadHandler.text);
                onSuccess?.Invoke(detailData);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask RequsetPassReward(BattlePassGetReward data, UnityAction<string> onSuccess,
            UnityAction onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.passUserReward, UserInfoManager.Instance.userId), "POST");

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetUserDailyQuestDetail(UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getUserDailyQuestInfo, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserDailyQuestExp(string questType, UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            string url = Domain.baseUrl + string.Format(Url.getDailyQuestAmount, UserInfoManager.Instance.userId, questType);

            UnityWebRequest req = new(url, "POST");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserDailyCompleteMissionReward(int boxExp, UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.getDailyCompleteMissionReward, UserInfoManager.Instance.userId, boxExp), "POST");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserAchievementDetail(UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserAchievementInfo, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserAchievementReward(string rewardType, UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserAchievementReward, UserInfoManager.Instance.userId, rewardType);

            UnityWebRequest req = new(url, "POST");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserLimitedShopPityInfo(UnityAction<ResPityDatas> onSuccess, UnityAction<string> onFailed)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserLimitedShopPityInfo, UserInfoManager.Instance.userId);
            UnityWebRequest req = new(url, "GET");
            //UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserLimitedShopPityInfo, UserInfoManager.Instance.userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                List<ResPityData> response = JsonMapper.ToObject<List<ResPityData>>(res.downloadHandler.text);
                ResPityDatas datas = new();
                datas.pityDatas = response;
                onSuccess?.Invoke(datas);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask RequestPityStep(ReqPityStepData reqData, UnityAction<List<ResPityStepData>> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.userPityStep, UserInfoManager.Instance.userId), "POST");

            string json = JsonUtility.ToJson(reqData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                List<ResPityStepData> resData = JsonMapper.ToObject<List<ResPityStepData>>(res.downloadHandler.text);
                onSuccess?.Invoke(resData);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask RequestPityReward(ReqPityReward reqData, UnityAction<string> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.userPityRewards, UserInfoManager.Instance.userId), "POST");

            string json = JsonUtility.ToJson(reqData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                onSuccess?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                onFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }



        public async UniTask UserIapStateCheck(RequestValidateIAP data)
        {
            string formaturl = string.Format(Url.getUserIapPurchaseState, UserInfoManager.Instance.userId, data.purchaseId);
            string url = Domain.baseUrl + formaturl;
            Debug.Log($"url : {url}");

            UnityWebRequest req = new(url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log($"UserIapStateCheck : {res.downloadHandler.text}");

                ResponsePendingIAP response = GetT<ResponsePendingIAP>(res.downloadHandler.text);

                IapBuyType iapBuyType = (IAPManager.Instance.GetIapBuyType(response.appProductId));

                switch (iapBuyType)
                {
                    case IapBuyType.NONE:
                        break;

                    case IapBuyType.BATTLEPASS_PREMIUM:
                        if (response.state.Equals("VALID") || response.state.Equals("PAYED"))
                        {
                            IAPManager.Instance.PendingBattlePassPremiumPurchase();
                        }
                        break;

                    case IapBuyType.BATTLEPASS_EXP:
                        if (response.state.Equals("VALID") || response.state.Equals("PAYED"))
                        {
                            IAPManager.Instance.PendingBattlePassExpPurchase();
                        }
                        break;

                    case IapBuyType.MONTHLY_GEM:
                        if (response.state.Equals("VALID"))
                        {
                            onSuccessMonthlyPackagePurchase = () =>
                            {
                                IAPManager.Instance.PendingMonthlyPackagePurchase(response.appProductId);
                            };

                            RequestPay requestPay = new()
                            {
                                purchaseId = data.purchaseId
                            };

                            _ = RequestMonthlyPayIAP(requestPay);
                        }
                        else if (response.state.Equals("PAYED"))
                        {
                            IAPManager.Instance.PendingMonthlyPackagePurchase(response.appProductId);
                        }
                        break;

                    default:
                        if (response.state.Equals("VALID"))
                        {
                            RequestPay requestPay = new()
                            {
                                purchaseId = data.purchaseId
                            };
                            _ = RequestPayIAP(requestPay);
                        }
                        break;
                }
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
            }

            req.Dispose();
        }


        public async UniTask RequestValidateInAppPurChase(RequestValidateIAP data)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.validateInAppPurchase, "POST");

            string json = JsonUtility.ToJson(data);
            Debug.Log("In Req valid : " + json);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                Debug.Log("Req valid Iap Success");
                ResponseValidateIAP responseValidateIAP = GetT<ResponseValidateIAP>(res.downloadHandler.text);

                if (responseValidateIAP.valid)
                {
                    switch (IAPManager.Instance.iapBuyType)
                    {
                        case IapBuyType.NONE:
                            break;

                        case IapBuyType.BATTLEPASS_PREMIUM:
                            onSuccessPassPremiumInAppPurchase?.Invoke();
                            onSuccessPassPremiumInAppPurchase = null;
                            break;

                        case IapBuyType.BATTLEPASS_EXP:
                            onSuccessPassExpInAppPurchase?.Invoke();
                            onSuccessPassExpInAppPurchase = null;
                            break;

                        case IapBuyType.MONTHLY_GEM:
                            RequestPay monthlyRequestPay = new()
                            {
                                purchaseId = data.purchaseId
                            };
                            _ = RequestMonthlyPayIAP(monthlyRequestPay);
                            break;

                        default:
                            RequestPay requestPay = new()
                            {
                                purchaseId = data.purchaseId
                            };
                            _ = RequestPayIAP(requestPay);
                            break;

                    }
                }
                else
                {
                    Debug.LogError("Not Paid");
                }

                IAPManager.Instance.iapBuyType = IapBuyType.NONE;
            }
            catch (Exception ex)
            {
                onPendingInAppPurchase = () =>
                {
                    RequestIAPError(req.responseCode.ToString(), data);
                    onPendingInAppPurchase = null;
                };

                Debug.Log(ex);
                Debug.Log(req.downloadHandler.text);
                IAPManager.Instance.iapBuyType = IapBuyType.NONE;
            }
        }

        public void RequestIAPError(string errorMesseage, RequestValidateIAP data)
        {
            if (errorMesseage == "400")
            {
                _ = UserIapStateCheck(data);
            }
        }

        public UnityAction onPendingInAppPurchase;
        public UnityAction onSuccessPassPremiumInAppPurchase;
        public UnityAction onSuccessPassExpInAppPurchase;
        public UnityAction<int> onSuccessWallet;
        public UnityAction onSuccessMonthlyPackagePurchase;

        public async UniTask RequestPayIAP(RequestPay data)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.paidInAppPurchase, "POST");

            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log("RequsetPayIap Success");
                //Debug.Log(res.downloadHandler.text);

                ResponsePay responsePay = GetT<ResponsePay>(res.downloadHandler.text);
                Debug.Log($"Pay RequestPayIap ResponsePay payed : {responsePay.payed}");

                if (responsePay.payed)
                {
                    GetUserWalletHistories();

                    IAPManager.Instance.ConfirmPendingProduct();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public async UniTask RequestMonthlyPayIAP(RequestPay data)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.paidInAppPurchase, "POST");

            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log($"RequestPayIap Success : {res.downloadHandler.text}");

                ResponsePay responsePay = GetT<ResponsePay>(res.downloadHandler.text);
                Debug.Log($"Pay RequestPayIap ResponsePay payed : {responsePay.payed}");

                if (responsePay.payed)
                {
                    GetUserMonthlyPackageWalletHistories();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }


        public async UniTask RequestOpenHatchingStone(UserItem data, UnityAction<HatchingOpenData, string> OnSuccess,
            UnityAction<string> OnFailed)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.hatchOpenItem, UserInfoManager.Instance.userId), "POST");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                HatchingOpenData responseHatchingStone = GetT<HatchingOpenData>(res.downloadHandler.text);
                OnSuccess?.Invoke(responseHatchingStone, data.item);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                OnFailed?.Invoke(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask RequestBuyItem(RequestBuyItem data, UnityAction<ResRewardTemplate> OnSuccess, UnityAction<string> OnFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.buyItem, UserInfoManager.Instance.userId), "POST");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                ResRewardTemplate responseRandomBox = GetT<ResRewardTemplate>(res.downloadHandler.text);

                OnSuccess?.Invoke(responseRandomBox);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                OnFailed?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }

        public async UniTask ClassUpCharacter(MoneyType moneyType, ReqCharacterClassUp data, UnityAction<UserClassUpResponse, MoneyType> Success,
            UnityAction Failed)
        {
            UnityWebRequest req =
                new(
                    Domain.baseUrl + string.Format(Url.characterClassUp, UserInfoManager.Instance.userId), "POST");

            string json = JsonUtility.ToJson(data);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                UserClassUpResponse ucuData = GetT<UserClassUpResponse>(res.downloadHandler.text);
                Debug.Log("Success Class Up");
                Success?.Invoke(ucuData, moneyType);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);

                ErrorMessage(req.responseCode.ToString());
                Failed?.Invoke();
            }

            req.Dispose();
        }

        public async void SendPlayRecord(PlayRecord data)
        {
            disposedPlayRecords.Push(data);
            int stackCount = disposedPlayRecords.Count;

            for (int i = 0; i < stackCount; i++)
            {
                await PlayRecords(disposedPlayRecords.Pop());
            }
        }

        public async UniTask PlayRecords(PlayRecord data)
        {
            string url = string.Format(Url.playRecord, UserInfoManager.Instance.userId);

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            string json = JsonUtility.ToJson(data);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log("Play Record Success");
                Debug.Log(res.downloadHandler.text);
                res.Dispose();
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ServerErrorMessage serverErrorMessage = GetT<ServerErrorMessage>(req.downloadHandler.text);
                switch (serverErrorMessage.status)
                {
                    case 401:
                        _ = RefreshToken();
                        OnComplete_RefreshToken = () => { _ = PlayRecords(data); };
                        break;
                    case 406:
                        break;
                    case 400:
                        Debug.Log(serverErrorMessage.errorCode);
                        _ = Logout();
                        break;
                    case 422:
                        Debug.Log(serverErrorMessage.errorCode);
                        _ = Logout();
                        break;
                }
            }

            req.Dispose();
        }

        public async UniTask<string> GetUserMonthlyTikHistory(string dateValue)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserMonthlyTikHistory, "DEFENGO",
                UserInfoManager.Instance.userId, dateValue);

            UnityWebRequest req = new(url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                string data = res.downloadHandler.text;
                res.Dispose();
                return data;
            }
            catch
            {
                req.Dispose();
                return default;
            }
        }

        // Api 3.5.1
        public async UniTask GetUserDailyRankInfo(string date, UnityAction<string> OnSuccesResponse)
        {
            string url = Domain.baseUrl +
                         string.Format(Url.getUserDailyRankInfo, UserInfoManager.Instance.userId, "DEFENGO", date);

            UnityWebRequest req = new(url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccesResponse?.Invoke(res.downloadHandler.text);
            }
            catch
            {
            }

            req.Dispose();
        }

        // Api 3.5.2

        public async UniTask GetUserDailyRankInfoDetail(string date, string targetUserId,
            UnityAction<ResponseRankDetail> Success, UnityAction Failed)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserRankDetatil, date, targetUserId);
            //string.Format(Url.getUserRankDetatil, targetUserId, "DEFENGO", date);

            //int tempUserId = int.Parse(targetUserId);
            //RequestRankDetail requestRankDetail = new()
            //{
            //    userId = tempUserId,
            //    aggregatedDate = date,
            //    clientId = "DEFENGO"
            //};
            UnityWebRequest req = new(url, "GET");

            //string json = JsonUtility.ToJson(requestRankDetail);

            //Debug.Log("Wave Data : " + json);
            //byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            //req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log("Success");
                Debug.Log(res.downloadHandler.text);
                ResponseRankDetail data = GetT<ResponseRankDetail>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Failed?.Invoke();
            }

            req.Dispose();
        }


        public async UniTask GetUserNicknameValid(string nickname, UnityAction OnSuccess, UnityAction<string> OnFailed)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserNicknameValid, "DEFENGO", nickname);

            UnityWebRequest req = new(url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);

                OnSuccess?.Invoke();
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);

                switch (req.responseCode)
                {
                    case 400:
                        ServerErrorMessage data = GetT<ServerErrorMessage>(req.downloadHandler.text);
                        OnFailed?.Invoke(data.errorCode);
                        break;
                    case 401:
                        _ = RefreshToken();
                        break;
                    default:
                        break;
                }
            }

            req.Dispose();
        }

        public async UniTask GetUserGameNickname()
        {
            string url = Domain.baseUrl +
                         string.Format(Url.getUserNicknameValid, "DEFENGO", UserInfoManager.Instance.userId);

            UnityWebRequest req = new(url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
            }
            catch
            {
            }

            req.Dispose();
        }


        public async UniTask GetDailyLeaderBoardInfo(string date, UnityAction<string> OnSuccessResponse,
            UnityAction OnFailedResponse)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.dailyLeaderBoard, "DEFENGO", date), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccessResponse?.Invoke(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("failed");

                ErrorMessage(req.responseCode.ToString());
                OnFailedResponse?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetLeaderBoardBestWaves(LeaderBoardType type, int roundId, UnityAction<string> OnSuccessResponse,
            UnityAction OnFailedResponse)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.LeaderBoardBestWaves, type, roundId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccessResponse?.Invoke(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("failed");
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                OnFailedResponse?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetMyLeaderBoardRankInfo(LeaderBoardType type, int roundId, UnityAction<string> OnSuccessResponse,
            UnityAction OnFailedResponse)
        {
            UnityWebRequest req =
                new(
                    Domain.baseUrl + string.Format(Url.getMyLeaderBoardRankData, UserInfoManager.Instance.userId, type,
                        roundId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccessResponse?.Invoke(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("failed");

                ErrorMessage(req.responseCode.ToString());
                OnFailedResponse?.Invoke();
            }

            req.Dispose();
        }

        public int requestFinishCount = 0;

        public async UniTask GameFinished(Dictionary<string, string> data)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            UnityWebRequest req =
                UnityWebRequest.Put(Domain.baseUrl + string.Format(Url.gameFinished, data["id"]), bodyRaw);
            req.method = "PATCH";
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Content-length", bodyRaw.Length.ToString());

            try
            {
                var res = await req.SendWebRequest();
                disposedPlayRecords.Clear();
                requestFinishCount = 0;
                Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                requestFinishCount++;
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        public async UniTask UpdateUserState(Dictionary<string, string> data, UnityAction Callback)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            UnityWebRequest req =
                UnityWebRequest.Put(Domain.baseUrl + string.Format(Url.updateUserState, data["userId"]), bodyRaw);
            req.method = "PATCH";

            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Content-length", bodyRaw.Length.ToString());

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                UserInfoManager.Instance.SetUserState(GetT<UserState>(res.downloadHandler.text));
                Callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Debug.Log("Update Failed");
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        public async UniTask UpdateUserProfile(int profileIdx, UnityAction<int> Success, UnityAction<long> Fail)
        {
            UnityWebRequest req =
                new(Domain.baseUrl + string.Format(Url.setUserProfileIdx, UserInfoManager.Instance.userId, profileIdx),
                    "POST");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                JSONNode json = JSONNode.Parse(res.downloadHandler.text);
                int resProfileIdx = json["equippedProfileId"];

                UserInfoManager.Instance.userState.equippedProfileId = resProfileIdx;
                //Debug.Log("OnSuccess Profile Request");
                //UserInfoManager.Instance.SetUserState(GetT<UserState>(res.downloadHandler.text));
                Success?.Invoke(resProfileIdx);
            }
            catch (Exception ex)
            {
                Fail?.Invoke(req.responseCode);
                Debug.Log(ex);
                Debug.Log("Update Failed");
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        public async UniTask EditUserInfo(Dictionary<string, object> data, UnityAction<bool> Callback)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            UnityWebRequest req =
                UnityWebRequest.Put(Domain.baseUrl + string.Format(Url.uaaUsers, data["id"]), bodyRaw);
            req.method = "PATCH";

            req.SetRequestHeader(contentType, "application/merge-patch+json");
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Content-length", bodyRaw.Length.ToString());

            try
            {
                var res = await req.SendWebRequest();
                UserVM user = GetT<UserVM>(res.downloadHandler.text);
                UserInfoManager.Instance.SetUserNickname(user.nickname);
                Callback?.Invoke(true);

                Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                Callback?.Invoke(false);
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask EditUserNickname(string json, UnityAction<bool> CallBack)
        {
            string url = Domain.baseUrl +
                         string.Format(Url.editUserNickname, "DEFENGO", UserInfoManager.Instance.userId);
            UnityWebRequest req = new(url, "PUT");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                GetUserGameNickname data = GetT<GetUserGameNickname>(res.downloadHandler.text);
                UserInfoManager.Instance.SetUserNicknameInfo(data);
                CallBack?.Invoke(true);

                //Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                CallBack?.Invoke(false);
                ErrorMessage(req.responseCode.ToString());
            }

            req.Dispose();
        }

        public async UniTask GetUserWalletInfo(UnityAction<int> Success, string symbol)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserWallet, symbol, "DEFENGO"), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                JSONNode json = JSONNode.Parse(res.downloadHandler.text);
                int value = json["amount"];
                Success?.Invoke(value);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserProfileDetail(LeaderBoardType type, string userId, int roundId, UnityAction<ResponseProfileData> Success,
            UnityAction Failed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getUserLeaderBoardDetail, type, roundId, userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                // Debug.Log(res.downloadHandler.text);
                ResponseProfileData data = GetT<ResponseProfileData>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Failed?.Invoke();
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetTotalUserProfileDetail(string userId, UnityAction<TotalProfileData> Success,
            UnityAction Failed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getTotalUserProfileData, userId), "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                // Debug.Log(res.downloadHandler.text);
                TotalProfileData data = GetT<TotalProfileData>(res.downloadHandler.text);
                Success?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Failed?.Invoke();
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        public void SetUserSlotData(Dictionary<string, object> data, UnityAction Success)
        {
            StartCoroutine(WebPatchRequestAsync($"/game/api/defengo-user-slots/users/{UserInfoManager.Instance.userId}",
                data, Success));
        }

        public IEnumerator WebPatchRequestAsync(string url, Dictionary<string, object> data, UnityAction Success)
        {
            string requestBodyString = JsonConvert.SerializeObject(data);

            //Debug.Log(requestBodyString);
            byte[] requestBodyData = Encoding.UTF8.GetBytes(requestBodyString);
            //Debug.Log(Domain.baseUrl + url);
            UnityWebRequest req = UnityWebRequest.Put(Domain.baseUrl + url, requestBodyData);
            req.method = "PATCH";

            //Debug.Log(accessToken);
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Content-length", requestBodyData.Length.ToString());

            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            //Debug.Log(req.responseCode);

            //Debug.Log(req.result);
            switch (req.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    Success?.Invoke();
                    //Debug.Log(req.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    ErrorMessage(req.responseCode.ToString());
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    ErrorMessage(req.responseCode.ToString());
                    break;
                case UnityWebRequest.Result.DataProcessingError:

                    break;
                default:
                    break;
            }

            req.Dispose();
        }

        public async UniTask TerminationAccount(TerminateAccountInfo data, UnityAction Success, UnityAction Failed)
        {
            string url = Domain.baseUrl + Url.terminationAccount;

            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            UnityWebRequest req = new(url, "PUT");
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                Success?.Invoke();
                SingleToneManager.Instance.SetLogout();
                SecurePlayerPrefs.DeleteKey("accessToken");
                SecurePlayerPrefs.DeleteKey("refreshToken");
                SceneLoadManager.Instance.SwitchingScene(0);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                Failed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask TerminationAccountV2(TerminateAccountInfo data, UnityAction Success, UnityAction Failed)
        {
            string url = string.Format(Domain.baseUrl + Url.terminationAccountV2, UserInfoManager.Instance.userId);

            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            UnityWebRequest req = new(url, "PUT");
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                Success?.Invoke();
                SingleToneManager.Instance.SetLogout();
                SecurePlayerPrefs.DeleteKey("accessToken");
                SecurePlayerPrefs.DeleteKey("refreshToken");
                SceneLoadManager.Instance.SwitchingScene(0);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                Failed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask RestoreAccountV2(UnityAction Success, UnityAction<TerminationStatusInfo> Failed)
        {
            string url = string.Format(Domain.baseUrl + Url.restoreAccountV2, UserInfoManager.Instance.userId);
            Debug.Log("Restore Url : " + url);
            UnityWebRequest req = new(url, "PUT");
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log("Restore Account Status : " + res.responseCode);
                _ = GetUserResourceByTokenModule(() => { _ = GetUserGameData(); });
                Success?.Invoke();
            }
            catch
            {
                TerminationStatusInfo data = GetT<TerminationStatusInfo>(req.downloadHandler.text);
                Failed.Invoke(data);
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask UserAdHistory(string code, UnityAction Success)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.postUserAdHistory, "POST");
            DateTime now = DateTime.UtcNow;
            string adIdData =
                $"{AdmobManager.Instance.adUnitId}_{SystemInfo.deviceUniqueIdentifier}_{Calculator.GetTime(now)}";

            UserAdHistory data = new()
            {
                clientId = "DEFENGO",
                code = code,
                userId = int.Parse(UserInfoManager.Instance.userId),
                adId = adIdData
            };

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                Success?.Invoke();
                // Refresh Energy Value
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
            }

            req.Dispose();
        }

        public async UniTask RegistUserAdValid(string code, string type, UnityAction OnSuccess, UnityAction OnFailed)
        {
            string userId = UserInfoManager.Instance.userId;
            string url = string.Format(Url.registUserAdHistory, userId, type);
            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            DateTime now = DateTime.UtcNow;
            string adIdData =
                $"{AdmobManager.Instance.adUnitId}_{SystemInfo.deviceUniqueIdentifier}_{Calculator.GetTime(now)}";

            UserAdHistory data = new()
            {
                clientId = "DEFENGO",
                code = code,
                userId = int.Parse(UserInfoManager.Instance.userId),
                adId = adIdData
            };

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Fingerprint",
                Crypto(Application.version, UserInfoManager.Instance.userId.ToString(),
                    SecurePlayerPrefs.GetString("accessToken")));

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                OnSuccess?.Invoke();
                // Refresh Energy Value
            }
            catch
            {
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
            }

            req.Dispose();
        }

        public async UniTask RegistUserAdValid(string code, string type, UnityAction<RandomBoxIndex> OnSuccess,
            UnityAction OnFailed)
        {
            string userId = UserInfoManager.Instance.userId;
            string url = string.Format(Url.registUserAdHistory, userId, type);
            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            DateTime now = DateTime.UtcNow;
            string adIdData =
                $"{AdmobManager.Instance.adUnitId}_{SystemInfo.deviceUniqueIdentifier}_{Calculator.GetTime(now)}";

            UserAdHistory data = new()
            {
                clientId = "DEFENGO",
                code = code,
                userId = int.Parse(UserInfoManager.Instance.userId),
                adId = adIdData
            };

            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);
            req.SetRequestHeader("Fingerprint",
                Crypto(Application.version, UserInfoManager.Instance.userId.ToString(),
                    SecurePlayerPrefs.GetString("accessToken")));

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                RandomBoxIndex randomBoxIndex = GetT<RandomBoxIndex>(res.downloadHandler.text);

                OnSuccess?.Invoke(randomBoxIndex);
                // Refresh Energy Value
            }
            catch
            {
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
            }

            req.Dispose();
        }

        public async UniTask UserAdValidInfo(string adType, UnityAction<UserAdValidInfo> OnSuccess,
            UnityAction OnFailed)
        {
            string url = string.Format(Url.getUserAdValidInfo, UserInfoManager.Instance.userId, adType);
            //Debug.Log(url);
            UnityWebRequest req = new(Domain.baseUrl + url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                UserAdValidInfos data = JsonUtility.FromJson<UserAdValidInfos>(res.downloadHandler.text);
                OnSuccess?.Invoke(data.userAds[0]);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                // 엑세스 토큰 만료 401 -> 리프레시 토큰으로 요청
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask UserAdValid(UnityAction<UserAdInfo> OnSuccess, UnityAction OnFailed)
        {
            //Debug.Log("asdasdasd");
            string url = string.Format(Url.getUserAdValid, "DEFENGO", UserInfoManager.Instance.userId);
            //Debug.Log(url);
            UnityWebRequest req = new(Domain.baseUrl + url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);

                UserAdInfo data = JsonUtility.FromJson<UserAdInfo>(res.downloadHandler.text);
                //OnSuccess?.Invoke(data);

                OnSuccess?.Invoke(data);
            }
            catch
            {
                // 엑세스 토큰 만료 401 -> 리프레시 토큰으로 요청
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }


        // 20.4.1
        public async UniTask GetUserInboxList(bool isActive, UnityAction<UserInboxInfo, bool> OnSuccess,
            UnityAction OnFailed)
        {
            string url = string.Format(Url.getUserInboxInfo, UserInfoManager.Instance.userId);

            UnityWebRequest req = new(Domain.baseUrl + url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                UserInboxInfo data = GetT<UserInboxInfo>(res.downloadHandler.text);

                OnSuccess?.Invoke(data, isActive);
            }
            catch (Exception e)
            {
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
                Debug.Log(e);
            }

            req.Dispose();
        }

        public async UniTask ConnectDiscordInfo(int id, UnityAction<DiscordConnect> OnSuccess, UnityAction OnFailed)
        {
            string url = string.Format(Url.connectDiscord, id);

            UnityWebRequest req = new(Domain.baseUrl + url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                DiscordConnect discordConnect = GetT<DiscordConnect>(res.downloadHandler.text);

                OnSuccess?.Invoke(discordConnect);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                OnFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask ConnectDiscord(UnityAction<DiscordConnect> OnSuccess)
        {
            string url = string.Format(Url.connectDiscord, UserInfoManager.Instance.userId);

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                DiscordConnect discordConnect = GetT<DiscordConnect>(res.downloadHandler.text);
                UserInfoManager.Instance.discordConnect = discordConnect;
                OnSuccess?.Invoke(discordConnect);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            req.Dispose();
        }

        // 20.4.2
        public async UniTask GetUserInboxDetail(int id, int inboxId, UnityAction<string> OnSuccess,
            UnityAction OnFailed)
        {
            string url = string.Format(Url.getUserInboxDetail, UserInfoManager.Instance.userId, id, inboxId);

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccess?.Invoke(res.downloadHandler.text);
                //Debug.Log(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                OnFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask SwapToken(string json, UnityAction OnSuceess, UnityAction<string> OnFailed)
        {
            string url = Url.swapToken;

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                // Debug.Log(res.downloadHandler.text);
                OnSuceess?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log(e);

                JSONNode error = JSONNode.Parse(req.downloadHandler.text);
                OnFailed?.Invoke(error["errorMessage"]);
            }

            req.Dispose();
        }

        public async UniTask ExchangeToken(double value, UnityAction OnSuccess, UnityAction<string> OnFailed)
        {
            //solana tik userId

            string url = string.Format(Url.exchangeToken, "solana", "STIK", UserInfoManager.Instance.userId);

            // Debug.Log(url);
            ExchangeToken data = new()
            {
                type = "deposit",
                uiAmount = value,
                fee = 0,
                encodedTransaction = ""
            };

            string json = JsonUtility.ToJson(data);
            //Debug.Log(json);

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                OnSuccess?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                JSONNode error = JSONNode.Parse(req.downloadHandler.text);
                OnFailed?.Invoke(error["errorCode"]);
            }

            req.Dispose();
        }

        public async UniTask AbusingRecord(string abusingData)
        {
            //solana tik userId

            string url = Url.abusesingRecord;

            AbusingRecord data = new()
            {
                clientId = "DEFENGO",
                userId = int.Parse(UserInfoManager.Instance.userId),
                abusingData = abusingData
            };

            string json = JsonUtility.ToJson(data);

            UnityWebRequest req = new(Domain.baseUrl + url, "POST");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log("Send Abusing Report");
                ClearLocalStorage();
                Application.Quit();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            req.Dispose();
        }

        // public async UniTask GetWeeklyReward(UnityAction<WeeklyLeaderBoard> OnSuccess, UnityAction OnFailed)
        // {
        //     string url = Url.getWeeklyRewardBoard;
        //     //string.Format(Url.getUserInboxInfo, UserInfoManager.Instance.userId);

        //     UnityWebRequest req = new(Domain.baseUrl + url, "GET");
        //     req.downloadHandler = new DownloadHandlerBuffer();
        //     req.SetRequestHeader(contentType, contentTypeValue);
        //     string accessToken = SecurePlayerPrefs.GetString("accessToken");
        //     req.SetRequestHeader(authorization, bearer + accessToken);

        //     try
        //     {
        //         var res = await req.SendWebRequest();
        //         WeeklyLeaderBoard data = GetT<WeeklyLeaderBoard>(res.downloadHandler.text);
        //         OnSuccess?.Invoke(data);
        //     }
        //     catch (Exception e)
        //     {
        //         OnFailed?.Invoke();
        //         Debug.Log(req.downloadHandler.text);
        //         Debug.Log(e);
        //     }

        //     req.Dispose();
        // }

        public async UniTask GetLeaderBoardRewardData(LeaderBoardType type, UnityAction<string> OnSuccess, UnityAction OnFailed)
        {
            string url = string.Format(Url.getLeaderBoardRewardData, UserInfoManager.Instance.userId, type);
            UnityWebRequest req = new(Domain.baseUrl + url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                OnSuccess?.Invoke(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                OnFailed?.Invoke();
                Debug.Log(req.downloadHandler.text);
                Debug.Log(e);
            }

            req.Dispose();
        }

        public async UniTask DeleteUserInbox(UnityAction<UserInboxInfo, bool> OnSuccess, UnityAction OnFailed)
        {
            string url = string.Format(Url.deleteUserInbox, UserInfoManager.Instance.userId);

            UnityWebRequest req = new(Domain.baseUrl + url, "DELETE");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();

                UserInboxInfo data = GetT<UserInboxInfo>(res.downloadHandler.text);

                OnSuccess?.Invoke(data, false);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                OnFailed?.Invoke();
            }

            req.Dispose();
        }

        public async UniTask GetTokenProductList(UnityAction<string> OnSuccess, UnityAction OnFailed,
            string productType)
        {
            string url = string.Format(Url.getTokenProductList, productType);

            UnityWebRequest req = new(Domain.baseUrl + url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);

            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(req.downloadHandler.text);
                OnSuccess?.Invoke(res.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                OnFailed();
            }
        }

        public async UniTask GetBanCharacter(UnityAction<string> unityAction)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.userBanCharacter, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader(contentType, contentTypeValue);
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(authorization, bearer + accessToken);

            //현재 본인의 덱캐릭터랑 비교해서 벤되어있는캐릭터가있다면 그것만 표시해줌
            try
            {
                var res = await req.SendWebRequest();

                List<int> banList = JsonConvert.DeserializeObject<List<int>>(req.downloadHandler.text);
                UserSlot userSlot = UserSlotManager.Instance.userSlot;
                List<int> userBanList = new();

                int focusIdx = UserSlotManager.Instance.focusIdx - 1;

                for (int i = 0; i < userSlot.slots[focusIdx].slotCharacterIds.Length; ++i)
                {
                    for (int j = 0; j < banList.Count; ++j)
                    {
                        if (!userBanList.Contains(banList[j]) && banList[j] == userSlot.slots[focusIdx].slotCharacterIds[i])
                        {
                            userBanList.Add(banList[j]);
                        }
                    }
                }

                StringBuilder builder = new();

                for (int i = 0; i < userBanList.Count; ++i)
                {
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append($"<color=#EE3350>{LanguageManager.Instance.GetStringData($"UI_Character_{userBanList[i]}")}</color>");
                }
                string message = LanguageManager.Instance.GetStringData("CHARACTER_BAN_DESC") + builder;

                unityAction?.Invoke(message);
            }
            catch (Exception e)
            {
                Debug.Log(req.downloadHandler.text);
                Debug.Log(e);
            }
            req.Dispose();
        }

        public async UniTask GetUserSkinEquip()
        {
            string url = Domain.baseUrl + string.Format(Url.getUserSkinEquip, UserInfoManager.Instance.userId);

            UnityWebRequest req = new(url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask GetUserSkinCharacter(int character_id, UnityAction<ResUserSkins> success)
        {
            string url = Domain.baseUrl + string.Format(Url.getUserSkinCharacter, UserInfoManager.Instance.userId, character_id);

            UnityWebRequest req = new(url, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);

                ResUserSkins data = JsonMapper.ToObject<ResUserSkins>(res.downloadHandler.text);
                success?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
            }

            req.Dispose();
        }

        public async UniTask PatchUserSkinChange(int character_id, int skin_id, UnityAction<ResUserSkins> OnSuccess, UnityAction<string> OnFailed)
        {
            string url = Domain.baseUrl + string.Format(Url.patchUserSkinChange, UserInfoManager.Instance.userId, character_id, skin_id);

            UnityWebRequest req = new(url, "PATCH");

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                //Debug.Log(res.downloadHandler.text);
                ResUserSkins data = JsonMapper.ToObject<ResUserSkins>(res.downloadHandler.text);
                OnSuccess?.Invoke(data);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                ServerErrorMessage data = JsonMapper.ToObject<ServerErrorMessage>(req.downloadHandler.text);
                OnFailed?.Invoke(data.errorCode);
            }

            req.Dispose();


        }

        public async UniTask PostUserSkinBuy(ReqSkinBuyData data, UnityAction<ResUserSkins> OnSuccessBuy, UnityAction<string> OnFailedBuy)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.GameAPI + $"user-skins/users/{UserInfoManager.Instance.userId}", "POST");
            string json = JsonMapper.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);

                ResUserSkins res_data = JsonMapper.ToObject<ResUserSkins>(res.downloadHandler.text);
                OnSuccessBuy?.Invoke(res_data);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                OnFailedBuy?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }

        public async UniTask PostUserSkinPackageBuy(RequestBuyItem data, UnityAction<ResRewardTemplate> OnSuccessBuy, UnityAction<string> OnFailedBuy)
        {
            UnityWebRequest req = new(Domain.baseUrl + Url.GameAPI + $"stores/users/{UserInfoManager.Instance.userId}/buy", "POST");
            string json = JsonMapper.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);

                ResRewardTemplate res_data = JsonMapper.ToObject<ResRewardTemplate>(res.downloadHandler.text);
                OnSuccessBuy?.Invoke(res_data);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                OnFailedBuy?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }


        private async UniTask RequestNetwork(NetworkType type, string url, string json_data = "", UnityAction<string> success = null, UnityAction<string> fail = null)
        {
            UnityWebRequest req = new(url, type.ToString());
            if (string.IsNullOrEmpty(json_data) == false)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json_data);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);

                success?.Invoke(res.downloadHandler.text);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                fail?.Invoke(req.downloadHandler.text);
            }
            req.Dispose();
        }

        public void ErrorMessage(string errorData)
        {
            Debug.Log("error code : " + errorData);
            if (errorData == "401")
            {
                Debug.Log("401 error");
                _ = RefreshToken();
            }
        }

        public void ResponseError(string data, UnityAction<string> unityAction = null)
        {
            try
            {
                ServerErrorMessage serverErrorMessage = GetT<ServerErrorMessage>(data);
                Debug.Log("[Server Error Message] : " + serverErrorMessage.status);
                switch (serverErrorMessage.status)
                {
                    case 400:
                        break;
                    case 401:
                        _ = RefreshToken();
                        break;
                    case 404:
                        break;
                    case 406:
                        break;
                    case 429:
                        unityAction?.Invoke("MSG_Too_Many_Request");
                        break;
                    case 430:
                        break;
                    case 422:
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                Debug.Log("response Error3");
                unityAction?.Invoke("UI_Network_Unstable");
            }
        }

        public void RegisterUserResponseError(string data, UnityAction<string> unityAction, UnityAction<string> OnFailedGameLogOut)
        {
            try
            {
                ServerErrorMessage serverErrorMessage = GetT<ServerErrorMessage>(data);
                Debug.Log("[Server Error Message] : " + serverErrorMessage.status);
                string message = "";
                switch (serverErrorMessage.status)
                {
                    case 400:
                        message = LanguageManager.Instance.GetStringData("UI_Not_Enough_Currency");
                        unityAction?.Invoke(message);
                        break;
                    case 401:
                        OnComplete_RefreshToken = () =>
                        {
                            HomeScreen.Instance.RequestStartGame();
                        };
                        _ = RefreshToken();
                        break;
                    case 404:
                        break;
                    case 406:
                        break;
                    case 429:
                        message = LanguageManager.Instance.GetStringData("MSG_Too_Many_Request");
                        unityAction?.Invoke(message);
                        break;
                    case 430:
                        break;
                    case 422:
                        _ = GetBanCharacter(unityAction);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                Debug.Log("response Error : Register UserError");
                string message = "";
                message = LanguageManager.Instance.GetStringData("UI_Network_Unstable_Logout");
                OnFailedGameLogOut?.Invoke(message);
            }
        }

        private async UniTask GetRouletteTable()
        {
            Dictionary<string, List<int>> datas = await SendToServerAsync<Dictionary<string, List<int>>>(
                Url.getRouletteTable, SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.SetRoulettTableData(datas);
        }

        public async UniTask GetRouletteGroup(int playId, UnityAction<ReqRouletteGroupData> onSuccess, UnityAction onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.getRouletteGroup, UserInfoManager.Instance.userId, playId), "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                ReqRouletteGroupData rouletteGroupData = JsonMapper.ToObject<ReqRouletteGroupData>(res.downloadHandler.text);
                onSuccess?.Invoke(rouletteGroupData);
            }
            catch
            {
                Debug.Log(req.downloadHandler.text);
                ErrorMessage(req.responseCode.ToString());
                onFailed?.Invoke();
            }
            req.Dispose();
        }

        public async UniTask PostRouletteReward(int playId, RoulettePaidType type, UnityAction<ReqRouletteRewardData> onSuccess, UnityAction<string> onFailed)
        {
            UnityWebRequest req = new(Domain.baseUrl + string.Format(Url.postRouletteReward, UserInfoManager.Instance.userId), "POST");
            
            ResponeRoulettePlayId data = new();
            data.playId = playId;
            data.paidType = type.ToString();
            
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            req.SetRequestHeader(contentType, contentTypeValue);
            req.SetRequestHeader(authorization, bearer + accessToken);

            try
            {
                var res = await req.SendWebRequest();
                Debug.Log(res.downloadHandler.text);
                ReqRouletteRewardData datas = GetT<ReqRouletteRewardData>(res.downloadHandler.text);

                onSuccess?.Invoke(datas);
            }
            catch
            {
                ErrorMessage(req.responseCode.ToString());
                Debug.Log(req.downloadHandler.text);
                ServerErrorMessage error = GetT<ServerErrorMessage>(req.downloadHandler.text);
                onFailed?.Invoke(error.errorCode);
            }

            req.Dispose();
        }

        public async UniTask GetLimitedShop(UnityAction<LimitedStoreItem> onSuccess, UnityAction<string> onFailed)
        {
            try
            {
                LimitedStoreItem limitedShopData = await SendToServerAsync<LimitedStoreItem>(
                    string.Format(Url.getLimitedShop, UserInfoManager.Instance.userId),
                    SendType.GET,
                    TokenType.ACCESSTOKEN);

                onSuccess?.Invoke(limitedShopData);
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
            }
        }

        public void SetUserToken(TokenVM data)
        {
            Debug.Log("Set Token");
            accessToken = data.accessToken;
            refreshToken = data.refreshToken;

            SecurePlayerPrefs.SetString("accessToken", data.accessToken);
            SecurePlayerPrefs.SetString("refreshToken", data.refreshToken);
        }

        public void SetUserToken()
        {
            accessToken = SecurePlayerPrefs.GetString("accessToken");
            refreshToken = SecurePlayerPrefs.GetString("refreshToken");
        }

        public static string Crypto(string appVersion, string userId, string jwt)
        {
            string data = appVersion + userId + jwt;

            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = new SHA256CryptoServiceProvider().ComputeHash(bytes);
            var encryptedData = new StringBuilder();

            foreach (var h in hash)
            {
                encryptedData.AppendFormat("{0:x2}", h);
            }

            return encryptedData.ToString();
        }

        public void ClearLocalStorage()
        {
            SecurePlayerPrefs.DeleteAll();
        }

        public T GetT<T>(string json)
        {
            T result = JsonUtility.FromJson<T>(json);
            return result;
        }

        
    }
}
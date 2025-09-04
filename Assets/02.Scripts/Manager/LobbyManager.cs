using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using System;
using DG.Tweening;
using Newtonsoft.Json;

namespace Framework.UI
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        public ScreenTemplate[] lobbyScreens;
        public TextMeshProUGUI text_Nickname;
        public TextMeshProUGUI text_Energy;
        public TextMeshProUGUI text_Gem;
        public TextMeshProUGUI text_Taika;
        public TextMeshProUGUI text_Stik;
        public TextMeshProUGUI text_EnergyChargeCount;
        public DOTweenAnimation doTweenAnim;
        public Image image_Profile;
        public Image image_backGround;
        public Image image_limitedProfile;
        public MenuType menuType;
        public GameObject objectPurchasePopup;
        public GameObject obejctProfile;
        public GameObject objectTaika;
        public GameObject objectBottomDim;
        public Transform rect_Destination;
        public Button button_Energy;
        public Button button_Gem;
        public Button button_Profile;
        public Button button_Setting;
        public Button button_Stik;
        public IEnumerator countTimer;

        //public ToastMessage toastMessage;

        public int energy;

        void Start()
        {
            Initialize();
            Application.targetFrameRate = 60;
        }

        private void Awake()
        {
            Instance = this;
        }

        public void PurchasePopup(bool isActive)
        {
            objectPurchasePopup.SetActive(isActive);
        }

        //public void ActiveShopScreen(bool isActive)
        //{
        //    text_Taika.text = UserInfoManager.Instance.userTikWalletHistory.balance.amount.ToString();
        //    obejctProfile.SetActive(!isActive);
        //    objectTaika.SetActive(isActive);
        //}

        public void ActiveShopScreen(bool isActive, PageState pageState)
        {
            text_Taika.text = UserInfoManager.Instance.userTikWalletHistory.balance.amount.ToString();
            double stikBalance = Double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
            text_Stik.text = $"{stikBalance:0.####}";

            obejctProfile.SetActive(!isActive);
            objectTaika.SetActive(isActive);

            bool isSpecial = pageState == PageState.SPECIAL;

            button_Stik.gameObject.SetActive(isSpecial);
            button_Energy.gameObject.SetActive(!isSpecial);
        }

        public void ShopScreenPageChange(PageState pageState)
        {
            bool isSpecial = pageState == PageState.SPECIAL;

            button_Stik.gameObject.SetActive(isSpecial);
            button_Energy.gameObject.SetActive(!isSpecial);
        }


        // private void CheckPrevLoginTime()
        // {
        //     //todo DailyRewardPopup 2.9.0 이후로 삭제 : 배포를 생각해보면 9시 이후에 받을수는있는데 리워드 받는데 문제생김
        //     string prevGetRewardDate = SecurePlayerPrefs.GetString("prevRewardDate");
        //     GetUserWallet();
        //     bool isFirst = string.IsNullOrEmpty(prevGetRewardDate);
        //     Transaction transaction = UserInfoManager.Instance.GetDailyRewardTransaction();
        //     if (isFirst)
        //     {
        //         if (transaction != null)
        //         {
        //             PopupManager.Instance.GetPopUp<DailyRewardPopup>("dailyReward").Initialize(transaction, CheckSkinPackage);
        //         }
        //         else
        //             CheckSkinPackage();
        //     }
        //     else
        //     {
        //         //데일리 보상이 있으면 보상 팝업이 닫힌후 패키지 체크 없으면 바로 패키지 체크
        //         if (transaction != null)
        //         {
        //             DateTime prevDate = Convert.ToDateTime(prevGetRewardDate);
        //             prevDate = new(prevDate.Year, prevDate.Month, prevDate.Day);
        //             DateTime transactionDate = Convert.ToDateTime(transaction.createdDate);
        //             DateTime compareDate = new(transactionDate.Year, transactionDate.Month, transactionDate.Day);

        //             int convertInt = DateTime.Compare(prevDate, compareDate);

        //             if (convertInt < 0)
        //             {
        //                 PopupManager.Instance.GetPopUp<DailyRewardPopup>("dailyReward").Initialize(transaction, CheckSkinPackage);
        //             }
        //             else
        //                 CheckSkinPackage();
        //         }
        //         else
        //             CheckSkinPackage();
        //     }
        // }

        public void GetUserWallet()
        {
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        public void SuccessWallet(int gem)
        {
            StartCoroutine(ChangeValueSequenceAsync(gem, UserInfoManager.Instance.gemValue, text_Gem));
            UserInfoManager.Instance.gemValue = gem;
        }

        public void SetProfileImage(UserProfileData userProfileData)
        {
            switch (userProfileData.profileType)
            {
                case ProfileType.Character:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                case ProfileType.Skin:
                    image_limitedProfile.gameObject.SetActive(false);
                    image_backGround.gameObject.SetActive(true);
                    image_Profile.gameObject.SetActive(true);

                    SkinGradeType gradeType = DataManager.Instance.SkinTableDataList.Find(a => a.id == userProfileData.profileIndex).skin_grade_type;
                    image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[gradeType].color_Profile;

                    image_Profile.sprite = userProfileData.sprite_image;
                    break;

                default:
                    image_limitedProfile.gameObject.SetActive(true);
                    image_backGround.gameObject.SetActive(false);
                    image_Profile.gameObject.SetActive(false);

                    image_limitedProfile.sprite = userProfileData.sprite_image;
                    break;
            }
        }

        public void Initialize()
        {
            GetEnergyValue();

            if (UserInfoManager.Instance.userState.equippedProfileId == 0)
            {
                UserInfoManager.Instance.userState.equippedProfileId = 1;
            }

            if (UserInfoManager.Instance.userState.userProfileIds.Length == 0)
            {
                int[] arr = { 1, 2, 3, 4, 5, 1001 };
                UserInfoManager.Instance.userState.userProfileIds = arr;
            }

            UserProfileData userProfileData =
                DataManager.Instance.dic_userProfileData[UserInfoManager.Instance.userState.equippedProfileId];

            SetProfileImage(userProfileData);

            for (int i = 0; i < lobbyScreens.Length; i++)
            {
                ScreenTemplate screen = lobbyScreens[i];

                if (!screen.gameObject.activeSelf)
                {
                    screen.gameObject.SetActive(true);
                }

                bool isActive = screen.menuType == MenuType.HOME;
                screen.ScreenSequence(isActive);
                screen.Initialize();
            }

            UserState userState = UserInfoManager.Instance.userState;

            energy = userState.energyAmount;

            string[] temp = UserInfoManager.Instance.nickname.Split('@');
            text_Nickname.text = temp[0];

            string energyText = energy < 10
                ? $"<color=#ff7d7d>{energy}/{ConfigData.ENERGY_TOTAL}"
                : $"<color=#ffffff>{energy}</color>/{ConfigData.ENERGY_TOTAL}";
            text_Energy.text = energyText;

            button_Gem.onClick.AddListener(() =>
            {
                ShopScreen.Instance.isGemPage = true;
                ShopScreen.Instance.pageState = PageState.GEM;
                LobbyNavigator.Instance.OnClick_Screen(MenuType.SHOP);
            });
            button_Energy.onClick.AddListener(() =>
            {
                ShopScreen.Instance.isGemPage = false;
                ShopScreen.Instance.pageState = PageState.SHOP;
                LobbyNavigator.Instance.OnClick_Screen(MenuType.SHOP);
            });

            button_Stik.onClick.AddListener(() =>
            {
                ShopScreen.Instance.isGemPage = false;
                ShopScreen.Instance.pageState = PageState.SHOP;
                LobbyNavigator.Instance.OnClick_Screen(MenuType.WALLET);
            });

            //button_Profile.onClick.AddListener(() => PopupManager.Instance.GetPopUp<ProfilePopup>("profile").ActivePopup());
            button_Profile.onClick.AddListener(() =>
                {
                    var userProfilePopup = PopupManager.Instance.GetPopUp<UserProfileDetailPopup>("userProfileDetail");
                    userProfilePopup.ActivePopup();
                    userProfilePopup.ShowMyProfile();
                    // userProfilePopup.SetInfo(UserInfoManager.Instance.userId, UserInfoManager.Instance.userState.equippedProfileId);
                });
            button_Setting.onClick.AddListener(() =>
                PopupManager.Instance.GetPopUp<SettingPopup>("setting").ActivePopup());

            //toastMessage.Initialize();

            //testCode
            //UserInfoManager.Instance.userState.finishedTutorial = false; 
            //bool isUserTutorial = true;

            bool isUserTutorial = !UserInfoManager.Instance.userState.finishedTutorial;
            if (isUserTutorial)
            {
                LobbyTutorialManager.Instance.Initialize();
            }
            else
            {
                CheckActivePopup();
            }
        }

        public void CheckActivePopup()
        {
            GetUserWallet();
            CheckLeaderBoardRewardWeeklyPopup();
        }
        // Weekly Reward Popup
        public async void CheckLeaderBoardRewardWeeklyPopup()
        {
            await NetworkManager.Instance.GetLeaderBoardRewardData(LeaderBoardType.WAVE_WEEKLY, (data) =>
            {
                LeaderBoardRewardStatusData rewardData = JsonConvert.DeserializeObject<LeaderBoardRewardStatusData>(data);

                if (rewardData.rewardStatus == "REWARD")
                {
                    LeaderBoardRewardPopup popup = PopupManager.Instance.GetPopUp<LeaderBoardRewardPopup>("leaderBoardReward");
                    popup.SetRewardList(LeaderBoardType.WAVE_WEEKLY, rewardData.finalRank);

                    popup.OnCompletePopup = () =>
                    {
                        CheckLeaderBoardRewardDailyPopup();
                    };
                }
                else
                {
                    CheckLeaderBoardRewardDailyPopup();
                }
            }, () => { CheckLeaderBoardRewardDailyPopup(); });
        }
        // Daily Reward Popup
        public async void CheckLeaderBoardRewardDailyPopup()
        {
            await NetworkManager.Instance.GetLeaderBoardRewardData(LeaderBoardType.WAVE_DAILY, (data) =>
            {
                LeaderBoardRewardStatusData rewardData = JsonConvert.DeserializeObject<LeaderBoardRewardStatusData>(data);

                if (rewardData.rewardStatus == "REWARD")
                {
                    LeaderBoardRewardPopup popup = PopupManager.Instance.GetPopUp<LeaderBoardRewardPopup>("leaderBoardReward");
                    popup.SetRewardList(LeaderBoardType.WAVE_DAILY, rewardData.finalRank);

                    popup.OnCompletePopup = () =>
                    {
                        CheckSkinPackage();
                    };
                }
                else
                {
                    CheckSkinPackage();
                }
            }, () => { CheckSkinPackage(); });
        }

        public void GetEnergyValue()
        {
            NetworkManager.Instance.GetEnergyValue(SuccessData, FailedData);
        }

        public void SuccessData(ResponeUserPlay data)
        {
            UserInfoManager.Instance.userState.energyAmount = data.energyAmount;
            UserInfoManager.Instance.userState.energyRemainCoolTime = data.energyRemainCoolTime;
            bool isCount = data.energyRemainCoolTime > 0;
            energy = data.energyAmount;
            string energyText = energy < 10 ? $"<color=#ff7d7d>{energy}</color>/{ConfigData.ENERGY_TOTAL}" : $"<color=#ffffff>{energy}</color>/{ConfigData.ENERGY_TOTAL}"; ;
            text_Energy.text = energyText;
            text_EnergyChargeCount.gameObject.SetActive(isCount);
            if (isCount)
            {
                int count = 3600 - data.energyRemainCoolTime;

                if (countTimer != null)
                {
                    StopCoroutine(countTimer);
                }

                countTimer = SetCountDown(count);
                StartCoroutine(countTimer);
            }
        }

        public void FailedData()
        {

        }

        public void SetNickName(string nickname)
        {
            string[] temp = nickname.Split('@');
            text_Nickname.text = temp[0];
        }

        public IEnumerator SetCountDown(int remainCooltime)
        {
            int temp = remainCooltime;
            for (int i = 0; i < remainCooltime; i++)
            {
                int minute = temp / 60;
                int seconds = temp % 60;
                text_EnergyChargeCount.text = minute.ToString("00") + ":" + seconds.ToString("00");
                yield return new WaitForSeconds(1f);
                temp--;
            }

            GetEnergyValue();
        }

        public void ActiveScreen(MenuType menuType)
        {
            if (this.menuType == menuType) return;

            int targetIdx = 11;

            for (int i = 0; i < lobbyScreens.Length; i++)
            {
                ScreenTemplate screen = lobbyScreens[i];
                bool isActive = screen.menuType == menuType;
                if (isActive)
                {
                    targetIdx = i;
                    continue;
                }
                screen.ScreenSequence(isActive);
            }
            lobbyScreens[targetIdx].ScreenSequence(true);
            this.menuType = menuType;
        }

        public void AttractorAction(float target, float current)
        {
            //StartCoroutine(AttractorSequence(0, target, current));
        }

        public void ChangeValueGem(float target, float current)
        {
            StartCoroutine(ChangeValueSequenceAsync(target, current, text_Gem));
        }

        public IEnumerator ChangeValueSequenceAsync(float target, float current, TextMeshProUGUI text)
        {
            float duration = 1f; // 카운팅에 걸리는 시간 설정.
            float temp = target - current;

            if (temp >= 0)
            {
                float offset = (target - current) / duration;

                while (current < target)
                {
                    current += offset * Time.deltaTime;
                    text.text = ((int)current).ToString();
                    yield return null;
                }
            }
            else
            {
                float offset = (current - target) / duration;

                while (current > target)
                {
                    current -= offset * Time.deltaTime;
                    text.text = ((int)current).ToString();
                    yield return null;
                }
            }

            current = target;

            text.text = ((int)current).ToString();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                GetEnergyValue();
            }
        }

        private void CheckSkinPackage()
        {
            if (UserInfoManager.Instance.isSkinPackageOpened == false)
            {
                // SpecialStore store = DataManager.Instance.specialStoreDTO.specialStore.Find(a => a.type == SpecialStorePackageType.SKIN);
                // if (store != null)
                // {
                //     StoreTableData table = DataManager.Instance.StoreTableDataList.Find(a => a.store_group_id == store.skin.storeGroupId);
                //     if (table == null || table.is_activated == false)
                //     {
                //         CheckEventList();
                //         return;
                //     }

                //     UserInfoManager.Instance.isSkinPackageOpened = true;
                //     SkinPackagePopup popup = PopupManager.Instance.GetPopUp<SkinPackagePopup>("skinPackage");
                //     popup.SetUI(store, table, CheckEventList);
                //     popup.ActivePopup();
                // }
            }
            else
                CheckEventList();
        }

        private void CheckEventList()
        {
            if (CheckDate())
                PopupManager.Instance.GetPopUp<EventListPopup>("eventList").ActivePopup();
        }

        private bool CheckDate()
        {
            string prevDate = SecurePlayerPrefs.GetString("limitedPrevDate");

            if (string.IsNullOrEmpty(prevDate))
            {
                string now = DateTime.UtcNow.ToString("yyyy-MM-dd");
                SecurePlayerPrefs.SetString("limitedPrevDate", now);
                return true;
            }
            else
            {
                DateTime tempPrevDate = DateTime.Parse(prevDate);

                int compareInt = DateTime.Compare(tempPrevDate, DateTime.UtcNow);

                bool isPrev = compareInt >= 0;

                return isPrev;
            }
        }

        public void UpdateWalletInfo(MoneyType type)
        {
            _ = NetworkManager.Instance.GetUserWalletInfo((int value) =>
            {
                switch (type)
                {
                    case MoneyType.GEM:
                        UserInfoManager.Instance.gemValue = value;
                        text_Gem.text = $"{value}";
                        break;
                }

            }, "PTIK");
        }
    }
}

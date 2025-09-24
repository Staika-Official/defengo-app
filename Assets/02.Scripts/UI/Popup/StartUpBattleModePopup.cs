using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.UI
{
    public class StartUpBattleModePopup : PopupTemplate
    {
        public ButtonComponent button_Play;
        public ButtonComponent button_PlayWithFriend;
        [SerializeField] private TextMeshProUGUI text_Season;
        [SerializeField] private TextMeshProUGUI text_SeasonTime;
        [SerializeField] private TextMeshProUGUI text_League;
        [SerializeField] private TextMeshProUGUI text_Lp;
        [SerializeField] private TextMeshProUGUI text_BattleCost;
        [SerializeField] private GameObject go_Promotion;
        [SerializeField] private Slider slider_Ribbon;
        [SerializeField] private List<Transform> trans_PromotionCheckers;
        public SkeletonGraphic rankAnim;

        public override void ActivePopup()
        {
            PopUpSequence(true);
            LoadData();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });
            button_Play.onPointerUp += () =>
            {
                NetworkConnect networkConnect = Instantiate(HomeScreen.Instance.networkPrefab).GetComponent<NetworkConnect>();
                networkConnect.ConnectToLobby(false);
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").ActivePopup();
                DontDestroyOnLoad(networkConnect);
            };
            button_PlayWithFriend.onPointerUp += () =>
            {
                NetworkConnect networkConnect = Instantiate(HomeScreen.Instance.networkPrefab).GetComponent<NetworkConnect>();
                networkConnect.ConnectToLobby(true);
                PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking").ActivePopup();
                DontDestroyOnLoad(networkConnect);
            };
        }

        public async void LoadData()
        {
            button_Play.SetInterectible(UserInfoManager.Instance.gemValue >= ConfigData.BATTLE_MODE_PLAY_COST);
            text_BattleCost.text = $"x{ConfigData.BATTLE_MODE_PLAY_COST}";
            await NetworkManager.Instance.GetBattleLeaderboard((data) =>
            {
                text_SeasonTime.text = $"{data.season.fromDate} ~ {data.season.toDate}";
                text_Season.text = $"Season {data.season.id}";
            }, () =>
            {
                text_SeasonTime.text = $"";
                text_Season.text = $"No Season";
            });
            await NetworkManager.Instance.GetMyBattleLeaderboard((rank) =>
            {
                var rankConfig = DataManager.Instance.GetRankTierConfig(rank.finalRank);
                text_League.text = rankConfig.description;
                rankAnim.AnimationState.SetAnimation(1, $"{rankConfig.tierGradeType}_{GetRankName(rankConfig.tierType)}Idle", true);
                go_Promotion.SetActive(rank.isPromotion);
                slider_Ribbon.gameObject.SetActive(!rank.isPromotion);
                text_Lp.text = $"{(int)rank.lp}";
                slider_Ribbon.value = rank.lp / ConfigData.RANK_TIER_LP_CONDITION;
                if (rank.isPromotion)
                {
                    for (int i = 0; i < rank.promotionConditionNumbers.Count; i++)
                    {
                        trans_PromotionCheckers[i].GetChild(rank.promotionConditionNumbers[i]).gameObject.SetActive(true);
                    }
                }
            }, (defRank) =>
            {
                var rankConfig = DataManager.Instance.GetRankTierConfig(1);
                text_League.text = rankConfig.description;
                rankAnim.AnimationState.SetAnimation(0, $"{(int)rankConfig.tierType}_{GetRankName(rankConfig.tierType)}Idle", true);
                go_Promotion.SetActive(false);
                slider_Ribbon.gameObject.SetActive(true);
                text_Lp.text = $"{0}";
                slider_Ribbon.value = 0;
            });
        }

        string GetRankName(RankTierType tierType)
        {
            string str = (int)tierType switch
            {
                0 => "Bronze",
                1 => "Silver",
                2 => "Gold",
                3 => "Platinum",
                4 => "Diamond",
                5 => "Master",
                6 => "Legend",
                _ => "Bronze"
            };
            return str;
        }
    }
}
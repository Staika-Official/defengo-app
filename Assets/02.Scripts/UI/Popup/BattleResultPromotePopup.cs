using System.Collections;
using System.Collections.Generic;
using Framework.Game.Defense;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class BattleResultPromotePopup : PopupTemplate
    {
        public Animator animator;
        public SkeletonGraphic skeleton_Current;
        public SkeletonGraphic skeleton_Next;
        public TextMeshProUGUI text_InGameRank;
        public TextMeshProUGUI text_Rank;
        public Slider slider_Lp;
        public GameObject go_Promo;
        public GameObject go_Normal;
        public TextMeshProUGUI text_LP;
        public TextMeshProUGUI text_MaxPromo;
        public List<Transform> trans_PromotionCheckers;
        public TextMeshProUGUI text_promoProgress;
        public List<PvpBonusScoreItem> pvpBonusScoreItems;
        public List<RandomReward> randomRewards;

        public LeagueRankStatus leagueRankStatus;

        public override void ActivePopup()
        {
            PopUpSequence(true);
            Show();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
                Debug.Log("OnClick Home");
                SceneLoadManager.onCompleteLoadScene = () =>
                {
                    SoundManager.Instance.PlaySound(SoundKey.BGM_LOBBY);
                };
                if (NetworkConnect.Instance != null && NetworkConnect.Instance.runner != null)
                    NetworkConnect.Instance.runner.Shutdown();

                GameManager.Instance.objectPoolManager.AllClear();
                SceneLoadManager.Instance.SwitchingScene(2);
            });
        }

        public async void Show()
        {
            await NetworkManager.Instance.GetBattleSummary(GameManager.Instance.playId, async (response) =>
            {
                await NetworkManager.Instance.GetMyBattleLeaderboard((newRank) =>
                {
                    StartCoroutine(ShowSequence(response, newRank));
                }, (defRank) =>
                {
                    StartCoroutine(ShowSequence(response, defRank));
                });
            }, () =>
            {

            });
        }
        IEnumerator ShowSequence(GetBattleSummaryResponse summaryData, MyBattleLeaderboardInfo newRank)
        {
            Debug.Log($"{gameObject.name} Show sequence");
            var oldRank = NetworkConnect.Instance.myCurrentRank;
            var oldCofig = DataManager.Instance.GetRankTierConfig(oldRank.finalRank);
            var newCofig = DataManager.Instance.GetRankTierConfig(newRank.finalRank);

            if (oldRank.isPromotion && newRank.isPromotion)
            {
                leagueRankStatus = LeagueRankStatus.NormalPromo;
            }
            else if (oldRank.isPromotion)
            {
                if (newCofig.tierType == oldCofig.tierType)
                    leagueRankStatus = LeagueRankStatus.PromoFail;
                else
                    leagueRankStatus = LeagueRankStatus.PromoSuccess;
            }
            else if (newRank.isPromotion)
            {
                leagueRankStatus = LeagueRankStatus.EnterPromo;
            }
            else
            {
                if (newCofig.tierType == oldCofig.tierType)
                {
                    if (newCofig.tierGradeType == oldCofig.tierGradeType)
                        leagueRankStatus = LeagueRankStatus.Normal;
                    else if (newCofig.tierGradeType > oldCofig.tierGradeType)
                        leagueRankStatus = LeagueRankStatus.TierUp;
                }
                else if (newCofig.tierType > oldCofig.tierType)
                {
                    leagueRankStatus = LeagueRankStatus.PromoSuccess;
                }
                else
                {
                    leagueRankStatus = LeagueRankStatus.TierDown;
                }
            }

            Debug.Log($"League rank status {leagueRankStatus}");

            var data = NetworkConnect.Instance.GetSortedDictPlayerData();
            int ingameRank = data.Find(x => x.playerIdx == NetworkConnect.Instance.playerIdx).rank;
            text_InGameRank.text = $"{ingameRank}{GetRankSuffix(ingameRank)}";
            text_LP.text = $"{(int)newRank.lp}";
            text_Rank.text = $"{newCofig.description}";
            slider_Lp.value = newRank.lp / 100f;
            text_MaxPromo.text = $"/ {oldCofig.promotionConditionMax}";

            summaryData.bonusDetails.Add(new BattleBonusDetail()
            {
                bonusType = "BATTLE RESULT",
                value = (int)summaryData.lpDelta
            });

            for (int i = 0; i < summaryData.bonusDetails.Count; i++)
            {
                pvpBonusScoreItems[i].gameObject.SetActive(true);
                pvpBonusScoreItems[i].text_Reason.text = summaryData.bonusDetails[i].bonusType.Replace('_', ' ');
                string txtLP = (int)summaryData.bonusDetails[i].value >= 0 ? $"(+{(int)summaryData.bonusDetails[i].value})" : $"({(int)summaryData.bonusDetails[i].value})";
                pvpBonusScoreItems[i].text_Score.text = $"{txtLP}";
            }
            if (summaryData.rewards != null)
            {
                for (int i = 0; i < summaryData.rewards.Count; i++)
                {
                    randomRewards[i].gameObject.SetActive(true);
                    randomRewards[i].Initialize(summaryData.rewards[i]);
                }
            }

            skeleton_Current.AnimationState.ClearTracks();
            skeleton_Next.AnimationState.ClearTracks();
            skeleton_Current.Initialize(true);
            skeleton_Next.Initialize(true);

            if (leagueRankStatus == LeagueRankStatus.TierUp)
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}TierChange", false);
                skeleton_Current.AnimationState.AddAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true, 0);
            }
            else if (leagueRankStatus == LeagueRankStatus.PromoSuccess)
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)oldCofig.tierType}_{GetRankName(oldCofig.tierType)}To{GetRankName(newCofig.tierType)}", false);
                skeleton_Current.AnimationState.AddAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true, 0);
            }
            else if (leagueRankStatus == LeagueRankStatus.EnterPromo)
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true);
                skeleton_Next.AnimationState.AddAnimation(0, $"{(int)newCofig.tierType + 1}_{GetRankName(newCofig.tierType + 1)}Idle", true, 0);
            }
            else
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true);
            }

            switch (leagueRankStatus)
            {
                case LeagueRankStatus.Normal:
                    animator.SetTrigger("resultNormal");
                    break;
                case LeagueRankStatus.TierUp:
                    animator.SetTrigger("resultNormal");
                    animator.SetTrigger("tierUp");
                    break;
                case LeagueRankStatus.TierDown:
                    animator.SetTrigger("resultNormal");
                    animator.SetTrigger("tierDown");
                    break;
                case LeagueRankStatus.EnterPromo:
                    animator.SetTrigger("resultNormal");
                    animator.SetTrigger("openPromo");
                    break;
                case LeagueRankStatus.NormalPromo:
                    animator.SetTrigger("resultPromo");
                    break;
                case LeagueRankStatus.PromoSuccess:
                    animator.SetTrigger("resultPromo");
                    animator.SetTrigger("promoSuccess");
                    break;
                case LeagueRankStatus.PromoFail:
                    animator.SetTrigger("resultPromo");
                    animator.SetTrigger("promoFail");
                    break;
            }

            if (newRank.isPromotion)
            {
                text_promoProgress.text = $"{newRank.promotionConditionNumbers.FindAll(x => x == 1).Count}";
                for (int i = 0; i < trans_PromotionCheckers.Count; i++)
                {
                    if (i < newCofig.promotionConditionMax)
                    {
                        trans_PromotionCheckers[i].gameObject.SetActive(true);
                        if (i < newRank.promotionConditionNumbers.Count)
                        {
                            trans_PromotionCheckers[i].GetChild(newRank.promotionConditionNumbers[i]).gameObject.SetActive(true);
                            trans_PromotionCheckers[i].GetChild(1 - newRank.promotionConditionNumbers[i]).gameObject.SetActive(false);
                        }
                        else
                        {
                            trans_PromotionCheckers[i].GetChild(0).gameObject.SetActive(false);
                            trans_PromotionCheckers[i].GetChild(1).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        trans_PromotionCheckers[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (oldRank.isPromotion)
            {
                if (leagueRankStatus == LeagueRankStatus.PromoSuccess)
                    oldRank.promotionConditionNumbers.Add(1);
                else if (leagueRankStatus == LeagueRankStatus.PromoFail)
                    oldRank.promotionConditionNumbers.Add(0);
                text_promoProgress.text = $"{oldRank.promotionConditionNumbers.FindAll(x => x == 1).Count}";
                for (int i = 0; i < trans_PromotionCheckers.Count; i++)
                {
                    if (i < oldCofig.promotionConditionMax)
                    {
                        trans_PromotionCheckers[i].gameObject.SetActive(true);
                        if (i < oldRank.promotionConditionNumbers.Count)
                        {
                            trans_PromotionCheckers[i].GetChild(oldRank.promotionConditionNumbers[i]).gameObject.SetActive(true);
                            trans_PromotionCheckers[i].GetChild(1 - oldRank.promotionConditionNumbers[i]).gameObject.SetActive(false);
                        }
                        else
                        {
                            trans_PromotionCheckers[i].GetChild(0).gameObject.SetActive(false);
                            trans_PromotionCheckers[i].GetChild(1).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        trans_PromotionCheckers[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < trans_PromotionCheckers.Count; i++)
                {
                    trans_PromotionCheckers[i].gameObject.SetActive(false);
                }
                text_promoProgress.text = "";
                text_MaxPromo.text = "";
            }
            yield return null;
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
                _ => "Legend"
            };
            return str;
        }

        string GetRankSuffix(int rank)
        {
            switch (rank)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }
    }

    public enum LeagueRankStatus
    {
        Normal,
        TierUp,
        TierDown,
        EnterPromo,
        NormalPromo,
        PromoSuccess,
        PromoFail
    }
}


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
        public TextMeshProUGUI text_LP;
        public List<Transform> trans_PromotionCheckers;
        public TextMeshProUGUI text_promoProgress;
        public List<PvpBonusScoreItem> pvpBonusScoreItems;

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
                NetworkConnect.Instance.runner.Shutdown();
                SceneLoadManager.Instance.SwitchingScene(2);

                GameManager.Instance.objectPoolManager.AllClear();
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

            if (oldRank.finalRank != newRank.finalRank)
            {
                if (oldCofig.tierType == newCofig.tierType)
                {
                    if (oldCofig.tierGradeType < newCofig.tierGradeType)
                        leagueRankStatus = LeagueRankStatus.TierUp;
                    else if (oldCofig.tierGradeType > newCofig.tierGradeType)
                        leagueRankStatus = LeagueRankStatus.TierDown;
                }
                else if (oldCofig.tierType < newCofig.tierType)
                    leagueRankStatus = LeagueRankStatus.PromoSuccess;
                else
                    leagueRankStatus = LeagueRankStatus.TierDown;
            }
            else
            {
                if (!oldRank.isPromotion)
                    leagueRankStatus = LeagueRankStatus.Normal;
                else
                {
                    if (oldRank.promotionConditionNumbers.Count == 5)
                        leagueRankStatus = LeagueRankStatus.PromoFail;
                    else if (oldRank.promotionConditionNumbers.Count == 0)
                        leagueRankStatus = LeagueRankStatus.EnterPromo;
                    else
                        leagueRankStatus = LeagueRankStatus.NormalPromo;
                }
            }
            
            text_InGameRank.text = $"{NetworkConnect.Instance.networkGameManager.rank}";
            text_LP.text = $"{(int)newRank.lp}";
            text_Rank.text = $"{newCofig.description}";
            text_promoProgress.text = $"{newRank.promotionConditionNumbers.FindAll(x => x == 1).Count}";
            slider_Lp.value = newRank.lp / 100f;
            for (int i = 0; i < summaryData.bonusDetails.Count; i++)
            {
                pvpBonusScoreItems[i].gameObject.SetActive(true);
                pvpBonusScoreItems[i].text_Reason.text = summaryData.bonusDetails[i].bonusType.Replace('_', ' ');
                pvpBonusScoreItems[i].text_Score.text = $"+{summaryData.bonusDetails[i].value}";
            }
            if (leagueRankStatus == LeagueRankStatus.TierUp)
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}TierChange", false);
                skeleton_Current.AnimationState.AddAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true, 0);
            }
            else if (leagueRankStatus == LeagueRankStatus.PromoSuccess)
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)oldCofig.tierType}_{GetRankName(oldCofig.tierType)}To{GetRankName(oldCofig.tierType + 1)}", false);
                skeleton_Current.AnimationState.AddAnimation(0, $"{(int)oldCofig.tierType}_{GetRankName(oldCofig.tierType + 1)}Idle", true, 0);
            }
            else
            {
                skeleton_Current.AnimationState.SetAnimation(0, $"{(int)newCofig.tierType}_{GetRankName(newCofig.tierType)}Idle", true);
            }

            for (int i = 0; i < newRank.promotionConditionNumbers.Count; i++)
            {
                trans_PromotionCheckers[i].GetChild(newRank.promotionConditionNumbers[i]).gameObject.SetActive(true);
                trans_PromotionCheckers[i].GetChild(1 - newRank.promotionConditionNumbers[i]).gameObject.SetActive(false);
            }

            switch (leagueRankStatus)
            {
                case LeagueRankStatus.Normal:
                    animator.SetTrigger("resultNormal");
                    yield return new WaitForSeconds(0.8f);
                    break;
                case LeagueRankStatus.TierUp:
                    animator.SetTrigger("resultNormal");
                    yield return new WaitForSeconds(0.8f);
                    animator.SetTrigger("tierUp");
                    break;
                case LeagueRankStatus.TierDown:
                    animator.SetTrigger("resultNormal");
                    yield return new WaitForSeconds(0.8f);
                    animator.SetTrigger("tierDown");
                    break;
                case LeagueRankStatus.EnterPromo:
                    animator.SetTrigger("resultNormal");
                    yield return new WaitForSeconds(0.8f);
                    animator.SetTrigger("tierUp");
                    yield return new WaitForSeconds(0.35f);
                    animator.SetTrigger("enterPromo");
                    break;
                case LeagueRankStatus.NormalPromo:
                    animator.SetTrigger("resultPromo");
                    yield return new WaitForSeconds(0.3f);
                    break;
                case LeagueRankStatus.PromoSuccess:
                    animator.SetTrigger("resultPromo");
                    yield return new WaitForSeconds(0.3f);
                    animator.SetTrigger("promoSuccess");
                    break;
                case LeagueRankStatus.PromoFail:
                    animator.SetTrigger("resultPromo");
                    yield return new WaitForSeconds(0.3f);
                    animator.SetTrigger("promoFail");
                    break;
            }
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Network;
using UnityEngine.UI.Extensions;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class InGameRankItem : MonoBehaviour
    {
        public TextMeshProUGUI text_tier;
        public TextMeshProUGUI text_nickname;
        public TextMeshProUGUI text_waveCount;
        public TextMeshProUGUI text_Rank;
        public Image image_RankBadge;
        public Image image_Profile;
        public Image image_limitedProfile;
        public Image image_backGround;
        public Image image_cellBackGround;
        public Gradient2 decoGradient;
        public Gradient2 gradient;

        public GameObject defeatObject;
        public GameObject betaLockObject;
        public GameObject infoObject;

        public void Initialize(NetworkBattleData data)
        {
            defeatObject.SetActive(data.isGameOver);

            infoObject.SetActive(true);
            betaLockObject.SetActive(false);

            text_tier.text = "1";
            text_nickname.text = data.nickname;
            text_waveCount.text = "1";
            text_tier.text = data.rankTier;

            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];

            if (userProfileData.isCharacter)
            {
                image_Profile.gameObject.SetActive(true);
                image_limitedProfile.gameObject.SetActive(false);
                image_backGround.gameObject.SetActive(true);
                image_Profile.sprite = userProfileData.sprite_image;

                image_backGround.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
            }
            else
            {
                image_Profile.gameObject.SetActive(false);
                image_backGround.gameObject.SetActive(false);
                image_limitedProfile.gameObject.SetActive(true);
                image_limitedProfile.sprite = userProfileData.sprite_image;
            }

            bool isMine = data.playerIdx == NetworkConnect.Instance.playerIdx;

            SetCellColorInfo(data.rank, isMine);
        }

        public void LockInitialize()
        {
            infoObject.SetActive(false);
            betaLockObject.SetActive(true);
        }

        public void SetWaveCount(int roundId)
        {
            text_waveCount.text = $"{roundId}";
        }

        public void SetCellColorInfo(int rank, bool isMine)
        {
            bool isPodium = rank <= 3;

            Sprite rankImage = DataManager.Instance.uiPropertyData.dic_RankingBoxProperties[(RankingType)rank].sprite_Icon;

            image_RankBadge.gameObject.SetActive(isPodium);
            if (isPodium)
            {
                image_RankBadge.sprite = rankImage;
            }
            //gradient.enabled = isPodium;
            text_Rank.gameObject.SetActive(!isPodium);

            string startColorKey = isMine ? "#FFF4AD" : "#BCE6FA";
            string endColorKey = isMine ? "#FFF1DE" : "#BCE6FA";
            string cellColorKey = isMine ? "#D19941" : "#7BC5FF";
            string startDecoColorKey = isMine ? "#E1B870" : "#75BDF3";
            string endDecoColorKey = isMine ? "#F2D9A3" : "#A2D7F9";
            ColorUtility.TryParseHtmlString(startColorKey, out Color startColor);
            ColorUtility.TryParseHtmlString(endColorKey, out Color endColor);
            ColorUtility.TryParseHtmlString(startDecoColorKey, out Color startDecoColor);
            ColorUtility.TryParseHtmlString(endDecoColorKey, out Color endDecoColor);
            ColorUtility.TryParseHtmlString(cellColorKey, out Color cellColor);

            image_cellBackGround.color = cellColor;

            gradient.EffectGradient = new UnityEngine.Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new(startColor, 0), new(endColor, 1)
                }
            };

            decoGradient.EffectGradient = new UnityEngine.Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new(startDecoColor, 0), new(endDecoColor, 1)
                }
            };
        }
    }
}

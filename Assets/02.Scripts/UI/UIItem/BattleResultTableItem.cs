using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using UnityEngine.UI.Extensions;

namespace Framework.UI
{
    public class BattleResultTableItem : MonoBehaviour
    {
        public Image image_characterPortrait;
        public Image image_characterBackground;
        public Image image_rankIcon;
        public Image image_limitedProfile;

        public TextMeshProUGUI text_nickname;
        public TextMeshProUGUI text_waveCount;
        public TextMeshProUGUI text_rank;
        public CharacterCard[] characterCards;
        public GameObject betaLock;
        public GameObject info;

        public Gradient2 gradient2;

        public void Initialize(NetworkBattleData data)
        {
            info.SetActive(true);
            betaLock.SetActive(false);

            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[data.profileId];

            if (userProfileData.isCharacter)
            {
                image_characterPortrait.gameObject.SetActive(true);
                image_limitedProfile.gameObject.SetActive(false);
                image_characterBackground.gameObject.SetActive(true);
                image_characterPortrait.sprite = userProfileData.sprite_image;
                image_characterBackground.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[userProfileData.characterGrade].color_BackGround;
            }
            else
            {
                image_characterPortrait.gameObject.SetActive(false);
                image_characterBackground.gameObject.SetActive(false);
                image_limitedProfile.gameObject.SetActive(true);
                image_limitedProfile.sprite = userProfileData.sprite_image;
            }

            text_nickname.text = data.nickname;
            text_waveCount.text = $"{data.waveCount}";

            bool isPodium = data.rank <= 3 && data.rank != 0;

            if (isPodium)
            {
                RankingBoxProperty rankingBoxProperty = DataManager.Instance.uiPropertyData.dic_RankingBoxProperties[(RankingType)data.rank];
                image_rankIcon.sprite = rankingBoxProperty.sprite_Icon;
            }
            else
            {
                text_rank.text = $"{data.rank}";
            }

            for (int i = 0; i < characterCards.Length; i++)
            {
                CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)data.decList[i]];
                characterCards[i].InitializeFriendly(characterData);
            }

            bool isMine = data.playerIdx == NetworkConnect.Instance.playerIdx;
            string startColorKey = isMine ? "#FFF4B2" : "#C1E9FF";
            string endColorKey = isMine ? "#FFF2D7" : "#98D7FA";
            ColorUtility.TryParseHtmlString(startColorKey, out Color startColor);
            ColorUtility.TryParseHtmlString(endColorKey, out Color endColor);
            if (isMine)
            {
                gradient2.EffectGradient = new UnityEngine.Gradient()
                {
                    colorKeys = new GradientColorKey[]
                    {
                        new(startColor, 0), new(endColor, 1)
                    }
                };
            }
        }

        public void LockInitialize()
        {
            info.SetActive(false);
            betaLock.SetActive(true);

            ColorUtility.TryParseHtmlString("#C1E9FF", out Color startColor);
            ColorUtility.TryParseHtmlString("#98D7FA", out Color endColor);

            gradient2.EffectGradient = new UnityEngine.Gradient()
            {
                colorKeys = new GradientColorKey[]
                    {
                        new(startColor, 0), new(endColor, 1)
                    }
            };
        }
    }
}
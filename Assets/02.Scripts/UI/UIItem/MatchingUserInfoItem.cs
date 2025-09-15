using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
//using UnityEditor.Localization.Plugins.XLIFF.V20;

namespace Framework.UI
{
    public class MatchingUserInfoItem : MonoBehaviour
    {
        public TextMeshProUGUI text_nickname;
        public CharacterCard[] characterCards;
        public Image image_Profile;
        public Image image_limitedProfile;
        public Image image_backGround;
        public GameObject userInfo;
        public GameObject locked;
        public GameObject matching;

        public void UserInfoInitialize(NetworkBattleData networkBattleData)
        {
            userInfo.SetActive(true);
            locked.SetActive(false);
            matching.SetActive(false);

            UserProfileData userProfileData = DataManager.Instance.dic_userProfileData[networkBattleData.profileId];

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

            text_nickname.text = networkBattleData.nickname;

            for (int i = 0; i < characterCards.Length; i++)
            {
                int characterIdx = networkBattleData.decList[i];
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                CharacterCard characterCard = characterCards[i];
                characterCard.InitializeFriendly(data);
            }
        }
    }
}

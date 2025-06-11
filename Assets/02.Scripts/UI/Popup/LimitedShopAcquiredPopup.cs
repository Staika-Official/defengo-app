using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using Framework.Util;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class LimitedShopAcquiredPopup : PopupTemplate
    {
        public SkeletonGraphic anim_Character;

        public GameObject object_BackGround;
        
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_CharacterName;
        public TextMeshProUGUI text_CharacterGrade;
        public TextMeshProUGUI text_CharacterInfo;

        public ScrollRect scroll_CharacterInfo;

        public RectTransform rect_scroll;
        public RectTransform rect_CharacterInfo;
        
        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
            
            text_Title.text = LanguageManager.Instance.GetStringData("UI_LIMITED_SHOP_ACQUIRED");
        }

        public override void ActivePopup()
        {
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }


        public void SetLimitedShopAcquiredCharacterPopup(RewardData reward)
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)reward.IntValue];
            
            //캐릭터 애님 초기화
            anim_Character.Init(data.anim, data.characterType);

            //캐릭터 정보 셋
            text_CharacterName.text = LanguageManager.Instance.GetStringData(data.characterNameTextKey);
            text_CharacterInfo.text = LanguageManager.Instance.GetStringData(data.characterDescTextKey);

            text_CharacterGrade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_CharacterGrade.text = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");

            //캐릭터 애님 셋
            bool isOcean = data.characterType == CharacterType.SUB;
            string animKey = isOcean ? "Idle_Inventory" : "Idle";
            object_BackGround.SetActive(!isOcean);

            //스크롤 렉트 셋
            StartCoroutine(SizeCheckToScrollEnable());
            PopUpSequence(true);
        }

        public IEnumerator SizeCheckToScrollEnable()
        {
            yield return new WaitForEndOfFrame();
            if (rect_scroll.rect.height > rect_CharacterInfo.rect.height)
            {
                scroll_CharacterInfo.vertical = false;
            }
            else
            { 
                scroll_CharacterInfo.vertical = true;
            }
        }
    }
}


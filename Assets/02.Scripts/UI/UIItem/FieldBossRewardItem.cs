using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Game.Defense;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;
using UnityEngine.Events;
using Newtonsoft.Json;
using Framework.Network;

namespace Framework.UI
{
    public class FieldBossRewardItem : MonoBehaviour
    {
        public Button button_selcectReward;
        public TextMeshProUGUI text_itemName;
        public TextMeshProUGUI text_description;
        public Image image_characterIcon;
        public Image image_itemIcon;
        public Image image_buffIcon;

        public Image[] images_stars;
        public Image image_grayStar;
        public Image image_fiveStar;

        public Animation clickAnimation;

        [Header("Card Title Property")]
        public Image image_cardTitle;
        public Image image_lineColor;
        public Image image_line;
        public Image image_titleColor;
        public Image image_highlight;
        public FieldBossBuffType fieldBossBuffType;
        public UnityAction SelectAction;
        public UnityAction PopupSelectAction;

        bool selected = false;

        private void Start()
        {
            button_selcectReward.onClick.AddListener(() =>
            {
                OnClick_SelectReward(fieldBossBuffType);
            });
        }

        public void Initialize(FieldBossRewardData data, UnityAction selectCallback)
        {
            fieldBossBuffType = (FieldBossBuffType)data.buff_type;
            selected = false;
            PopupSelectAction = selectCallback;
            
            image_grayStar.gameObject.SetActive(false);
            image_fiveStar.gameObject.SetActive(false);
            image_buffIcon.gameObject.SetActive(false);
            image_itemIcon.gameObject.SetActive(false);
            image_characterIcon.gameObject.SetActive(false);

            for (int i = 0; i < images_stars.Length; i++)
            {
                images_stars[i].gameObject.SetActive(false);
            }

            bool isCharacterType = data.buff_type == 0 || data.buff_type == 10;
            selected = false;

            SetColor(isCharacterType);

            if (isCharacterType)
            {
                image_characterIcon.gameObject.SetActive(true);
                int starGrade = (int)data.value_1;
                if (starGrade > 5) starGrade = 5;

                if (starGrade == 0)
                {
                    image_grayStar.gameObject.SetActive(true);
                    for (int i = 0; i < images_stars.Length; i++)
                    {
                        images_stars[i].gameObject.SetActive(false);
                    }
                    image_fiveStar.gameObject.SetActive(false);
                }
                else if (starGrade == 5)
                {
                    image_grayStar.gameObject.SetActive(false);
                    image_fiveStar.gameObject.SetActive(true);
                }
                else
                {
                    image_grayStar.gameObject.SetActive(false);
                    image_fiveStar.gameObject.SetActive(false);
                    for (int i = 0; i < starGrade; i++)
                    {
                        if (i < images_stars.Length)
                            images_stars[i].gameObject.SetActive(true);
                    }
                }

                int characterDecIdx = UnityEngine.Random.Range(0, 5);
                DecData decData = GameManager.Instance.characterSpawner.currentDecData[characterDecIdx];
                image_characterIcon.sprite = DataManager.Instance.dic_CharacterData[decData.characterIndex].sprite_ChracterPortrait;
                text_itemName.text = LanguageManager.Instance.GetStringData("UI_Fieldboss_Reward_Character_Title");
                text_description.text = LanguageManager.Instance.GetStringData("UI_Fieldboss_Reward_Character_Title");

                SelectAction = () =>
                {
                    UIManager.Instance.summonButton.SetFixedCharacter(decData.characterIndex, starGrade);
                };
            }
            else
            {
                image_buffIcon.gameObject.SetActive(true);
                image_itemIcon.gameObject.SetActive(true);

                FieldBossRewardIcon fieldBossRewardIcon = DataManager.Instance.uiPropertyData.dic_FieldBossRewardItemIcon[fieldBossBuffType];
                image_itemIcon.sprite = fieldBossRewardIcon.sprite_itemIcon;
                image_buffIcon.sprite = fieldBossRewardIcon.sprite_buffIcon;

                text_description.text = LanguageManager.Instance.GetStringData("UI_Fieldboss_Reward_Buff_Title");
                string desc = LanguageManager.Instance.GetStringData($"UI_Fieldboss_Reward_Buff_{(int)fieldBossBuffType}");

                switch (fieldBossBuffType)
                {
                    case FieldBossBuffType.GET_GEM:
                        SelectAction = () =>
                        {
                            GameManager.Instance.ChangeGem((int)data.value_1);
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.DECREASE_RELOCATION_COST:
                        SelectAction = () =>
                        {
                            GameManager.Instance.DecreaseRelocationCost(data.value_1);
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_ALL:
                        SelectAction = () =>
                        {
                            GameManager.Instance.attackAllBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_LESS_RANGE:
                        SelectAction = () =>
                        {
                            GameManager.Instance.attackRangeLessBuffValue += data.value_2;
                            GameManager.Instance.attackRangeLessBuffTrigger = data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2 * 10, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_MORE_RANGE:
                        SelectAction = () =>
                        {
                            GameManager.Instance.attackRangeMoreBuffValue += data.value_2;
                            GameManager.Instance.attackRangeMoreBuffTrigger = data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2 * 10, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_MISSION_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.missionRewardBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.DECREASE_BOSS_COOLTIME:
                        SelectAction = () =>
                        {
                            UIManager.Instance.bossCoolTimeBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_BOSS_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.bossRewardBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_SPEED_ALL:
                        SelectAction = () =>
                        {
                            GameManager.Instance.attackSpeedBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.DECREASE_UPGRADE_COST:
                        SelectAction = () =>
                        {
                            GameManager.Instance.BuffUpgradeCost(data.value_1);
                        };
                        text_itemName.text = string.Format(desc, data.value_1 * 10, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_MONSTER_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.monsterRewardBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.DECREASE_SUMMON_PRICE:
                        SelectAction = () =>
                        {
                            GameManager.Instance.BuffSummonCost((int)data.value_1);
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2, data.value_3);
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_RANGE_ALL:
                        SelectAction = () =>
                        {
                            GameManager.Instance.attackRangeBuffValue += data.value_1;
                        };
                        text_itemName.text = string.Format(desc, data.value_1, data.value_2, data.value_3);
                        break;
                }
            }
        }

        public void SetColor(bool isCharacter)
        {
            Debug.Log("Set Color");
            int idx = isCharacter ? 1 : 0;
            TitlePropertyColor titlePropertyColor = DataManager.Instance.uiPropertyData.dic_fieldBossRewardCardTitleColor[idx];
            image_cardTitle.color = titlePropertyColor.color_titleColor;
            image_titleColor.color = titlePropertyColor.color_titleColor;
            image_lineColor.color = titlePropertyColor.color_lineColor;
            image_line.color = titlePropertyColor.color_line;
            image_highlight.color = titlePropertyColor.color_highlight;
        }

        public void OnClick_SelectReward(FieldBossBuffType fieldBossBuffType)
        {
            if (selected)
                return;
            Debug.Log("Onclick Select Reward Type : " + fieldBossBuffType);
            selected = true;
            SelectAction?.Invoke();
            PopupSelectAction?.Invoke();
            NetworkConnect.Instance.networkGameManager.Rpc_SelectdFieldBossReward(NetworkConnect.Instance.playerIdx);
            clickAnimation.Play();
        }

        public void SelectComplete()
        {
            Debug.Log("SelectComplete wave Idx :" + GameManager.Instance.waveIdx);
            UIManager.Instance.ChangeWaveValue(GameManager.Instance.waveIdx);
            UIManager.Instance.fieldBossRewardPopup.PopUpSequence(false);
        }
    }
}

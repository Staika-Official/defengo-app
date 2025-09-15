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

        private void Start()
        {
            button_selcectReward.onClick.AddListener(() =>
            {
                OnClick_SelectReward(fieldBossBuffType);
            });
        }

        public void Initialize(FieldBossRewardData data)
        {
            fieldBossBuffType = (FieldBossBuffType)data.buff_type;
            image_grayStar.gameObject.SetActive(false);
            image_fiveStar.gameObject.SetActive(false);
            image_buffIcon.gameObject.SetActive(false);
            image_itemIcon.gameObject.SetActive(false);
            image_characterIcon.gameObject.SetActive(false);

            for (int i = 0; i < images_stars.Length; i++)
            {
                images_stars[i].gameObject.SetActive(false);
            }

            bool isCharacterType = data.buff_type == 0;

            SetColor(isCharacterType);

            if (isCharacterType)
            {
                image_characterIcon.gameObject.SetActive(true);
                int starGrade = (int)data.value_1;

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
                        images_stars[i].gameObject.SetActive(true);
                    }
                }

                int characterDecIdx = UnityEngine.Random.Range(0, 5);
                DecData decData = GameManager.Instance.characterSpawner.currentDecData[characterDecIdx];
                image_characterIcon.sprite = DataManager.Instance.dic_CharacterData[decData.characterIndex].sprite_ChracterPortrait;
                text_itemName.text = "Summon Character";
                text_description.text = "Summon Character";

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

                text_itemName.text = data.title;
                text_description.text = string.Format(data.desc, data.value_1, data.value_2, data.value_3);

                switch (fieldBossBuffType)
                {
                    case FieldBossBuffType.GET_GEM:
                        SelectAction = () =>
                        {
                            GameManager.Instance.ChangeGem((int)data.value_1);
                        };
                        break;
                    case FieldBossBuffType.DECREASE_RELOCATION_COST:
                        SelectAction = () =>
                        {
                            GameManager.Instance.DecreaseRelocationCost(data.value_1);
                        };
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_ALL:
                        SelectAction = () =>
                        {
                            foreach (var character in GameManager.Instance.characterSpawner.summonedCharacters)
                            {
                                character.AttackValueBuff(true, data.value_1);
                            }
                        };
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_LESS_RANGE:
                        SelectAction = () =>
                        {
                            foreach (var character in GameManager.Instance.characterSpawner.summonedCharacters)
                            {
                                if (character.attackRange <= data.value_1)
                                    character.AttackValueBuff(true, data.value_2);
                            }
                        };
                        break;
                    case FieldBossBuffType.INCREASE_ATTACK_DAMAGE_MORE_RANGE:
                        SelectAction = () =>
                        {
                            foreach (var character in GameManager.Instance.characterSpawner.summonedCharacters)
                            {
                                if (character.attackRange >= data.value_1)
                                    character.AttackValueBuff(true, data.value_2);
                            }
                        };
                        break;
                    case FieldBossBuffType.INCREASE_MISSION_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.missionRewardBuffValue += data.value_1;
                        };
                        break;
                    case FieldBossBuffType.DECREASE_BOSS_COOLTIME:
                        SelectAction = () =>
                        {
                            UIManager.Instance.bossCoolTimeBuffValue += data.value_1;
                        };
                        break;
                    case FieldBossBuffType.INCREASE_BOSS_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.bossRewardBuffValue += data.value_1;
                        };
                        break;
                    case FieldBossBuffType.DECREASE_UPGRADE_COST:
                        SelectAction = () =>
                        {
                            GameManager.Instance.upgradeCostBuffValue += data.value_1;
                        };
                        break;
                    case FieldBossBuffType.INCREASE_MONSTER_REWARD:
                        SelectAction = () =>
                        {
                            GameManager.Instance.monsterRewardBuffValue += data.value_1;
                        };
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
            Debug.Log("Onclick Select Reward Type : " + fieldBossBuffType);

            SelectAction?.Invoke();
            clickAnimation.Play();
        }

        public void SelectComplete()
        {
            Debug.Log("SelectComplete wave Idx :" + GameManager.Instance.waveIdx);
            UIManager.Instance.ChangeWaveValue(GameManager.Instance.waveIdx);
            GameManager.Instance.monsterSpawner.WaveEnd();
            UIManager.Instance.fieldBossRewardPopup.PopUpSequence(false);
        }
    }
}

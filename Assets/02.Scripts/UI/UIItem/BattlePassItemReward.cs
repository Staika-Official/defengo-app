using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Network;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;
using System;
using Framework.Sound;
using Framework.Util;

namespace Framework.UI
{
    public class BattlePassItemReward : MonoBehaviour
    {
        public Image image_Reward;

        public ButtonComponent button_GetItem;

        public TextMeshProUGUI text_Amount;

        public TextMeshProUGUI text_Recive;

        public GameObject inActiveObject;
        public GameObject checkReward;
        public GameObject lockReward;
        public GameObject effect;

        public BattlePassRewardType rewardCategory;

        public BattlePassReward rewardData;

        public virtual void Initialize(BattlePassReward data, int userLevel)
        {
            rewardData = data;
        
            //보상 디폴트 상태
            lockReward.SetActive(false);
            inActiveObject.SetActive(true);
            checkReward.SetActive(false);
            button_GetItem.gameObject.SetActive(false);
            button_GetItem.button.interactable = true;
            effect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            text_Amount.text = $"x{data.amount}";
            text_Recive.text = LanguageManager.Instance.GetStringData("UI_Receive");
        
            rewardCategory = (BattlePassRewardType)Enum.Parse(typeof(BattlePassRewardType), data.category);
        
            EffectActivate();
        
            string rewardName = "";
            //보상이미지 타입 관리해서 보여줘야함
            if(data.rewardType.Contains("GEM"))
            {
                if (data.amount < 100)
                    rewardName = $"{data.rewardType}_SMALL";
                else
                    rewardName = $"{data.rewardType}_LOW";
            }
            else
            {
                rewardName = data.rewardType;
            }
        
            image_Reward.sprite = DataManager.Instance.uiPropertyData.dic_BattlePassItemIcon[rewardName];
        
            button_GetItem.onPointerUp = () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_GetItem();
                button_GetItem.onPointerUp = null;
            };
        }

        public void EffectActivate()
        {
            BattlePassDisplayType type = (BattlePassDisplayType)Enum.Parse(typeof(BattlePassDisplayType), rewardData.displayType);
            if (type == BattlePassDisplayType.HIGHLIGHT)
            {
                effect.SetActive(true);
            }
            else
            {
                effect.SetActive(false);
            }
        }


        public void OnClick_GetItem()
        {
            button_GetItem.gameObject.SetActive(false);
            BattlePassPopup popup = PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass");
            popup.ButtonInteractable(false);

            //test용 주석
            //string tempdata = "";
            //popup.GetAsset(tempdata, BattlePassRewardType.GEM, transform);

            BattlePassGetReward data = new()
            {
                battlePassId = UserInfoManager.Instance.userPassId,
                rewardId = rewardData.rewardId
            };


            _ = NetworkManager.Instance.RequsetPassReward(data, SuccessGetItem, FailedGetItem);
        }

        public void SuccessGetItem(string data)
        {
            BattlePassPopup popup = PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass");

            checkReward.SetActive(true);

            switch (rewardCategory)
            {
                case BattlePassRewardType.RANDOM_BOX:
                    popup.GetRandomBox(data, rewardData.rewardType);
                    break;
                case BattlePassRewardType.HATCHING_ORB:
                    popup.GetAsset(data, rewardCategory, transform);
                    break;
                case BattlePassRewardType.GEM:
                    popup.GetAsset(data, rewardCategory, transform);
                    break;
                case BattlePassRewardType.ENERGY:
                    break;
                case BattlePassRewardType.TIK:

                    break;
                case BattlePassRewardType.STIK:

                    break;
                case BattlePassRewardType.GO:

                    break;
                case BattlePassRewardType.STIK_RANDOMBOX:

                    break;
                case BattlePassRewardType.PTIK_RANDOMBOX:

                    break;
                case BattlePassRewardType.TIK_RANDOMBOX:

                    break;
                case BattlePassRewardType.PTIK:
                    break;
            }
        }

        public void FailedGetItem()
        {
            BattlePassPopup PassPopup = PopupManager.Instance.GetPopUp<BattlePassPopup>("battlePass");
            PassPopup.ButtonInteractable(true);

            button_GetItem.gameObject.SetActive(true);

            button_GetItem.onPointerUp = () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_GetItem();
                button_GetItem.onPointerUp = null;
            };

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Pass_Reward");
        }

    }
}
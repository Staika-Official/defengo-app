using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using TMPro;
using UnityEngine;

namespace Framework.UI
{
    public class QuestRewardPopup : PopupTemplate
    {
        public Animator anim;
        public RandomReward[] randomRewards;

        public TextMeshProUGUI text_Title;

        public IEnumerator randomResultOpenAction;

        private List<RandomReward> list_RandomReward = new();
        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
        }

        public void CompleteMissionReward(List<UserItem> datas)
        {
            anim.Rebind();
            ActivePopup();

            button_Close.onClick.RemoveAllListeners();
            text_Title.text = LanguageManager.Instance.GetStringData("UI_Quest_Reward_Title");

            if (null != randomResultOpenAction)
            {
                StopCoroutine(randomResultOpenAction);
                randomResultOpenAction = null;
            }

            for (int i = 0; i < randomRewards.Length; ++i)
            {
                randomRewards[i].gameObject.SetActive(false);
            }

            randomResultOpenAction = SetRandomResultSequence(datas);
            StartCoroutine(randomResultOpenAction);
        }

        public IEnumerator SetRandomResultSequence(List<UserItem> data)
        {
            while (true)
            {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "QuestReward_Idle") break;
                yield return null;
            }

            list_RandomReward.Clear();

            for (int i = 0; i < data.Count; ++i)
            {
                randomRewards[i].gameObject.SetActive(true);
                randomRewards[i].Initialize(data[i]);

                list_RandomReward.Add(randomRewards[i]);
            }

            button_Close.onClick.AddListener(GetReward);
        }

        public void GetReward()
        {
            for (int i = 0; i < list_RandomReward.Count; ++i)
            {
                GetAsset(list_RandomReward[i]);
            }

            InActivePopup();
        }

        public void GetAsset(RandomReward reward)
        {
            RandomRewardType rewardType;

            if (reward.userItemData.item.Contains("HATCHING_ORB"))
            {
                rewardType = RandomRewardType.HATCHING_ORB;
            }
            else
            {
                rewardType = (RandomRewardType)Enum.Parse(typeof(RandomRewardType), reward.userItemData.item);
            }

            switch (rewardType)
            {
                case RandomRewardType.ENERGY:
                    AttractorManager.Instance.SetAttractor("achievementEnergy", reward.image_RewardIcon.transform, LobbyManager.Instance.GetEnergyValue);
                    break;
                case RandomRewardType.GEM:
                    AttractorManager.Instance.SetAttractor("achievementGem", reward.image_RewardIcon.transform, GetWallet);
                    break;
                case RandomRewardType.HATCHING_ORB:
                    AttractorManager.Instance.SetHatchingAttractor("achievementHatching", reward.userItemData.item, reward.image_RewardIcon.transform, GetHatchingStone);
                    break;
            }
        }

        public void GetWallet()
        {
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        public void GetHatchingStone()
        {
            _ = NetworkManager.Instance.GetHatchingStone();
        }

        public void SuccessWallet(int gemValue)
        {
            int preGemValue = UserInfoManager.Instance.gemValue;
            UserInfoManager.Instance.gemValue = gemValue;

            //로비 젬 표시 변경
            LobbyManager.Instance.text_Gem.text = $"{gemValue}";

            //월정액 패키치 젬 표시 변경
            StartCoroutine(LobbyManager.Instance.ChangeValueSequenceAsync(gemValue, preGemValue, LobbyManager.Instance.text_Gem));
        }

        public void SetRewardSkinPackage(List<RewardData> reward_list)
        {
            anim.Rebind();
            ActivePopup();

            button_Close.onClick.RemoveAllListeners();
            text_Title.text = LanguageManager.Instance.GetStringData("UI_Unlock_Skin_Package");

            if (null != randomResultOpenAction)
            {
                StopCoroutine(randomResultOpenAction);
                randomResultOpenAction = null;
            }

            for (int i = 0; i < randomRewards.Length; ++i)
            {
                randomRewards[i].gameObject.SetActive(false);
            }

            randomResultOpenAction = SetSkinPackageResultSequence(reward_list);
            StartCoroutine(randomResultOpenAction);
        }

        private IEnumerator SetSkinPackageResultSequence(List<RewardData> reward_list)
        {
            while (true)
            {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "QuestReward_Idle") break;
                yield return null;
            }

            list_RandomReward.Clear();
            reward_list = reward_list.OrderBy(a => a.Type).ToList();
            for (int i = 0; i < reward_list.Count; ++i)
            {
                randomRewards[i].gameObject.SetActive(true);
                switch (reward_list[i].Type)
                {
                    case RewardType.CHARACTER:
                        CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)reward_list[i].IntValue];
                        randomRewards[i].Initialize(characterData, reward_list[i].Amount);

                        //캐릭터 레벨이 0 이라면 새로운 캐릭터
                        if (characterData.characterClassLevel == 0)
                        {
                            characterData.characterClassLevel = (int)characterData.characterGrade + 1;
                            characterData.characterQuantity = reward_list[i].Amount;
                            characterData.isNew = true;
                            characterData.isNewInventory = true;

                        }
                        else
                        {
                            characterData.characterQuantity += reward_list[i].Amount;
                            characterData.isNewInventory = false;
                            characterData.isNew = false;
                        }
                        break;

                    case RewardType.CHARACTER_SKIN:
                        randomRewards[i].InitializeSkin(reward_list[i].IntValue);
                        DataManager.Instance.userCharacters.skins.Add(reward_list[i].IntValue);
                        DataManager.Instance.userCharacters.equippedSkins.Add(reward_list[i].IntValue);
                        int characterid = DataManager.Instance.SkinTableDataList.Find(a => a.id == reward_list[i].IntValue).character_id;
                        DataManager.Instance.dic_CharacterData[(CharacterIndex)characterid].SetChangeSkin(reward_list[i].IntValue);
                        break;

                    case RewardType.PROFILE:
                        randomRewards[i].InitializeProfile(reward_list[i].IntValue);
                        UserProfileData data = DataManager.Instance.dic_userProfileData[reward_list[i].IntValue];
                        UserInfoManager.Instance.userState.equippedProfileId = reward_list[i].IntValue;
                        LobbyManager.Instance.SetProfileImage(data);
                        data.isOwned = true;
                        break;

                    case RewardType.GEM:
                        UserItem userItem = new UserItem();
                        userItem.item = "GEM";
                        userItem.quantity = reward_list[i].Amount;
                        randomRewards[i].Initialize(userItem);
                        list_RandomReward.Add(randomRewards[i]);
                        break;
                }
            }

            button_Close.onClick.AddListener(GetReward);
        }

    }
}

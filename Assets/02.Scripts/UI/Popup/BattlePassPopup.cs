using System;
using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Framework.UI
{
    public class BattlePassPopup : PopupTemplate
    {
        //패스 제목
        public TextMeshProUGUI text_Title;
        //패스 서브제목
        public TextMeshProUGUI text_SubTitle;
        //패스 활성화 버튼 텍스트
        public TextMeshProUGUI text_PremiumBuy;
        //패스 남은 기한
        public TextMeshProUGUI text_PassToDate;
        //무료 패스 텍스트
        public TextMeshProUGUI text_FreePassName;
        //프리미엄 패스 텍스트
        public TextMeshProUGUI text_PremiumPassName;

        //패스 백그라운드
        //public Image image_BackGround;
        public RawImage image_BackGround;
        //패스 아이콘
        //public Image image_PassIcon;
        public RawImage image_PassIcon;

        //프리미엄 구매 버튼
        public Button button_PassPrimium;

        //현재 레벨
        public TextMeshProUGUI text_Level;
        //현재 경험치
        public TextMeshProUGUI text_Exp;

        //경험치 게이지
        public Image image_ExpGauge;
        public Image image_ExpBackGroundGauge;
        //레벨업 게이지
        public Image image_LevelGauge;
        public Image image_LevelBackGroundGauge;

        //레벨업 버튼
        public Button button_LevelUp;

        //레벨업 텍스트 (젬 표시)
        public TextMeshProUGUI text_LevelUp;

        //노말 아이템에 보상이없으면 빈 표시를 해주기위한 오브젝트
        public GameObject nullObject;

        public int maxLevel;
        public readonly int maxCrystal = 100;

        public List<BattlePassNormalReward> normalRewards = new();
        public List<BattlePassPremiumReward> premiumRewards = new();
        public List<BattlePassLevel> levels = new();
        public List<GameObject> lines = new();
        public List<GameObject> nullObjects = new();

        public Queue<BattlePassNormalReward> queueNormalRewards = new();
        public Queue<BattlePassPremiumReward> queuePremiumRewards = new();
        public Queue<BattlePassLevel> queueLevels = new();
        public Queue<GameObject> queueLines = new();
        public Queue<GameObject> queueNullObject = new();

        public RectTransform scrollRect;

        public RectTransform normalViewPortRect;
        public RectTransform premiumViewPortRect;
        public RectTransform levelViewPortRect;
        public RectTransform lineViewPortRect;
        public RectTransform viewPort;

        public RectTransform parentViewPort;
        public RectTransform upperRect;
        public RectTransform frameRect;
        public RectTransform backGroundRect;
        public RectTransform attractorTarget_Gem;
        public RectTransform attractorTarget_HatchingStone;

        public BattlePassUserInfoDetailData detailData;

        public override void ActivePopup()
        {
            SetBattlePassInit();
        }

        public override void InActivePopup()
        {
            //배틀패스 레드닷 처리
            //SetBattlePassNotice();

            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_PassPrimium.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopupManager.Instance.GetPopUp<BattlePassPremiumPopup>("battlePassPremium").PremiumBuyPopupActive(detailData);
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            button_LevelUp.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopupManager.Instance.GetPopUp<BattlePassCrystalBuyPopup>("battlePassCrystalBuy").SetPopup(detailData.userLevel, maxLevel);
            });

        }

        public void SetBattlePassNotice()
        {
            _ = NetworkManager.Instance.GetUserPassDetail(SetPassNotice, Failed);
        }

        public void SetBattlePassInit()
        {
            _ = NetworkManager.Instance.GetUserPassDetail(SetBattlePassContent, Failed);
        }

        public async void PassImageSetUp(int passId, RawImage image, string imageType, string imageUrl)
        {
#if UNITY_IOS && !UNITY_EDITOR
            string dataPath = $"{Application.persistentDataPath}";
#elif UNITY_ANDROID && !UNITY_EDITOR
            string dataPath = Application.persistentDataPath;
#elif UNITY_EDITOR
            string dataPath = Application.dataPath;
#endif




#if UNITY_ANDROID || UNITY_EDITOR
            if (File.Exists(dataPath + passId + imageType + ".png"))
            {
                byte[] bgfile = File.ReadAllBytes(dataPath + passId + imageType + ".png");
                Texture2D texture2D = new(1, 1, TextureFormat.ASTC_6x6, false);
                texture2D.LoadImage(bgfile);

                image.texture = texture2D;
            }
            else
            {
                Texture2D texture2D = await NetworkManager.Instance.GetImageFromS3(imageUrl);

                byte[] bytes = texture2D.EncodeToPNG();
                //string fileName = imageUrl.Replace('/', ' ');

                File.WriteAllBytes(dataPath + passId + imageType + ".png", bytes);
                image.texture = texture2D;
            }
#elif UNITY_IOS && !UNITY_EDITOR
                Texture2D texture2D = await NetworkManager.Instance.GetImageFromS3(imageUrl);
                image.texture = texture2D;
#endif
        }

        public void SetBattlePassContent(BattlePassUserInfoDetailData data)
        {
            if (DateTime.UtcNow.Date > DateTime.Parse(data.toDate))
            {
                FailedPeriod();
                return;
            }

            detailData = data;

            //todo 임시용
            bool isKorRegion = LanguageManager.Instance.language == Language.KO;
            string passTitle = isKorRegion ? data.title : data.titleEn;
            string passSubTitle = isKorRegion ? data.subTitle : data.subTitleEn;
            text_Title.text = passTitle;
            text_SubTitle.text = passSubTitle;

            //todo 이전버전타이틀
            //text_Title.text = LanguageManager.Instance.GetStringData($"UI_BattlePass_Season_Name_{data.battlePassId}");
            //text_SubTitle.text = string.Format(LanguageManager.Instance.GetStringData("UI_BattlePass_Season"), data.battlePassId);

            text_PremiumBuy.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Active");

            text_FreePassName.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Free");
            text_PremiumPassName.text = LanguageManager.Instance.GetStringData("UI_BattlePass_Premium");

            string passBackgroundUrl = "PassBackGround";
            string passIconUrl = "PassIcon";

            string bgUrl = ImageTextExtraction(passBackgroundUrl, data.bgImageUrl);
            string iconUrl = ImageTextExtraction(passIconUrl, data.passImageUrl);

            PassImageSetUp(data.battlePassId, image_BackGround, bgUrl, data.bgImageUrl);
            PassImageSetUp(data.battlePassId, image_PassIcon, iconUrl, data.passImageUrl);

            //test코드
            //data.userLevel.userExp = 30;
            //data.userLevel.level = 5;

            PassMaxLevelCheck(data);

            DateTime toDate = DateTime.Parse(data.toDate);
            text_PassToDate.text = string.Format(LanguageManager.Instance.GetStringData("UI_BattlePass_Colse_Time"), toDate.ToString("yyyy.MM.dd"));

            //프리미엄 패스 구매 활성화 비활성화
            button_PassPrimium.gameObject.SetActive(!data.userLevel.passActivated);

            //유저 레벨 텍스트 설정
            text_Level.text = data.userLevel.level.ToString();
            //유저 경험치 텍스트 설정
            text_Exp.text = $"{data.userLevel.userExp}/{data.userLevel.requiredExp}";

            //경험치 게이지 fill설정
            image_ExpGauge.fillAmount = (float)data.userLevel.userExp / data.userLevel.requiredExp;
            image_ExpBackGroundGauge.fillAmount = (float)data.userLevel.userExp / data.userLevel.requiredExp;


            //레벨업 게이지 fill설정
            image_LevelBackGroundGauge.rectTransform.sizeDelta = new Vector2(image_LevelBackGroundGauge.rectTransform.sizeDelta.x, (data.userRewards.premium.Length + 1) * 323.3f);
            image_LevelGauge.fillAmount = (float)(data.userLevel.level + 0.5f) / (maxLevel + 1);

            if (data.userLevel.level == maxLevel)
            {
                image_LevelGauge.fillAmount = 1f;
            }

            text_LevelUp.text = $"<sprite=2>{maxCrystal}";

            button_LevelUp.gameObject.SetActive(false);

            SetPassItem(data);

            StartCoroutine(LateSetLevelUp());

            PopUpSequence(true);
        }

        public string ImageTextExtraction(string imageType, string url)
        {
            string tempUrl = imageType;

            string[] arrayUrl = url.Split('/');

            if (arrayUrl[^1].Contains('.'))
            {
                tempUrl += arrayUrl[^1].Split('.')[0];
            }

            return tempUrl;
        }

        //레드닷 체크를위한 데이터를 얻어오기작업
        public void SetPassNotice(BattlePassUserInfoDetailData data)
        {
            detailData = data;
            bool isRewardReceiveCheck = PassRewardCheck();
            HomeScreen.Instance.passNoticeObject.SetActive(isRewardReceiveCheck);
        }

        public bool PassRewardCheck()
        {
            for (int i = 0; i < detailData.userRewards.normal.Length; ++i)
            {
                if (detailData.userRewards.normal[i].isAvailable)
                {
                    return true;
                }
            }

            for (int i = 0; i < detailData.userRewards.premium.Length; ++i)
            {
                if (detailData.userRewards.premium[i].isAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        public void PassMaxLevelCheck(BattlePassUserInfoDetailData data)
        {
            int premiumPassLevel = data.userRewards.premium[data.userRewards.premium.Length - 1].passLevel;
            int normalPassLevel = data.userRewards.normal[data.userRewards.normal.Length - 1].passLevel;

            maxLevel = premiumPassLevel >= normalPassLevel ? premiumPassLevel : normalPassLevel;
        }

        public IEnumerator LateSetLevelUp()
        {
            yield return new WaitForEndOfFrame();

            if (detailData.userLevel.level < maxLevel)
            {
                button_LevelUp.gameObject.SetActive(true);
                button_LevelUp.transform.position = lines[detailData.userLevel.level].transform.position;
            }
        }

        public void Failed()
        {
            QueueClear();
        }

        public void QueueClear()
        {
            queueNormalRewards.Clear();
            queuePremiumRewards.Clear();
            queueLevels.Clear();
            queueLines.Clear();
            queueNullObject.Clear();
        }

        public void SetViewPort()
        {
            float sizeRatio = upperRect.sizeDelta.y + frameRect.sizeDelta.y;
            float totalRatio = backGroundRect.rect.height - sizeRatio;
            parentViewPort.sizeDelta = new Vector2(parentViewPort.sizeDelta.x, totalRatio);
        }

        public void SetPassItem(BattlePassUserInfoDetailData data)
        {
            int count = data.userRewards.premium.Length;
            //int count = 30;
            scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, data.userRewards.premium.Length * 328f);
            //scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, count * 328f);

            RefreshReward();

            SetViewPort();

            int curIndex = 0;

            for (int i = 0; i < count; ++i)
            {
                if ((i + 1) == data.userRewards.normal[curIndex].passLevel)
                {
                    BattlePassNormalReward obj = GetPassNormalReward();
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(normalViewPortRect, false);
                    obj.transform.localScale = Vector2.one;
                    obj.transform.SetSiblingIndex(i);

                    obj.Initialize(data.userRewards.normal[curIndex], data.userLevel.level);
                    ++curIndex;
                }
                else
                {
                    GameObject obj = GetNullObject();
                    obj.SetActive(true);
                    obj.transform.SetParent(normalViewPortRect, false);
                    obj.transform.localScale = Vector2.one;
                    obj.transform.SetSiblingIndex(i);

                }
            }

            curIndex = 0;

            for (int i = 0; i < count; ++i)
            {
                if ((i + 1) == data.userRewards.premium[curIndex].passLevel)
                {
                    BattlePassPremiumReward obj = GetPassPremiumReward();
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(premiumViewPortRect, false);
                    obj.transform.localScale = Vector2.one;
                    obj.transform.SetSiblingIndex(i);

                    obj.PremiumLock(data.userLevel.passActivated);
                    obj.Initialize(data.userRewards.premium[curIndex], data.userLevel.level);
                    ++curIndex;
                }
                else
                {
                    GameObject obj = GetNullObject();
                    obj.SetActive(true);
                    obj.transform.SetParent(premiumViewPortRect, false);
                    obj.transform.localScale = Vector2.one;
                    obj.transform.SetSiblingIndex(i);

                }

            }

            for (int i = 0; i < count; ++i)
            {
                GameObject obj = GetLine();
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(lineViewPortRect, false);
                obj.transform.localScale = Vector2.one;
            }


            for (int i = 0; i < count; ++i)
            {
                BattlePassLevel obj = GetPassLevel();
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(levelViewPortRect, false);
                obj.transform.localScale = Vector2.one;

                BattlePassLevelType levelType = BattlePassLevelType.CLEAR_LEVEL;

                //순서 보장
                if (data.userLevel.level < i)
                {
                    levelType = BattlePassLevelType.OTHER_LEVEL;
                }
                else if (data.userLevel.level == i)
                {
                    levelType = BattlePassLevelType.NEXT_LEVEL;
                }

                obj.Initialize(levelType, i + 1);
            }

            QueueClear();
        }

        public void RefreshReward()
        {
            for (int i = 0; i < normalRewards.Count; ++i)
            {
                normalRewards[i].gameObject.SetActive(false);
                queueNormalRewards.Enqueue(normalRewards[i]);
            }

            for (int i = 0; i < premiumRewards.Count; ++i)
            {
                premiumRewards[i].gameObject.SetActive(false);
                queuePremiumRewards.Enqueue(premiumRewards[i]);
            }

            for (int i = 0; i < levels.Count; ++i)
            {
                levels[i].gameObject.SetActive(false);
                queueLevels.Enqueue(levels[i]);
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].gameObject.SetActive(false);
                queueLines.Enqueue(lines[i]);
            }

            for (int i = 0; i < nullObjects.Count; ++i)
            {
                nullObjects[i].gameObject.SetActive(false);
                queueNullObject.Enqueue(nullObjects[i]);
            }

        }

        public BattlePassNormalReward GetPassNormalReward()
        {
            if (queueNormalRewards.Count == 0)
            {
                BattlePassNormalReward item = Instantiate(normalRewards[0].gameObject).GetComponent<BattlePassNormalReward>();
                normalRewards.Add(item);
                return item;
            }
            else
            {
                return queueNormalRewards.Dequeue();
            }
        }


        public BattlePassPremiumReward GetPassPremiumReward()
        {
            if (queuePremiumRewards.Count == 0)
            {
                BattlePassPremiumReward item = Instantiate(premiumRewards[0].gameObject).GetComponent<BattlePassPremiumReward>();
                premiumRewards.Add(item);
                return item;
            }
            else
            {
                return queuePremiumRewards.Dequeue();
            }
        }

        public BattlePassLevel GetPassLevel()
        {
            if (queueLevels.Count == 0)
            {
                BattlePassLevel level = Instantiate(levels[0].gameObject).GetComponent<BattlePassLevel>();
                levels.Add(level);
                return level;
            }
            else
            {
                return queueLevels.Dequeue();
            }
        }

        public GameObject GetLine()
        {
            if (queueLines.Count == 0)
            {
                GameObject obj = Instantiate(lines[0].gameObject);
                lines.Add(obj);
                return obj;
            }
            else
            {
                return queueLines.Dequeue();
            }
        }

        public GameObject GetNullObject()
        {
            if (queueNullObject.Count == 0)
            {
                GameObject obj = Instantiate(nullObject);
                nullObjects.Add(obj);

                return obj;
            }
            else
            {
                return queueNullObject.Dequeue();
            }
        }

        public void GetRandomBox(string data, string value)
        {
            RandomBoxIndex randomBoxInbox = JsonUtility.FromJson<RandomBoxIndex>(data);

            RandomBoxPopup popup = PopupManager.Instance.GetPopUp<RandomBoxPopup>("randomBoxOpen");
            popup.ActivePopup();
            popup.RandomBoxOpen(randomBoxInbox, value);
            ButtonInteractable(true);
        }

        public void GetAsset(string data, BattlePassRewardType type, Transform transform)
        {
            switch (type)
            {
                case BattlePassRewardType.HATCHING_ORB:
                    BattlePassRewardResponseData responseData = JsonUtility.FromJson<BattlePassRewardResponseData>(data);
                    AttractorManager.Instance.SetHatchingAttractor_Late("passHatching", responseData.rewardType, transform, attractorTarget_HatchingStone, GetStone);
                    break;
                case BattlePassRewardType.GEM:
                    AttractorManager.Instance.SetAttractor_Late("passGem", transform, GetWallet, attractorTarget_Gem);
                    break;
            }
        }

        public void ButtonInteractable(bool isActive)
        {
            for (int i = 0; i < normalRewards.Count; ++i)
            {
                normalRewards[i].button_GetItem.SetInterectible(isActive);
            }

            for (int i = 0; i < premiumRewards.Count; ++i)
            {
                premiumRewards[i].button_GetItem.SetInterectible(isActive);
            }
        }

        public void GetStone()
        {
            ButtonInteractable(true);
            _ = NetworkManager.Instance.GetHatchingStone();
        }

        public void GetWallet()
        {
            ButtonInteractable(true);
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        public void SuccessWallet(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }

        public void FailedPeriod()
        {
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Period_ends");
        }

    }
}


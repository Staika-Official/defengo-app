using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using Framework.GameData.Defense;
using Framework.Network;
using UniRx;
using Framework.Sound;
using System.Linq;

namespace Framework.UI
{
    public class RandomBoxPopup : PopupTemplate
    {
        [SerializeField] private Animator animator;
        [SerializeField] private InventoryItem inventoryCharacterItem;
        [SerializeField] private InventoryHatchingItem inventoryHatchItem;

        [SerializeField] private Image image_Bottom;
        [SerializeField] private Image image_Upper;
        [SerializeField] private Image image_UpperOpen;

        [SerializeField] private TextMeshProUGUI text_Grade;
        [SerializeField] private TextMeshProUGUI text_ItemName;
        [SerializeField] private TextMeshProUGUI text_Amount;
        [SerializeField] private TextMeshProUGUI text_GemValue;
        [SerializeField] private TextMeshProUGUI text_OnceGemPrice;
        [SerializeField] private TextMeshProUGUI text_ManyGemPrice;

        [SerializeField] private GameObject object_GroupButtons;
        [SerializeField] private GameObject object_GemValue;

        [SerializeField] private ButtonComponent button_Skip;
        [SerializeField] private ButtonComponent button_Once;
        [SerializeField] private ButtonComponent button_Many;

        [SerializeField] private IEnumerator openBoxSequence;

        [SerializeField] private GameObject randomResultObejct;
        [SerializeField] private GameObject parentResultObject;

        [SerializeField] private List<RandomResult> listRandomResult = new();
        [SerializeField] private Queue<RandomResult> queueRandomResult = new();

        [SerializeField] private List<ResRewardValueData> resultIndexList;

        [SerializeField] private GameObject fur;
        [SerializeField] private GameObject epicFx;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private Image[] radials;
        [SerializeField] private Image gradient;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float interval;

        private bool isNewCharacter;
        private bool isItem;
        private string itemKey;
        public readonly int scrollLockCheckCount = 13;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            LobbyNavigator.Instance.Notification(MenuType.INVENTORY, isNewCharacter);
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => OnClick_Close());
            button_Skip.onPointerUp = () => OnClick_Skip();
            button_Once.onPointerUp = () => OnClick_ReBuy(true);
            button_Many.onPointerUp = () => OnClick_ReBuy(false);
        }

        public void RandomBoxOpen(List<ResRewardValueData> responseData, string boxCode, RewardType type)
        {
            if (null != openBoxSequence)
            {
                StopCoroutine(openBoxSequence);
                openBoxSequence = null;
            }

            bool isFurActive = boxCode.Contains("BOX_LIMITED");
            fur.SetActive(isFurActive);

            //isItem : 캐릭터인지 아이템(부화석)인지 확인해야함
            //이 bool 변수로 실행하는 함수가 달라짐
            //isItem 이 변수로 함수를 구분짓는게 맞을지..
            isItem = type == RewardType.ITEM;

            string code = "";

            if (isFurActive)
                code = "BOX_LIMITED";
            else
                code = boxCode;

            RandomBoxSprite randomBoxSprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[code];

            image_Bottom.sprite = randomBoxSprite.sprite_Bottom;
            image_Upper.sprite = randomBoxSprite.sprite_Top;
            image_UpperOpen.sprite = randomBoxSprite.sprite_Top_Open;

            SetGroupButtons(false);

            SetItem(isItem);

            resultIndexList = responseData;

            DynamicResultCardCreate(resultIndexList);

            CharacterNewCheck(resultIndexList);

            openBoxSequence = RandomBoxOpenSequance(resultIndexList);
            StartCoroutine(openBoxSequence);
        }


        public void RandomBoxOpen(RandomBoxIndex responseData, string code, bool isSkip = false)
        {
            if (null != openBoxSequence)
            {
                StopCoroutine(openBoxSequence);
                openBoxSequence = null;
            }

            bool isFurActive = code == "BOX_LIMITED";

            fur.SetActive(isFurActive);

            //isItem : 캐릭터인지 아이템(부화석)인지 확인해야함
            //이 bool 변수로 실행하는 함수가 달라짐
            //isItem 이 변수로 함수를 구분짓는게 맞을지..
            isItem = responseData.type.Contains("ITEM");

            RandomBoxSprite randomBoxSprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[code];

            image_Bottom.sprite = randomBoxSprite.sprite_Bottom;
            image_Upper.sprite = randomBoxSprite.sprite_Top;
            image_UpperOpen.sprite = randomBoxSprite.sprite_Top_Open;

            SetGroupButtons(false);

            //인벤토리 아이템을 액티브 셋해주기 위함
            SetItem(isItem);

            resultIndexList = responseData.rewardList.ToList();

            //결과 카드가 부족하면 새롭게 생성시켜주는 함수
            DynamicResultCardCreate(resultIndexList);

            //캐릭터 new아이콘을 보여줘야함
            //RandomBoxOpenSequance 함수안에서 해줄 수 있지만
            //예외상황 으로 Skip버튼을 누르면 RandomBoxOpenSequance 실행을 중단할 경우 isNew셋팅이 안될 수 있기에 미리 셋 해줌
            CharacterNewCheck(resultIndexList);

            if (isSkip)
            {
                animator.Rebind();
                animator.SetTrigger("Skip");
                SetResult();
            }
            else
            {
                openBoxSequence = RandomBoxOpenSequance(resultIndexList);
                StartCoroutine(openBoxSequence);
            }
        }

        public void RandomBoxOpen(ResRewardTemplate responseData, string code, bool isSkip = false, string itemKey = null)
        {
            if (null != openBoxSequence)
            {
                StopCoroutine(openBoxSequence);
                openBoxSequence = null;
            }

            bool isFurActive = code == "BOX_LIMITED";
            
            this.itemKey = itemKey;

            fur.SetActive(isFurActive);

            //isItem : 캐릭터인지 아이템(부화석)인지 확인해야함
            //이 bool 변수로 실행하는 함수가 달라짐
            //isItem 이 변수로 함수를 구분짓는게 맞을지..
            isItem = responseData.RewardDataList.FindIndex(a => a.Type == RewardType.ITEM) >= 0;


            RandomBoxSprite randomBoxSprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[code];

            image_Bottom.sprite = randomBoxSprite.sprite_Bottom;
            image_Upper.sprite = randomBoxSprite.sprite_Top;
            image_UpperOpen.sprite = randomBoxSprite.sprite_Top_Open;

            SetGroupButtons(false);

            //인벤토리 아이템을 액티브 셋해주기 위함
            SetItem(isItem);
            resultIndexList = new List<ResRewardValueData>();
            foreach (RewardData data in responseData.RewardDataList)
            {
                resultIndexList.Add(new ResRewardValueData()
                {
                    value = data.StringValue,
                    amount = data.Amount,
                });
            }

            //결과 카드가 부족하면 새롭게 생성시켜주는 함수
            DynamicResultCardCreate(resultIndexList);

            //캐릭터 new아이콘을 보여줘야함
            //RandomBoxOpenSequance 함수안에서 해줄 수 있지만
            //예외상황 으로 Skip버튼을 누르면 RandomBoxOpenSequance 실행을 중단할 경우 isNew셋팅이 안될 수 있기에 미리 셋 해줌
            CharacterNewCheck(resultIndexList);

            if (isSkip)
            {
                animator.Rebind();
                animator.SetTrigger("Skip");
                SetResult();
            }
            else
            {
                openBoxSequence = RandomBoxOpenSequance(resultIndexList);
                StartCoroutine(openBoxSequence);
            }
        }

        public void DynamicResultCardCreate<T>(List<T> randomboxList)
        {
            RefreshResult();

            //결과 카드가 13개 미만이면 스크롤 잠구기
            if (randomboxList.Count < scrollLockCheckCount)
                scrollRect.vertical = false;
            else
                scrollRect.vertical = true;

            //매개변수로 받아온 데이터 갯수만큼 카드가 있는지 체크 해줌
            //갯수가 적다면 ResultCheck 함수 내부에서 카드를 생성시켜줌
            foreach (var box in randomboxList)
            {
                ResultCheck();
            }

            //757.798 10items
        }

        public void ResultCheck()
        {
            //queue갯수가 없다면 새롭게 생성시켜주는 부분
            if (queueRandomResult.Count == 0)
            {
                GameObject resultObj = Instantiate(randomResultObejct);
                RandomResult result = resultObj.GetComponent<RandomResult>();

                result.transform.SetParent(parentResultObject.transform);
                result.transform.localScale = Vector3.one;

                listRandomResult.Add(result);
            }
            else
            {
                queueRandomResult.Dequeue();
            }
        }

        public void RefreshResult()
        {
            //동적으로 카드가 부족한지 체크하기 위한 함수 풀링 기법
            queueRandomResult.Clear();

            for (int i = 0; i < listRandomResult.Count; ++i)
            {
                listRandomResult[i].gameObject.SetActive(false);
                queueRandomResult.Enqueue(listRandomResult[i]);
            }
        }

        public void SetItem(bool isItem)
        {
            //아이템인지 캐릭터인지 구분해서 카드오브젝트 변경
            inventoryCharacterItem.gameObject.SetActive(false);
            inventoryHatchItem.gameObject.SetActive(false);

            if (isItem)
            {
                inventoryHatchItem.gameObject.SetActive(true);
            }
            else
            {
                inventoryCharacterItem.gameObject.SetActive(true);
            }

        }

        public void OnClick_Skip()
        {
            animator.SetTrigger("Skip");
            StopCoroutine(openBoxSequence);
            SetResult();
        }

        public void OnClick_Close()
        {
            for (int i = 0; i < listRandomResult.Count; ++i)
            {
                listRandomResult[i].gameObject.SetActive(false);
            }

            SetGroupButtons(false);
            button_Skip.gameObject.SetActive(true);
            ShopRandomBoxPopup popup = PopupManager.Instance.GetPopUp<ShopRandomBoxPopup>("shopRandomBox");
            popup.FinishSeqeunce();

            if (isItem)
            {
                SetHatchingStone();
            }
            else
            {
                SetCharacter();
            }

            InActivePopup();
        }

        public void OnClick_ReBuy(bool isOnce)
        {
            ShopRandomBoxPopup popup = PopupManager.Instance.GetPopUp<ShopRandomBoxPopup>("shopRandomBox");
            popup.OnClick_BuyRandomBox(isOnce);
        }

        public async void SetCharacter()
        {
            UserCharacters userCharacters = await NetworkManager.Instance.SendToServerAsync<UserCharacters>(string.Format(Url.getUserCharacterInfo, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.userCharacters = userCharacters;
            DataManager.Instance.SetUserCharacterListNew();
        }

        public async void SetHatchingStone()
        {
            HatchingItemData hatchDatas = await NetworkManager.Instance.SendToServerAsync<HatchingItemData>(string.Format(Url.getHatchItem, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.LoadHatchingStoneItemData(hatchDatas);

            ShopScreen.Instance.hatchingStoneChange.Initialize();
            ShopScreen.Instance.hatchingStoneChange.SetHatchingScreen();
        }

        public IEnumerator BoxOpenSequence(ResRewardValueData randomBoxProperty)
        {
            string animName;

            if (isItem)
            {
                animName = HatchingOpen(randomBoxProperty);
            }
            else
            {
                animName = CharacterOpen(randomBoxProperty);
            }

            while (true)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animName) break;
                yield return null;
            }

            yield return new WaitForSeconds(GetAnimationDuration() + 0.5f);
        }

        public float GetAnimationDuration()
        {
            AnimatorClipInfo[] data = animator.GetCurrentAnimatorClipInfo(0);
            float duration = data[0].clip.length;
            return duration;
        }

        public string CharacterOpen(ResRewardValueData randomBoxProperty)
        {
            //캐릭터 오픈 함수
            CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)int.Parse(randomBoxProperty.value)];

            inventoryCharacterItem.newCharacter.SetActive(data.isNew);
            //캐릭터 카드 초기화
            inventoryCharacterItem.Initialize(data, false);
            text_Grade.text = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");
            text_Grade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_ItemName.text = LanguageManager.Instance.GetStringData($"UI_Character_{(int)data.characterIndex}");
            text_Amount.text = $"x{randomBoxProperty.amount}";

            bool isLegend = data.characterGrade == CharacterGrade.EPIC || data.characterGrade == CharacterGrade.LEGENDARY;

            string animKey = isLegend ? "EpicOpen" : "NormalOpen";
            string animName = isLegend ? "OpenBox_Epic" : "OpenBox";

            if (isLegend)
            {
                SetEffectColor(data.characterGrade);
            }

            animator.SetTrigger(animKey);

            return animName;
        }

        public string HatchingOpen(ResRewardValueData randomBoxProperty)
        {
            //부화석 오픈 함수

            UserItem data = DataManager.Instance.dic_hatchData[randomBoxProperty.value];

            //부화석 카드 초기화
            inventoryHatchItem.Initialize(data);

            text_Grade.text = LanguageManager.Instance.GetStringData($"UI_GRADE_{randomBoxProperty.value}");
            text_Grade.color = DataManager.Instance.uiPropertyData.dic_HatchingCardInfo[randomBoxProperty.value].color_GradeText;
            text_ItemName.text = LanguageManager.Instance.GetStringData($"UI_{randomBoxProperty.value}");
            text_Amount.text = $"x{randomBoxProperty.amount}";

            bool isLegend = randomBoxProperty.value.Contains("EPIC") || randomBoxProperty.value.Contains("LEGEND");

            string animKey = isLegend ? "EpicOpen" : "NormalOpen";
            string animName = isLegend ? "OpenBox_Epic" : "OpenBox";

            if (isLegend)
            {
                SetEffectColor(DataManager.Instance.GetHatchingGrade(randomBoxProperty.value));
            }

            animator.SetTrigger(animKey);

            return animName;
        }

        public IEnumerator RandomBoxOpenSequance(List<ResRewardValueData> randomBoxOpenData)
        {
            bool isOnce = randomBoxOpenData.Count == 1;

            button_Skip.gameObject.SetActive(!isOnce);
            SoundManager.Instance.PlaySound(SoundKey.SF_BOX_ENTER);

            animator.Rebind();
            animator.SetTrigger("OpenBox");
            animator.SetTrigger("StartOpen");

            while (true)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IdleBox") break;
                yield return null;
            }

            foreach (var data in randomBoxOpenData)
            {
                yield return StartCoroutine(BoxOpenSequence(data));
            }

            if (isOnce)
            {
                SetGroupButtons(true);
            }
            else
            {
                animator.SetTrigger("Result");
                while (true)
                {
                    if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "RandomResult") break;
                    yield return null;
                }
                SetResult();
            }
        }

        public void SetResult()
        {
            button_Skip.gameObject.SetActive(false);

            if (isItem)
            {
                StartCoroutine(ResultHatchingStoneSequence());
            }
            else
            {
                StartCoroutine(ResultCharacterSequence());
            }
        }

        public IEnumerator ResultCharacterSequence()
        {
            for (int i = 0; i < resultIndexList.Count; i++)
            {
                CharacterIndex characterIndex = (CharacterIndex)int.Parse(resultIndexList[i].value);

                CharacterData data = DataManager.Instance.dic_CharacterData[characterIndex];

                listRandomResult[i].gameObject.SetActive(true);
                listRandomResult[i].Initialize(data);

                yield return new WaitForSeconds(interval);
            }

            SetGroupButtons(true);
        }

        public IEnumerator ResultHatchingStoneSequence()
        {
            for (int i = 0; i < resultIndexList.Count; i++)
            {
                UserItem data = DataManager.Instance.dic_hatchData[resultIndexList[i].value];

                listRandomResult[i].gameObject.SetActive(true);
                listRandomResult[i].Initialize(data, resultIndexList[i].amount);

                yield return new WaitForSeconds(interval);
            }

            SetGroupButtons(true);
        }

        private void SetGroupButtons(bool active)
        {
            object_GroupButtons.SetActive(active);

            if (active)
            {
                if (itemKey == null)
                {
                    button_Many.gameObject.SetActive(false);
                    button_Once.gameObject.SetActive(false);
                    object_GemValue.gameObject.SetActive(false);
                    return;
                }
                
                ShopItem item = ShopScreen.Instance.dic_Item[itemKey];

                if (item.itemKey == "BOX_GOLD_10GEM")
                {
                    bool isOnce = resultIndexList.Count == 1;
                    float price = isOnce ? (int)(item.price * 0.1f) : item.price;

                    button_Once.gameObject.SetActive(isOnce);
                    button_Many.gameObject.SetActive(!isOnce);
                    
                    object_GemValue.gameObject.SetActive(true);
                    text_GemValue.text = $"{UserInfoManager.Instance.gemValue}";

                    if (isOnce)
                    {
                        text_OnceGemPrice.text = $"<sprite=2>{price}";
                        button_Once.SetInterectible(price <= UserInfoManager.Instance.gemValue);
                    }
                    else
                    {
                        text_ManyGemPrice.text = $"<sprite=2>{price}";
                        button_Many.SetInterectible(price <= UserInfoManager.Instance.gemValue);
                    }

                }
                else
                {
                    button_Many.gameObject.SetActive(false);
                    button_Once.gameObject.SetActive(false);
                    object_GemValue.gameObject.SetActive(false);
                }
            }
        }

        private void SetEffectColor(CharacterGrade grade)
        {
            RandomBoxTxtColor colors = DataManager.Instance.uiPropertyData.dic_RandomBoxEffetColor[grade];
            gradient.color = colors.colorName[0];
            for (int i = 0; i < radials.Length; i++)
            {
                radials[i].color = colors.colorName[1];
            }
            ParticleSystem.MainModule particle = this.particle.main;
            particle.startColor = colors.colorName[2];
        }

        private void CharacterNewCheck(List<ResRewardValueData> randomBoxOpenData)
        {
            //부화석 데이터면 셋을 막아줘야하기에 이러한 조건을 삽입
            //아이템이 하나 더 추가되면 클래스를 완전 분할해서 만들 예정이라 이러한 조건 사라짐
            if (isItem == true)
                return;

            foreach (var data in randomBoxOpenData)
            {
                CharacterData characterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)int.Parse(data.value)];
                if (characterData.characterClassLevel == 0)
                {
                    characterData.isNew = true;
                    characterData.characterClassLevel = (int)characterData.characterGrade + 1;
                    characterData.isNewInventory = true;
                    isNewCharacter = true;

                }
                else
                {
                    characterData.isNew = false;
                    characterData.characterQuantity++;
                    characterData.isNewInventory = false;
                    isNewCharacter = false;
                }
            }
        }
    }
}
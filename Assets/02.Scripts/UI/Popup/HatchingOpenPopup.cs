using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class HatchingOpenPopup : PopupTemplate
    {
        //해당 클래스는 부화석 -> 캐릭터 변환 용도로만 사용
        //이외 나무박스로 부화석 얻는거는 RandomBoxPopup에서 진행

        public Animator animator;
        public InventoryItem inventoryItem;

        //부화석 스프라이트 (애니메이션에 필요한 부화석 및 부화석 조각 스프라이트)
        public Image hatchingStoneImage;
        public Image hatchingStoneImage_Light;
        public Image hatchingStoneImage_Pieces_1;
        public Image hatchingStoneImage_Pieces_2;
        public Image hatchingStoneImage_Pieces_3;
        public Image hatchingStoneImage_Pieces_4;

        //스킵 버튼
        public ButtonComponent button_Skip;

        //캐릭터 등급
        public TextMeshProUGUI text_Grade;
        //캐릭터 이름
        public TextMeshProUGUI text_CharacterName;
        //캐릭터 갯수
        public TextMeshProUGUI text_Amount;

        //확인버튼
        public GameObject group_Buttons;

        public IEnumerator openHatchingStoneSequence;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            LobbyNavigator.Instance.Notification(MenuType.SHOP, true);
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Skip.gameObject.SetActive(false);
            button_Close.onClick.AddListener(() => OnClicked_Close());
        }

        public void HatchingStoneOpen(HatchingOpenData responseData, string code)
        {
            //부화석 타입별로 이미지가 다르기에 셋팅
            HatchingStoneSprite hatchingSprites = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[code];

            group_Buttons.gameObject.SetActive(false);

            hatchingStoneImage.sprite = hatchingSprites.sprite_Icon;
            hatchingStoneImage_Light.sprite = hatchingSprites.sprite_Icon_Light;
            hatchingStoneImage_Pieces_1.sprite = hatchingSprites.sprite_Pieces1;
            hatchingStoneImage_Pieces_2.sprite = hatchingSprites.sprite_Pieces2;
            hatchingStoneImage_Pieces_3.sprite = hatchingSprites.sprite_Pieces3;
            hatchingStoneImage_Pieces_4.sprite = hatchingSprites.sprite_Pieces3;

            //부화석 코루틴 시작
            openHatchingStoneSequence = HatchingStoneOpenSequence(responseData.rewardList);
            StartCoroutine(openHatchingStoneSequence);
        }

        public IEnumerator HatchingStoneOpenSequence(HatchReward[] rewards)
        {
            //사운드
            //SoundManager.Instance.PlaySound(SoundKey.SF_BOX_ENTER);

            //애니메이션 시작
            animator.Rebind();

            //캐릭터 카드 정보 셋
            OpenSequence(rewards);

            //부화석 오픈 애니메이션 트리거 On -> 부화석이 깨지는 애니메이션 진행
            animator.SetTrigger("HatchOpen");
            while (true)
            {
                //부화석이 깨지는 애니메이션이 아니라면 여기서 머물게해줌
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HatchOpen_Idle") break;
                yield return null;
            }

            //부화석이 전부다 깨지고 캐릭터 정보가 나오면 확인버튼을 눌러 종료시킬 수 있게함
            group_Buttons.SetActive(true);
        }


        public void OpenSequence(HatchReward[] rewards)
        {
            //캐릭터 정보 들고옴
            CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)int.Parse(rewards[0].value)];

            //캐릭터 레벨이 0 이라면 새로운 캐릭터
            if (data.characterClassLevel == 0)
            {
                inventoryItem.newCharacter.SetActive(true);
                data.characterClassLevel = (int)data.characterGrade + 1;
                data.characterQuantity = rewards[0].amount;
                data.isNew = true;
                data.isNewInventory = true;

            }
            else
            {
                inventoryItem.newCharacter.SetActive(false);
                data.characterQuantity += rewards[0].amount;
                data.isNewInventory = false;
                data.isNew = false;
            }

            //인벤토리아이템 셋팅
            inventoryItem.Initialize(data, false);

            //캐릭터 정보 셋
            text_Grade.text = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");
            text_Grade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_CharacterName.text = LanguageManager.Instance.GetStringData($"UI_Character_{(int)data.characterIndex}");
            text_Amount.text = $"x{rewards[0].amount}";
        }

        public async void SetCharacterAndHatchingStone()
        {
            //새롭게 얻은 캐릭터 정보 다시 초기화
            UserCharacters userCharacters = await NetworkManager.Instance.SendToServerAsync<UserCharacters>(string.Format(Url.getUserCharacterInfo, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.userCharacters = userCharacters;
            DataManager.Instance.SetUserCharacterListNew();

            //부화석 정보 (갯수) 초기화
            HatchingItemData hatchDatas = await NetworkManager.Instance.SendToServerAsync<HatchingItemData>(string.Format(Url.getHatchItem, UserInfoManager.Instance.userId), SendType.GET, TokenType.ACCESSTOKEN);
            DataManager.Instance.LoadHatchingStoneItemData(hatchDatas);
            //DataManager.Instance.hatchDatas = hatchDatas;

            ShopScreen.Instance.hatchingStoneChange.Initialize();
            ShopScreen.Instance.hatchingStoneChange.SetHatchingScreen();

            //팝업 닫음
            InActivePopup();
        }

        public void OnClicked_Close()
        {
            //확인버튼 누르면 다시 못누르게 막음
            group_Buttons.SetActive(false);

            //필요한 정보 초기화
            SetCharacterAndHatchingStone();

        }

    }

}
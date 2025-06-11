using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using UnityEngine;
using UnityEngine.UI;
using Framework.Network;
using UnityEngine.Events;
using TMPro;
using Framework.Sound;

namespace Framework.UI
{
    public class HatchingStoneItem : MonoBehaviour
    {
        //부화석 스프라이트
        public Image hatchingSprite;
        //부화석 버튼
        public Button hatchingButton;
        //부화석 클릭 시 활성화 백그라운드
        public GameObject backGroundOn;
        //부화석 갯수
        public TextMeshProUGUI textCount;
        //부화석 1000개일 시 알람 표시 오브젝트
        public GameObject notice;

        //부화석 정보 (타입, 갯수)
        public UserItem userItemInfo;

        //교환가능의 부화석 갯수
        public readonly int defalutCount = 1000;

        public bool DefalutCheck(int soruce, int defalut) => soruce >= defalut ? true : false;

        //관리자를 hatchingstonechange를 두고 hatchingstonechange가 버튼을 알고있게하자

        public void Start()
        {
            //todo : 사운드 어떻게 할껀지
            //부화석 클릭 시 콜백 함수
            hatchingButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                ClickedHatchingStone();
            });
        }

        public void Initialize(UserItem userItem)
        {
            //부화석 이미지 바꾸기
            hatchingSprite.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[userItem.item].sprite_Icon;

            //부화석 정보 초기화
            userItemInfo = null;
            userItemInfo = userItem;

            //백그라운드 오프
            backGroundOn.SetActive(false);
            //부화석 갯수
            textCount.text = userItemInfo.quantity.ToString();

            //알람표시 온오프
            bool isActive = false;
            isActive = DefalutCheck(userItemInfo.quantity, defalutCount);
            notice.SetActive(isActive);
        }

        public void SetActiveButton(bool isActive)
        {
            backGroundOn.SetActive(isActive);
        }

        public void ClickedHatchingStone()
        {
            //콜백 함수 진입점 : 부화석 클릭 시 
            bool isButtonActive = false;
            isButtonActive = DefalutCheck(userItemInfo.quantity, defalutCount);

            ShopScreen.Instance.hatchingStoneChange.ClickedHatch(userItemInfo.quantity, isButtonActive, userItemInfo.item);

            //샵스크린 콜백함수에서 모든 버튼을 끄고
            //여기서 다시 해당하는 버튼만 액티브
            SetActiveButton(true);
        }
    }
}

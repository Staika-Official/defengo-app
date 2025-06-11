using System.Collections;
using System.Collections.Generic;
using Framework.Util;
using UnityEngine;
using Framework.Network;
using UnityEngine.UI;
using Framework.GameData.Defense;
using TMPro;

namespace Framework.UI
{
    public class HatchingStoneChange : MonoBehaviour
    {
        //부화석 아이템을 들고있음
        //부화석 아이템이란 : 현재 상점에있는 부화석 프리팹들
        public List<HatchingStoneItem> hatchingList;

        //교환 활성화 오브젝트
        public GameObject exchangeButtonObject;
        //교환 활성화 버튼
        public Button exchangeButton;

        //설명위에있는 부화석 이미지
        public Image imagehatchingStoneInfo;

        public TextMeshProUGUI text_Title;

        //부화석 교환 갯수 텍스트
        public TextMeshProUGUI exchangeCountText;
        //부화석 소개 텍스트
        public TextMeshProUGUI hatchingInfoText;

        //비활성화된 교환버튼 텍스트 (이유 : 나라별로 교환 이라는 글자가 다르기 때문)
        public TextMeshProUGUI hatchingDefalutChangeText;

        //활성화된 교환버튼 텍스트 (이유 : 나라별로 교환 이라는 글자가 다르기 때문)
        public TextMeshProUGUI hatchingChangeText;
        //교환버튼이 2개인이유 오브젝트가 2개이다 (활성화, 비활성화)

        public readonly int hatchingExchangeDefalutCount = 1000;

        //현재 클릭한 부화석의 타입을 들고있어야하기에 선언
        public string currentStoneType;

        public void Start()
        {
            //교환버튼 클릭 시 호출되는 콜백함수 추가
            exchangeButton.onClick.AddListener(() =>
            {
                ClickedExchange();
            });
        }

        public void Initialize()
        {
            //api에서 호출한 부화석 데이터를 DataManager에 저장시키고
            //그값을 들고옴 안그러면 지속적으로 api호출이 이루어지기때문에 저장하면서 들고오고 최신화가 필요할 때마다 api호출해서 최신화 시킴
            Dictionary<string, UserItem> data = DataManager.Instance.dic_hatchData;
            int count = 0;

            //부화석 프리팹 초기화
            foreach (var item in data.Keys)
            {
                hatchingList[count].Initialize(data[item]);
                ++count;
            }

            //현재 부화석 갯수 0개로 시작해야함
            exchangeCountText.text = GetAmonutHatchingStoneIcon("HATCHING_ORB_COMMON",0);

            text_Title.text = LanguageManager.Instance.GetStringData("UI_HATCHING_Orb_TITLE");
            //언어별 텍스트 초기화
            hatchingInfoText.text = LanguageManager.Instance.GetStringData("UI_HatchingInfo");
            hatchingDefalutChangeText.text = $"<sprite=24>{LanguageManager.Instance.GetStringData("UI_HatchingExchange")}";
            hatchingChangeText.text = $"<sprite=23>{LanguageManager.Instance.GetStringData("UI_HatchingExchange")}";
            currentStoneType = null;

            //교환버튼 닫아줌
            exchangeButtonObject.SetActive(false);
        }

        public void SetHatchingScreen()
        {
            hatchingList[0].ClickedHatchingStone();
        }

        public void ClickedHatch(int amount, bool isActive, string stoneType)
        {
            //함수 호출 부분 : 부화석 프리팹을 클릭하면 호출하게 되어있음

            //모든 부화석의 현재 클릭됬다는 표시를 꺼줌
            for (int i = 0; i < hatchingList.Count; ++i)
            {
                hatchingList[i].SetActiveButton(false);
            }

            //현재 부화석 타입
            currentStoneType = stoneType;

            imagehatchingStoneInfo.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[stoneType].sprite_Icon;

            //부화석 갯수 표시
            exchangeCountText.text = GetAmonutHatchingStoneIcon(stoneType, amount);

            //교환 버튼 활성화
            exchangeButtonObject.SetActive(isActive);
        }

        public void ClickedExchange()
        {
            //함수 호출 시점 : 교환버튼 클릭 시 호출

            //버튼 중복 방지
            exchangeButtonObject.SetActive(false);

            //리퀘스트 날려줄 body작성
            //현재는 1000개가 고정인데 나중에 어떻게 변할지 모름
            UserItem data = new()
            {
                item = currentStoneType,
                quantity = hatchingExchangeDefalutCount
            };

            //부화석 오픈 리퀘스트를 날려줌
            _ = NetworkManager.Instance.RequestOpenHatchingStone(data, Success, Failed);
        }

        public void Success(HatchingOpenData openData, string code)
        {
            //리퀘스트 요청이 성공 시 이함수에 진입
            HatchingOpenPopup popup = PopupManager.Instance.GetPopUp<HatchingOpenPopup>("hatchOpen");

            popup.ActivePopup();

            popup.HatchingStoneOpen(openData, code);

        }

        public void Failed(string errorCode)
        {
            ServerErrorMessage error = NetworkManager.Instance.GetT<ServerErrorMessage>(errorCode);
            string errorMessage = $"{error.errorCode}_HATCHING";

            Initialize();
            SetHatchingScreen();

            PopupManager.Instance.GetPopUp<HatchingOpenPopup>("hatchOpen").InActivePopup();
            PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice").SetNoticeMessage(errorMessage);
        }

        public string GetAmonutHatchingStoneIcon(string HatchingType, int amount)
        {
            //부화석 별 스프라이트가 달라서 이러한 조건을 넣어줌
            string hatcingIconText = HatchingType switch
            {
                "HATCHING_ORB_COMMON" => $"<sprite=18>{amount}",
                "HATCHING_ORB_UNCOMMON" => $"<sprite=19>{amount}",
                "HATCHING_ORB_RARE" => $"<sprite=20>{amount}",
                "HATCHING_ORB_EPIC" => $"<sprite=21>{amount}",
                "HATCHING_ORB_LEGENDARY" => $"<sprite=22>{amount}",
                _ => ""
            };

            return hatcingIconText;
        }
    }
}

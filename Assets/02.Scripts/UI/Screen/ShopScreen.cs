using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Network;
using System;
using System.Linq;

namespace Framework.UI
{
    [Serializable]
    public class GroupSizeInfo
    {
        public RectTransform rect_BackGround;
        public RectTransform rect_Group;
        public int groupAmount;
        public PageState state;
    }

    public enum PageState
    {
        SHOP,
        GEM,
        SPECIAL
    }

    public class ShopScreen : ScreenTemplate
    {
        public static ShopScreen Instance;

        public Dictionary<string, ShopItem> dic_Item = new();
        public GameObject shopPackage;
        public List<SpecialShopGroup> specialShopGroupSrcList;
        public HatchingStoneChange hatchingStoneChange;

        public CanvasGroup canvasGroup_specialShopPage;
        public CanvasGroup canvasGroup_shopPage;
        public CanvasGroup canvasGroup_gemPage;

        public CustomToggle toggle_Custom;
        public CustomToggleSpecial toggle_Custom_Special;

        public GameObject object_EmptyScreen;

        public PageState pageState;
        public bool isGemPage;
        public List<InAppItem> inAppItems = new();
        public SerializableDictionary<StorePackageType, GroupSizeInfo> dic_GroupSizeInfo;

        public List<SpecialShopGroup> specialShopGroup = new();
        public List<SpecialShopGroup> specialShopCheckGroup = new();
        public Queue<SpecialShopGroup> queueSpecialGroup = new();

        public bool isSpecialStoreOpenCheck;
        public bool isSpecialEmpty;

        public RectTransform hatchingstoneRect;
        public RectTransform inAppRect;
        public RectTransform viewPortRect;

        public GameObject specialShopNotification;

        public GameObject purchaseDesc;


        private void Start()
        {
            Instance = this;
            specialShopGroupSrcList.ForEach(a => a.gameObject.SetActive(false));
        }

        public void PageSequence(CanvasGroup canvasGroup, bool isShow)
        {
            float alpha = isShow ? 1 : 0;

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }


        //토글의 페이지 별 셋 해줘야하는부분을 처리해주는 함수
        public void ShowPage(PageState pagestate)
        {
            switch (pagestate)
            {
                case PageState.SHOP:
                    PageSequence(canvasGroup_specialShopPage, false);
                    PageSequence(canvasGroup_shopPage, true);
                    PageSequence(canvasGroup_gemPage, false);
                    object_EmptyScreen.SetActive(false);
                    purchaseDesc.SetActive(false);
                    break;

                case PageState.GEM:
                    PageSequence(canvasGroup_specialShopPage, false);
                    PageSequence(canvasGroup_shopPage, false);
                    PageSequence(canvasGroup_gemPage, true);
                    object_EmptyScreen.SetActive(false);
                    purchaseDesc.SetActive(true);
                    break;

                case PageState.SPECIAL:
                    PageSequence(canvasGroup_specialShopPage, true);
                    PageSequence(canvasGroup_shopPage, false);
                    PageSequence(canvasGroup_gemPage, false);
                    purchaseDesc.SetActive(true);
                    //특별상점은 호출할 때마다 시간이나 아이템 갯수를 변경시켜줘야하기때문에 초기화함수를 집어넣어줌 
                    SetSpecialStore();
                    break;
            }

            //static manue 변경 (젬,스틱,에너지) 등 변경해줘야함
            LobbyManager.Instance.ShopScreenPageChange(pageState);
        }
        public override void ActiveScreen()
        {
            //스크린창 활성화 했을 때 기간제 상점이 있고없고의 유무에따라 토글을 다르게 처리
            if (isSpecialStoreOpenCheck)
            {
                toggle_Custom_Special.SetToggle(pageState);
            }
            else
            {
                toggle_Custom.SetOnOff(isGemPage);
            }

            //스크린창 활성화 했을 때 기간제 상점이 있고없고의 유무에따라 토글을 다르게 처리
            toggle_Custom_Special.gameObject.SetActive(isSpecialStoreOpenCheck);
            toggle_Custom.gameObject.SetActive(!isSpecialStoreOpenCheck);

            LobbyManager.Instance.ActiveShopScreen(true, pageState);

            //스크린 오픈 할 때 마다 부화석 갯수 초기화
            hatchingStoneChange.Initialize();
            hatchingStoneChange.SetHatchingScreen();
        }

        public override void InactiveScreen()
        {
            isGemPage = false;
            pageState = PageState.SHOP;

            //스크린창 활성화 했을 때 기간제 상점이 있고없고의 유무에따라 토글을 다르게 처리
            if (isSpecialStoreOpenCheck)
            {
                toggle_Custom_Special.SetToggle(pageState);
                toggle_Custom_Special.gameObject.SetActive(false);
            }
            else
            {
                toggle_Custom.SetOnOff(isGemPage);
                toggle_Custom.gameObject.SetActive(false);
            }

            LobbyManager.Instance.ActiveShopScreen(false, pageState);
        }

        public void InitTogglePage(bool isShopPage)
        {
            this.pageState = isShopPage ? PageState.SHOP : PageState.GEM;
            ShowPage(pageState);
            SetRectSize();
        }


        public void InitTogglePage(PageState pageState)
        {
            this.pageState = pageState;
            ShowPage(pageState);
            SetRectSize();
        }


        public void TogglePage(PageState pageState)
        {
            bool isShopPage = pageState == PageState.SHOP;
            this.pageState = pageState;

            //현재 특별상점 기간인지에따른 토글처리
            if (isSpecialStoreOpenCheck)
            {
                toggle_Custom_Special.SetToggle(pageState);
            }
            else
            {
                toggle_Custom.SetOnOff(!isShopPage);
            }

            ShowPage(pageState);
            SetRectSize();
        }

        public override void Initialize()
        {
            StoreItemDTO storeItemDTO = DataManager.Instance.storeItemDTO;

            SpecialStoreDTO specialStoreDTO = DataManager.Instance.specialStoreDTO;

            for (int i = 0; i < storeItemDTO.storeItems.Length; i++)
            {
                StoreItem storeItem = storeItemDTO.storeItems[i];

                if (UserInfoManager.Instance.IsBlockRegion && storeItem.moneyType.Contains("STIK"))
                {
                    continue;
                }

                GameObject obj = Instantiate(shopPackage);

                ShopItem item = obj.GetComponent<ShopItem>();
                item.Initialize(storeItem, StoreType.NORMAL);
                //Debug.Log(storeItemDTO.storeItems[i].itemType);
                dic_GroupSizeInfo[item.storePackageType].groupAmount++;

                RectTransform targetTransform = dic_GroupSizeInfo[item.storePackageType].rect_Group;
                obj.transform.SetParent(targetTransform);
                obj.transform.localScale = Vector3.one;

                dic_Item.Add(item.itemKey, item);
            }

            //특별상점 기간제에서 고정으로 변경됨 국가에 따른 토글처리
            isSpecialStoreOpenCheck = !UserInfoManager.Instance.IsBlockRegion;//specialStoreDTO.specialStore.Length > 0;

            //특별상점 기간이 종료되면 토글 처리 일반 토글로할지 스페셜상점 토글로 할지 정해야함
            if (isSpecialStoreOpenCheck)
            {
                SpecialStoreItemInit(specialStoreDTO);

                toggle_Custom_Special.Initialize();

                //토글이 초기화 되면 호출해주는 함수
                toggle_Custom_Special.onCompleteToggle = (pageState) =>
                {
                    //인잇토글페이지를 오버로딩한 이유
                    //특별상점과 일방상점의 토글의 처리방식이 다르기때문
                    InitTogglePage(pageState);
                };
            }
            else
            {
                toggle_Custom.Initialize();

                toggle_Custom.onCompleteToggle = (isGemPage) =>
                {
                    //인잇토글페이지를 오버로딩한 이유
                    //특별상점과 일방상점의 토글의 처리방식이 다르기때문
                    InitTogglePage(isGemPage);
                };
            }

            //상점의 렉트변경
            SetGroupSize();

            List<string> key_list = IAPManager.Instance.iapKeyList;

            for (int i = 0; i < inAppItems.Count; i++)
            {
                if (i < key_list.Count)
                {
                    inAppItems[i].gameObject.SetActive(true);
                    string key = IAPManager.Instance.iapKeyList[i];
                    inAppItems[i].Initialize(key);
                }
                else
                    inAppItems[i].gameObject.SetActive(false);
            }

            hatchingStoneChange.gameObject.SetActive(true);
            hatchingStoneChange.Initialize();
        }

        public void SetSpecialStore()
        {
            _ = NetworkManager.Instance.GetSpecialStore(SpecialStoreItemInit);
        }

        public void SpecialStoreItemInit(SpecialStoreDTO specialStoreDTO)
        {
            //현재 생성될 갯수 체크 변수
            int specialGroupCount = 0;

            //풀링 준비
            RefreshSepcialShopGroup();

            //처음 초기화하는게아니라 리프레쉬가 계속 될 예정이니
            //이전 상품들을 지워줘야함
            foreach (var checkGroup in specialShopCheckGroup)
            {
                foreach (var item in checkGroup.SpecialShopItemList)
                {
                    if (dic_Item.ContainsKey(item.itemKey))
                    {
                        dic_Item.Remove(item.itemKey);
                    }
                }
            }

            //이전상품들을 지워줬다면 클리어
            specialShopCheckGroup.Clear();

            //받아온 특별상점 리스트 순회
            // for (int i = 0; i < specialStoreDTO.specialStore.Length; ++i)
            // {
            //현재 기간
            //DateTime fromDate = DateTime.Parse(specialStoreDTO.specialStore[i].fromDate);
            // DateTime fromDate = DateTime.UtcNow;
            // //마감 기간
            // DateTime toDate = DateTime.Parse(specialStoreDTO.specialStore[i].toDate);
            // //남은 기간
            // TimeSpan duration = toDate - fromDate;

            // //해당 카테고리가 마감기한을 넘어섰다면 continue
            // if (fromDate > toDate)
            // {
            //     continue;
            // }

            SpecialShopGroup group = GetSpecialShopGroup(SpecialStorePackageType.ITEM);

            //현재 그룹에 아이템을 만들면서 한개라도 만들어졌는지 체크 후 그룹 생성 로직
            //list_Item.Count 넣어주는 이유 -> 아이템 인덱스셋팅후 list_Item를 이용해서 아이템을 꺼내오기 위함
            if (group.SetGroupItemCheck(specialStoreDTO, UserInfoManager.Instance.IsBlockRegion))
            {
                group.gameObject.SetActive(true);
                group.transform.SetParent(canvasGroup_specialShopPage.transform);
                group.transform.localScale = Vector3.one;
                group.Initialize();
                specialShopCheckGroup.Add(group);
                ++specialGroupCount;

                foreach (var item in group.SpecialShopItemList)
                {
                    dic_Item.Add(item.itemKey, item);
                }
            }
            // }

            isSpecialEmpty = specialGroupCount <= 0;

            if (!isSpecialEmpty)
            {
                //남은기간 별 소팅해주는 처리
                if (specialShopCheckGroup.Count != 0)
                {
                    specialShopCheckGroup = specialShopCheckGroup.OrderBy(group => group.periodDay).ToList();
                    for (int i = 0; i < specialShopCheckGroup.Count; ++i)
                    {
                        specialShopCheckGroup[i].transform.SetSiblingIndex(i);
                    }
                }
            }

            object_EmptyScreen.SetActive(isSpecialEmpty);

            //새로운 상품이 만들어졌을 때 notice키고 켜주기
            if (specialGroupCount != SecurePlayerPrefs.GetInt("SpecialShop"))
            {
                SecurePlayerPrefs.SetInt("SpecialShop", specialGroupCount);
                specialShopNotification.SetActive(true);
            }
            else
            {
                specialShopNotification.SetActive(false);
            }

            queueSpecialGroup.Clear();

            //todo
            //렉트 사이즈 조절
            SetSpecialGroupSize();
            SetRectSize();
        }

        public void RefreshSepcialShopGroup()
        {
            for (int i = 0; i < specialShopGroup.Count; ++i)
            {
                specialShopGroup[i].gameObject.SetActive(false);
                queueSpecialGroup.Enqueue(specialShopGroup[i]);
            }
        }

        public SpecialShopGroup GetSpecialShopGroup(SpecialStorePackageType type)
        {
            int count = queueSpecialGroup.Count(a => a.specialStorePackageType == type);
            if (queueSpecialGroup.Count == 0)
            {
                SpecialShopGroup src = specialShopGroupSrcList.Find(a => a.specialStorePackageType == type);
                src.gameObject.SetActive(true);
                SpecialShopGroup groupObj = Instantiate<SpecialShopGroup>(src);

                src.gameObject.SetActive(false);
                specialShopGroup.Add(groupObj);
                return groupObj;
            }
            else
            {
                return queueSpecialGroup.Dequeue();
            }
        }


        //그룹별 렉트 사이즈 셋
        public void SetGroupSize()
        {
            foreach (var item in dic_GroupSizeInfo)
            {
                int a = dic_GroupSizeInfo[item.Key].groupAmount;

                if (a == 0)
                {
                    dic_GroupSizeInfo[item.Key].rect_BackGround.gameObject.SetActive(false);
                    continue;
                }
                float b = a / 4;

                dic_GroupSizeInfo[item.Key].rect_BackGround.sizeDelta = new Vector2(1000, (b + 1) * 500);
            }
        }

        //그룹별 렉트 사이즈 셋
        public void SetSpecialGroupSize()
        {
            foreach (var group in specialShopCheckGroup)
            {
                int groupAmount = group.groupAmount;

                int groupDelta = groupAmount / 4;

                group.rect_BackGround.sizeDelta = new Vector2(1000, (groupDelta + 1) * 500);
            }
        }

        public void SetRectSize()
        {
            //기본 사이즈 크기
            float sizeY = 500f;

            switch (pageState)
            {
                case PageState.SPECIAL:
                    foreach (var item in specialShopCheckGroup)
                    {
                        //120으로 지정해준 이유 페이지에 아이템 그룹별 정렬이 120 간격으로 정렬되어있기 때문
                        sizeY += item.rect_BackGround.sizeDelta.y + 120f;
                    }
                    break;
                case PageState.SHOP:
                    foreach (var item in dic_GroupSizeInfo)
                    {
                        if (item.Value.state == PageState.SHOP)
                        {
                            sizeY += dic_GroupSizeInfo[item.Key].rect_BackGround.sizeDelta.y + 120f;
                        }
                    }
                    //부화석렉트 크기
                    sizeY += hatchingstoneRect.sizeDelta.y;
                    break;
                case PageState.GEM:
                    foreach (var item in dic_GroupSizeInfo)
                    {
                        if (item.Value.state == PageState.GEM)
                        {
                            sizeY += dic_GroupSizeInfo[item.Key].rect_BackGround.sizeDelta.y + 120f;
                        }
                    }
                    //인앱상품렉트 크기
                    sizeY += inAppRect.sizeDelta.y;
                    break;
            }

            viewPortRect.sizeDelta = new Vector2(1080f, sizeY);
        }


        //상품구매 팝업창 띄워주는 함수
        public void BuyItemBox(StorePackageType storePackageType, string itemKey)
        {
            ShopItem item = dic_Item[itemKey];

            //todo
            //ShopItem item = list_Item[itemIndex];

            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    {
                        ShopPopup popup = PopupManager.Instance.GetPopUp<ShopPopup>("shop");
                        popup.SetShopItemInfo(item, storePackageType);
                        popup.PopUpSequence(true);
                    }
                    break;
                case StorePackageType.RANDOM_BOX:
                    {
                        ShopRandomBoxPopup shopRandomBoxPopup = PopupManager.Instance.GetPopUp<ShopRandomBoxPopup>("shopRandomBox");
                        shopRandomBoxPopup.SetRandoxBoxInfo(item);
                        shopRandomBoxPopup.ActivePopup();
                    }
                    break;
                case StorePackageType.GEM:
                    {
                        ShopPopup shopPopup = PopupManager.Instance.GetPopUp<ShopPopup>("shop");
                        shopPopup.SetShopItemInfo(item, storePackageType);
                        shopPopup.PopUpSequence(true);
                    }
                    break;
                case StorePackageType.ITEM:
                    {
                        ShopPopup popup = PopupManager.Instance.GetPopUp<ShopPopup>("shop");
                        popup.SetShopItemInfo(item, storePackageType);
                        popup.PopUpSequence(true);
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
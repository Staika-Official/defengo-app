using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;
using Framework.GameData.Defense;
using Framework.Network;
using SimpleJSON;
using UnityEngine.Events;



public class IAPManager : MonoBehaviour, IDetailedStoreListener, IStoreController
{
    // Start is called before the first frame update
    public static IAPManager Instance;

    public string environment = "production";

    IStoreController m_StoreController; // The Unity Purchasing system.
    private string productId;
    private Product product;
    private StoreTableData mProductTableData;
    public IapBuyType iapBuyType { get; set; }

    public List<StoreTableData> ActiveStoreDataList = new List<StoreTableData>();

    public List<string> iapKeyList { get { return mIAPKeyList; } }
    private List<string> mIAPKeyList = new List<string>();

    public readonly string iapPassPremiumKey = "tier55";
    public readonly string iapPassCrystalKey = "tier56";

    public readonly string baseKey = "io.staika.game.defengo.inapp.";

    public ProductCollection products => throw new NotImplementedException();

    //iap도큐먼트 지울 예정
    //https://docs.unity3d.com/Packages/com.unity.purchasing@4.12/api/UnityEngine.Purchasing.IStoreController.html

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);

            if (Instance is null)
            {
                Instance = this;
            }
        }
        catch (Exception exception)
        {
            Debug.Log("Iap Init Failed");
            // An error occurred during initialization.
            Debug.LogError(exception);
        }
    }

    public void Initialize()
    {
        //ConfigurationBuilder.instance => 어떤 스토어로 구성을 할것인지 -> (1.스토어 구성 2.오버로딩 있음 도큐먼트 참고)
        //StandardPurchasingModule => Unity가 지원하는 표준 스토어를 위한 모듈 (앱스토어, 구글플레이)
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        List<StoreTableData> table_list = DataManager.Instance.StoreTableDataList;
        foreach (StoreTableData data in table_list)
        {
            if (data.is_activated == false)
                continue;

            builder.AddProduct(baseKey + data.inapp_tier, data.product_type);
            ActiveStoreDataList.Add(data);

            if (data.inapp_type == IapBuyType.GEM)
                mIAPKeyList.Add(data.inapp_tier);
        }



        //Initialize(1.리스너, 2.구성 빌더)
        //1.IDetailedStoreListener 향후 거래에 대한 콜백을 받으려면 리스너를 설정 해야함
        //2.ConfigurationBuilder에 바인딩 된 구매 목록 정의 
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseProduct(string inapp_tier)
    {
        this.productId = baseKey + inapp_tier;

        mProductTableData = ActiveStoreDataList.Find(a => a.inapp_tier == inapp_tier);

        //구매할 상품의 id설정 -> 구매성공 실패 여부를 호출해준다.
        m_StoreController.InitiatePurchase(productId);
    }

    public Product GetProductInfo(string inapp_tier)
    {
        //m_StoreController에 있는 products(제품 컬렉션)변수에 접근후
        //WithID 메서드를 사용해서 받아온 매개변수id와 일치하는 제품을 들고온다.
        Product product = m_StoreController.products.WithID(baseKey + inapp_tier);
        return product;
    }

    //Unity IAP가 모든 제품 메타 데이터를 검색하여 구매할 준비가 되면 호출
    //(이러한 호출은 우리가 직접하지않고 UnityPurchasing Init을 할때 우리가 리스너를 현재 매니저로 설정해둬서 이벤트 발생 시 호출을해줌)
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //m_StoreController란 IStoreController 인터페이스이며
        //UnityPurchasing에 등록해준 상품을 제어하기 위한 컨트롤러이며
        //컨트롤러 내부 변수(products)에 접근해서 등록된 상품을 들고오거나 셋 할 수 있다.
        m_StoreController = controller;

        for (int i = 0; i < m_StoreController.products.all.Length; i++)
        {
            //소모한 구매목록의 영수증이있냐
            //영수증이없다면? Pending된 데이터가 없다
            if (!m_StoreController.products.all[i].hasReceipt) continue;

            //구매 영수증
            Debug.Log("receipt : " + m_StoreController.products.all[i].receipt);

            //영수증이 있는지없는지
            Debug.Log(m_StoreController.products.all[i].hasReceipt);

            //상품정의에 접근해서 아이디 뽑아옴 
            productId = m_StoreController.products.all[i].definition.id;
            Debug.Log(productId);
            Debug.Log($"Has Receipt Init ProductId{productId}");
            //ConfirmPendingPurchase(m_StoreController.products.all[i]);
        }
    }

    public List<StoreTableData> GetTableDataList(IapBuyType iapBuyType)
    {
        return ActiveStoreDataList.FindAll(a => a.inapp_type == iapBuyType);
    }

    public string GetMonthlyProductId(int index, bool isBaseKeyMerge = true)
    {
        //월간패키지가 1개라 임시로 처리 여러개가 될 경우 수정필요
        if (isBaseKeyMerge)
            return baseKey + ActiveStoreDataList.Find(a => a.inapp_type == IapBuyType.MONTHLY_GEM).inapp_tier;
        
        return ActiveStoreDataList.Find(a => a.inapp_type == IapBuyType.MONTHLY_GEM).inapp_tier;
    }

    public void SetIapBuyType(string productId)
    {
        StoreTableData data = ActiveStoreDataList.Find(a => baseKey + a.inapp_tier == productId);
        if (data != null)
            iapBuyType = data.inapp_type;
    }

    public IapBuyType GetIapBuyType(string productId)
    {
        StoreTableData data = ActiveStoreDataList.Find(a => baseKey + a.inapp_tier == productId);
        if (data != null)
            return data.inapp_type;

        return IapBuyType.NONE;
    }

    //Unity IAP가 초기화 실패할 경우 호출
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    //구매 성공 시 호출 -> 리턴을 펜딩을 준다 맨 및줄 코드를 봐라 (펜딩을 주는 이유 : 서버에 데이터 저장을 위해)
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Process Purchase@@@@@@@@@@@@@@@");

        //Retrieve the purchased product
        var product = args.purchasedProduct;
        this.product = product;
        //Debug.Log(product.definition.id);

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //Add the purchased product to the players inventory
            if (product.definition.id == productId)
            {
                JSONNode json = JSONNode.Parse(product.receipt);
                string receipt = json["Payload"];
                string purchaseId = json["TransactionID"];
                string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "android";

                RequestValidateIAP data = new()
                {
                    receipt = receipt,
                    platform = deviceInfo,
                    productId = productId,
                    purchaseId = purchaseId
                };

                SetIapBuyType(productId);

                _ = NetworkManager.Instance.RequestValidateInAppPurChase(data);
            }
        }
        else
        {
            //Add the purchased product to the players inventory
            Debug.Log("Google IAP");
            if (product.definition.id == productId)
            {
                string temp = product.receipt.Replace(@"\", "");
                JSONNode json = JSONNode.Parse(temp);
                string purchaseId = json["Payload"]["orderId"];
                if (string.IsNullOrEmpty(purchaseId))
                    purchaseId = json["TransactionID"];
                purchaseId.Replace('"', ' ');
                string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "android";

                RequestValidateIAP data = new()
                {
                    receipt = product.receipt,
                    platform = deviceInfo,
                    productId = productId,
                    purchaseId = purchaseId
                };

                SetIapBuyType(productId);

                _ = NetworkManager.Instance.RequestValidateInAppPurChase(data);
            }
        }

        //D_inapp
        float price = mProductTableData.adjust_price;

        Framework.Util.AdjustInitializer.TrackRevenue("pdxyxw", price, "KRW");

        //펜딩을 줘서 우리가 직접 서버에 데이터 저장하고 결제 완료하게 해주자 (두가지 버전 컴플리트와 펜딩)
        return PurchaseProcessingResult.Pending;
    }

    //구매 실패 시 호출
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        //OnCompletePurchase.Invoke(false);
        Framework.UI.LobbyManager.Instance.PurchasePopup(false);

        //이벤트 초기화
        NetworkManager.Instance.onSuccessWallet = null;
        NetworkManager.Instance.onSuccessPassPremiumInAppPurchase = null;
        NetworkManager.Instance.onSuccessPassExpInAppPurchase = null;
        iapBuyType = IapBuyType.NONE;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }

    public void InitiatePurchase(Product product, string payload)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(string productId, string payload)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(Product product)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(string productId)
    {
        throw new NotImplementedException();
    }

    public void FetchAdditionalProducts(HashSet<ProductDefinition> additionalProducts, Action successCallback, Action<InitializationFailureReason> failCallback)
    {
        throw new NotImplementedException();
    }

    public void FetchAdditionalProducts(HashSet<ProductDefinition> additionalProducts, Action successCallback, Action<InitializationFailureReason, string> failCallback)
    {
        throw new NotImplementedException();
    }

    //구매 성공을 하면 ProcessPurchase이 호출되는데 반환값을 줘야한다
    //2가지 반환값이 있는데 complete와 pending을 반환해주는데 pending이 되면 구매를 처리하고있는 상태이며
    //ConfirmPendingProduct를 호출해줘야한다 (complete는 도큐먼트 찾아보면 알 수 있음 우리는 펜딩)

    //구매 성공은 했지만 pending이 반환이 되면 호출을해줘야함
    //서버에 데이터를 저장하고있고 데이터 저장이끝나면 이 함수 호출
    public void ConfirmPendingProduct()
    {
        ConfirmPendingPurchase(product);
    }

    public void ConfirmPendingPurchase(Product product)
    {
        m_StoreController.ConfirmPendingPurchase(product);

        Debug.Log("Product Receipt : " + product.receipt);
        Debug.Log("Confirm product : " + product.definition.id);
    }


    //결제는됐지만 인앱 -> 지갑 -> 게임 순으로 진행되는데 (지갑 -> 게임)으로 서버 리퀘스트 날리던도중 네트워크가 불안정하면
    //게임 서버로 리퀘스트가 안날아가서 콘텐츠가 활성화가 안되는 문제로인해 함수를 작성
    //이함수는 결제를 완료가 안될때 재로그인 시점에 지갑측에서 제대로 영수증검증까지 되고 db에 잘쌓여있는지 다시 확인하고 함수 호출하는방식으로 진행됨
    public void PendingMonthlyPackagePurchase(string appProductId)
    {
        UnityAction<MonthlyPackageDTO> action = (data) =>
        {
            for (int i = 0; i < data.specialStore.Length; ++i)
            {
                MonthlyPackageType type = (MonthlyPackageType)Enum.Parse(typeof(MonthlyPackageType),
                    data.specialStore[i].storeCategoryType);

                string productId = GetMonthlyProductId((int)type);

                if (productId.Equals(appProductId))
                {
                    RequestMonthlyPackage packageData = new()
                    {
                        storeId = data.specialStore[i].storeId,
                        type = "BUY"
                    };
                    _ = NetworkManager.Instance.BuyUserMonthlyPackage(packageData, null, null);
                    break;
                }
            }
        };
        _ = NetworkManager.Instance.GetUserMonthlyPackage(action, null);
    }

    public void PendingBattlePassPremiumPurchase()
    {
        BattlePassBuyExp data = new()
        {
            payType = "INAPP",
            battlePassId = UserInfoManager.Instance.userPassId,
            price = 0f
        };

        _ = NetworkManager.Instance.RequestPassBuyPremium(data, null, null);
    }

    public void PendingBattlePassExpPurchase()
    {
        BattlePassBuyExp data = new()
        {
            payType = "INAPP",
            battlePassId = UserInfoManager.Instance.userPassId,
            price = 0f
        };

        _ = NetworkManager.Instance.RequestPassBuyExp(data, null, null);
    }

}

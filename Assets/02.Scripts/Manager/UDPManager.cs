//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UDP;

//public class UDPManager : MonoBehaviour
//{
//    public static UDPManager Instance;
//    public static List<string> productIds = new();

//    void Start()
//    {
//        if (Instance is null)
//        {
//            Instance = this;
//            IInitListener listener = new InitListener();
//            StoreService.Initialize(listener);
//        }
//    }

//    private void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.L))
//        {
//            PurchaseProduct(productIds[0]);
//        }
//    }

//    public void PurchaseProduct(string productId)
//    {
//        IPurchaseListener listener = new PurchaseListener();
//        StoreService.Purchase(productId, "payLoad", listener);
//    }

//    private class InitListener : IInitListener
//    {
//        public void OnInitialized(UserInfo userInfo)
//        {
//            Debug.Log("[Game]On Initialized succeeded");
            
//            IPurchaseListener listener = new PurchaseListener();
//            StoreService.QueryInventory(listener);
//        }

//        public void OnInitializeFailed(string message)
//        {
//            Debug.Log("[Game]OnInitializeFailed: " + message);
//        }
//    }

//    public class PurchaseListener : IPurchaseListener
//    {
//        public void OnPurchase(PurchaseInfo purchaseInfo)
//        {
//            Debug.Log("On Purchase !!!!!!");
//            Debug.Log(purchaseInfo.DeveloperPayload);
//            Debug.Log(purchaseInfo.ProductId);
//            Debug.Log(purchaseInfo.GameOrderId);
//            Debug.Log(purchaseInfo.OrderQueryToken);
//            Debug.Log(purchaseInfo.StorePurchaseJsonString);
//        }

//        public void OnPurchaseConsume(PurchaseInfo purchaseInfo)
//        {
//            Debug.Log("On Purchase Consume !!!!!!");
//            Debug.Log(purchaseInfo.DeveloperPayload);
//            Debug.Log(purchaseInfo.ProductId);
//        }

//        public void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo)
//        {
            
//        }

//        public void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo)
//        {
            
//        }

//        public void OnPurchasePending(string message, PurchaseInfo purchaseInfo)
//        {
            
//        }

//        public void OnPurchaseRepeated(string productId)
//        {
            
//        }

//        public void OnQueryInventory(Inventory inventory)
//        {
//            string message = "";

//            foreach (var productInfo in inventory.GetProductDictionary())
//            {
//                Debug.Log("[Game] Returned product: " + productInfo.Key + " " + productInfo.Value.ProductId);
//                message += string.Format("{0}:\n" +
//                                         "\tTitle: {1}\n" +
//                                         "\tDescription: {2}\n" +
//                                         "\tConsumable: {3}\n" +
//                                         "\tPrice: {4}\n" +
//                                         "\tCurrency: {5}\n" +
//                                         "\tPriceAmountMicros: {6}\n" +
//                                         "\tItemType: {7}\n",
//                    productInfo.Key,
//                    productInfo.Value.Title,
//                    productInfo.Value.Description,
//                    productInfo.Value.Consumable,
//                    productInfo.Value.Price,
//                    productInfo.Value.Currency,
//                    productInfo.Value.PriceAmountMicros,
//                    productInfo.Value.ItemType
//                );

//                productIds.Add(productInfo.Value.ProductId);
//            }

//            Debug.Log(message);

//            Debug.Log("On Query Inventory");
//        }

//        public void OnQueryInventoryFailed(string message)
//        {
            
//        }
//    }


//}

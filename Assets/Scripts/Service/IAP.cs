using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
// using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

// [System.Serializable]
// public class ExchangeRateResponse
// {
//     public Rates rates;
// }

// [System.Serializable]
// public class Rates
// {
//     public float AED, AFN, ALL, AMD, ANG, AOA, ARS, AUD, AWG, AZN, BAM, BBD, BDT, BGN, BHD, BIF, 
//         BMD, BND, BOB, BRL, BSD, BTN, BWP, BYN, BZD, CAD, CDF, CHF, CLP, CNY, COP, CRC, CUP, CVE, 
//         CZK, DJF, DKK, DOP, DZD, EGP, ERN, ETB, EUR, FJD, FKP, FOK, GBP, GEL, GGP, GHS, GIP, GMD, 
//         GNF, GTQ, GYD, HKD, HNL, HRK, HTG, HUF, IDR, ILS, IMP, INR, IQD, IRR, ISK, JEP, JMD, JOD, 
//         JPY, KES, KGS, KHR, KID, KMF, KRW, KWD, KYD, KZT, LAK, LBP, LKR, LRD, LSL, LYD, MAD, MDL, 
//         MGA, MKD, MMK, MNT, MOP, MRU, MUR, MVR, MWK, MXN, MYR, MZN, NAD, NGN, NIO, NOK, NPR, NZD, 
//         OMR, PAB, PEN, PGK, PHP, PKR, PLN, PYG, QAR, RON, RSD, RUB, RWF, SAR, SBD, SCR, SDG, SEK, 
//         SGD, SHP, SLL, SOS, SRD, SSP, STN, SYP, SZL, THB, TJS, TMT, TND, TOP, TRY, TTD, TVD, TWD, 
//         TZS, UAH, UGX, USD, UYU, UZS, VES, VND, VUV, WST, XAF, XCD, XOF, XPF, YER, ZAR, ZMW, ZWL;
    
// }


[Serializable]
public class ConsumableItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
}

[Serializable]
public class NonConsumableItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
}

[Serializable]
public class SubscriptionItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
    // public int timeDuration; // in Day
}


public class IAP : MonoBehaviour
{
    public AudioManager audioManager;

    public SaveDataJson saveDataJson;

    StoreController storeController;
    public ConsumableItem starterBundle;
    public ConsumableItem islandBundle;
    public ConsumableItem cityBundle;
    public ConsumableItem saleClassic;
    public ConsumableItem mushroomBundle;
    public NonConsumableItem removeAds;
    // public SubscriptionItem sItem;
    // public GameObject AdsPurchasedWindow;
    // public Shop shop;

    // public Data data;
    // public Payload payload;
    // public PayloadData payloadData;

    public TextMeshProUGUI starterBundleTxt;
    public TextMeshProUGUI islandBundleTxt;
    public TextMeshProUGUI cityBundleTxt;
    public TextMeshProUGUI saleClassicTxt;
    public TextMeshProUGUI mushroomBundleTxt;

    // private bool allowToShowShopBanner = true;

    public GameObject BtrRemoveAdsInShop;
    public GameObject BtrRemoveAdsInSetting;
    public Shop shop;
    public AdsManager adsManager;

    public GameObject RemoveAdsBtn;
    public GameObject SaleBtn;
    public GameObject SaleDialog;
    // public RemoveAds removeAds;
    // private string exchangeRateApiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

    void Start()
    {
        InitializeIAP();
    }

    private void OnDestroy()
    {
        storeController.OnPurchasePending -= OnPurchasePending;
        storeController.OnStoreDisconnected -= OnStoreDisconnected;
        storeController.OnProductsFetched -= OnProductsFetched;
        storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        storeController.OnPurchasesFetched -= OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
        storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        storeController.OnPurchaseDeferred -= OnPurchaseDeferred;
        storeController.OnPurchaseFailed -= OnPurchaseFailed;
    }

    public void ConnumableBtn(string val)
    {
        audioManager.PlaySFX("click");

        if (val == starterBundle.Id) storeController.PurchaseProduct(starterBundle.Id);
        else if (val == islandBundle.Id) storeController.PurchaseProduct(islandBundle.Id);
        else if (val == cityBundle.Id) storeController.PurchaseProduct(cityBundle.Id);
        else if (val == saleClassic.Id) storeController.PurchaseProduct(saleClassic.Id);
        else if (val == mushroomBundle.Id) storeController.PurchaseProduct(mushroomBundle.Id);
    }

    public void NonConnumableBtn()
    {
        storeController.PurchaseProduct(removeAds.Id);
    }

    // public void Subscription()
    // {
        // storeController.InitiatePurchase(sItem.Id);
    // }

    // void SetupBuilder()
    // {
    //     var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
    //     builder.AddProduct(starterBundle.Id, ProductType.Consumable);
    //     builder.AddProduct(islandBundle.Id, ProductType.Consumable);
    //     builder.AddProduct(cityBundle.Id, ProductType.Consumable);
    //     builder.AddProduct(saleClassic.Id, ProductType.Consumable);
    //     builder.AddProduct(mushroomBundle.Id, ProductType.Consumable);
    //     builder.AddProduct(removeAds.Id, ProductType.NonConsumable);
    //     // builder.AddProduct(sItem.Id, ProductType.Subscription);
    //     UnityPurchasing.Initialize(this, builder);
    // }

    // void CheckNonConsumable(string id)
    // {
    //     if (storeController != null)
    //     {
    //         var product = storeController.products.WithID(id);
    //         if (product != null)
    //         {
    //             if (product.hasReceipt || (bool)saveDataJson.GetData("RemoveAds"))//purchased
    //             {
    //                 RemoveAds();
    //             }
    //             else
    //             {
    //                 ShowAds();
    //             }
    //         }
    //     }
    // }

    public void RemoveAds()
    {
        DisplayAds(false);
    }

    void ShowAds()
    {
        DisplayAds(true);
    }

    void DisplayAds(bool x)
    {
        if (!x)
        {
            // StartCoroutine(CloseShopBanner(0));

            // AdLoadedNewArea.transform.parent.gameObject.SetActive(false);
            // AdLoadedSetting.transform.parent.gameObject.SetActive(false);
            // AdLoadedSmallShop.transform.parent.gameObject.SetActive(false);
            adsManager.DestroyBannerAd();
            if ((bool)saveDataJson.GetData("RemoveAds"))
            {
                RemoveAdsBtn.SetActive(false);
                SaleBtn.SetActive(false);
                SaleDialog.SetActive(false);
                // BtrRemoveAdsInSetting.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
                // BtrRemoveAdsInSetting.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
                // BtrRemoveAdsInShop.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
                // BtrRemoveAdsInShop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
                // BtrRemoveAdsInSetting.GetComponent<Button>().interactable = false;
                // BtrRemoveAdsInShop.GetComponent<Button>().interactable = false;
            }

            // if(removeAds.gameObject.activeSelf) removeAds.Exit();
        }
        else
        {
            saveDataJson.SaveData("RemoveAds", false);
        }
    }

    // void ActivateElitePass()
    // {
    //     setupElitePass(true);
    // }

    // void DeActivateElitePass()
    // {
    //     setupElitePass(false);
    // }

    // string FormatDateTime(DateTime dateTime)
    // {
    //     return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    // }

    // void setupElitePass(bool x)
    // {
    //     if (x)// active
    //     {
    //         // shop.ExitLegendarySub();
    //         // home.CheckUnlockMap();
    //         // RemoveAds();

    //         // DateTime currentTime = DateTime.Now;
    //         // string todayLegendarySub = (string)saveDataJson.GetData("TodayLegendarySUB");
    //         // BtnSub.SetActive(false);
    //         // if(todayLegendarySub == "" || DateTime.ParseExact(todayLegendarySub, "dd/MM/yyyy HH:mm:ss", null).Date < currentTime.Date)
    //         // {
    //         //     string todayLegendarySubstring = FormatDateTime(currentTime);
    //         //     saveDataJson.SaveData("TodayLegendarySUB", todayLegendarySubstring);
    //         //     shop.AddHint(sItem.Id);
    //         // }

    //     }
    //     else
    //     {
    //         // saveDataJson.SaveData("LegendarySUB", false);
    //         // saveDataJson.SaveData("TodayLegendarySUB", "");
    //         // saveDataJson.SaveData("OpenAllMaps", false);
    //         // LoadAds();
    //     }
    // }

    // void CheckSubscription(string id)
    // {
    //     var subProduct = storeController.products.WithID(id);
    //     if (subProduct != null)
    //     {
    //         try
    //         {
    //             if (subProduct.hasReceipt)
    //             {
    //                 var subManager = new SubscriptionManager(subProduct, null);
    //                 var info = subManager.getSubscriptionInfo();

    //                 if (info.isSubscribed() == Result.True)
    //                 {
    //                     print("We are subscribed");
    //                     ActivateElitePass();
    //                 }
    //                 else
    //                 {
    //                     print("Un subscribed");
    //                     DeActivateElitePass();
    //                 }

    //             }
    //             else
    //             {
    //                 print("receipt not found !!");
    //                 DeActivateElitePass();
    //             }
    //         }
    //         catch (Exception)
    //         {
    //             print("It only work for Google store, app store, amazon store, you are using fake store!!");
    //         }
    //     }
    //     else
    //     {
    //         print("product not found !!");
    //     }
    // }

    // public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    // {
    //     Debug.LogWarning($"Purchase failed: {product.definition.id}. Reason: {failureDescription.message}");
    //     // throw new NotImplementedException();
    // }

    // public void OnInitializeFailed(InitializationFailureReason error)
    // {
    //     Debug.LogWarning($"IAP Initialization failed: {error}");
    //     // throw new NotImplementedException();
    // }

    // public void OnInitializeFailed(InitializationFailureReason error, string message)
    // {
    //     Debug.LogWarning($"IAP Initialization failed: {error}. Message: {message}");
    //     // throw new NotImplementedException();
    // }

    // public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    // {
    //     var product = purchaseEvent.purchasedProduct;

    //     if (product.definition.id == removeAds.Id)//non consumable
    //     {
    //         saveDataJson.SaveData("RemoveAds", true);
    //         RemoveAds();
    //     }
    //     // else if (product.definition.id == sItem.Id)//subscribed
    //     // {
    //     //     saveDataJson.SaveData("LegendarySUB", true);
    //     //     saveDataJson.SaveData("OpenAllMaps", true);
    //     //     ActivateElitePass();
    //     // }
    //     else if (product.definition.id == starterBundle.Id || product.definition.id == islandBundle.Id || product.definition.id == cityBundle.Id ||
    //             product.definition.id == saleClassic.Id || product.definition.id == mushroomBundle.Id)
    //     {
    //         string receipt = product.receipt;
    //         data = JsonUtility.FromJson<Data>(receipt);
    //         int quantity = 1;
    //         if (data.Payload != "ThisIsFakeReceiptData")
    //         {
    //             payload = JsonUtility.FromJson<Payload>(data.Payload);
    //             payloadData = JsonUtility.FromJson<PayloadData>(payload.json);
    //             quantity = payloadData.quantity;
    //         }

    //         // for (int i = 0; i < quantity; i++)
    //         // {
    //         shop.AddPackage(product.definition.id, quantity);
    //         if (product.definition.id == "sale_classic")
    //         {
    //             saveDataJson.SaveData("RemoveAds", true);
    //             RemoveAds();
    //         }
    //         // }
    //     }

    //     return PurchaseProcessingResult.Complete;
    // }

    // public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    // {
    //     Debug.LogWarning($"Purchase failed: {product.definition.id}. Reason: {failureReason}");
    //     // throw new NotImplementedException();
    // }

    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     CheckNonConsumable(removeAds.Id);
    //     // CheckSubscription(sItem.Id);

    //     // ConvertPriceToLocalCurrency(sItem);
    // }

    async void InitializeIAP()
    {
        storeController = UnityIAPServices.StoreController();
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnStoreDisconnected += OnStoreDisconnected;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnPurchaseDeferred += OnPurchaseDeferred;
        await storeController.Connect();

        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        var initialProductsToFetch = new List<ProductDefinition>
        {
            new(starterBundle.Id, ProductType.Consumable),
            new(islandBundle.Id, ProductType.Consumable),
            new(cityBundle.Id, ProductType.Consumable),
            new(saleClassic.Id, ProductType.Consumable),
            new(mushroomBundle.Id, ProductType.Consumable),
            new(removeAds.Id, ProductType.NonConsumable),
        };

        storeController.FetchProducts(initialProductsToFetch);
        Debug.LogWarning(storeController);
    }

    void OnPurchasePending(PendingOrder pendingOrder)
    {
        Debug.LogWarning("OnPurchasePending_____________________");
        if (pendingOrder.Info.PurchasedProductInfo.Count > 0)
        {
            storeController.ConfirmPurchase(pendingOrder);
        }
        else
        {
            StartCoroutine(ConfirmWhenReady(pendingOrder));
        }
    }

    IEnumerator ConfirmWhenReady(PendingOrder pendingOrder)
    {
        float timeout = 5f; // tối đa 5 giây chờ
        float elapsed = 0f;

        while (pendingOrder.Info.PurchasedProductInfo.Count == 0 && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        storeController.ConfirmPurchase(pendingOrder);
    }

    void OnPurchaseConfirmed(Order order)
    {
        Debug.LogWarning("OnPurchaseConfirmed_____________________");

        foreach (var purchasedProduct in order.Info.PurchasedProductInfo)
        {

            string productId = purchasedProduct.productId;
            // string productId = order.Info.PurchasedProductInfo[0].productId;

            if (productId == removeAds.Id)
            {
                saveDataJson.SaveData("RemoveAds", true);
                RemoveAds();
            }
            else
            {
                try
                {
                    var unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(order.Info.Receipt);

                    // Parse lớp thứ 2 (Payload)
                    var googleWrapper = JsonUtility.FromJson<GooglePayloadWrapper>(unifiedReceipt.Payload);

                    // Parse lớp thứ 3 (json)
                    var purchaseData = JsonUtility.FromJson<GooglePurchaseData>(googleWrapper.json);

                    Debug.Log($"Product: {purchaseData.productId}, Quantity: {purchaseData.quantity}");

                    shop.AddPackage(productId, purchaseData.quantity);
                    if (productId== "sale_classic")
                    {
                        saveDataJson.SaveData("RemoveAds", true);
                        RemoveAds();
                    }
                }
                catch
                {
                    shop.AddPackage(productId, 1);
                    if (productId== "sale_classic")
                    {
                        saveDataJson.SaveData("RemoveAds", true);
                        RemoveAds();
                    }
                }
            }
        }
    }

[Serializable]
public class UnifiedReceipt
{
    public string Store;
    public string TransactionID;
    public string Payload;
}

[Serializable]
public class GooglePayloadWrapper
{
    public string json;
    public string signature;
}

[Serializable]
public class GooglePurchaseData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public int quantity; // Đây là cái bạn cần
}

    void OnPurchaseFailed(FailedOrder failedOrder)
    {
        Debug.LogError("FailedOrder: " + failedOrder);
    }
    void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        Debug.LogWarning("DeferredOrder: " + deferredOrder);
    }


    void OnProductsFetched(List<Product> products)
    {
        storeController.FetchPurchases();
    }

    void OnProductsFetchFailed(ProductFetchFailed fail)
    {
        Debug.LogError($"Products fetch failed: {fail.FailureReason} - {fail.FailedFetchProducts}");
    }

    void OnPurchasesFetched(Orders orders)
    {
        Debug.LogWarning("OnPurchasesFetched: " + orders);
        ConvertPrice();
        // Debug.Log(storeController.GetProductById(sItem.Id));

        bool isRemoveAds = false;

        foreach (var order in orders.ConfirmedOrders)
        {
            foreach (var product in order.Info.PurchasedProductInfo)
            {
                if (product.productId == removeAds.Id)
                {
                    isRemoveAds = true;
                }
            }
            if (isRemoveAds) break;
        }

        // if(isSubscription) ActivateElitePass();
        // else DeActivateElitePass();

        if (isRemoveAds|| (bool)saveDataJson.GetData("RemoveAds"))
        {
            RemoveAds();
        }
        else
        {
            ShowAds();
        }
    }

    void OnPurchasesFetchFailed(PurchasesFetchFailureDescription PurchasesFetchFailureDescription)
    {
        // DeActivateElitePass();
        Debug.LogError("OnPurchasesFetchFailed: " + PurchasesFetchFailureDescription);
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription storeConnectionFailureDescription)
    {
        Debug.LogError("OnStoreDisconnected: " + storeConnectionFailureDescription);
    }

    void ConvertPrice()
    {
        foreach (Product product in storeController.GetProducts())
        {
            ConvertPriceToLocalCurrency(product, product.definition.id);
        }
    }

    void ConvertPriceToLocalCurrency(Product product, string id)
    {
        if(product == null) return;
        ProductMetadata metadata = product.metadata;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        RegionInfo region = new RegionInfo(currentCulture.Name);

        string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
        if (id == starterBundle.Id) starterBundleTxt.text = txt;
        else if (id == islandBundle.Id) islandBundleTxt.text = txt;
        else if (id == cityBundle.Id) cityBundleTxt.text = txt;
        else if (id == saleClassic.Id) saleClassicTxt.text = txt;
        else if (id == mushroomBundle.Id) mushroomBundleTxt.text = txt;

    }
}
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using GoogleMobileAds.Ump.Api;
using Firebase.Extensions;

public class AdsManager : MonoBehaviour //, IDetailedStoreListener
{
    // private string _appId = "ca-app-pub-5342144217301971~4597278331";

    // private string _adOpenId = "ca-app-pub-5342144217301971/7131603028";
    // private string _adBannerId = "ca-app-pub-5342144217301971/3699129638";
    // private string _adRewardId = "ca-app-pub-5342144217301971/7207624641";
    // private string _adInterstitialId = "ca-app-pub-5342144217301971/8520706314";
    // private string NativeAdUnitId = "ca-app-pub-5342144217301971/3399746738";

    // key test
    private string _adOpenId = "ca-app-pub-5342144217301971/4202714322";
    private string _adBannerId = "ca-app-pub-5342144217301971/2586380327";
    private string _adRewardId = "ca-app-pub-5342144217301971/3480659590";
    private string _adInterstitialId = "ca-app-pub-5342144217301971/1273298651";
    private string NativeAdUnitId = "ca-app-pub-5342144217301971/3131211786";

    public static AdsManager Instance;

    public GameObject InterAds;
    public GameObject game;
    public Loading loading;

    // private NativeAd nativeAdSetting;
    // public RawImage AdIconTextureSetting;
    // public TextMeshProUGUI AdHeadlineSetting;
    // public TextMeshProUGUI AdDescriptionSetting;
    // public GameObject AdLoadedSetting;
    // public GameObject AdLoadingSetting;
    public GameObject AdsMessage;
    public AudioManager audioManager;

    // private NativeAd nativeAdNewArea;
    // public RawImage AdIconTextureNewArea;
    // public TextMeshProUGUI AdHeadlineNewArea;
    // public TextMeshProUGUI AdDescriptionNewArea;
    // public GameObject AdLoadedNewArea;
    // public GameObject AdLoadingNewArea;

    // private NativeAd nativeAdSmallShop;
    // public RawImage AdIconTextureSmallShop;
    // public TextMeshProUGUI AdHeadlineSmallShop;
    // public TextMeshProUGUI AdDescriptionSmallShop;
    // public GameObject AdLoadedSmallShop;
    // public GameObject AdLoadingSmallShop;
    public GameObject Thanks;
    public GameObject CountDown;
    public GameObject BtnSub;

    private BannerView _bannerView;
    private AppOpenAd _appOpenAd;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    private int countOpenAd = 0;
    private bool stopCountDownInter = false;
    private bool pauseCountDownInter = false;
    // private bool allowVibration;

    ConsentForm _consentForm;

    public SaveDataJson saveDataJson;
    // public SmallShop SmallShop;
    // public OpenedGift openedGift;
    // public RandomSpin randomSpin;
    // public LuckyReward luckyReward;
    // public GameObject hintIconInGame;


    private string nameReward;

    private bool isFocus = false;
    private bool isProcessing = false;
    private bool showOpenAd = false;
    private bool loadedOpenAd = false;
    // private bool stopShowOpenAd = false;
    private bool resetTimeInter = false;
    private bool allowToShowShopBannerHint = true;
    private bool waitOpenads = true;
    private bool IsLoadFireBase = false;

    private Firebase.FirebaseApp app;

    public GameObject ShopHintBanner;
    public GameObject ShopBanner;
    public RemoteConfigAds remoteConfigAds;


    [Header("GameObject")]
    
    public Lose lose;
    public ShopTool shopTool;

    public EnergyTimer energyTimer;
    public DialogPlay dialogPlay;
    public LuckySpin luckySpin;
    public BuyEnergy buyEnergy;

    void Awake()
    {
        waitOpenads = true;
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }


    void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        // audioManager.PlayMusic();
        LoadAds();
        Vibration.Init();

        // allowVibration = (bool)saveDataJson.GetData("Vibration");

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                app = Firebase.FirebaseApp.DefaultInstance;
            } else {
                UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    // [Obsolete]
    void LoadAds()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // RequestNativeAd();

            LoadAppOpenAd();
            
            CreateBannerView();

            LoadInterstitialAd();

            LoadRewardedAd();

            // LoadBannerAd();
        });
    }

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    public void DoneLoadFireBase()
    {
        IsLoadFireBase = true;
    }

#region Banner
    private bool bannerLoaded = false;
    public void CreateBannerView()
    {
        // Debug.LogWarning("Creating banner view"); 
        // If we already have a banner, destroy the old one.
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        if (_bannerView != null) DestroyBannerAd();
        // Create a 320x50 banner at top of the screen
        // AdSize adSize = new AdSize(320, 100);
        _bannerView = new BannerView(_adBannerId, AdSize.Banner, AdPosition.Bottom);
        ListenToAdEvents();
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            // Debug.LogWarning("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadBannerAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds") || bannerLoaded) return;
        if(remoteConfigAds.allConfigData.Banner == 0) return;
        if((int)saveDataJson.GetData("OpenedMap") < remoteConfigAds.allConfigData.Banner) return;
        if(_bannerView == null) CreateBannerView();
        string uuid = Guid.NewGuid().ToString();
        // _bannerView = new BannerView(_adBannerId, AdSize.Banner, AdPosition.Bottom);
        var adRequest = new AdRequest();
        // adRequest.Extras.Add("collapsible", "bottom");
        // adRequest.Extras.Add("collapsible_request_id", uuid);
    
        _bannerView.LoadAd(adRequest);

        // Invoke("LoadNewBanner", 60f);
    }

    void LoadNewBanner() 
    {
        CreateBannerView();
        LoadBannerAd();
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            bannerLoaded = true;
            // Debug.LogWarning("Banner view loaded an ad with response : "
            //     + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            // Debug.LogWarning("Banner view failed to load an ad with error : "
            //     + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Banner view paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            // Debug.LogWarning("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Banner view full screen content closed.");
        };
    }

#endregion

#region Interstitial
    public void StopCountDownInter()
    {
        stopCountDownInter = true;
        CountDown.SetActive(false);
    }

    public void PauseCountDownInter()
    {
        pauseCountDownInter = true;
        CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
    }

    public void ContinueCountDownInter()
    {
        pauseCountDownInter = false;
        CountDown.transform.GetChild(0).GetComponent<Slider>().DOPlay();
    }

    public void StartCountDownInter()
    {
        stopCountDownInter = false;
        // StartCoroutine(CountdownToInter());
    }

    public IEnumerator CountdownToInter(int currentTime = 120)
    {
        if(game.gameObject.activeSelf)
        {
            while (currentTime > 0)
            {
                yield return new WaitForSeconds(1f);
                if(stopCountDownInter) break;
                if(!pauseCountDownInter) currentTime -= 1;
                if(resetTimeInter) {
                    currentTime = 120;
                    resetTimeInter = false;
                }
                if(currentTime <= 5) ShowAdCountDown(currentTime);
                // Debug.Log(currentTime + " ?? " + Time.deltaTime);
            }

            if(!stopCountDownInter && !(bool)saveDataJson.GetData("RemoveAds")){
                // if(_interstitialAd != null && _interstitialAd.CanShowAd()) InterAds.SetActive(true);
                Invoke("ShowInterstitialAd", 1.5f);
            } else stopCountDownInter = false;
        }
    }

    void ShowAdCountDown (int time = 0)
    {
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        RectTransform CountDownRect =  CountDown.GetComponent<RectTransform>();
        if(!CountDown.activeSelf)
        {
            if (_interstitialAd == null || !_interstitialAd.CanShowAd()) return;
            CountDown.transform.GetChild(0).GetComponent<Slider>().value = 1;
            CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
            CountDown.transform.GetChild(0).GetComponent<Slider>().DOValue(0, 6, false);
            CountDownRect.anchoredPosition = new Vector2 (0, CountDownRect.sizeDelta.y / 2);
            CountDownRect.DOAnchorPos(new Vector2(0, -71), 0.5f);
            CountDown.SetActive(true);
        }
        CountDown.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{time}";
        if(time == 1) Invoke("ShowAdCountDown", 1);
        else if(time == 0) CountDownRect.DOAnchorPos(new Vector2(0, CountDownRect.sizeDelta.y / 2), 0.5f).OnComplete(() => CountDown.SetActive(false));
    }

    // void ShowInterAd ()
    // {
    //     ShowInterstitialAd();
    // }

    void ContinueGame(string txt = "")
    {
        // InterAds.SetActive(false);
        if(txt == "thanks")
        {
            // Thanks.SetActive(true);
            Invoke("WaitForThanks", 1);
        }else WaitForThanks();
    }

    void WaitForThanks()
    {
        // Thanks.SetActive(false);
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        StartCountDownInter();
    }

    private bool allowShowInter = true;

    IEnumerator RefuseShowInter ()
    {
        allowShowInter = false;
        // Debug.Log(remoteConfigAds.allConfigData.Interstitial.time_wait_next_ad);
        yield return new WaitForSeconds(remoteConfigAds.allConfigData.Interstitial.time_wait_next_ad);
        allowShowInter = true;
    }

    public void ShowInterstitialAd(int map, int position = 1)
    {
        if(!allowShowInter) return;
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        Interstitial InterstitialStatus = remoteConfigAds.allConfigData.Interstitial;
        if(InterstitialStatus.show_position != position) return;
        if(InterstitialStatus.start == 0) return;
        map = map == -1 ? (int)saveDataJson.GetData("OpenedMap") : map;
        if(map < InterstitialStatus.start) return;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            // Debug.LogWarning("Showing interstitial ad.");
            audioManager.PauseAllAudio();
            _interstitialAd.Show();
        }
        else
        {
            // Debug.LogWarning("Interstitial ad is not ready yet.");
            LoadInterstitialAd();
            // ContinueGame();
        }
    }

    public void LoadInterstitialAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
              _interstitialAd.Destroy();
              _interstitialAd = null;
        }

        // Debug.LogWarning("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // adRequest.Keywords.Add("unity-admob-sample");
        // send the request to load the ad.
        InterstitialAd.Load(_adInterstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    // Debug.LogWarning("interstitial ad failed to load an ad " +
                    //                "with error : " + error);
                    return;
                }

                // Debug.LogWarning("Interstitial ad loaded with response : "
                //           + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlersInterstitial(_interstitialAd);
            });
    }

    private void RegisterEventHandlersInterstitial(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Interstitial ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            // Debug.LogWarning("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            audioManager.UnPauseAllAudio();
            StartCoroutine(RefuseShowInter());
            // ContinueGame("thanks");

            // Debug.LogWarning("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // ContinueGame();
            
            // Debug.LogWarning("Interstitial ad failed to open full screen content " +
            //                "with error : " + error);
            LoadInterstitialAd();
        };
    }

#endregion

#region Reward
    private Coroutine closeHintBannerCoroutine;
    public IEnumerator CountDownForHintSale()
    {
        allowToShowShopBannerHint = true;
        yield return new WaitForSeconds(300); //300
        if(allowToShowShopBannerHint) ShowSHintBanner();
    }

    public void StopCountDownForHintSale()
    {
        allowToShowShopBannerHint = false;
        if(closeHintBannerCoroutine != null) StopCoroutine(closeHintBannerCoroutine);
        // StartCoroutine(CloseHintBanner(0));
    }

    public void ShowSHintBanner()
    {
        if(ShopBanner.activeSelf) 
        {
            Invoke("ShowSHintBanner", 1);
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // StartCoroutine(CountDownForComboDeluxe());
            return;
        };
        ShopHintBanner.SetActive(true);
        RectTransform ShopHintBannerRect =  ShopHintBanner.GetComponent<RectTransform>();

        ShopHintBannerRect.anchoredPosition = new Vector2 (-ShopHintBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71);
        ShopHintBannerRect.DOPause();
        ShopHintBannerRect.DOAnchorPos(new Vector2(0, -71), 0.5f);
        
        // closeHintBannerCoroutine = StartCoroutine(CloseHintBanner());
    }

    IEnumerator CloseHintBanner(int time = 10)
    {
        yield return new WaitForSeconds(time);
        if(ShopHintBanner.activeSelf)
        {
            RectTransform ShopHintBannerRect =  ShopHintBanner.GetComponent<RectTransform>();
            ShopHintBannerRect.DOPause();
            ShopHintBannerRect.DOAnchorPos(new Vector2(ShopHintBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71), 0.5f).OnComplete(() => {
                ShopHintBanner.SetActive(false);
                if(allowToShowShopBannerHint) 
                {
                    // StopCoroutine(closeHintBannerCoroutine);
                    // StartCoroutine(CountDownForHintSale());
                }
            });
        }
    }

    void GetReward ()
    {
        resetTimeInter = true;
        // CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
        // CountDown.SetActive(false);
        audioManager.UnPauseAllAudio();
        switch(nameReward)
        {
            case "addMoreTime":
                lose.AddMoreTime();
                break;
            case "clearAllSlots":
                lose.ClearAllSlots();
                break;
            case "BuyTool":
                shopTool.AddMoreTool();
                break;
            case "all support start":
                dialogPlay.PlayGameWithFullSupport();
                break;
            case "LuckyWheel":
                luckySpin.GetReward();
                break;
            case "add 1 energy":
                buyEnergy.AddOneEnergy();
                break;
            default:
                break;
        }
    }

    public bool CheckRewardAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd()) return true;
        ShowAdsMessage();
        return false;
    }

    public void ShowRewardedAd(string name)
    {
        // const string rewardMsg =
        //     "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            nameReward = name;
            audioManager.PauseAllAudio();
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                // Debug.LogWarning(String.Format(rewardMsg, reward.Type, reward.Amount));
                // GetReward(name);
            });
        }
        else {
            ShowAdsMessage();
            LoadRewardedAd();
        };
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
              _rewardedAd.Destroy();
              _rewardedAd = null;
        }

        // Debug.LogWarning("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adRewardId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    // Debug.LogWarning("Rewarded ad failed to load an ad " +
                                //    "with error : " + error);
                    return;
                }

                // Debug.LogWarning("Rewarded ad loaded with response : "
                //           + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlersReward(_rewardedAd);
            });
    }

    private void RegisterEventHandlersReward(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Rewarded ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            // Debug.LogWarning("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            GetReward();
            // Debug.LogWarning("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Debug.LogWarning("Rewarded ad failed to open full screen content " +
            //                "with error : " + error);
            LoadRewardedAd();
        };
    }

#endregion

#region OpenApp
    // public void StopShowOpenAd()
    // {
    //     stopShowOpenAd = true;
    // }

    public void ShowAppOpenAd()
    {
        // Debug.LogWarning("Rewarded ad full screen content");
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        if(remoteConfigAds.allConfigData.OpenAds == 0) return;
        if((int)saveDataJson.GetData("OpenedMap") < remoteConfigAds.allConfigData.OpenAds) return;


        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            // Debug.LogWarning("Showing app open ad.");
            audioManager.PauseAllAudio();
            _appOpenAd.Show();
        }
        else
        {
            // Debug.LogWarning("App open ad is not ready yet");
            if(waitOpenads)
            {
                LoadAppOpenAd();
                showOpenAd = true;
            }
            else showOpenAd = false;
        }
    }

    void SetOpenAdStatus(bool status = true)
    {
        loadedOpenAd = status;
        // loading.SetOpenAdStatus(status);
    }

    public void ChangeStatusStartGame ()
    {
        waitOpenads = false;
    }

    public void LoadAppOpenAd()
    {
        if (_appOpenAd != null)
        {
              _appOpenAd.Destroy();
              _appOpenAd = null;
        }
        SetOpenAdStatus(false);
        // Debug.LogWarning("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(_adOpenId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogWarning("app open ad failed to load an ad " +
                                   "with error : " + error);
                    countOpenAd++;
                    Debug.LogWarning(countOpenAd);
                    // if(countOpenAd <= 3) Invoke("LoadAppOpenAd", 1f);
                    // LoadAppOpenAd();
                    SetOpenAdStatus(true);
                    return;
                }

                Debug.LogWarning("App open ad loaded with response : "
                          + ad.GetResponseInfo());

                _appOpenAd = ad;
                RegisterEventHandlersOpenAd(ad);
                SetOpenAdStatus(true);
            });
    }

    private void RegisterEventHandlersOpenAd(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("App open ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            // Debug.LogWarning("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            // Debug.LogWarning("App open ad full screen content closed.");
            audioManager.UnPauseAllAudio();
            LoadAppOpenAd();
            // loading.ContinueLoading();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Debug.LogWarning("App open ad failed to open full screen content " +
            //                "with error : " + error);
            if(waitOpenads) LoadAppOpenAd();
            // StartCoroutine(ShowAppOpenAdWithDelay());
        };
    }

    private void OnAppStateChanged(AppState state)
    {
        Debug.LogWarning("App State changed to : "+ state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            // if (IsAdAvailable)
            // {
                showOpenAd = true;
                // StartCoroutine(ShowAppOpenAdWithDelay());
            // }
            // else
        }
    }

    IEnumerator ShowAppOpenAdWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ShowAppOpenAd();
    }

    void Update()
    {
        // if(showOpenAd && loadedOpenAd && !stopShowOpenAd)
        if(!IsLoadFireBase) return;
        if(showOpenAd && loadedOpenAd)
        {
            showOpenAd = false;
            ShowAppOpenAd();
        }
    }

#endregion

#region NativeAds
    [Obsolete]
    private void RequestNativeAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        LoadSettingNative();
        // LoadNewAreaNative();
        // LoadSmallShopNative();
    }

    [Obsolete]
    private void LoadSettingNative()
    {
        AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedSetting;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

        var adRequest = new AdRequest();
        adLoader.LoadAd(adRequest);
    }

    // [Obsolete]
    // private void LoadNewAreaNative()
    // {
    //     AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
    //         .ForNativeAd()
    //         .Build();
    //     adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedNewArea;
    //     adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

    //     var adRequest = new AdRequest();
    //     adLoader.LoadAd(adRequest);
    // }

    // [Obsolete]
    // private void LoadSmallShopNative()
    // {
    //     AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
    //         .ForNativeAd()
    //         .Build();
    //     adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedSmallShop;
    //     adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

    //     var adRequest = new AdRequest();
    //     adLoader.LoadAd(adRequest);
    // }

    // [System.Obsolete]
    [Obsolete]
    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // NativeAdEventArgs
        // Debug.LogWarning("Native ad failed to load: " + args.LoadAdError.GetMessage());
        // Invoke("RequestNativeAd", 10f);
    }


    private void HandleNativeAdLoadedSetting(object sender, NativeAdEventArgs args)
    {
        // this.nativeAdSetting = args.nativeAd;

        // AdIconTextureSetting.texture = nativeAdSetting.GetIconTexture();
        // AdHeadlineSetting.text = nativeAdSetting.GetHeadlineText();
        // AdDescriptionSetting.text = nativeAdSetting.GetBodyText();

        // //register gameobjects with native ads api
        // if (!nativeAdSetting.RegisterIconImageGameObject(AdIconTextureSetting.gameObject))
        // {
        //     // Debug.LogWarning("error registering icon");
        // }
        // if (!nativeAdSetting.RegisterHeadlineTextGameObject(AdHeadlineSetting.gameObject))
        // {
        //     // Debug.LogWarning("error registering headline");
        // }
        // if (!nativeAdSetting.RegisterBodyTextGameObject(AdDescriptionSetting.gameObject))
        // {
        //     // Debug.LogWarning("error registering description");
        // }

        // //disable loading and enable ad object
        // AdLoadedSetting.gameObject.SetActive(true);
        // AdLoadingSetting.gameObject.SetActive(false);
    }

    // private void HandleNativeAdLoadedNewArea(object sender, NativeAdEventArgs args)
    // {
    //     this.nativeAdNewArea = args.nativeAd;

    //     AdIconTextureNewArea.texture = nativeAdNewArea.GetIconTexture();
    //     AdHeadlineNewArea.text = nativeAdNewArea.GetHeadlineText();
    //     AdDescriptionNewArea.text = nativeAdNewArea.GetBodyText();

    //     //register gameobjects with native ads api
    //     if (!nativeAdNewArea.RegisterIconImageGameObject(AdIconTextureNewArea.gameObject))
    //     {
    //         // Debug.LogWarning("error registering icon");
    //     }
    //     if (!nativeAdNewArea.RegisterHeadlineTextGameObject(AdHeadlineNewArea.gameObject))
    //     {
    //         // Debug.LogWarning("error registering headline");
    //     }
    //     if (!nativeAdNewArea.RegisterBodyTextGameObject(AdDescriptionNewArea.gameObject))
    //     {
    //         // Debug.LogWarning("error registering description");
    //     }

    //     //disable loading and enable ad object
    //     AdLoadedNewArea.gameObject.SetActive(true);
    //     AdLoadingNewArea.gameObject.SetActive(false);
    // }

    // private void HandleNativeAdLoadedSmallShop(object sender, NativeAdEventArgs args)
    // {
    //     this.nativeAdSmallShop = args.nativeAd;

    //     AdIconTextureSmallShop.texture = nativeAdSmallShop.GetIconTexture();
    //     AdHeadlineSmallShop.text = nativeAdSmallShop.GetHeadlineText();
    //     AdDescriptionSmallShop.text = nativeAdSmallShop.GetBodyText();

    //     //register gameobjects with native ads api
    //     if (!nativeAdSmallShop.RegisterIconImageGameObject(AdIconTextureSmallShop.gameObject))
    //     {
    //         // Debug.LogWarning("error registering icon");
    //     }
    //     if (!nativeAdSmallShop.RegisterHeadlineTextGameObject(AdHeadlineSmallShop.gameObject))
    //     {
    //         // Debug.LogWarning("error registering headline");
    //     }
    //     if (!nativeAdSmallShop.RegisterBodyTextGameObject(AdDescriptionSmallShop.gameObject))
    //     {
    //         // Debug.LogWarning("error registering description");
    //     }

    //     //disable loading and enable ad object
    //     AdLoadedSmallShop.gameObject.SetActive(true);
    //     AdLoadingSmallShop.gameObject.SetActive(false);
    // }

#endregion

#region  GDPR Consent Message
    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();
    }
#endregion

#region Log Events
    public void LogEvent(string name, string val)
    {
        // if(app != null) Firebase.Analytics.FirebaseAnalytics.LogEvent("Completed_Map", name, val);
    }

    public void LogEvent(string name)
    {
        if(app != null) Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
    }
#endregion

#region Vibration
    // Rung theo mẫu với độ mạnh khác nhau
    public void VibratePattern(long[] pattern, int[] amplitudes, int repeat = -1)
    {
        #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android && pattern.Length == amplitudes.Length)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");

                if (HasVibrationPermission())
                {
                    // Tạo hiệu ứng rung theo mẫu
                    AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, amplitudes, repeat);
                    vibrator.Call("vibrate", vibrationEffect);
                }
            }
        #endif
    }

    // Dừng rung
    public void StopVibration()
    {
        #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrator.Call("cancel");
            }
        #endif
    }

    // Kiểm tra quyền rung
    private bool HasVibrationPermission()
    {
        #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                
                return vibrator.Call<bool>("hasVibrator");
            }
        #endif
        return false;
    }

    // Rung một lần với độ mạnh và thời gian tùy chỉnh
    public void Vibrate(long milliseconds, int amplitude)
    {
        #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                
                if (HasVibrationPermission())
                {
                    // Tạo hiệu ứng rung với amplitude từ 1-255
                    AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, Mathf.Clamp(amplitude, 1, 255));
                    vibrator.Call("vibrate", vibrationEffect);
                }
            }
        #endif
    }

    IEnumerator PatternExample()
    {
        yield return new WaitForSeconds(1f);

        // Mẫu rung: [thời gian chờ, thời gian rung, thời gian chờ, thời gian rung]
        long[] pattern = new long[] { 0, 100, 100, 200, 100, 300 };
        // Độ mạnh tương ứng cho mỗi khoảng thời gian
        int[] amplitudes = new int[] { 0, 50, 0, 100, 0, 200 };
        
        // -1 nghĩa là không lặp lại mẫu
        VibratePattern(pattern, amplitudes, -1);
    }

    // public void ChangeStatusOfVibration(bool status)
    // {
    //     allowVibration = status;
    // }

    public void VibrationDevice (int num = 0)
    {
        if(!(bool)saveDataJson.GetData("Vibration")) return;
        if(num == 0) Vibrate(65, 100);
        else if(num == 1) PatternVibrate2();


        // Use Vibration.Vibrate(); for a classic default ~400ms vibration

        // Pop vibration: weak boom (For iOS: only available with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibratePop();

        // Peek vibration: strong boom (For iOS: only available on iOS with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibratePeek();

        // Nope vibration: series of three weak booms (For iOS: only available with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibrateNope();

        // Vibration.VibrateNope();


        // Rung một lần: 500ms với độ mạnh 200
        // Vibrate(50, 100);

        // Rung theo mẫu
        // StartCoroutine(PatternExample());
    }

    void PatternVibrate1()
    {
        // Mẫu rung: [thời gian chờ, thời gian rung, thời gian chờ, thời gian rung]
        long[] pattern = new long[] { 0, 10, 20, 40, 20, 10 };
        // Độ mạnh tương ứng cho mỗi khoảng thời gian
        int[] amplitudes = new int[] { 0, 30, 45, 60, 45, 30 };
        
        // -1 nghĩa là không lặp lại mẫu
        VibratePattern(pattern, amplitudes, -1);
    }

    void PatternVibrate2()
    {
        long[] pattern = new long[] { 0, 10, 20, 40, 20, 10 };
        int[] amplitudes = new int[] { 0, 50, 80, 120, 80, 50 };

        VibratePattern(pattern, amplitudes, -1);
    }
#endregion Vibration

#region OtherTasks
    public void ShowAdsMessage(string txt = "No ads available")
    {
        AdsMessage.SetActive(true);
        Image AdsMessageImage = AdsMessage.GetComponent<Image>();
        TextMeshProUGUI AdsMessageText = AdsMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        AdsMessageText.text = txt;

        AdsMessageImage.DOPause();
        AdsMessageText.DOPause();

        AdsMessageImage.DOFade(0.83f, 0.5f);
        AdsMessageText.DOFade(1f, 0.5f);
        AdsMessageImage.DOFade(0f, 0.5f).SetDelay(2f);
        AdsMessageText.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(() => {AdsMessage.SetActive(false);});
    }

    public void Rate()
    {
        audioManager.PlaySFX("click");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.vnstart.triple.city");
    }

    public void ShareLink()
    {
#if UNITY_ANDROID

        if (!isProcessing)
        {
            audioManager.PlaySFX("click");
            StartCoroutine(ShareTextInAnroid());
        }

#else
		Debug.Log("No sharing set up for this platform.");
#endif
    }

    public void MoreGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=Vnstart+LLC");
    }

    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }


#if UNITY_ANDROID
    public IEnumerator ShareTextInAnroid()
    {
        var shareSubject = "Play Find It on your phone"; //Subject text
        var shareMessage = "Get game from this link: " + //Message text
                           "https://play.google.com/store/apps/details?id=com.vnstart.triple.city"; //Your link

        isProcessing = true;

        if (!Application.isEditor)
        {
            //Create intent for action send
            AndroidJavaClass intentClass =
                new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject =
                new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>
                ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //put text and subject extra
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");

            intentObject.Call<AndroidJavaObject>
                ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>
                ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            //call createChooser method of activity class
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            AndroidJavaObject currentActivity =
                unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser =
                intentClass.CallStatic<AndroidJavaObject>
                ("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }

        yield return new WaitUntil(() => isFocus);
        isProcessing = false;
    }
#endif


    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://vnstart.com/privacy-policy.html");
    }

    public void OpenTermsOfService()
    {
        Application.OpenURL("https://policies.google.com/terms");
    }

#endregion
}

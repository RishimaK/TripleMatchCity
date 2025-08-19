using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioManager audioManager;
    public AdsManager adsManager;
    public SaveDataJson saveDataJson;
    public GameObject GameUI;
    public GameObject MiniGameUI;
    public GameObject Game;
    public Setting setting;
    public Shop shop;
    public TextMeshProUGUI lvTxt;
    public TextMeshProUGUI energyTxt;
    private bool enabledToClick = true;

    public SkeletonGraphic LvChest;
    public SkeletonGraphic StarChest;
    public ChestStar chestStar;
    public LevelChest levelChest;

    public DailyReward dailyReward;
    public DialogPlay dialogPlay;
    public DialogRemoveAds dialogRemoveAds;
    public Sale dialogSale;
    public Button BtnMiniGame;
    public Image PlayBtn;
    public GameObject SaleDialog;
    public Rate rate;
    public LuckySpin luckySpin;
    public GameObject luckySpinBtn;


    [Header("Sprites")]
    public Sprite SpriteNormalGame;
    public Sprite SpriteHardGame;

    public Sprite SpriteNormalGameInDialog;
    public Sprite SpriteHardGameInDialog;

    public Sprite SpriteNormalGameDialog;
    public Sprite SpriteHardGameDialog;
    

    void Awake()
    {
        if (starsAmount == 0) starsAmount = 3;

        initialPos = new Vector2[starsAmount];
        initialRotation = new Quaternion[starsAmount];
        
        for (int i = 0; i < pileOfStars.transform.childCount; i++)
        {
            initialPos[i] = pileOfStars.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            initialRotation[i] = pileOfStars.transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }
    }

    void Start()
    {
        // Application.targetFrameRate = 60;
        // saveDataJson.SaveData("OpenedMap", 3);
        // saveDataJson.SaveData("PlayAgain", true);
        // saveDataJson.SaveData("Energy", 99);
        int OpenedMap = (int)saveDataJson.GetData("OpenedMap");
        setting.CheckValue();
        SetLevel();
        if(OpenedMap == 1) EnterGame();
        else dailyReward.CheckDailylyReward();

        if(OpenedMap < 7) BtnMiniGame.interactable = false;
        CheckChallengeMapImage();
    }

    public void CheckSptriteLevel(int lv, Image btn, Image dialog, bool isChallenge = false)
    {
        string txt = "Normal";
        switch (lv)
        {
            case 12: case 22: case 30: case 39: case 49:
                txt = "Hard";
                break;
        }

        if(txt == "Hard" && !isChallenge) 
        {
            if(dialog != null) 
            {
                dialog.sprite = SpriteHardGameDialog;
                btn.sprite = SpriteHardGameInDialog;
            }
            else btn.sprite = SpriteHardGame;
            return;
        }

        if(dialog != null) 
        {
            dialog.sprite = SpriteNormalGameDialog;
            btn.sprite = SpriteNormalGameInDialog;
        }
        else btn.sprite = SpriteNormalGame;
    }

    public void SetLevel()
    {
        int mapNum = (int)saveDataJson.GetData("OpenedMap");
        if(mapNum >= 9) luckySpinBtn.SetActive(true);
        if(saveDataJson.TakeMapData().map.Length == mapNum) mapNum--;

        lvTxt.text = $"{mapNum}";
        CheckSptriteLevel(mapNum, PlayBtn, null);
    }

    void CountCoins()
    {
        shop.CountCoins(10);
    }

    public void PlayChestAnimation(int mapNum)
    {
        // Invoke("PlayStarChestAnimation", 0.5f);
        // StartCoroutine(PlayStarChestAnimation());
        audioManager.ChangeMusicBackground("bgm_home");

        int star = chestStar.AddMoreStar();
        if(mapNum == 1) 
        {
            shop.AddMoreStuff(10);
            return;
        }
        if(star < 20) levelChest.CheckLevelReward();
        CountStars();
        Invoke("CountCoins", 1f);

        if(mapNum == 2) dailyReward.CheckDailylyReward();
        else if(mapNum == 3) Debug.Log("Open diaglog removeads");
        else if(mapNum == 5) dialogSale.OpenAnimation();
        else if(mapNum == 6) BtnMiniGame.interactable = true;
        else if(mapNum == 8) 
        {
            luckySpinBtn.SetActive(true);
            luckySpin.OpenDialog();
        }
        rate.CheckRate(mapNum);
        adsManager.LoadBannerAd();
    }

    IEnumerator PlayStarChestAnimation()
    {
        int starChest = (int)saveDataJson.GetData("StarChest");
        // if(starChest >= 20) enabledToClick = false;
        yield return new WaitForSeconds(0.5f);
        StarChest.AnimationState.SetAnimation(0, "update", false);
        yield return new WaitForSeconds(StarChest.Skeleton.Data.FindAnimation("update").Duration);
        if(starChest >= 20) chestStar.PlayRewardAnimation();
        StarChest.AnimationState.SetAnimation(0, "animation", true);
        // Debug.Log(StarChest.Skeleton.Data.FindAnimation("update").Duration);
    }

    // public IEnumerator WaitForAnimationEnd(string animationName)
    // {
    //     skeletonGraphic.AnimationState.SetAnimation(0, animationName, false);

    //     // Chờ cho đến khi animation kết thúc
    //     while (true)
    //     {
    //         var currentTrack = skeletonGraphic.AnimationState.GetCurrent(0);
    //         if (currentTrack == null || currentTrack.IsComplete)
    //         {
    //             yield break;
    //         }
    //         yield return null;
    //     }
    // }

    public void OpenDialogPlay()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        int energy = (int)saveDataJson.GetData("Energy");
        string infiniteEnergy = (string)saveDataJson.GetData("InfiniteEnergy");

        if(energy <= 0 && (infiniteEnergy == null || infiniteEnergy == "")) 
        {
            adsManager.ShowAdsMessage("Out of energy");
            return;
        };

        if((int)saveDataJson.GetData("OpenedMap") < 5) EnterGame();
        else dialogPlay.OpenAnimation((int)saveDataJson.GetData("OpenedMap"));
    }

    public void EnterGame(bool thunder = false, bool addTime = false)
    {
        gameObject.SetActive(false);
        GameUI.SetActive(true);
        Game.SetActive(true);
        adsManager.ShowInterstitialAd((int)saveDataJson.GetData("OpenedMap"), 0);
        Game.GetComponent<MainGame>().LoadGame(thunder, addTime);
    }

    public void EnterMiniGame(int map)
    {
        gameObject.SetActive(false);
        MiniGameUI.SetActive(true);
        Game.SetActive(true);
        adsManager.ShowInterstitialAd((int)saveDataJson.GetData("OpenedMap"), 0);
        Game.GetComponent<MiniGame>().LoadGame(map);
    }

#region Challenge
    public Sprite Challenge1;
    public Sprite Challenge2;
    public Sprite Challenge3;
    public Sprite Challenge4;
    public Sprite Challenge5;
    public Sprite ChallengeGray1;
    public Sprite ChallengeGray2;
    public Sprite ChallengeGray3;
    public Sprite ChallengeGray4;
    public Sprite ChallengeGray5;

    public GameObject ListChallenge;
    public void CheckChallengeMapImage()
    {
        ListChallenge.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -5000);
        List<int> CompletedChallengeMap = (List<int>)saveDataJson.GetData("CompletedChallengeMap");
        Transform mapTransform;
        for (int i = 0; i < CompletedChallengeMap.Count; i++)
        {
            mapTransform = ListChallenge.transform.GetChild(CompletedChallengeMap[i] - 1);
            mapTransform.GetChild(0).GetComponent<Image>().sprite = 
                i == 0 ? Challenge1 : i == 1 ? Challenge2 : i == 2 ? Challenge3 : i == 3 ? Challenge4 : Challenge5;
            mapTransform.GetChild(1).gameObject.SetActive(false);
            mapTransform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void ChangeMapChallengeImage(int map)
    {
        Transform mapTransform = ListChallenge.transform.GetChild(map - 1);
        if(!mapTransform.GetChild(1).gameObject.activeSelf) return;
        mapTransform.GetChild(0).GetComponent<Image>().sprite = 
            map == 1 ? Challenge1 : map == 2 ? Challenge2 : map == 3 ? Challenge3 : map == 4 ? Challenge4 : Challenge5;
        mapTransform.GetChild(1).gameObject.SetActive(false);
        mapTransform.GetChild(2).gameObject.SetActive(true);
    }

    public void OpenDialogPlayChallenge(int map)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        int energy = (int)saveDataJson.GetData("Energy");
        string infiniteEnergy = (string)saveDataJson.GetData("InfiniteEnergy");

        if(energy <= 0 && (infiniteEnergy == null || infiniteEnergy == "")) 
        {
            adsManager.ShowAdsMessage("Out of energy");
            return;
        };

        dialogPlay.OpenAnimation((int)saveDataJson.GetData("OpenedMap"), map);
    }

    public void EnterChallengeGame(int map, bool thunder = false, bool addTime = false)
    {
        gameObject.SetActive(false);
        adsManager.ShowInterstitialAd((int)saveDataJson.GetData("OpenedMap"), 0);
        GameUI.SetActive(true);
        Game.SetActive(true);
        Game.GetComponent<ChallengeGame>().LoadGame(map, thunder, addTime);
    }
#endregion

#region Star Collecting Animation
    [Header("Star Collecting Animation")]
    [SerializeField] private GameObject pileOfStars;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int starsAmount = 3;

   public void CountStars()
    {
        pileOfStars.SetActive(true);
        var delay = 0f;
        Transform target = StarChest.transform;
        int pileOfStarsLength = pileOfStars.transform.childCount;
        for (int i = 0; i < pileOfStarsLength; i++)
        {
            Transform child = pileOfStars.transform.GetChild(i);
            bool allowCheck = false;
            if(i == pileOfStarsLength - 1) allowCheck = true;
            child.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
            child.DOMove(target.position, 0.8f).SetDelay(delay + 0.5f)
                .SetEase(Ease.InBack).OnComplete(() => audioManager.PlaySFX("collect"));

            child.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            child.DOScale(0f, 0.3f).SetDelay(delay + 1.3f).SetEase(Ease.OutBack)
                .OnComplete(() => {
                    ResetStar(child);
                    // if(allowCheck) chestStar.CheckRewardChestStart();
                    if(allowCheck) StartCoroutine(CheckSaleDialog());
                });
            delay += 0.1f;
            target.DOScale(1.1f, 0.1f).SetLoops(3,LoopType.Yoyo).SetEase(Ease.InOutSine)
                .SetDelay(1.2f).OnComplete(() => target.localScale = new Vector3(1,1,1));
        }
    }

    void ResetStar(Transform item)
    {
        int i = item.GetSiblingIndex();
        item.GetComponent<RectTransform>().anchoredPosition = initialPos[i];
        item.GetComponent<RectTransform>().rotation = initialRotation[i];
    }

    IEnumerator CheckSaleDialog()
    {
        yield return new WaitUntil(() => SaleDialog.activeSelf == false);
        chestStar.CheckRewardChestStart();
    }
#endregion
}

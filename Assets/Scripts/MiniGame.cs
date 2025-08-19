using UnityEngine;
using TMPro;
using System;
using Spine.Unity;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MiniGame : MonoBehaviour
{
    public AdsManager adsManager;
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;
    private SaveDataJson.Minigame MapData;

    // private float leftLimit;
    // private float rightLimit;
    // private float bottomLimit;
    // private float upperLimit;
    private float leftLimit = -4.184f;
    private float rightLimit = 4.184f;
    private float bottomLimit = -6.34f;
    private float upperLimit = 10.34f;

    private float leftLimit2 = 0;
    private float bottomLimit2 = 0;

    private float defaultRightLimit;
    private float defaultBottomLimit;

    private bool moveAllowed;
    private bool beTouched = true;
    private bool _draggMove = false;
    private bool onSpecialTop = false;
    private bool _isDragActive = false;
    private bool _isScreenChange = false;
    private bool addMoreItem = false;
    private bool showAllItem = false;
    // private bool isChallenge = false;
    private bool isGameComplete = false;
    private bool mapAction = true;
    private bool isTutorial = false;

    private Vector3 offset;
    private Vector3 offset2;
    private int fistFingerId = 0;
    private Vector3 touchPos;
    private Vector3 _worldPosition;

    private Vector2 _checkPos;
    private Vector2 _screenPosition;


    private dragable1 _lastDragged; 

    public Canvas canvas;

    public Camera cam;

    public ImageResources imageResources;

    private float zoomMin = 3;
    private float zoomMax = 7;

    private int totalItem = 0;
    private int totalCurent = 0;
    public int MapNum;
    // private int gift;
    private int currentTotalItem = 0;

    public Shop shop;

    public GameObject miniGame;
    public GameObject Compass;
    public GameObject hand;
    public GameObject home;
    public GameObject info;
    // public GameObject mapGame;
    public GameObject listMap;
    public GameObject NewArea;
    public GameObject btnDown;
    public GameObject areaName;
    public GameObject InterAds;
    public GameObject infoItem;
    public GameObject listCloud;
    public GameObject gameScreen;
    public GameObject ItemPrefab;
    public GameObject totalInTrophy;
    public GameObject ListItemToFind;
    public GameObject completeDialog;
    public GameObject totalInStatistical;
    public GameObject wrongItem;
    public GameObject Limited;
    // public GameObject OpenedGift;
    public GameObject PoolItem;
    public GameObject MaskUI;
    public GameObject LightEff;
    public GameObject ListParticle;
    public GameObject MainParticle;
    // public GameObject cloudChangeScreen;
    // private GameObject ChallengeItem;

    public GameObject CircleTutotialComPass;
    public Sale sale;

    private GameObject ListToFind;
    private GameObject ListItemFinded;
    private GameObject HiddenItemList;
    public Slider ProgressInTrophy;
    public Slider ProgressInStatistical;

    public ShopTool shopTool;
    public TextMeshProUGUI hint;
    public TextMeshProUGUI compassTxt;
    // public TextMeshProUGUI GiftIconTxt;
    public GameObject tutorialTxt;

    private Vector3 testPos;
    private string tutorialItem = "pizza (15)";
    public GameObject handTutorial;
    public GameObject ZoomTutorial;
    public GameObject TutorialListItem;
    private bool lockAllItem = false;

    void Awake()
    {
        MiniGame[] controllers = FindObjectsOfType<MiniGame>();
        if(controllers.Length > 1)
        {
            Destroy(gameObject);
        }
        
        // transform.DOShakePosition(5, 5, 10, 90f);

        // SceneManager.LoadSceneAsync("Name scene");

        // Application.Quit();
        // item di chuyển 3 đơn vị mỗi giây
        // Item.transform.DOMove(new Vector3(2,0,0), 3).SetSpeedBased(true);
        SetDefaultLimit();
    }
    void Start()
    {
    }

    void SetListItemToFind()
    {
        int listItemToFindLength = ListItemToFind.transform.parent.childCount;
        if(listItemToFindLength <= 1) return;
        for (int i = 1; i < listItemToFindLength;)
        {
            listItemToFindLength--;
            Transform child = ListItemToFind.transform.parent.GetChild(i);
            child.SetParent(ListItemToFind.transform);
        }

        for (int i = 0; i< ListItemToFind.transform.childCount; i++)
        {
            RectTransform child = ListItemToFind.transform.GetChild(i).GetComponent<RectTransform>();
            child.anchoredPosition = new Vector2(45 + 120 * i, child.anchoredPosition.y);
        }
    }

    private void SetValue()
    {
        string[] itemList = MapData.ItemList;
        int[] QuantityList = MapData.QuantityList;
        int numItem = itemList.Length;
        int findedItem = 0;
        ListItemToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(120f * numItem, 130);

        for(int i = 0; i < numItem; i++)
        {
            GameObject newItem = ObjectPoolManager.SpawnObject(ItemPrefab, Vector3.zero,Quaternion.identity);
            RectTransform itemTransform = newItem.GetComponent<RectTransform>();
            itemTransform.SetParent(ListItemToFind.transform);
            itemTransform.sizeDelta = new Vector2(90,90);
            itemTransform.localScale = new Vector3(1,1,1);
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(45 + 120 * i, 11);

            Image img = newItem.transform.GetChild(1).GetComponent<Image>();
            img.sprite = imageResources.TakeImage(imageResources.ListMiniGameItem, itemList[i]);
            img.SetNativeSize();

            newItem.name = itemList[i];
 
            int count = 0;
            if(ListItemFinded.transform.childCount > 0){
                foreach (Transform obj in ListItemFinded.transform)
                {
                    if(newItem.name == obj.gameObject.name.Split(" (")[0]) count++;
                }
            }
            findedItem += count;
            newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{count}/{QuantityList[i]}";
            if(count == QuantityList[i]) {
                newItem.transform.GetChild(2).gameObject.SetActive(true);
                itemTransform.SetParent(ListItemToFind.transform.parent);
            }
        }
        totalItem = 0;
        foreach(int num in QuantityList) totalItem += num;

        for (int i = 1; i < MapData.Milestones.Length; i++)
        {
            int child = MapData.Milestones[i];
            if (findedItem < child) {
                totalCurent = child;
                break;
            }
            else if(i == MapData.Milestones.Length - 1)
            {
                totalCurent = totalItem;
                break;
            }
        }

        currentTotalItem = findedItem;

        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{findedItem}/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{findedItem}/{totalCurent}";
        SetProgressBar(findedItem, totalCurent, "set");

        SetListItemToFind();
        isGameComplete = false;
        if(findedItem == totalCurent) isGameComplete = true;
    }

    public void SetProgressBar(float curent, float total, string txt = "") 
    {
        float val = curent / total;

        if(txt == "set")
        {
            ProgressInTrophy.value = val;
            ProgressInStatistical.value = val;
        }
        else
        {
            ProgressInTrophy.DOPause();
            ProgressInStatistical.DOPause();
            ProgressInTrophy.DOValue(val, 0.2f, false);
            ProgressInStatistical.DOValue(val, 0.2f, false);
        }
    }

    void CheckItemInMap()
    {
        List<string> itemsFinded = (List<string>) saveDataJson.GetData($"ItemMap{MapNum}");
        List<string> itemsNeedToShow = (List<string>) saveDataJson.GetData($"ShowHiddenItems{MapNum}");

        if(itemsFinded == null || itemsFinded.Count == 0) return;
        foreach (string name in itemsFinded)
        {
            Transform child = ListToFind.transform.Find(name);
            if(!child) child = HiddenItemList.transform.Find(name);
            if(!child) continue;
            child.gameObject.SetActive(false);
            child.SetParent(ListItemFinded.transform);
        }

        if(itemsNeedToShow.Count > 0){
            foreach (string name in itemsNeedToShow)
            {
                Transform child = HiddenItemList.transform.Find(name);
                if(!child) continue;
                child.gameObject.SetActive(true);
                child.SetParent(ListToFind.transform);
            }
        }
    }

    void ShowFooter()
    {
        if(btnDown.activeSelf) return;

        Transform footer = btnDown.transform.parent.parent;
        footer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 140);

        btnDown.SetActive(true);
        footer.Find("Status/Up").gameObject.SetActive(false);
        footer.Find("Statistical").gameObject.SetActive(false);

        GameObject btnSetting = gameScreen.transform.Find("Bt_Setting").gameObject;
        GameObject btnHome = gameScreen.transform.Find("Bt_home").gameObject;
        // GameObject giftIcon = gameScreen.transform.Find("GiftIcon").gameObject;
        btnSetting.SetActive(true);
        btnHome.SetActive(true);

        btnSetting.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, btnSetting.GetComponent<RectTransform>().anchoredPosition.y);
        // giftIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, giftIcon.GetComponent<RectTransform>().anchoredPosition.y);
        btnHome.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, btnHome.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void LoadGame(int mapNum) 
    {
        cam.orthographicSize = 7;
        adsManager.LogEvent($"Enter_MiniGame_Map_{MapNum}");
        audioManager.ChangeMusicBackground("bgm_findit");

        gameObject.GetComponent<MainGame>().enabled = false;
        gameObject.GetComponent<TouchControl>().enabled = false;
        gameObject.GetComponent<MiniGame>().enabled = true;
        miniGame.SetActive(true);
        // GameObject.Find($"Game/MiniGame").SetActive(true);

        beTouched = false;
        MapData = saveDataJson.TakeMiniGameData().map[mapNum];
        MapNum = mapNum;
        ListToFind = GameObject.Find($"Game/MiniGame/Map/Map{mapNum}/AvailableItemList");
        ListItemFinded = GameObject.Find($"Game/MiniGame/Map/Map{mapNum}/ListItemFinded");
        HiddenItemList = GameObject.Find($"Game/MiniGame/Map/Map{mapNum}/HiddenItemList");

        ListToFind.SetActive(true);

        bool[] a = (bool[])saveDataJson.GetData("AddMoreItem");
        bool[] b = (bool[])saveDataJson.GetData("ShowAllItem");
        addMoreItem = a.Length < mapNum ? false :  a[mapNum - 1];
        showAllItem = b.Length < mapNum ? false :  b[mapNum - 1];

        ChangeNameArea(MapData.AreaName);
        listMap.transform.Find($"Map{mapNum}").gameObject.SetActive(true);

        hint.text = $"{(int)saveDataJson.GetData("Hint")}";
        compassTxt.text = $"{(int)saveDataJson.GetData("CompassMiniGame")}";

        CheckItemInMap();
        SetValue();
        checkLimited();

        AfterChangeScreenAnimation();
    }

    public void AfterChangeScreenAnimation()
    {
        home.SetActive(false);
        gameScreen.SetActive(true);
        CheckItemToOpenArea();

        // if(MapNum >= 2 || (int)saveDataJson.GetData("OpenedMiniGameMap") >= 2) 
        // {
        //     // StartCoroutine(adsManager.CountDownForComboDeluxe());
        //     compassTxt.transform.parent.gameObject.SetActive(true);
        // }
        // if(MapNum >= 3 || (int)saveDataJson.GetData("OpenedMiniGameMap") >= 3) adsManager.StartCountDownInter();
        // StartCoroutine(adsManager.CountDownForHintSale());
    }

    void ChangeNameArea (string txt)
    {
        areaName.GetComponent<TextMeshProUGUI>().text = txt;
    }

    void SetDefaultLimit()
    {
        // cam.orthographicSize = 7;
        float _height = 2f * 7;
        float _width = _height * cam.aspect;
        defaultRightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
        defaultBottomLimit = -(Limited.transform.GetChild(2).position.y - _height / 2);
    }


    void CheckItemToOpenArea()
    {   
        if(isGameComplete) {
            // if(cloudAnimation.gameObject.activeSelf)
            // {
            //     Invoke("CheckItemToOpenArea", Time.deltaTime);
            //     return;
            // }
            PlayCloundAnimation();
            return;    
        }
        if(totalCurent == totalItem) PlayAnimationCloud(3, "Start");
        for (int i = MapData.Milestones.Length - 1; i >= 0; i--)
        {
            if (MapData.Milestones[i] <= totalCurent) PlayAnimationCloud(i, "Start");
        }
    }

    void OpenNewArea(int areaNum = 0) 
    {
        if(areaNum == 4) return;
        addMoreItem = true;
        saveDataJson.SaveData("AddMoreItem", addMoreItem, MapNum);
        NewArea.SetActive(true);
        NewArea.GetComponent<NewArea>().SetImage(areaNum, MapNum);
    }

    public void PlayAnimationCloud(int areaNum = 0, string txt = null)
    {
        if(txt == "StayCamera") CloudAnimation(areaNum);
        else 
        {
            beTouched = false;
            // buttonControl.OffBtn();
            Vector3 pos = new Vector3(3.75f, 6.675f, transform.position.z);
            if(areaNum == 1) pos = new Vector3(-3.75f, 6.675f, transform.position.z);
            else if(areaNum == 2) pos = new Vector3(3.75f, -6.675f, transform.position.z);
            else if(areaNum == 3) pos = new Vector3(-3.75f, -6.675f, transform.position.z);
            pos = new Vector3
            (
                x:Mathf.Clamp(value:pos.x, min:leftLimit, max:rightLimit),
                y:Mathf.Clamp(value:pos.y, min:bottomLimit, max:upperLimit),
                pos.z
            );
            if(txt == "Start"){
                transform.position = pos;
                CloudAnimation(areaNum);
            } else
                transform.DOMove(pos, 0.5f).SetDelay(0.5f).OnComplete(() => CloudAnimation(areaNum));
        };
    }

    void CloudAnimation(int areaNum)
    {
        Transform clouds = listCloud.transform.GetChild(areaNum);
        audioManager.PlaySFX("cloud_open");
        for(int i = 0; i < 3; i++)
        {
            SkeletonAnimation cloud = clouds.GetChild(i).GetComponent<SkeletonAnimation>();
            cloud.timeScale = 1;
            clouds.GetChild(i).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Tan", false);
            if(i == 0) Invoke("AnimationCompleteHandler", 1.167f);
        }
    }

    void AnimationCompleteHandler()
    {
        // if((bool)saveDataJson.GetData("PlayTutorial"))
        // {
        //     gameScreen.transform.Find("Footer/Hint").gameObject.SetActive(false);
        //     // OpenTutorial();
        // }else
        // {
            mapAction = true;
            beTouched = true;
            // buttonControl.OnBtn();
            if(MapNum == 2 && (int)saveDataJson.GetData("OpenedMiniGameMap") == 2 && ((string[])saveDataJson.GetData($"ItemMap{MapNum}")).Length == 0)
            {
                compassTxt.transform.parent.gameObject.SetActive(true);
                // PlayCompassTutorial();
            }
        // }
    }

    void checkLimited() 
    {
        int[] Milestones = MapData.Milestones;

        if(totalCurent == Milestones[0])
        {
            leftLimit = 3f;
            // rightLimit = 4.184f;
            rightLimit = defaultRightLimit;
            bottomLimit = 6f;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }
        else if(totalCurent == Milestones[1])
        {
            // leftLimit = -4.184f;
            // rightLimit = 4.184f;
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = 6f;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }
        else if(totalCurent == Milestones[2]){
            // leftLimit = -4.184f;
            // rightLimit = 4.184f;
            // bottomLimit = -6.34f;
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = defaultBottomLimit;
            upperLimit = 10.34f;
            leftLimit2 = 3f;
            bottomLimit2 = 6f;
        }
        else
        {
            // leftLimit = -4.184f;
            // rightLimit = 4.184f;
            // bottomLimit = -6.34f;
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = defaultBottomLimit;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }

        if(cam.orthographicSize != 7){
            cam.orthographicSize = 7;
        }
        ChangeLimited(7);
    }
#region Tutorial
    // void OpenTutorial()
    // {
    //     if(MaskUI.activeSelf) return;
    //     isTutorial = true;
    //     mapAction = false;
    //     Transform target = ListToFind.transform.Find(tutorialItem);

    //     if(target == null) 
    //     {
    //         beTouched = true;
    //         buttonControl.OnBtn();
    //         // gameScreen.transform.Find("Footer/Hint").gameObject.SetActive(true);
    //         // saveDataJson.SaveData("PlayTutorial", false);
    //         OpenHintListItem();
    //         return;
    //     }
    //     MaskUI.SetActive(true);
    //     Transform mask = MaskUI.transform.GetChild(0);
    //     mask.position = target.position;
    //     MaskUI.transform.GetChild(1).SetParent(mask);
    //     mask.GetComponent<RectTransform>().localScale = new Vector3(10,10,1);
    //     tutorialTxt.SetActive(true);
    //     tutorialTxt.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
    //         CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Tap Item");
    //     mask.DOPause();
    //     mask.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutCubic).OnComplete(() => {
    //         beTouched = true;
    //         PlayHandTutorial(target);
    //     });
    // }

    // public void CloseMask(string val = "")
    // {
    //     if(!handTutorial.activeSelf && !MaskUI.activeSelf) return;
    //     if(val == "touch") beTouched = true;
    //     else beTouched = false;
    //     handTutorial.SetActive(false);
    //     MaskUI.SetActive(false);
    //     tutorialTxt.SetActive(false);
    //     MaskUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(10f,10f,1f);
    //     Transform hintBtn = gameScreen.transform.Find("Footer/Hint");
    //     Transform compassBtn = gameScreen.transform.Find("Footer/CompassBtn");
    //     if(hintBtn.gameObject.activeSelf)
    //     {
    //         hintBtn.GetChild(0).gameObject.SetActive(true);
    //         hintBtn.GetChild(1).gameObject.SetActive(false); 
    //     }
    //     if(compassBtn.gameObject.activeSelf)
    //     {
    //         // buttonControl.DeclineClickCompassTut();
    //         hintBtn.gameObject.SetActive(true);
    //         compassBtn.GetChild(0).gameObject.SetActive(true);
    //         compassBtn.GetChild(1).gameObject.SetActive(false); 
    //     }

    //     if(!isTutorial)
    //     {
    //         mapAction = true;
    //         beTouched = true;
    //     }
    // }

    // void OpenHintListItem()
    // {
    //     TutorialListItem.SetActive(true);
    //     RectTransform tutorialTxt = TutorialListItem.transform.GetChild(1).GetComponent<RectTransform>();
    //     beTouched = true;
    //     PlayTutorialTxtAnimation(tutorialTxt);
    // }

    // void PlayTutorialTxtAnimation(RectTransform target, int h = 492)
    // {
    //     target.DOAnchorPos(new Vector2(target.anchoredPosition.x, h), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => {
    //         h = h == 492 ? 466 : 492;
    //         PlayTutorialTxtAnimation(target, h);
    //     });
    // }

    // void OpenHintTutorial()
    // {
    //     mapAction = false;
    //     buttonControl.OffBtn();
    //     beTouched = false;
    //     Transform hintBtn = gameScreen.transform.Find("Footer/Hint");
    //     hintBtn.GetChild(0).gameObject.SetActive(false);
    //     hintBtn.GetChild(1).gameObject.SetActive(true);

    //     // buttonControl.AllowClickCompassTut();

    //     hintBtn.gameObject.SetActive(true);
    //     shop.AddHint("tutorial");
    //     tutorialTxt.SetActive(true);
    //     tutorialTxt.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
    //         CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Tap Hint");

    //     MaskUI.SetActive(true);
    //     Transform mask = MaskUI.transform.GetChild(0);

    //     mask.DOPause();
    //     if(MaskUI.transform.childCount == 2) MaskUI.transform.GetChild(1).SetParent(mask);

    //     mask.position = new Vector3(hintBtn.position.x, hintBtn.position.y, mask.position.z);
    //     MaskUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(10f,10f,1f);
    //     mask.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutCubic).OnComplete(() => {
    //         saveDataJson.SaveData("PlayTutorial", false);
    //         PlayHandTutorial(hintBtn);
    //         isTutorial = false;
    //     });
    // }

    // void CheckIsCompassTutorial(GameObject item)
    // {
    //     if(!handTutorial.activeSelf && !MaskUI.activeSelf) return;
    //     CircleTutotialComPass.SetActive(true);

    //     CircleTutotialComPass.transform.position = item.transform.position;
    //     HandTutorialAnimation(CircleTutotialComPass, 1);
    // }

    // void PlayCompassTutorial()
    // {
    //     if(MaskUI.activeSelf) return;
    //     mapAction = false;
    //     buttonControl.OffBtn();
    //     beTouched = false;
    //     Transform compassBtn = gameScreen.transform.Find("Footer/CompassBtn");
    //     compassBtn.GetChild(0).gameObject.SetActive(false);
    //     compassBtn.GetChild(1).gameObject.SetActive(true);
    //     gameScreen.transform.Find("Footer/Hint").gameObject.SetActive(false);
    //     compassBtn.gameObject.SetActive(true);

    //     shop.AddMoreStuff(0,0,3);
    //     tutorialTxt.SetActive(true);
    //     tutorialTxt.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
    //         CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Tap Compass");

    //     MaskUI.SetActive(true);
    //     Transform mask = MaskUI.transform.GetChild(0);

    //     mask.DOPause();
    //     if(MaskUI.transform.childCount == 2) MaskUI.transform.GetChild(1).SetParent(mask);

    //     mask.position = new Vector3(compassBtn.position.x, compassBtn.position.y, mask.position.z);
    //     MaskUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(10f,10f,1f);
    //     mask.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutCubic).OnComplete(() => {
    //         saveDataJson.SaveData("PlayTutorial", false);
    //         PlayHandTutorial(compassBtn);
    //         isTutorial = false;
    //     });
    // }

    // void OpenTutorialZoom()
    // {
    //     ZoomTutorial.SetActive(true);
    // }

    // public void EndTutorial()
    // {
    //     beTouched = true;
    //     mapAction = true;
    //     ZoomTutorial.SetActive(false);
    //     buttonControl.OnBtn();
    // }

    // void PlayHandTutorial(Transform target)
    // {
    //     handTutorial.SetActive(true);
    //     handTutorial.transform.DOPause();

    //     handTutorial.transform.position = target.position - new Vector3(0.5f, 0.5f, 0);
    //     HandTutorialAnimation(handTutorial, -1);
    // }

    // void HandTutorialAnimation(GameObject hand, int val)
    // {
    //     if(!hand.activeSelf) return;
    //     hand.transform.DOScale(new Vector3(1f * val, 1f, 1f), 0.5f).OnComplete(() => {
    //         if(!hand.activeSelf) return;
    //         hand.transform.DOScale(new Vector3(1.2f * val, 1.2f, 1f), 0.5f).OnComplete(() => {
    //             HandTutorialAnimation(hand, val);
    //         });
    //     });
    // }

    // void PlayListCardTutorial()
    // {
    //     ShowFooter();

    //     mapAction = false;
    //     lockAllItem = true;
    //     buttonControl.OffBtn();
    //     beTouched = false;
    //     CollectionIC.SetActive(true);
    //     ListCard.SetActive(true);

    //     GameObject hand = CollectionIC.transform.GetChild(1).gameObject;
    //     hand.SetActive(true);
    //     HandTutorialAnimation(hand, 1);

    //     GameObject itemRandom = ListCard.transform.GetChild(0).gameObject;
    //     Vector3 mapGamePosition = mapGame.transform.position;
    //     float xx = mapGamePosition.x - itemRandom.transform.position.x;
    //     float yy = mapGamePosition.y - itemRandom.transform.position.y;
    //     float currentCam = cam.orthographicSize;
    //     float timeDuration = 0.4f;

    //     Vector3 newPosition = new Vector3(xx, yy, mapGamePosition.z);
    //     ChangeLimited(currentCam, 3);
    //     newPosition = new Vector3
    //     (
    //         x:Mathf.Clamp(value:newPosition.x, min:leftLimit, max:rightLimit),
    //         y:Mathf.Clamp(value:newPosition.y, min:bottomLimit, max:upperLimit),
    //         newPosition.z
    //     );

    //     mapGame.transform.DOMove(newPosition, timeDuration).OnComplete(() => 
    //     {
    //         beTouched = true;
    //     });
    //     AnimationCamera(currentCam, 3, timeDuration);
    //     collectionInfo.OffBtn();
    // }

    // void PlayListCardTutorial2()
    // {
    //     buttonControl.OffBtn();
    //     beTouched = false;
    //     CollectionIC.SetActive(true);
    //     ListCard.SetActive(true);
    //     collectionInfo.OnBtn();

    //     GameObject hand = CollectionIC.transform.GetChild(0).gameObject;
    //     collectionInfo.LightTutAction();
    //     hand.SetActive(true);
    //     HandTutorialAnimation(hand, 1);
    // }

    // public void AfterCardTutorial()
    // {
    //     buttonControl.OnBtn();
    //     beTouched = true;
    //     mapAction = true;
    //     lockAllItem = false;
    // }
#endregion
    void Update()
    {
        if(!beTouched) return;
        if(Input.touchCount > 0) infoItem.SetActive(false);
        HandleTouchItems();
        if(mapAction) HandleZoomAndMove();
    }

    void HandleZoomAndMove()
    {
        if (Input.touchCount == 0) return;

        if(Input.touchCount == 2){
            _isScreenChange = true;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            TakeOffset(touch1, "2");
    
            if(EventSystem.current != null &&
                (EventSystem.current.IsPointerOverGameObject(touch1.fingerId)
                || EventSystem.current.IsPointerOverGameObject(touch0.fingerId))) return;

            Vector2 touch0LastPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1LastPos = touch1.position - touch1.deltaPosition;
            float distTouch = (touch0LastPos - touch1LastPos).magnitude;
            float curretDistTouch = (touch0.position - touch1.position).magnitude;
            float difference = curretDistTouch - distTouch;
            Zoom(difference * 0.003f);
            if(touch0.phase == TouchPhase.Ended && touch1.phase == TouchPhase.Ended)
            {
                _isScreenChange = false;
                ResetDragState();
            }
        }
        else if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.fingerId != fistFingerId) {
                offset = offset2;
                fistFingerId = touch.fingerId;
            };

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    moveAllowed = !(EventSystem.current != null && 
                        EventSystem.current.IsPointerOverGameObject(touch.fingerId));

                    _isScreenChange = false;
                    fistFingerId = touch.fingerId;
                    testPos = cam.ScreenToWorldPoint(touch.position);
                    TakeOffset(touch);
                    break;
                case TouchPhase.Moved:
                    if(moveAllowed)
                    {
                        Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                        // if(Vector3.Distance(testPos, currentPos) >= 0.1f)
                        if(_isScreenChange || (testPos - currentPos).magnitude >= 0.1f)
                        {
                            _isScreenChange = true;

                            currentPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
                            transform.position = currentPos - offset;
                            SetLimited();
                        } 
                    }
                    break;
                case TouchPhase.Ended:
                    ResetDragState();
                    _isScreenChange = false;
                    break;
            }                
        }
    }

    void TakeOffset(Touch touch, string val = null)
    {
        touchPos = cam.ScreenToWorldPoint(touch.position);
        touchPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
        // offset = touchPos - mapGame.transform.position;
        if(val == null) offset = touchPos - transform.position;
        else offset2 = touchPos - transform.position;
    }

    void HandleTouchItems()
    {
        if (!Input.GetMouseButton(0) && Input.touchCount == 0) return;
        if(_isDragActive && (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            Drop();
            return;
        }

        _screenPosition = Input.touchCount > 0 ? Input.GetTouch(0).position
            : new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // if(Input.GetTouch(0).phase == TouchPhase.Ended)
        // {
        //     if(isTutorial)
        //     {
        //         if(TutorialListItem.activeSelf){
        //             TutorialListItem.transform.GetChild(1).GetComponent<RectTransform>().DOPause();
        //             TutorialListItem.SetActive(false);
        //             ZoomTutorial.SetActive(true);
        //             return;
        //         }
        //     }
        // }

        _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
        if(_isDragActive) Drag();
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(_worldPosition, Vector2.zero);
            if(hit.collider != null)
            {
                dragable1 dragable = hit.transform.gameObject.GetComponent<dragable1>();

                if(dragable != null)
                {
                    _lastDragged = dragable;
                    InitDrag();
                }
            } 
            else
            {
                if(Input.GetTouch(0).phase == TouchPhase.Ended && !_isScreenChange){
                    wrongItem.SetActive(true);
                    StartCoroutine(PlayAnimationWrongItem());
                }
            }
        }
    }

    void InitDrag()
    {
        _isDragActive = true;
        _draggMove = false;
        _checkPos = Input.GetTouch(0).position;
    }

    void Drag()
    {   
        if(_checkPos != Input.GetTouch(0).position) _draggMove = true;
        // _lastDragged.transform.position = new Vector2(_worldPosition.x, _worldPosition.y);
        // Camera.main.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue()
    }

    void Drop()
    {
        dragable1 item = _lastDragged;
        if(!_isScreenChange && !item.finded)
        {
            if(item.transform.parent.name != "Content")
            {
                if(lockAllItem && item.transform.parent.name == "AvailableItemList") {
                    _isDragActive = false;
                    _draggMove = false;
                    return;
                }
                if(isTutorial)
                {
                    if(((string[])saveDataJson.GetData($"ItemMap{MapNum}")).Length == 0)
                    {
                        // if(item.name == tutorialItem) CloseMask();
                        // else
                        // {
                            _isDragActive = false;
                            _draggMove = false;
                            return;
                        // }
                    }
                    else if(currentTotalItem == 5)
                    {
                        mapAction = false;
                        // buttonControl.OffBtn();
                        beTouched = false;
                    }
                }
                currentTotalItem += 1;

                HandleItemAfterTouch(item);
            }
            else if(!_draggMove) showInfoItem(item);
        }
        ResetDragState();
    }

    void ResetDragState()
    {
        _isDragActive = false;
        _lastDragged = null;
        _draggMove = false;
    }

    IEnumerator PlayAnimationWrongItem()
    {
        wrongItem.transform.DOPause();
        wrongItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        Vector3 pos = cam.ScreenToWorldPoint((Vector3)Input.GetTouch(0).position);
        wrongItem.transform.position = new Vector3(pos.x, pos.y, wrongItem.transform.position.z);
        yield return new WaitForSeconds(0.2f);
        wrongItem.transform.DOScale(new Vector3(0f,0f,1f), 0.5f).SetEase(Ease.InBounce).OnComplete(() => wrongItem.SetActive(false));
    }

    void HandleItemAfterTouch(dragable1 item)
    {
        audioManager.PlaySFX("choose");
        adsManager.VibrationDevice();
        if(hand.name == item.name) StopHandAnimation();
        // if(CircleTutotialComPass.activeSelf) CircleTutotialComPass.SetActive(false);

        item.finded = true;
        string itemName = item.name.Split(" (")[0];
        Transform target = ListItemToFind.transform.Find(itemName);
        SkeletonAnimation itemAnimation = item.GetComponent<SkeletonAnimation>();

        ShowInfoScore(item, target);
        CheckHiddenItem(item);
        PlayAnimItemList(target);
        int currentOrder = item.GetComponent<Renderer>().sortingOrder;
        item.GetComponent<Renderer>().sortingOrder = 110;

        if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("animation") != null)
        {
            itemAnimation.AnimationState.SetAnimation(0, "animation", false);
            StartCoroutine(CompleteAnimationItem(item, currentOrder, 1));
            item.transform.DOScale(new Vector3(1.5f,1.5f,1.5f), 0.5f).OnComplete(() => {
                CheckValue(item, target, itemName);
            });
        }
        else
        {
            if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("Idle") != null)
            {
                itemAnimation.AnimationState.ClearTrack(0);
                // itemAnimation.Skeleton.UpdateWorldTransform();
            }

            Vector3 currentPosition = item.transform.position - transform.position;
            item.transform.SetParent(canvas.transform);
            AddParticleToItem(item.gameObject);
            ShowLightEff(item);

            item.transform.DOScale(new Vector3(189.996f,189.996f,189.996f), 0.5f).OnComplete(() => {
                Vector3 targetPosition = target.position;
                if(!btnDown.activeSelf) targetPosition = btnDown.transform.position;
                audioManager.PlaySFX("collect_item");
                item.transform.DOMove(targetPosition , 0.5f).OnComplete(() => {
                    if(item.transform.childCount == 2){
                        LightEff.transform.SetParent(canvas.transform);
                        LightEff.SetActive(false);
                    }

                    StartCoroutine(RemoveParticle(item, currentPosition, currentOrder));
                    CheckValue(item, target, itemName);
                });
            });
        }
    }

    void AddParticleToItem(GameObject item)
    {
        Transform particle = null;
        foreach(Transform child in ListParticle.transform)
        {
            if(!child.gameObject.activeSelf) particle = child;
        }
        if(particle == null)
        {
            particle = Instantiate(MainParticle.transform , Vector3.zero, Quaternion.identity);
        }
        particle.SetParent(item.transform);
        float scaleSize = 1 - (5 - cam.orthographicSize) * 0.2f;

        particle.localScale = new Vector3(scaleSize,scaleSize,1);
        particle.localPosition = Vector3.zero;
        particle.gameObject.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator RemoveParticle (Transform item)
    {
        Transform particle = item.transform.GetChild(0);
        item.GetComponent<Image>().enabled = false;
        particle.SetParent(item.transform.parent);
        yield return new WaitForSeconds(0.3f);

        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetParent(ListParticle.transform);
        particle.gameObject.SetActive(false);
        item.GetComponent<Image>().enabled = true;
    }

    IEnumerator RemoveParticle (dragable1 item, Vector3 currentPosition, int currentOrder)
    {
        Transform particle = item.transform.GetChild(0);
        if(item.GetComponent<SpriteRenderer>() != null) item.GetComponent<SpriteRenderer>().enabled = false;
        else item.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(0.3f);
        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetParent(ListParticle.transform);
        particle.gameObject.SetActive(false);

        if(item.GetComponent<SpriteRenderer>() != null) item.GetComponent<SpriteRenderer>().enabled = true;
        else item.GetComponent<MeshRenderer>().enabled = true;
        item.gameObject.SetActive(false);
        SkeletonAnimation itemAnimation = item.GetComponent<SkeletonAnimation>();
        if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("Idle") != null) itemAnimation.AnimationState.SetAnimation(0, "Idle", true);
        item.transform.SetParent(ListItemFinded.transform);
        item.transform.localScale = new Vector3(1,1,1);
        item.transform.position = currentPosition + transform.position;
        item.GetComponent<Renderer>().sortingOrder = currentOrder;
    }

    void ShowLightEff(dragable1 item)
    {
        RectTransform LightEffRect = LightEff.GetComponent<RectTransform>();
        LightEff.SetActive(true);
        LightEff.transform.SetParent(item.transform);
        LightEffRect.localPosition = new Vector3(0,0,9);
        LightEffRect.localScale = new Vector3(0.02f,0.02f,1);
        // LightEff.transform.DOPause();
        LightEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
    }

    IEnumerator CompleteAnimationItem (dragable1 item, int currentOrder, int num)
    {
        yield return new WaitForSeconds(0.667f);
        if(num <= 2)
        {
            num++;
            item.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", false);
            StartCoroutine(CompleteAnimationItem(item, currentOrder, num));
        }
        else
        {
            // item.GetComponent<SkeletonAnimation>().AnimationState.ClearTrack(0);
            item.GetComponent<Renderer>().sortingOrder = currentOrder;
            item.gameObject.SetActive(false);
            item.transform.SetParent(ListItemFinded.transform);
            item.transform.localScale = new Vector3(1,1,1);
        }
    }

    void CheckHiddenItem(dragable1 item)
    {
        if(HiddenItemList.transform.childCount == 0) return;
        Vector3 checkPos = item.transform.position - transform.position;
        if(totalCurent > MapData.Milestones[2] && !addMoreItem){
            int count = TakeNumOfItemsInArea(4);
            if(count <= 5) OpenItemOffScreen(checkPos);
            return;
        }
        else if(!addMoreItem) return;
        int openedArea;
        // RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        // float canvasWidth = canvasRect.sizeDelta.x * canvasRect.localScale.x;
        // float canvasHeight = canvasRect.sizeDelta.y * canvasRect.localScale.y;
        // float checkX = mapGame.transform.position.x + canvasWidth / 2;
        // float checkY = mapGame.transform.position.y + canvasHeight / 2;
        // if(totalCurent == MapData.Milestones[1]) {
        //     if(checkX >= 0) return;
        //     openedArea = 2;
        // }
        // else 
        if(totalCurent == MapData.Milestones[2]) {
            if(checkPos.x >= 0 || checkPos.y <= 0) return;
            openedArea = 3;
            TakeListItemHidden(2);
        }
        else if(totalCurent > MapData.Milestones[2]) {
            if(checkPos.x < 0 || checkPos.y < 0) return;
            openedArea = 4;
            //     if(count <= 5 || showAllItem){
            //         OpenItemOffScreen(checkPos);
            //     }
            // }


            // if(checkPos.x >= 0 && checkPos.y >= 0){
            //     if(count <= 5 || showAllItem){
            //         OpenItemOffScreen(checkPos);
            //     }
            //     return;
            // }
            // else {
            //     if(count <= 5 || showAllItem){
            //         OpenItemOffScreen(checkPos);
            //         return;
            //     } else {
            //         Debug.Log("??");
            //         openedArea = 4;
                
            //     }
            // }
        }
        else return;
        TakeListItemHidden(openedArea);
        // if(openedArea != 4) {
        addMoreItem = false;
        saveDataJson.SaveData("AddMoreItem", addMoreItem, MapNum);
        // }
    }

    void TakeListItemHidden(int openedArea)
    {
        GameObject[] listItemAvailable = {};
        int countChild = HiddenItemList.transform.childCount;
        bool addNewItem = false;

        for(int i = 0; i < countChild; i++)
        {
            Transform child = HiddenItemList.transform.GetChild(i);
            Vector3 childPos = child.transform.position - transform.position;
            if(
                (openedArea == 2 && childPos.x < 0 && childPos.y < 0) ||
                (openedArea == 3 && childPos.x > 0 && childPos.y < 0) ||
                (openedArea == 4 && childPos.x < 0 && childPos.y >= 0)
            )
            {
                addNewItem = true;
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;
            }
            if(addNewItem){
                addNewItem = false;
                if(listItemAvailable.Length % 2 != 0)
                {
                    child.gameObject.SetActive(true);
                    child.transform.SetParent(ListToFind.transform);
                    saveDataJson.SaveData($"ShowHiddenItems{MapNum}", child.name);
                    countChild--;
                    i--;
                }
            }
        }
    }

    int TakeNumOfItemsInArea(int area)
    {
        int count = 0;
        foreach(Transform child in ListToFind.transform)
        {
            Vector3 childPos = child.transform.position - transform.position;
            if((area == 1 && childPos.x < 0 && childPos.y <= 0) ||
                (area == 2 && childPos.x >= 0 && childPos.y <= 0) ||
                (area == 3 && childPos.x < 0 && childPos.y > 0) ||
                (area == 4 && childPos.x >= 0 && childPos.y > 0)
            ) count++;
        }
        return count;
    }

    void OpenItemOffScreen(Vector3 checkPos)
    {
        showAllItem = true;
        saveDataJson.SaveData("ShowAllItem", showAllItem, MapNum);

        int countChild = HiddenItemList.transform.childCount;
        for(int i = 0; i < countChild; i++)
        {   
            Transform child = HiddenItemList.transform.GetChild(i);
            Vector3 childPos = child.position - transform.position;
            // Vector3 viewportPosition = cam.WorldToViewportPoint(child.transform.position);
            // if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
            // {
            //     child.gameObject.SetActive(true);
            //     child.transform.SetParent(ListToFind.transform);
            //     saveDataJson.SaveData($"ShowHiddenItems{MapNum}", child.name);
            //     countChild--;
            //     i--;
            // }

            if ((checkPos.x >= 0 && checkPos.y >= 0 && (childPos.x < 0 || childPos.y < 0)) ||
                ((checkPos.x < 0 || checkPos.y < 0) && childPos.x >= 0 && childPos.y >= 0))
            {
                child.gameObject.SetActive(true);
                child.transform.SetParent(ListToFind.transform);
                saveDataJson.SaveData($"ShowHiddenItems{MapNum}", child.name);
                countChild--;
                i--;
            }
        }

        if(HiddenItemList.transform.childCount == 0)
        {
            addMoreItem = false;
            showAllItem = false;
            saveDataJson.SaveData("AddMoreItem", addMoreItem, MapNum);
            saveDataJson.SaveData("ShowAllItem", showAllItem, MapNum);
        }
    }

    private void CheckValue(dragable1 item, Transform target, string itemName) 
    {
        // CheckGift();
        TextMeshProUGUI targetTxt = target.GetChild(0).GetComponent<TextMeshProUGUI>();
        int currentQuantity = Convert.ToInt32(targetTxt.text.Split("/")[0]);

        targetTxt.text = 
            $"{currentQuantity + 1}/{MapData.QuantityList[Array.IndexOf(MapData.ItemList, itemName)]}";

        if(btnDown.activeSelf){
            target.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f).SetEase(Ease.OutQuad).OnComplete(() => {
                if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) target.transform.GetChild(2).gameObject.SetActive(true);  
                target.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.InQuad).OnComplete(() => {
                    if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) ChangePositionOfDoneItem(target);
                    // if(isTutorial) 
                    // {
                    //     if(item.name == tutorialItem) OpenHintListItem();
                    //     else if(currentTotalItem >= 6 && !tutorialTxt.activeSelf) OpenHintTutorial();
                    // }
                });
            });
        }else if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) {
            target.transform.GetChild(2).gameObject.SetActive(true);
            ChangePositionOfDoneItem(target);
        }else if (!btnDown.activeSelf && isTutorial)
        {
            // if(item.name == tutorialItem) OpenHintListItem();
            // else if(currentTotalItem >= 6 && !tutorialTxt.activeSelf) OpenHintTutorial();
        }


        if(currentTotalItem >= totalCurent) ChecktotalCurent(totalCurent);
        else {
            totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
            totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
            SetProgressBar(currentTotalItem, totalCurent);
        }

        saveDataJson.SaveData($"ItemMap{MapNum}", item.name);

        if(currentTotalItem == totalItem) PlayCloundAnimation();
    }

    void ChangePositionOfDoneItem (Transform target)
    {
        int i = ListItemToFind.transform.childCount - 1;
        int limited = target.GetSiblingIndex();
        // buttonControl.OffBtn();
        Vector2 targetPos =  ListItemToFind.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
        for (; i > limited; i--)
        {
            Transform child = ListItemToFind.transform.GetChild(i);
            child.GetComponent<RectTransform>().DOAnchorPos(ListItemToFind.transform.GetChild(i - 1).GetComponent<RectTransform>().anchoredPosition, 0.2f);
        }
        target.SetParent(ListItemToFind.transform.parent);
        target.SetParent(ListItemToFind.transform);
        target.GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.2f);
    }

    // void CheckGift()
    // {
    //     gift++;
    //     if(gift == 30)
    //     {
    //         gift = 0;
    //         OpenedGift.SetActive(true);
    //         OpenedGift.GetComponent<OpenedGift>().PlayAnimation();
    //     } 
    //     GiftIconTxt.text = $"{gift}/30";
    //     saveDataJson.SaveData("Gift", gift);
    // }

    // IEnumerator CheckIfGiftOpen(int area)
    // {
    //     yield return new WaitForSeconds(Time.deltaTime);
    //     if(OpenedGift.activeSelf) StartCoroutine(CheckIfGiftOpen(area));
    //     else OpenNewArea(area);
    // }

    void ChecktotalCurent(int currentTotal = 0)
    {
        int[] Milestones = MapData.Milestones;
        int i = 0;
        for(; i < 3; i++)
        {
            if(currentTotal == Milestones[i]) 
            {
                if(i < 2) {
                    totalCurent = Milestones[i + 1];
                    break;    
                }
                else
                {
                    totalCurent = totalItem;
                    break;
                }
            }
        }
        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
        SetProgressBar(currentTotalItem, totalCurent);
        checkLimited();
        // StartCoroutine(CheckIfGiftOpen(i + 1));
        OpenNewArea(i + 1);
    }

    void PlayAnimItemList(Transform target)
    {
        if(Math.Round(target.transform.position.x, 1) == -0.5) return;
        ListItemToFind.transform.DOPause();

        float curentPos = ListItemToFind.GetComponent<RectTransform>().anchoredPosition.x;
        float targetX = -(45 + 120 * target.GetSiblingIndex()) + 285; 
        float limitRight = -ListItemToFind.GetComponent<RectTransform>().sizeDelta.x + ListItemToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        if(targetX < limitRight) targetX = limitRight;
        else if(targetX > 0) targetX = 0;
        StartCoroutine(MoveItemList(curentPos, targetX, 0.19f));
    }

    IEnumerator MoveItemList (float curentPos, float targetX, float timeDuration, float num = 0)
    {
        float distanceX = targetX - curentPos > 0 ? -(targetX - curentPos) : targetX  - curentPos;
        float xx = distanceX / (timeDuration / Time.deltaTime);
        num += Time.deltaTime;
        yield return new WaitForSeconds(Time.deltaTime);
        // ListItemToFind.transform.position += new Vector3(xx,0,0);
        if(num < timeDuration) 
        {
            if(curentPos < targetX) ListItemToFind.GetComponent<RectTransform>().anchoredPosition -= new Vector2(xx, 0);
            else ListItemToFind.GetComponent<RectTransform>().anchoredPosition += new Vector2(xx, 0);
            StartCoroutine(MoveItemList(curentPos, targetX, timeDuration, num));
        }else ListItemToFind.GetComponent<RectTransform>().anchoredPosition = new Vector2(targetX, ListItemToFind.GetComponent<RectTransform>().anchoredPosition.y);;
    }

    void ShowInfoScore(dragable1 item, Transform target)
    {
        info.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        Image obj = info.GetComponent<Image>();
        Image obj1 = info.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI obj2 = info.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI obj3 = info.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        info.transform.position = item.transform.position + new Vector3(0,1f,0);
        info.SetActive(true);

        string txt = item.name.Split(" (")[0];
        info.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = txt;
        obj1.sprite = imageResources.TakeImage(imageResources.ListMiniGameItem, txt);
        obj1.SetNativeSize();
        obj1.transform.localScale = new Vector3(0.9f,0.9f,1);

        TextMeshProUGUI targetTxt = target.GetChild(0).GetComponent<TextMeshProUGUI>();
        int currentQuantity = Convert.ToInt32(targetTxt.text.Split("/")[0]);
        // Convert.ToInt32(targetTxt.text.Substring(0, targetTxt.text.Length - 3));
        obj3.text = $"{currentQuantity + 1}/{MapData.QuantityList[Array.IndexOf(MapData.ItemList, txt)]}";
        obj.DOPause();
        obj1.DOPause();
        obj2.DOPause();
        obj3.DOPause();

        obj.DOFade(1f, 0.2f);
        obj1.DOFade(1f, 0.2f);
        obj2.DOFade(1f, 0.2f);
        obj3.DOFade(1f, 0.2f).OnComplete(() => {
            obj.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj1.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj2.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj3.DOFade(0f, 0.5f).SetDelay(0.7f).OnComplete(() => info.SetActive(false));
        });

    }

    void showInfoItem(dragable1 item)
    {
        audioManager.PlaySFX("click");
        infoItem.SetActive(true);

        infoItem.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x + ListItemToFind.GetComponent<RectTransform>().anchoredPosition.x, 100);
        TextMeshProUGUI txt = infoItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = item.name;

        float textLength = infoItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().preferredWidth;
        float newWidth = textLength;
        RectTransform rectTransform = txt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);      
    }

    private void Zoom(float increment)
    {
        float currentCamSize = cam.orthographicSize;
        cam.orthographicSize = Mathf.Clamp(value:cam.orthographicSize - increment, zoomMin, zoomMax);
        
        ChangeLimited(currentCamSize);
    }

    void ChangeLimited(float currentCamSize, float val = 0)
    {
        if (val == 0) val = cam.orthographicSize;
        upperLimit += currentCamSize - val;
        bottomLimit -= currentCamSize - val;
        leftLimit -= (currentCamSize - val)*cam.aspect;
        rightLimit += (currentCamSize - val)*cam.aspect;

        if(bottomLimit2 != 0) bottomLimit2 -= currentCamSize - val;
        if(leftLimit2 != 0) leftLimit2 -= (currentCamSize - val)*cam.aspect;
        SetLimited();

        float scaleSize = 1 - (5 - val) * 0.2f;
        info.transform.localScale = new Vector3(scaleSize,scaleSize,1);
    }

    GameObject[] TakeAvailiableItemsToFind()
    {
        GameObject[] listItemAvailable = {};

        int[] Milestones = MapData.Milestones;
        int areaMap = 1;
        if(totalCurent == Milestones[1]) areaMap = 2;
        else if(totalCurent == Milestones[2]) areaMap = 3;
        else if(totalCurent > Milestones[2]) areaMap = 4;
        for(int i = 0; i < ListToFind.transform.childCount; i++)
        {   
            Transform child = ListToFind.transform.GetChild(i);
            if(child.GetComponent<dragable1>().finded) continue;
            Vector3 childPos = child.transform.position - transform.position;
            if(
                (areaMap == 1 && childPos.x <= 0 && childPos.y <= 0) ||
                (areaMap == 2 && childPos.y <= 0) || areaMap == 4 ||
                (areaMap == 3 && (childPos.x <= 0 || childPos.y <= 0))
            )
            {
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;
            }
        }
        return listItemAvailable;
    }


    GameObject TakeRandomItem()
    {
        GameObject[] listItemAvailable = TakeAvailiableItemsToFind();

        if(listItemAvailable.Length == 0) return null;
        int ran = new System.Random().Next(0, listItemAvailable.Length);
        return listItemAvailable[ran];
    }
    public void FindRandomItem()
    {
        if(hand.activeSelf) return;
        int hintNum = (int)saveDataJson.GetData("Hint");
        if(hintNum <= 0)
        {
            shopTool.OpenAnimation("Hint");
            return;
        }
        if(ListToFind && ListToFind.transform.childCount == 0) return;

        // StopHandAnimation();
        beTouched = false;

        // GameObject[] listItemAvailable = TakeAvailiableItemsToFind();

        // if(listItemAvailable.Length == 0) return;
        // int ran = new System.Random().Next(0, listItemAvailable.Length);
        GameObject itemRandom = TakeRandomItem();
        if(itemRandom == null) return;

        Vector3 mapGamePosition = transform.position;
        float xx = mapGamePosition.x - itemRandom.transform.position.x;
        float yy = mapGamePosition.y - itemRandom.transform.position.y;
        float currentCam = cam.orthographicSize;
        float timeDuration = 0.4f;

        Vector3 newPosition = new Vector3(xx, yy, mapGamePosition.z);

        ChangeLimited(currentCam, 3);
        newPosition = new Vector3
        (
            x:Mathf.Clamp(value:newPosition.x, min:leftLimit, max:rightLimit),
            y:Mathf.Clamp(value:newPosition.y, min:bottomLimit, max:upperLimit),
            newPosition.z
        );

        transform.DOMove(newPosition, timeDuration).OnComplete(() => 
        {
            beTouched = true;
            PlayHandAnimation(itemRandom);
            if(isTutorial) {
                tutorialItem = itemRandom.name;
                // OpenTutorial();
            }
        });
        AnimationCamera(currentCam, 3, timeDuration);

        hintNum -= 1;
        hint.text = $"{hintNum}";
        // shop.ChangeHintText(hintNum);
        saveDataJson.SaveData("Hint", hintNum);
    }

    void PlayHandAnimation(GameObject itemRandom) 
    {
        hand.transform.position = itemRandom.transform.position - new Vector3(-0.5f,0.5f,0);
        hand.name = itemRandom.name;
        hand.SetActive(true);
        // Vector3 curentPos = hand.transform.position;
        // hand.transform.DOMove(new Vector3(curentPos.x + 0.2f, curentPos.y - 0.2f, curentPos.z), 0.4f).SetLoops(-1, LoopType.Yoyo);
    }

    void StopHandAnimation()
    {
        hand.SetActive(false);
        // hand.transform.DOPause();
    }

    void AnimationCamera(float currentCam, int size, float timeDuration)
    {
        float val = cam.orthographicSize - (currentCam - size) / (timeDuration/Time.deltaTime);
        cam.orthographicSize = val <= 3 ? 3 : val;
        if(val > 3) StartCoroutine(callBack(currentCam, size, timeDuration));
    }
    IEnumerator callBack (float currentCam, int size, float timeDuration)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        AnimationCamera(currentCam, size, timeDuration);
    }

    void RadaFindIteminCamera(string place)
    {
        /// tìm items đang hiện trên camera
        GameObject[] listItemAvailable = {};

        for(int i = 0; i < ListToFind.transform.childCount; i++)
        {   
            Transform child = ListToFind.transform.GetChild(i);
            Vector3 childPos = child.transform.position;

            Vector3 viewportPosition = cam.WorldToViewportPoint(childPos);
            // if (viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z >= 0)
            if (viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1)
            {
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                // child.localScale = new Vector3(2,2,1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;               
            }
        }
    }

    public void UseCompass()
    {
        if(Compass.activeSelf) return;
        int compass = (int)saveDataJson.GetData("CompassMiniGame");
        if(compass <= 0) 
        {
            shopTool.OpenAnimation("CompassMiniGame");
            return;
        }

        CancelInvoke("TimeOutOfUseCompass");
        Invoke("TimeOutOfUseCompass", 20);
        GameObject itemRandom = TakeRandomItem();

        if(itemRandom == null) return;
        Compass.SetActive(true);
        // CheckIsCompassTutorial(itemRandom);

        // shop.AddMoreStuffInMiniGame(0,-1);
        compass -= 1;
        saveDataJson.SaveData("CompassMiniGame", compass);
        compassTxt.text = $"{compass}";

        StartCoroutine(RotationCompass(itemRandom));
    }

    IEnumerator RotationCompass (GameObject item)
    {
        if(item != null) UpdateCompassRotation(item);

        yield return new WaitForSeconds(Time.deltaTime);
        if(item == null || !item.activeSelf || item.GetComponent<dragable1>().finded)
        {
            item = TakeRandomItem();
        }

        if(Compass.activeSelf) StartCoroutine(RotationCompass(item));
    }

    private void UpdateCompassRotation(GameObject targetItem)
    {
        Vector3 vectorToTarget = targetItem.transform.position - Compass.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        Compass.transform.rotation = Quaternion.Slerp(Compass.transform.rotation, q, Time.deltaTime * 20);
    }

    void TimeOutOfUseCompass()
    {
        Compass.SetActive(false);
    }


    void SetLimited()
    {
        float left = leftLimit;
        float bottom = bottomLimit;

        if(leftLimit2 != 0 && transform.position.x < leftLimit2 && !onSpecialTop) bottom = bottomLimit2;
        else if(bottomLimit2 != 0 && transform.position.y < bottomLimit2)
        {
            left = leftLimit2;
            onSpecialTop = true;
        }
        else onSpecialTop = false;

        transform.position = new Vector3
        (
            x:Mathf.Clamp(value:transform.position.x, min:left, max:rightLimit),
            y:Mathf.Clamp(value:transform.position.y, min:bottom, max:upperLimit),
            transform.position.z
        );
    }

    void ResetData()
    {
        totalItem = 0;
        totalCurent = 0;
        fistFingerId = 0;
        cam.orthographicSize = 15;
        currentTotalItem = 0;
        // hand.SetActive(false);
        info.SetActive(false);
        infoItem.SetActive(false);
        wrongItem.SetActive(false);
        ListToFind.SetActive(false);
        Compass.SetActive(false);
        MapData = null;

        leftLimit = -4.184f;
        rightLimit = 4.184f;
        bottomLimit = -6.34f;
        upperLimit = 10.34f;

        // beTouched = true;
        // _draggMove = false;
        // onSpecialTop = false;
        // _isDragActive = false;
        // _isScreenChange = false;
        // addMoreItem = false;
        // showAllItem = false;
        // GiftIconTxt.transform.parent.parent.gameObject.SetActive(false);
        transform.position = new Vector3(0,0,transform.position.z);
        int listItemToFindLength = ListItemToFind.transform.childCount;

        for (int i = 0; i < listItemToFindLength;)
        {
            listItemToFindLength--;
            Transform child = ListItemToFind.transform.GetChild(i);
            child.transform.GetChild(2).gameObject.SetActive(false);
            child.gameObject.name = "MiniGameItemPrefab";
            ObjectPoolManager.ReturnObjectToPool(child.gameObject);
            child.transform.SetParent(PoolItem.transform);
        }
        ListItemToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 130);
        listMap.transform.Find($"Map{MapNum}").gameObject.SetActive(false);
        gameScreen.SetActive(false);
        ShowFooter();
        beTouched = true;
        gameObject.SetActive(false);
        gameObject.GetComponent<MainGame>().enabled = true;
        gameObject.GetComponent<TouchControl>().enabled = true;
        gameObject.GetComponent<MiniGame>().enabled = false;
        miniGame.SetActive(false);
        // buttonControl.OnBtn();
        audioManager.ChangeMusicBackground("bgm_home");
    }

    public void PlayCloundAnimation(string info = null) {
        beTouched = false;
        // buttonControl.OffBtn();
        ListItemToFind.transform.position = new Vector3(0,ListItemToFind.transform.position.y,ListItemToFind.transform.position.z);
        audioManager.PlaySFX("cloud_close");
        for(int i = 0; i < 4; i++) 
        {
            foreach(Transform child in listCloud.transform.GetChild(i))
            {
                SkeletonAnimation cloud = child.GetComponent<SkeletonAnimation>();
                cloud.AnimationState.SetAnimation(0, "Idle", true);
                cloud.timeScale = 0.1f;
            }
        }

        if(info != null) {
            // cloudAnimation.PlayChangeScreenAnimation("ComeBackHome");
            Invoke("ComeBackHome", 2f);
        }
        else 
        {
            // adsManager.StopCountDownInter();
            // adsManager.StopCountDownForHintSale();
            // adsManager.StopCountDownForComboDeluxe();
            Invoke("GameComplete", 2f);
            // if(!OpenedGift.activeSelf) cloudAnimation.PlayChangeScreenAnimation("GameComplete");
        }
    }

    void ComeBackHome()
    {
        ResetData();
        home.SetActive(true);
        sale.CheckTimeToOpenDiaLog();
        adsManager.ShowInterstitialAd((int)saveDataJson.GetData("OpenedMap"));
        // adsManager.StopCountDownInter();
        // adsManager.StopCountDownForHintSale();
        // adsManager.StopCountDownForComboDeluxe();
    }

    void GameComplete()
    {
        // if(OpenedGift.activeSelf) {
        //     Invoke("GameComplete", 0.5f);
        //     return;
        // }

        // if(!cloudAnimation.gameObject.activeSelf) {
        //     cloudAnimation.PlayChangeScreenAnimation("GameComplete");

        //     Invoke("ShowCompleteDilog",2f);
        // }else
        ShowCompleteDilog();
        adsManager.ShowInterstitialAd((int)saveDataJson.GetData("OpenedMap"));
    }

    void ShowCompleteDilog()
    {
        adsManager.LogEvent($"CompleteMiniGame_{MapNum}");
        int ListItemFindedLength = ListItemFinded.transform.childCount;
        List<string> showHiddenItems = (List<string>)saveDataJson.GetData($"ShowHiddenItems{MapNum}");

        for (int i = 0; i < ListItemFindedLength;)
        {
            ListItemFindedLength--;
            Transform child = ListItemFinded.transform.GetChild(i);
            child.gameObject.SetActive(true);

            if(showHiddenItems.Contains(child.name)) child.SetParent(HiddenItemList.transform);
            else child.SetParent(ListToFind.transform);
            child.GetComponent<dragable1>().finded = false;
        }
        saveDataJson.SaveData($"ShowHiddenItems{MapNum}", null);
        saveDataJson.SaveData($"ItemMap{MapNum}", null);
        ResetData();

        gameObject.SetActive(false);
        home.SetActive(true);
        sale.CheckTimeToOpenDiaLog();
        shop.CountCoins(50);
        // completeDialog.SetActive(true);
        // completeDialog.GetComponent<Complete>().SetImage(MapNum);
    }

    public void PauseGameAction()
    {
        beTouched = false;
    }

    public void ContinueGameAction()
    {
        beTouched = true;
    }
}
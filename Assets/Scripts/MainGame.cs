using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    public AdsManager adsManager;
    private GameObject ListItemInMap;
    private GameObject currentMap;
    public GameObject ListItemCollected;
    public GameObject ListItemToFind;
    public GameObject Home;
    public GameObject GameUI;
    public Setting setting;
    public GameObject ItemPrefab;
    public TouchControl touchControl;
    public Button settingBtn;
    // private bool combining = false;
    private bool usingSupport = false;

    public Loading loading;
    public SupportTools supportTools;
    public GameObject ParticleList;
    public GameObject originalParticleFollow;
    public GameObject originalParticleMerge;
    public TextMeshProUGUI txtLv;
    public Sale sale;

    public GameObject particleItemToFind;

    [Header("Manager")]
    public ImageResources imageResources;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    SaveDataJson.Map mapData;

    public MoveFollowPath moveFollowPath;
    public Win win;
    public Lose lose;
    public WeatherControl weatherControl;
    private int mapNum;

    [Header("Timer")]
    // bool stopCountDownToGameOver = false;
    bool pauseCountDownToGameOver = false;
    public TextMeshProUGUI timerTxt;
    private Coroutine timerCoroutine;

    void Start()
    {
        // originalParticle = ParticleList.transform.GetChild(1).GetChild(0).gameObject;
    }

#region Tutorial
    [Header("Tutorial")]
    public GameObject FrTutorial;
    public GameObject HandTouchItem;
    public GameObject HandZoom;
    public GameObject BgTutorial;
    public GameObject BgInMap;
    public GameObject Arrow;
    public GameObject ToolTutorialArrow;

    private string currentTutorialItem = "";
    private int countTutorialItem = 0;
    private Transform ToolForTutorial;


    void CheckTutorial()
    {
        StartCoroutine(PlayAniationItemToFind());

        if((bool)saveDataJson.GetData("PlayAgain")) return;

        if(mapNum == 1)
        {
            pauseCountDownToGameOver = true;
            touchControl.StopMapAction();
            BgTutorial.SetActive(true);
            FrTutorial.SetActive(true);
            HandTouchItem.SetActive(true);
            BgInMap.SetActive(true);

            FrTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tap 3 identical Items to collect them";

            ListItemCollected.transform.SetParent(BgTutorial.transform);
            ListItemToFind.transform.SetParent(BgTutorial.transform);
            FrTutorial.transform.SetParent(BgTutorial.transform);
            SetTutorialItem();
            // HandTutorialAnimation();
        }
        else if(mapNum == 2) PlayTutorialZoom();
        else if(mapNum ==  4) PlayToolTutorial("Magnet", 3);
        else if(mapNum ==  8) PlayToolTutorial("Undo", 2);
        else if(mapNum ==  10) PlayToolTutorial("Compass", 1);
        else if(mapNum ==  12) PlayToolTutorial("FreezeTimer", 1);
    }

    void SetTutorialItem()
    {
        if(countTutorialItem >= 3) return;
        string itemName = mapData.ItemList[0];
        foreach(Transform child in ListItemInMap.transform)
        {
            if(child.name.Split(" (")[0]== itemName)
            {
                countTutorialItem++;
                currentTutorialItem = child.name;
                child.GetComponent<Renderer>().sortingOrder = 91;
                HandTouchItem.transform.position = child.position + new Vector3(1.2f,-1.4f,0);

                touchControl.MoveMapToTutorialItem(child);
                break;
            }
        }
    }

    void PlayTutorialZoom()
    {
        BgTutorial.SetActive(true);
        FrTutorial.SetActive(true);
        HandZoom.SetActive(true);
        BgInMap.SetActive(true);
        pauseCountDownToGameOver = true;
        touchControl.StopTouchItem();

        FrTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Zoom in to see more clearly";
        FrTutorial.transform.SetParent(BgTutorial.transform);
    }

    public void TurnOffZoomTutorial()
    {
        if(!HandZoom.activeSelf) return;
        pauseCountDownToGameOver = false;

        HandZoom.SetActive(false);
        FrTutorial.SetActive(false);
        BgTutorial.SetActive(false);
        BgInMap.SetActive(false);
        FrTutorial.transform.SetParent(GameUI.transform.Find("Footer"));
        touchControl.AllowTouchItem();
    }

    // void HandTutorialAnimation()
    // {
    //     if(!HandTutorial.activeSelf) return;
    //     HandTutorial.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() => {
    //         if(!HandTutorial.activeSelf) return;
    //         HandTutorial.transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.5f).OnComplete(() => {
    //             HandTutorialAnimation();
    //         });
    //     });
    // }

    void PlayListItemTutorial()
    {
        HandTouchItem.SetActive(false);
        touchControl.AllowMapAction();
    
        FrTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Collect All Goal Items to finish the level";
        Arrow.SetActive(true);
    }

    public void TurnOffTutorial()
    {
        if(!Arrow.activeSelf) return;
        countTutorialItem = 0;
        currentTutorialItem = "";
        BgTutorial.SetActive(false);
        FrTutorial.SetActive(false);
        BgInMap.SetActive(false);
        Arrow.SetActive(false);

        ListItemCollected.transform.SetParent(GameUI.transform.Find("Footer"));
        ListItemToFind.transform.SetParent(GameUI.transform.Find("Header/FrItem/Scroll View/Viewport"));
        FrTutorial.transform.SetParent(GameUI.transform.Find("Footer"));
    }
    
    void PlayToolTutorial(string txt, int num)
    {
        PauseGameAction();
        if(txt == "Undo")
        {
            GameObject item = GameObject.Find($"Game/MainGame/Map/Map{mapNum}/ListItemToFind").transform.GetChild(0).gameObject;
            AddItemToListItemCollected(dragable.allDragables[item.name]);
        }
        ToolForTutorial = supportTools.transform.Find(txt);
        ToolTutorialArrow.SetActive(true);
        BgTutorial.SetActive(true);
        BgInMap.SetActive(true);
        ToolTutorialArrow.transform.position = ToolForTutorial.position + new Vector3(0,3f,0);

        saveDataJson.SaveData(txt, num);
        ToolForTutorial.SetParent(BgTutorial.transform);
        ToolForTutorial.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void TurnOffToolTutorial()
    {
        if(!ToolTutorialArrow.activeSelf) return;
        ContinueGameAction();

        BgInMap.SetActive(false);
        BgTutorial.SetActive(false);
        ToolTutorialArrow.SetActive(false);

        ToolForTutorial.SetParent(supportTools.transform);
        ToolForTutorial.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        // ListItemCollected.transform.SetParent(GameUI.transform.Find("Footer"));

        int num = 0;
        if(ToolForTutorial.name == "Magnet") num = 0;
        else if(ToolForTutorial.name == "Undo") num = 1;
        else if(ToolForTutorial.name == "Compass") num = 2;
        else if(ToolForTutorial.name == "FreezeTimer") num = 3;

        ToolForTutorial.SetSiblingIndex(num);
    }
#endregion Tutorial

#region Handel Data Game
    public void LoadGame(bool thunder = false, bool addTime = false)
    {
        touchControl.AllowMapAction();
        touchControl.AllowTouchItem();
        // set the initial value of the map
        SetMapValue();
        // set initial map limit movement value
        touchControl.SetDefaultLimit(currentMap);
        // set List and quantity items need to find
        SetListItemToFind();
        moveFollowPath.CheckPath(mapNum);
        // start Count down to game over
        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(CountdownToGameOver());
        weatherControl.SetWeather();
        CheckSupportTools(thunder, addTime);
        PlayLoadingScreen();
        audioManager.ChangeMusicBackground("bgm_maingame");

        if(mapNum == 1 || mapNum == 2) settingBtn.interactable = false;
        else settingBtn.interactable = true;
    }

    void PlayLoadingScreen()
    {
        if(loading.gameObject.activeSelf) 
        {
            CheckTutorial();
            return;
        }
        PauseGameAction();
        loading.PlayAnimLoading(1f);
        win.ResetAnim();
        Invoke("EndLoading", 1.5f);
    }

    void EndLoading()
    {
        ContinueGameAction();
        CheckTutorial();
    }

    IEnumerator PlayAniationItemToFind()
    {
        foreach(Transform child in ListItemToFind.transform)
        {
            yield return new WaitForSeconds(0.1f);
            child.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f).OnComplete(() => {
                child.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
            });
            PlarParticleItemToFind(child);
        }
    }

    void PlarParticleItemToFind(Transform item)
    {
        Transform particle = null;
        Transform list = GameUI.transform.Find("Header/FrItem/Scroll View/Viewport/ListParticle");
        foreach(Transform child in list)
        {
            if(!child.gameObject.activeSelf) particle = child;
        }

        if(particle == null)
        {
            particle = Instantiate(originalParticleMerge.transform , Vector3.zero, Quaternion.identity);
            particle.SetParent(list);
        }
        particle.gameObject.SetActive(true);
        particle.position = item.position;
        particle.localScale = Vector3.one;

        particle.GetComponent<ParticleSystem>().Play();
    }

    void SetListItemToFind()
    {
        mapData = saveDataJson.TakeMapData().map[mapNum];
        int itemNumber = mapData.ItemList.Length;
        string[] itemList = mapData.ItemList;
        int[] quantityList = mapData.QuantityList;
        ListItemToFind.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(140f * itemNumber, ListItemToFind.GetComponent<RectTransform>().sizeDelta.y);
        for(int i = 0; i < itemNumber; i ++)
        {
            GameObject newItem = ObjectPoolManager.SpawnObject(ItemPrefab, Vector3.zero, Quaternion.identity);
            newItem.transform.SetParent(ListItemToFind.transform);
            newItem.name = itemList[i];
            RectTransform itemTransform = newItem.GetComponent<RectTransform>();
            itemTransform.SetParent(ListItemToFind.transform);
            // itemTransform.sizeDelta = new Vector2(130,130);
            itemTransform.localScale = new Vector3(1,1,1);
            itemTransform.localPosition = new Vector3(75 + 140 * i, -72.29f, 0);

            Image img = newItem.transform.GetChild(0).GetComponent<Image>();
            img.sprite = imageResources.TakeImage(imageResources.ListItem, itemList[i]);
            FitItemSizeToParent(img.GetComponent<RectTransform>());
            newItem.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{quantityList[i]}"; 
            newItem.transform.GetChild(2).gameObject.SetActive(false);
            itemTransform.localScale = Vector3.zero;
        }
    }

    void FitItemSizeToParent(RectTransform item)
    {
        item.GetComponent<Image>().SetNativeSize();

        Vector2 itemSize = item.sizeDelta;
        Vector2 imageSizeInCanvas = GetImageSizeInCanvas(item.transform.parent.GetComponent<Image>());
        Vector2 parentSize = ConvertCanvasSizeToUnityUnits(imageSizeInCanvas);

        float scaleRatio = 1;
        if(itemSize.x >= itemSize.y && itemSize.x > parentSize.x)
        {
            scaleRatio = parentSize.x * 1.5f / itemSize.x;
        }
        else if(itemSize.y >= itemSize.x && itemSize.y > parentSize.y)
        {
            scaleRatio = parentSize.y * 1.5f / itemSize.y;
        }

        item.transform.localScale = new Vector3(Math.Abs(100 * scaleRatio), 100 * scaleRatio, 1f);
    }

    void SetMapValue()
    {
        mapNum = (int)saveDataJson.GetData("OpenedMap");
        if(saveDataJson.TakeMapData().map.Length == mapNum) mapNum--;
        GameObject map = GameObject.Find($"Game/MainGame/Map/Map{mapNum}");
        map.SetActive(true);
        currentMap = map.transform.Find("Map_Bg").gameObject;
        ListItemInMap = map.transform.Find("ListItemToFind").gameObject;
        txtLv.text = $"Level {mapNum}";
        supportTools.CheckToolAllows(mapNum, ListItemInMap);
    }

    public void ResetData()
    {
        gameObject.transform.position = Vector3.zero;
        usingSupport = false;
        // combining = false;
        // stopCountDownToGameOver = false;
        pauseCountDownToGameOver = false;
        Camera.main.orthographicSize = 15;
        moveFollowPath.Reset();
        weatherControl.TurnOffWeather();

        // Debug.Log(mapNum);
        GameObject.Find($"Game/MainGame/Map/Map{mapNum}").SetActive(false);
        supportTools.CompassArrow.SetActive(false);
        supportTools.iceParticle.gameObject.SetActive(false);

        ResetListItem();
    }

    void ResetListItem()
    {
        foreach(Transform child in ListItemInMap.transform)
        {
            child.gameObject.SetActive(true);
        }

        for(int i = 0; i < 7; i++)
        {
            Transform child = ListItemCollected.transform.GetChild(i);
            if(child.childCount != 0)
            {
                Transform item = child.GetChild(0);
                SetItemToFistValue(item);
                item.gameObject.SetActive(true);
            }
        }

        int listItemToFindLength = ListItemToFind.transform.childCount;

        for (int i = 0; i < listItemToFindLength; i++)
        {
            Transform child = ListItemToFind.transform.GetChild(i);
            child.gameObject.name = "ItemPrefab";
            ObjectPoolManager.ReturnObjectToPool(child.gameObject);
            child.SetParent(ListItemToFind.transform.parent);
            i--;
            listItemToFindLength--;
        }
    }

    void SetItemToFistValue(Transform item)
    {
        dragable itemStatus = dragable.allDragables[item.name];
        item.gameObject.SetActive(false);
        item.SetParent(ListItemInMap.transform);
        item.transform.DOKill();

        item.position = itemStatus.FirstPosition + gameObject.transform.position;
        item.GetComponent<Renderer>().sortingOrder = itemStatus.Layer;
        item.localScale = itemStatus.FirstScale;
        itemStatus.status = "wait";
        if(itemStatus.InitialSkin != null)
        {
            item.GetComponent<SkeletonAnimation>().skeleton.SetSkin(itemStatus.InitialSkin);
        }
    }

    public void AddMoreTime(int time)
    {
        StartCoroutine(CountdownToGameOver(time));
    }
    private int Timer = 0;
    public IEnumerator CountdownToGameOver(int moreTime = 0)
    {
        Timer = moreTime == 0 ? mapData.Time : moreTime;
        UpdateTimeDisplay(Timer);
        while (Timer > 0)
        {
            yield return new WaitForSeconds(1f);
            // if(stopCountDownToGameOver) break;
            if(!pauseCountDownToGameOver) Timer -= 1;
            UpdateTimeDisplay(Timer);
        }

        // if(!stopCountDownToGameOver){
            PauseGameAction();
            lose.SetValue("TimeOut");
        // } else stopCountDownToGameOver = false;
    }

    void UpdateTimeDisplay(int time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

#endregion Handel Data Game

#region Handle Game Over
    void CheckSlot()
    {
        bool loseGame = true;
        for(int i = 0; i < 7; i++)
        {
            Transform child = ListItemCollected.transform.GetChild(i);
            if(
                child.childCount == 0 || (child.childCount == 1 &&
                dragable.allDragables[child.GetChild(0).name].status != "found")
            )
            {
                loseGame = false;
                break;
            }
        }

        if(loseGame)
        {
            PauseGameAction();
            lose.SetValue("OutOfSlots");
        }
    }

    void GameCompleted()
    {
        // stopCountDownToGameOver = true;
        if(win.gameObject.activeSelf) return;
        PauseGameAction();
        saveDataJson.SaveData("PlayAgain", false);
        win.PlayAnimation(mapNum);
        adsManager.LogEvent($"CompletedMap_{mapNum}");

        saveDataJson.SaveData("OpenedMap", mapNum + 1);
    }
#endregion Handle Game Over

#region Handle Collect Item
    public void AddItemToListItemCollected (dragable item, string txt = "")
    {
        if(txt == "tool") usingSupport = true;
        if (ListItemCollected.transform.GetChild(6).childCount != 0) return;

        if(BgTutorial.activeSelf && currentTutorialItem != "" && item.name != currentTutorialItem) return;

        item.transform.DOKill();
        item.GetComponent<Renderer>().sortingOrder = 110;
    
        Transform newParent = ChooseParentForItem(item);

        AddParticleToItem(item.transform, "Follow");

        item.transform.SetParent(newParent);
        SetItemValue(item.name.Split(" (")[0]);

        CaculateScale(item.transform, newParent);

        Vector3 path = new Vector3(newParent.position.x, item.transform.position.y + 5, item.transform.position.z);
        item.status = "moving";
        adsManager.VibrationDevice();
        // if(txt == "tool") return;

        if(item.name.Length >= 3 && item.name.Substring(0, 3) == "Car")
        {
            if(item.GetComponent<SkeletonAnimation>() != null) item.GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1");
        }

        StartCoroutine(EvaluateSlerpPoints(item.gameObject, item.transform.position, path, 0));
        if(BgTutorial.activeSelf) SetTutorialItem();
    }

    Transform ChooseParentForItem(dragable item)
    {
        Transform listItemCollectedTransform = ListItemCollected.transform;
        string itemBaseName = item.name.Split(" (")[0];
        Transform newParent = null;
        bool isInsert = false;
        bool insert = false;
        int minChild = 0;
        int emptyBox = 0;
        for(int i = 0; i < 7; i++)
        {
            Transform child = listItemCollectedTransform.GetChild(i);
            Transform nextChild = i < 6 ? listItemCollectedTransform.GetChild(i + 1) : null;

            if(isInsert)
            {
                if(child.childCount == 0) emptyBox++;
                else
                {
                    if(nextChild?.childCount == 0) isInsert = false;

                    if(emptyBox < 1)
                    {
                        child.GetChild(0).SetParent(nextChild);
                        if(!isInsert)
                        {
                            StartCoroutine(CheckItemToRemove(minChild, dragable.allDragables[nextChild.GetChild(nextChild.childCount - 1).name]));
                        }
                    }
                    else emptyBox--;
                }

                if(!insert) {newParent = child;}
                insert = true;
            }
            else if (child.childCount == 0)
            {
                if(newParent == null) newParent = child;
            }
            else if(child.childCount != 0)
            {
                if(child.GetChild(0).name.Split(" (")[0] == itemBaseName && (nextChild?.childCount == 0 ||
                    (nextChild?.childCount != 0 && nextChild.GetChild(0).name.Split(" (")[0] != itemBaseName)))
                {
                    minChild = i + 1;
                    isInsert = true;
                }
                else if(newParent != null && child.childCount != 0 && !insert &&
                    listItemCollectedTransform.GetChild(i - 1).childCount == 0)
                {
                    newParent = null;
                }
            }
        }

        return newParent;
    }

    IEnumerator CheckItemToRemove(int val, dragable item)
    {
        yield return new WaitUntil(() => item.status == "found");
        Transform slot;
        Transform slotChild;
        for(int i = 6; i > val; i--)
        {
            slot = ListItemCollected.transform.GetChild(i);
            slotChild = slot.childCount == 0 ? null : slot.GetChild(0);
            if(slotChild == null || dragable.allDragables[slotChild.name].status == "moving") continue;
            slotChild.DOKill();
            slotChild.DOLocalMove(slot.transform.position, 0.1f);
        }
    }

    void CaculateScale (Transform item, Transform newParent)
    {
        Vector3 itemSize;
        if(item.GetComponent<SpriteRenderer>()) itemSize = item.GetComponent<SpriteRenderer>().bounds.size;
        else itemSize = item.GetComponent<MeshRenderer>().bounds.size;

        Vector2 imageSizeInCanvas = GetImageSizeInCanvas(newParent.GetComponent<Image>());
        Vector2 parentSize = ConvertCanvasSizeToUnityUnits(imageSizeInCanvas);

        float scaleRatio = 1;
        if(itemSize.x >= itemSize.y && itemSize.x > parentSize.x)
        {
            scaleRatio = parentSize.x * 1.5f / itemSize.x;
        }
        else if(itemSize.y >= itemSize.x && itemSize.y > parentSize.y)
        {
            scaleRatio = parentSize.y * 1.5f / itemSize.y;
        }
        // float itemScale = item.localScale.y * dragable.allDragables[item.name].FirstScale.y;
        float itemScale = 100 * dragable.allDragables[item.name].FirstScale.y * scaleRatio;

        // item.localScale = new Vector3(itemScale, itemScale, 1f);
        PlayMoveScaleAnimation(item, itemScale);

        // item.DOScale(new Vector3(itemScale * 0.7f, itemScale * 0.7f, 1f), 0.15f).SetEase(Ease.InOutQuad);
        // item.DOScale(new Vector3(itemScale * 1.2f, itemScale * 1.2f, 1f), 0.07f).SetEase(Ease.InOutQuad).SetDelay(0.15f);
        // item.DOScale(new Vector3(itemScale * 0.9f, itemScale * 0.9f, 1f), 0.05f).SetEase(Ease.InOutQuad).SetDelay(0.22f);
        // item.DOScale(new Vector3(itemScale , itemScale, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.27f);
    }

    Vector2 GetImageSizeInCanvas(Image targetImage)
    {
        RectTransform imageRectTransform = targetImage.rectTransform;
        return new Vector2(
            imageRectTransform.rect.width * imageRectTransform.localScale.x,
            imageRectTransform.rect.height * imageRectTransform.localScale.y
        );
    }

    void BounceItem(Transform item)
    {
        float scale = item.localScale.x / 9 * 10;
        float itemHigh = item.GetComponent<Renderer>().bounds.size.y / 9 * 10;
        // float posX = item.position.x;
        float posY = item.parent.position.y;
        float posZ = item.position.z;
        // Debug.Log("//");
        item.DOScale(new Vector3(scale * 1.1f, scale * 0.9f, 1f), 0.03f).SetEase(Ease.InOutQuad);
        item.DOMove(new Vector3(item.position.x, posY - itemHigh / 20, posZ), 0.03f);

        item.DOScale(new Vector3(scale * 0.95f, scale * 1.05f, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.03f);
        item.DOMove(new Vector3(item.position.x, posY - itemHigh / -40, posZ), 0.03f).SetDelay(0.03f);

        item.DOScale(new Vector3(scale, scale, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.06f);
        item.DOMove(new Vector3(item.position.x, posY, posZ), 0.03f).SetDelay(0.06f);
    }

    Vector2 ConvertCanvasSizeToUnityUnits(Vector2 canvasSize)
    {
        RectTransform canvasRectTransform = Home.transform.parent.parent.GetComponent<RectTransform>();
        // Lấy tỷ lệ giữa kích thước màn hình và kích thước Canvas
        Vector2 screenToCanvasRatio = new Vector2(
            Screen.width / canvasRectTransform.rect.width,
            Screen.height / canvasRectTransform.rect.height
        );

        // Chuyển đổi kích thước Canvas sang pixel
        Vector2 sizeInPixels = new Vector2(
            canvasSize.x * screenToCanvasRatio.x,
            canvasSize.y * screenToCanvasRatio.y
        );

        // Chuyển đổi từ pixel sang Unity units
        return sizeInPixels / Screen.dpi * 2.54f; // 2.54 là số cm trong 1 inch
    }

    IEnumerator CheckItemsToCombine(GameObject obj)
    {
        int index = obj.transform.parent.GetSiblingIndex();
        Transform child1 = obj.transform;
        Transform child2 = ListItemCollected.transform.GetChild(index - 1).GetChild(0);
        Transform child3 = ListItemCollected.transform.GetChild(index - 2).GetChild(0);

        yield return new WaitUntil(() => dragable.allDragables[obj.name].status != "rearranging");
        // yield return new WaitForSeconds(0);
        // int index = obj.transform.parent.GetSiblingIndex();
        // Transform child1 = obj.transform;
        // Transform child2 = ListItemCollected.transform.GetChild(index - 1).GetChild(0);
        // Transform child3 = ListItemCollected.transform.GetChild(index - 2).GetChild(0);

        dragable.allDragables[child1.name].status = "combining";
        dragable.allDragables[child2.name].status = "combining";
        dragable.allDragables[child3.name].status = "combining";
        CombineItems(child1, child2, child3);
    }

    void CombineItems(Transform child1, Transform child2, Transform child3)
    {
        Vector3 localPos1 = child1.parent.InverseTransformPoint(child2.position);
        Vector3 localPos3 = child3.parent.InverseTransformPoint(child2.position);

        audioManager.PlaySFX("merge");
        child1.DOLocalMove(localPos1, 0.3f);
        child3.DOLocalMove(localPos3, 0.3f).OnComplete(() => 
        {
            adsManager.VibrationDevice(1);
            AddParticleToItem(child2,"Merge","nochange");
            // combining = false;
            SetItemToFistValue(child1);
            SetItemToFistValue(child2);
            SetItemToFistValue(child3);
            // RearrangeListItemCollected();

            StartCoroutine(RearrangeListItemCollected());
            CheckItemValue();
        });
        moveFollowPath.StopMovingParent(child1, child2, child3);
    }

    void SetItemValue (string itemName)
    {
        Transform frItem = ListItemToFind.transform.Find(itemName);
        if(frItem == null) return;
        TextMeshProUGUI itemValue = frItem.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        itemValue.text = (int.Parse(itemValue.text) - 1).ToString();

        if(itemValue.text == "0") 
        {
            // ListItemToFind.transform.Find(itemName).GetChild(2).gameObject.SetActive(true);
            frItem.gameObject.SetActive(false);
            int length = ListItemToFind.transform.childCount;
            int frItemIndex = frItem.GetSiblingIndex();
            frItem.SetSiblingIndex(length - 1);
            for(int i = frItemIndex; i < length; i++)
            {
                ListItemToFind.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(new Vector2(75 + 140 * i,0), 0.2f);
            }
        }
    }

    void CheckItemValue()
    {
        bool isGameOver = true;
        if(countTutorialItem == 3) PlayListItemTutorial();
        foreach(Transform child in ListItemToFind.transform)
        {
            if(child.gameObject.activeSelf)
            {
                isGameOver = false;
                break;
            }
        }
        if(isGameOver) GameCompleted();
    }

    IEnumerator RearrangeListItemCollected()
    {
        // yield return new WaitUntil(() => !combining);
        yield return new WaitForSeconds(0.5f);

        int count = 0;
        for(int i = 0; i < 7; i++)
        {
            Transform child = ListItemCollected.transform.GetChild(i);
            if(child.childCount == 0) count ++;
            else
            {
                child = ListItemCollected.transform.GetChild(i).GetChild(0);
                // Debug.Log(i + " / " + child);
                dragable childDrag = dragable.allDragables[child.name];
                // Debug.Log(childDrag.status);
                child.SetParent(ListItemCollected.transform.GetChild(i - count));
                if(childDrag.status == "moving") continue;
                childDrag.status = "rearranging";
                child.DOLocalMove(Vector3.zero, 0.2f)
                    .OnComplete(() => childDrag.status = "found");
            }
        }
    }

    void CheckListItemCollected (GameObject obj)
    {
        if(usingSupport)
        {
            usingSupport = false;
            supportTools.AllowUseTool();
        }

        int index = obj.transform.parent.GetSiblingIndex();
        string objName = obj.name.Split(" (")[0];

        if(index - 2 >= 0 && ListItemCollected.transform.GetChild(index - 2).childCount == 1)
        {
            string childName = ListItemCollected.transform.GetChild(index - 2).GetChild(0).name;
            string itemStatus = dragable.allDragables[childName].status;
            if(itemStatus != "wait" && itemStatus != "combining" && childName.Split(" (")[0] == objName)
            {
                StartCoroutine(CheckItemsToCombine(obj));
            }
            else CheckSlot();
        }
        else
        {
            CheckSlot();
        }
        
    }
#endregion Handle Collect Item

#region ParticleSystem
    void AddParticleToItem(Transform item, string title, string changeParent = "")
    {
        Transform particle = null;
        Transform list = ParticleList.transform.Find(title);
        float scaleSize = GameUI.transform.parent.parent.localScale.x * 100;

        foreach(Transform child in list)
        {
            if(!child.gameObject.activeSelf) particle = child;
        }

        if(particle == null)
        {
            if(title == "Follow") particle = Instantiate(originalParticleFollow.transform , Vector3.zero, Quaternion.identity);
            else particle = Instantiate(originalParticleMerge.transform , Vector3.zero, Quaternion.identity);
            particle.name = title;
            particle.SetParent(list);
        }

        particle.localScale = new Vector3(scaleSize,scaleSize,1);
        particle.GetChild(0).localScale = new Vector3(scaleSize,scaleSize,1);
        if(changeParent == "") 
        {
            particle.SetParent(item.transform);
            particle.localPosition = Vector3.zero;
        }
        else
        { 
            // particle.localPosition = new Vector3(0,20,0) +
            //     item.position / Home.transform.parent.parent.localScale.x;
            particle.localPosition = item.position;
            particle.localScale = new Vector3(scaleSize,scaleSize,1);
            particle.GetChild(0).GetChild(0).localScale = new Vector3(scaleSize,scaleSize,1);

            particle.GetChild(1).localScale = new Vector3(scaleSize,scaleSize,1);
            particle.GetChild(2).localScale = new Vector3(scaleSize,scaleSize,1);
            particle.GetChild(3).localScale = new Vector3(scaleSize,scaleSize,1);
            // Debug.Log(particle.position);
            StartCoroutine(RemoveParticle(particle, "this"));
        }

        // float scaleSize = 1 - (5 - Camera.main.orthographicSize) * 0.2f;

        // var shape = particle.GetComponent<ParticleSystem>().shape;
        // shape.scale = new Vector3(scaleSize,scaleSize,1);
        // Debug.Log(shape.scale);
        particle.gameObject.SetActive(true);

        particle.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator RemoveParticle (Transform item, string mess = "")
    {
        Transform particle;
        if(mess == "")
        {
            particle = item.transform.GetChild(0);
            Vector3 scale = particle.localScale;
            particle.SetParent(ParticleList.transform.Find(particle.name));
            particle.localScale = scale;
        }
        else
        {
            particle = item;
            // Debug.Log(particle.parent.lossyScale.x*100); 
        }
        // item.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(1f);

        // Transform parent = ParticleList.transform.Find(particle.name);    
        // particle.SetParent(parent);
        particle.GetComponent<ParticleSystem>().Stop();
        particle.gameObject.SetActive(false);
        // item.GetComponent<Image>().enabled = true;
    }
#endregion ParticleSystem

#region Animaion move an object in an arc
    void PlayMoveScaleAnimation (Transform obj, float scale)
    {
        float scaleY = obj.localScale.y;
        // Debug.Log(scaleY);
        // Debug.Log(scale);
        obj.DOScale(new Vector3(scaleY * 0.9f, scaleY * 1.1f, 1f), 0.03f).SetEase(Ease.InOutQuad);
        obj.DOScale(new Vector3(scaleY, scaleY, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.06f);
        obj.DOScale(new Vector3(scaleY * 0.9f, scaleY * 1.1f, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.1f);
        obj.DOScale(new Vector3(scale * 0.9f, scale * 1.1f, 1f), 0.27f).SetEase(Ease.InOutQuad).SetDelay(0.13f);

        // Sequence scaleSequence = DOTween.Sequence();
        // scaleSequence.Append(obj.DOScale(new Vector3(scaleX * 0.9f, scaleX * 1.1f, 1f), 0.03f)).SetEase(Ease.InOutQuad);
        // scaleSequence.Append(obj.DOScale(new Vector3(scaleX, scaleX, 1f), 0.03f)).SetEase(Ease.InOutQuad);
        // scaleSequence.Append(obj.DOScale(new Vector3(scale * 0.9f, scale * 1.1f, 1f), 0.03f)).SetEase(Ease.InOutQuad);
        // scaleSequence.Append(obj.DOScale(new Vector3(scale * 0.9f, scale * 1.1f, 1f), 0.03f)).SetEase(Ease.InOutQuad);
        // scaleSequence.Play();
    }

    IEnumerator EvaluateSlerpPoints(GameObject obj, Vector3 start, Vector3 path, float interpolateAmount) {
        // yield return new WaitForSeconds(Time.deltaTime);
        Vector3 addToEnd = new Vector3(0, obj.GetComponent<Renderer>().bounds.size.y / 20, 0);
        Vector3 end = obj.transform.parent.position + addToEnd;
        // Debug.Log(obj.GetComponent<Renderer>().bounds.size);
        // Debug.Log(obj.GetComponent<SpriteRenderer>().bounds.size);
        // float time = 0;

        while (obj.transform.position!= end)
        {
            float deltaTime = Time.deltaTime;
            interpolateAmount += deltaTime * 2.5f;
            // time += deltaTime;
            yield return new WaitForSeconds(deltaTime);
            obj.transform.position = QuadraticLerp(start, path, end, interpolateAmount);
            end = obj.transform.parent.position + addToEnd;

            // if(Vector3.Distance(obj.transform.position, end) <= 0.1f)
            if((end - obj.transform.position).magnitude <= 0.1f)
            {
                // Debug.Log(time);
                obj.transform.position = end;
                break;
            }
        }

        StartCoroutine(RemoveParticle(obj.transform));
        dragable.allDragables[obj.name].status = "found";
        CheckListItemCollected(obj);
        BounceItem(obj.transform);
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);
        return Vector3.Lerp(ab_bc, bc_cd, t);
    }
#endregion Animaion move an object in an arc

#region Button
    public void Exit(int currentMap = 0)
    {
        ResetData();
        if(currentMap == 1) LoadGame();
        else
        {
            GameUI.SetActive(false);
            gameObject.SetActive(false);
            Home.SetActive(true);
            audioManager.ChangeMusicBackground("bgm_home");
            Home.GetComponent<Home>().SetLevel();
            if(mapNum > 7) sale.CheckTimeToOpenDiaLog();
        }
    }

    public void PauseGame()
    {
        setting.SetAnotherPause();
    }
    
    public void PauseGameAction()
    {
        pauseCountDownToGameOver = true;
        touchControl.StopMapAction();
        touchControl.StopTouchItem();
    }

    public void ContinueGameAction()
    {
        pauseCountDownToGameOver = false;
        touchControl.AllowTouchItem();
        touchControl.AllowMapAction();
    }

    public void PauseCountDown()
    {
        pauseCountDownToGameOver = true;
    }

    public void ContinueCountDown()
    {
        pauseCountDownToGameOver = false;
    }
#endregion Button

#region Check Support Tools
    public Transform AddTime;
    void CheckSupportTools(bool thunder, bool addTime)
    {
        if(thunder) Invoke("UseThunder", 1.5f);
        if(addTime) Invoke("AddMoreTimeToCurrentTime", 1.5f);
    }

    void UseThunder()
    {
        supportTools.UseThunder();
        adsManager.LogEvent($"Use_Thunder");
    }

    void AddMoreTimeToCurrentTime()
    {
        adsManager.LogEvent($"Use_Tool_Add_Time_Start_Game");
        Transform effect = timerTxt.transform.GetChild(0);
        AddTime.localPosition = Vector3.zero;
        AddTime.localScale = Vector3.zero;
        AddTime.gameObject.SetActive(true);
        AddTime.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);

        AddTime.DOScale(1f, 1f).SetEase(Ease.OutBack);
        AddTime.DOMove(timerTxt.transform.position, 0.5f).SetDelay(1.567f).SetEase(Ease.InQuad).OnComplete(() => {
            effect.gameObject.SetActive(true);
            effect.GetComponent<ParticleSystem>().Play();
        });
        AddTime.DOScale(0f, 0.3f).SetDelay(2.067f).SetEase(Ease.OutBack);

        timerTxt.transform.DOScale(1.3f, 0.3f).SetDelay(2.067f).SetEase(Ease.OutBack).OnComplete(() => {
            AddTime.gameObject.SetActive(false);
            Timer += 30;
            int minutes = Mathf.FloorToInt(Timer / 60);
            int seconds = Mathf.FloorToInt(Timer % 60);

            timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        });
        timerTxt.transform.DOScale(1f, 0.3f).SetDelay(2.367f).SetEase(Ease.OutBack)
            .OnComplete(() => { effect.gameObject.SetActive(false); });
    }
#endregion
}









/**
    {
        "Map": 4,
        "ItemList": ["Human5", "Pepxi", "House9", "Car5", "Human4", "House15", "Stop", "House14"],
        "QuantityList": [6,3,9,3,6,3,3,6],
        "Moving": null,
        "Time": 180
    },
    {
        "Map": 5,
        "ItemList": ["Log", "House8", "House11", "Umbrella", "Human6", "Pepxi", "Car5"],
        "QuantityList": [6,6,6,9,9,6,3],
        "Moving": ["Car5"],
        "Time": 180
    },
    {
        "Map": 10,
        "ItemList": ["Tree3", "RedPanda"],
        "QuantityList": [30,12],
        "Moving": null,
        "Time": 300
    },
    {
        "Map": 12,
        "ItemList": ["Car2", "House4", "Human8", "Human1", "House14", "Human2", "Motobike2"],
        "QuantityList": [6,3,6,9,6,6,3],
        "Moving": ["Car2"],
        "Time": 180  
    }
**/
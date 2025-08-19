using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeGame : MonoBehaviour
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
    public GameObject challengeGame;
    public Slider scoreSlider;
    public TextMeshProUGUI txtLv;
    public Sale sale;

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
    }

#region Handel Data Game
    public void LoadGame(int map, bool thunder = false, bool addTime = false)
    {
        mapNum = map;
        gameObject.GetComponent<MainGame>().enabled = false;
        gameObject.GetComponent<ChallengeGame>().enabled = true;
        touchControl.AllowMapAction();

        touchControl.AllowTouchItem();
        // set the initial value of the map
        SetMapValue();
        // set initial map limit movement value
        touchControl.SetDefaultLimit(currentMap);
        // set List and quantity items need to find
        moveFollowPath.CheckPath(mapNum);
        // start Count down to game over
        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(CountdownToGameOver());
        // CheckTutorial();
        weatherControl.SetWeather();
        CheckSupportTools(thunder, addTime);

        PlayLoadingScreen();
        audioManager.ChangeMusicBackground("bgm_maingame");
    }

    void PlayLoadingScreen()
    {
        if(loading.gameObject.activeSelf) return;
        PauseGameAction();
        loading.PlayAnimLoading(2.5f);
        win.ResetAnim();
        Invoke("EndLoading", 3f);
    }

    void EndLoading()
    {
        ContinueGameAction();
    }

    // private int itemFound = 0;
    private int itemTotal;

    public void CaculateScoreSlider()
    {
        int itemFound = ((List<string>)saveDataJson.GetData($"ListChallenge{mapNum}")).Count;
        scoreSlider.value = (float)itemFound / (float)itemTotal;
        // Debug.Log(itemFound);
        if(scoreSlider.value == 1) GameCompleted();
    }

    void SetMapValue()
    {
        // if(saveDataJson.TakeMapData().map.Length == mapNum) mapNum--;

        GameObject map = challengeGame.transform.Find($"Map/MapChallenge{mapNum}").gameObject;
        itemTotal = map.transform.Find("ListItemToFind").childCount;
        map.SetActive(true);
        challengeGame.SetActive(true);
        scoreSlider.gameObject.SetActive(true);

        // scoreSlider.value = itemFound;
    
        currentMap = map.transform.Find("Map_Bg").gameObject;
        ListItemInMap = map.transform.Find("ListItemToFind").gameObject;
        txtLv.text = $"Level {mapNum}";
        supportTools.CheckToolAllows((int)saveDataJson.GetData("OpenedMap"), ListItemInMap);
        CheckItemInMap();
    }

    public void CheckItemInMap()
    {
        Transform listItemToFind = challengeGame.transform.Find($"Map/MapChallenge{mapNum}/ListItemToFind");
        List<string> ListitemFound = (List<string>)saveDataJson.GetData($"ListChallenge{mapNum}");
        List<string> ListitemInSlot = (List<string>)saveDataJson.GetData($"ListItemSlotChallenge{mapNum}");
        int itemFound = ListitemFound.Count;
        for (int i = 0; i < itemFound; i++)
        {
            Transform item = listItemToFind.Find(ListitemFound[i]);
            item.gameObject.SetActive(false);
        }
        scoreSlider.value = (float)itemFound / (float)itemTotal;

        for (int i = 0; i < ListitemInSlot.Count; i++)
        {
            Transform item = listItemToFind.Find(ListitemInSlot[i]);
            AddItemToListItemCollected(dragable.allDragables[item.name]);
        }
    }

    public void SaveFoundItem(Transform item)
    {
        saveDataJson.SaveData($"ListChallenge{mapNum}", item.name);
        saveDataJson.RemoveItemFromList($"ListItemSlotChallenge{mapNum}", item.name);
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
        gameObject.GetComponent<MainGame>().enabled = true;
        gameObject.GetComponent<ChallengeGame>().enabled = false;
        scoreSlider.gameObject.SetActive(false);

        // Debug.Log(mapNum);
        challengeGame.SetActive(false);
        challengeGame.transform.Find($"Map/MapChallenge{mapNum}").gameObject.SetActive(false);
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
        Timer = moreTime == 0 ? ((List<int>)saveDataJson.GetData("ChallengeRemainingTime"))[mapNum-1] : moreTime;
        UpdateTimeDisplay(Timer);
        while (Timer > 0)
        {
            yield return new WaitForSeconds(1f);

            if(!pauseCountDownToGameOver) Timer -= 1;
            UpdateTimeDisplay(Timer);
        }

        PauseGameAction();
        lose.SetValue("TimeOut", mapNum);
    }

    void UpdateTimeDisplay(int time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

#endregion Handel Data Game

#region Handle Game Over
    void CheckSlot(GameObject obj)
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
            lose.SetValue("OutOfSlots", mapNum);
        }
    }

    void GameCompleted()
    {
        // stopCountDownToGameOver = true;
        if(win.gameObject.activeSelf) return;
        PauseGameAction();
        win.PlayAnimation(mapNum);
        adsManager.LogEvent($"CompletedChallengeMap_{mapNum}");
        saveDataJson.SaveData("CompletedChallengeMap", mapNum);

        DeleteValue();
        Home.GetComponent<Home>().ChangeMapChallengeImage(mapNum);
    }

    public void DeleteValue()
    {
        saveDataJson.ChangeChallengeRemainingTime(mapNum - 1, 1800);

        saveDataJson.SaveData($"ListItemSlotChallenge{mapNum}", null);
        saveDataJson.SaveData($"ListChallenge{mapNum}", null);
    }
#endregion Handle Game Over

#region Handle Collect Item
    public void AddItemToListItemCollected (dragable item, string txt = "")
    {
        if(txt == "tool") usingSupport = true;
        if (ListItemCollected.transform.GetChild(6).childCount != 0) return;

        // if(BgTutorial.activeSelf && currentTutorialItem != "" && item.name != currentTutorialItem) return;
        item.transform.DOKill();
        item.GetComponent<Renderer>().sortingOrder = 110;

        Transform newParent = ChooseParentForItem(item);

        AddParticleToItem(item.transform, "Follow");

        item.transform.SetParent(newParent);
        // SetItemValue(item.name.Split(" (")[0]);

        CaculateScale(item.transform, newParent);

        Vector3 path = new Vector3(newParent.position.x, item.transform.position.y + 4, item.transform.position.z);
        item.status = "moving";
        adsManager.VibrationDevice();
        if(item.name.Length >= 3 && item.name.Substring(0, 3) == "Car")
        {
            if(item.GetComponent<SkeletonAnimation>() != null) item.GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1");
        }

        saveDataJson.SaveData($"ListItemSlotChallenge{mapNum}", item.name);
        StartCoroutine(EvaluateSlerpPoints(item.gameObject, item.transform.position, path, 0));
        // if(BgTutorial.activeSelf) SetTutorialItem();
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

        // item.DOScale(new Vector3(itemScale, itemScale, 1f), 0.2f);
        item.DOScale(new Vector3(itemScale * 0.7f, itemScale * 0.7f, 1f), 0.15f).SetEase(Ease.InOutQuad);
        item.DOScale(new Vector3(itemScale * 1.2f, itemScale * 1.2f, 1f), 0.07f).SetEase(Ease.InOutQuad).SetDelay(0.15f);
        item.DOScale(new Vector3(itemScale * 0.9f, itemScale * 0.9f, 1f), 0.05f).SetEase(Ease.InOutQuad).SetDelay(0.22f);
        item.DOScale(new Vector3(itemScale , itemScale, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.27f);
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
        float scale = item.transform.localScale.x;
        item.DOScale(new Vector3(scale * 0.97f, scale * 0.97f, 1f), 0.03f).SetEase(Ease.InOutQuad);
        item.DOScale(new Vector3(scale * 1.03f, scale * 1.03f, 1f), 0.06f).SetEase(Ease.InOutQuad).SetDelay(0.03f);
        item.DOScale(new Vector3(scale * 0.97f, scale * 0.97f, 1f), 0.06f).SetEase(Ease.InOutQuad).SetDelay(0.09f);
        item.DOScale(new Vector3(scale , scale, 1f), 0.03f).SetEase(Ease.InOutQuad).SetDelay(0.15f);
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

        dragable.allDragables[child1.name].status = "combining";
        dragable.allDragables[child2.name].status = "combining";
        dragable.allDragables[child3.name].status = "combining";
        CombineItems(child1, child2, child3);
    }

    void CombineItems(Transform child1, Transform child2, Transform child3)
    {
        Vector3 localPos1 = child1.parent.InverseTransformPoint(child2.position);
        Vector3 localPos3 = child3.parent.InverseTransformPoint(child2.position);
        SaveFoundItem(child1);
        SaveFoundItem(child2);
        SaveFoundItem(child3);
        audioManager.PlaySFX("merge");
        child1.DOLocalMove(localPos1, 0.3f);
        child3.DOLocalMove(localPos3, 0.3f).OnComplete(() => 
        {
            adsManager.VibrationDevice(1);
            AddParticleToItem(child2,"Merge","nochange");
            CaculateScoreSlider();
            // combining = false;
            SetItemToFistValue(child1);
            SetItemToFistValue(child2);
            SetItemToFistValue(child3);
            // RearrangeListItemCollected();

            StartCoroutine(RearrangeListItemCollected());
        });
        moveFollowPath.StopMovingParent(child1, child2, child3);
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
                // if(childDrag.status == "moving") continue;
                childDrag.status = "rearranging";
                child.SetParent(ListItemCollected.transform.GetChild(i - count));
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
            else CheckSlot(obj);
        }
        else
        {
            CheckSlot(obj);
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
    IEnumerator EvaluateSlerpPoints(GameObject obj, Vector3 start, Vector3 path, float interpolateAmount) {
        yield return new WaitForSeconds(Time.deltaTime);
        Vector3 end = obj.transform.parent.position;

        while (obj.transform.position != end)
        {
            interpolateAmount += Time.deltaTime * 2.5f;
            yield return new WaitForSeconds(Time.deltaTime);
            // Debug.Log("??");
            obj.transform.position = QuadraticLerp(start, path, end, interpolateAmount);
            end = obj.transform.parent.position;

            if(Vector3.Distance(obj.transform.position, end) <= 0.1f)
            {
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
    public void Exit(string txt = "")
    {
        ResetData();
        saveDataJson.ChangeChallengeRemainingTime(mapNum - 1, Timer);
        if(txt == "deleteValue") DeleteValue();
        GameUI.SetActive(false);
        gameObject.SetActive(false);
        Home.SetActive(true);
        sale.CheckTimeToOpenDiaLog();
        audioManager.ChangeMusicBackground("bgm_home");
        // Home.GetComponent<Home>().SetLevel();
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
        if(thunder) Invoke("UseThunder", 3f);
        if(addTime) Invoke("AddMoreTimeToCurrentTime", 3f);
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
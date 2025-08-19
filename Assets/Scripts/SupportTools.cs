using System;
using System.Collections;

// using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupportTools : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public MainGame mainGame;
    public ChallengeGame challengeGame;
    public GameObject ListItemCollected;
    public GameObject CompassArrow;
    public GameObject ListItemToFind;
    public GameObject canvas;

    public ParticleSystem iceParticle;
    public GameObject IceParticle;
    public ParticleSystem iceParticle1;
    public ParticleSystem iceParticle2;
    public ParticleSystem iceParticle3;
    public ParticleSystem iceParticle4;


    public MoveFollowPath moveFollowPath;
    public GameObject Thunder;

    [Header(" Text tool")]
    public TextMeshProUGUI TextMagnet;
    public TextMeshProUGUI TextUndo;
    public TextMeshProUGUI TextComPass;
    public TextMeshProUGUI TextFreezeTimer;

    private GameObject ListItemInMap;
    private int MapNum;
    private bool PauseTool = false;
    // private bool AddMoreTime = false;

    public ShopTool shopTool;

    public void CheckToolAllows (int mapNum, GameObject listItemInMap)
    {
        ListItemInMap = listItemInMap;
        MapNum = mapNum;

        TextMagnet.text = $"{(int)saveDataJson.GetData("Magnet")}";
        TextUndo.text = $"{(int)saveDataJson.GetData("Undo")}";
        TextComPass.text = $"{(int)saveDataJson.GetData("Compass")}";
        TextFreezeTimer.text = $"{(int)saveDataJson.GetData("FreezeTimer")}";

        int num = 0;
        if(mapNum >= 12) num = 4;
        else if(mapNum >= 10) num = 3;
        else if(mapNum >= 8) num = 2;
        else if(mapNum >= 4) num = 1;

        Transform tool;
        for(int i = 0; i < num; i++)
        {
            tool = transform.GetChild(i);
            tool.GetChild(1).gameObject.SetActive(false);
            tool.GetChild(0).GetChild(0).gameObject.SetActive(true);
            tool.GetComponent<Button>().enabled = true;
        }
    }

    public void AllowUseTool()
    {
        PauseTool = false;
    }

#region FreezeTimer
    private bool isFreezing = false;
    public void FreezeTimer()
    {
        if(isFreezing || PauseTool) return;
        int freezeTimerNum = (int)saveDataJson.GetData("FreezeTimer");
        if(freezeTimerNum == 0)
        {
            shopTool.OpenAnimation("FreezeTimer");
            return;
        }

        if(mainGame.enabled) mainGame.TurnOffToolTutorial();

        adsManager.LogEvent($"FreezeTimer");
        freezeTimerNum--;
        TextFreezeTimer.text = $"{freezeTimerNum}";
        saveDataJson.SaveData("FreezeTimer", freezeTimerNum);

        isFreezing = true;
        CancelInvoke("StopIceParticle");
        if(mainGame.enabled) mainGame.PauseCountDown();
        else if(challengeGame.enabled) challengeGame.PauseCountDown();

        iceParticle1.Clear();
        iceParticle2.Clear();
        iceParticle3.Clear();
        iceParticle4.Clear();

        ChangeParticleSize();

        IceParticle.gameObject.SetActive(true);
        iceParticle1.Play();
        var main = iceParticle1.main;
        main.loop = true;
        iceParticle2.Play();
        main = iceParticle2.main;
        main.loop = true;
        iceParticle3.Play();
        main = iceParticle3.main;
        main.loop = true;
        iceParticle4.Play();
        main = iceParticle4.main;
        main.loop = true;

        for(int i = 0 ; i < 4; i++)
        {
            IceParticle.transform.GetChild(0).GetChild(i).GetComponent<Image>().DOFade(1, 1.5f);
        }
        
        Invoke("ContinueCountDown", 15);
    }

    public void ChangeParticleSize()
    {

        float scale = 0.0008209766f * Camera.main.orthographicSize;
        float xx = -canvas.GetComponent<RectTransform>().rect.x * scale * 2;
        float yy = -canvas.GetComponent<RectTransform>().rect.y * scale * 2;

        float scaleSize= xx * 0.15f;

        iceParticle1.transform.position = new Vector3(-xx/2, 0, iceParticle1.transform.position.z);
        var shape = iceParticle1.shape;
        shape.scale = new Vector3(scaleSize, yy, 1);

        iceParticle2.transform.position = new Vector3(xx/2, 0, iceParticle2.transform.position.z);
        shape = iceParticle2.shape;
        shape.scale = new Vector3(scaleSize, yy, 1);

        iceParticle3.transform.position = new Vector3(0, -yy/2, iceParticle3.transform.position.z);
        shape = iceParticle3.shape;
        shape.scale = new Vector3(xx, scaleSize, 1);

        iceParticle4.transform.position = new Vector3(0, yy/2, iceParticle4.transform.position.z);
        shape = iceParticle4.shape;
        shape.scale = new Vector3(xx, scaleSize, 1);
    }

    void ContinueCountDown()
    {
        if(mainGame.enabled) mainGame.ContinueCountDown();
        else if(challengeGame.enabled) challengeGame.ContinueCountDown();
        isFreezing = false;

        var main = iceParticle1.main;
        main.loop = false;
        main = iceParticle2.main;
        main.loop = false;
        main = iceParticle3.main;
        main.loop = false;
        main = iceParticle4.main;
        main.loop = false;
        for(int i = 0 ; i < 4; i++)
        {
            IceParticle.transform.GetChild(0).GetChild(i).GetComponent<Image>().DOFade(0, 1.5f);
        }
        Invoke("StopIceParticle", 2);
    }

    void StopIceParticle()
    {
        IceParticle.gameObject.SetActive(false);
    }
#endregion FreezeTimer

#region Magnet
    public void Magnet()
    {
        if(PauseTool) return;
        int magnetNum = (int)saveDataJson.GetData("Magnet");

        audioManager.PlaySFX("click");
        if(magnetNum == 0)
        {
            shopTool.OpenAnimation("Magnet");
            return;
        }
    
        // PauseTool = true;
        TakeItemToFind();
    }

    void TakeItemToFind()
    {
        string itemName = "";
        int count = 0;
        string itemInChildName = "";
        Transform itemtoFind = null;
        for(int i = 0; i < 7; i++)
        {
            Transform child = ListItemCollected.transform.GetChild(i);

            if(child.childCount > 0)
            {
                itemInChildName = child.GetChild(0).name.Split(" (")[0];
                itemtoFind = ListItemToFind.transform.Find(itemInChildName);
                if(itemtoFind != null && itemtoFind.GetChild(1)
                    .GetChild(0).GetComponent<TextMeshProUGUI>().text == "0") continue;

                if(itemName == "")
                {
                    // take first item in ListItemCollected
                    count++;
                    itemName = itemInChildName;
                }
                else
                {
                    // check item after the first
                    if(itemInChildName == itemName) count++;
                    else break;
                }
            }
        }
        // Debug.Log(itemName);
        count = count == 3 ? 0 : count;
        MagnetItem(itemName, count);
    }

    void MagnetItem(string itemName, int count)
    {
        if(count >= 3) return;
        if(ListItemCollected.transform.GetChild(4 + count).childCount > 0) return;
        if(itemName == "")
        {
            if(mainGame.enabled)
            {
                foreach(Transform child in ListItemToFind.transform)
                {
                    if(child.gameObject.activeSelf
                        && child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text != "0")
                    {
                        itemName = child.name;
                        break;
                    }
                }
            }
            else if(challengeGame.enabled)
            {
                foreach(Transform child in ListItemInMap.transform)
                {
                    if(child.gameObject.activeSelf && dragable.allDragables[child.name].status == "wait")
                    {
                        itemName = child.name.Split(" (")[0];
                        break;
                    }
                }
            }
        }
        int length = ListItemInMap.transform.childCount;
        if(mainGame.enabled) mainGame.TurnOffToolTutorial();
        for(int i = length - 1; i >= 0; i --)
        {
            Transform child = ListItemInMap.transform.GetChild(i);
            if(child.gameObject.activeSelf && child.name.Split(" (")[0] == itemName)
            {
                dragable itemDragable = dragable.allDragables[child.name];
                if(itemDragable.status == "moving") continue;
                count++;
                
                if(mainGame.enabled) mainGame.AddItemToListItemCollected(dragable.allDragables[child.name], "tool");
                else if(challengeGame.enabled) challengeGame.AddItemToListItemCollected(dragable.allDragables[child.name], "tool");
                if(count == 3) 
                {
                    adsManager.LogEvent($"Magnet");

                    int magnetNum = (int)saveDataJson.GetData("Magnet");
                    magnetNum--;
                    TextMagnet.text = $"{magnetNum}";
                    saveDataJson.SaveData("Magnet", magnetNum);
                    break;
                };
            }
        }
    }
#endregion Magnet

#region Undo
    public void Undo()
    {
        if(PauseTool) return;
        int undoNum = (int)saveDataJson.GetData("Undo");

        audioManager.PlaySFX("click");
        if(undoNum == 0)
        {
            shopTool.OpenAnimation("Undo");
            return;
        }

        int num = ListItemCollected.transform.childCount;
        if(mainGame.enabled) mainGame.TurnOffToolTutorial();

        for(int i = num - 1; i >= 0; i--)
        {
            Transform child = ListItemCollected.transform.GetChild(i);
            if(child.childCount == 1 && dragable.allDragables[child.GetChild(0).name].status != "combining")
            {
                undoNum--;
                adsManager.LogEvent($"Undo");
                TextUndo.text = $"{undoNum}";
                saveDataJson.SaveData("Undo", undoNum);
                Transform item = ListItemToFind.transform.Find(child.GetChild(0).name.Split(" (")[0]);
                if(item != null)
                {
                    TextMeshProUGUI itemNum = item.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
                    itemNum.text = $"{int.Parse(itemNum.text) + 1}";
                }
                MoveItemToFistValue(child.GetChild(0));
                break;
            }
        }
    }

    public void ClearAllSlots()
    {
        Transform item;
        TextMeshProUGUI itemNum;
        Transform child;
        for(int i = 6; i >= 0; i--)
        {
            child = ListItemCollected.transform.GetChild(i);
            if(child.childCount == 1)
            {
                item = ListItemToFind.transform.Find(child.GetChild(0).name.Split(" (")[0]);
                if(item != null)
                {
                    itemNum = item.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
                    itemNum.text = $"{int.Parse(itemNum.text) + 1}";
                }
                MoveItemToFistValue(child.GetChild(0));
            }
        }
    }

    void MoveItemToFistValue(Transform item)
    {
        PauseTool = true;
        dragable itemStatus = dragable.allDragables[item.name];
        item.SetParent(ListItemInMap.transform);
        item.DOKill();
        item.DOScale(itemStatus.FirstScale, 0.2f);
        Transform parent = moveFollowPath.CheckItemInList(item.gameObject);

        if(parent == null)
        {
            Vector3 targetPosition = itemStatus.FirstPosition + mainGame.transform.position;
            Vector3 path = new Vector3(targetPosition.x, item.transform.position.y - 3, item.transform.position.z);
            StartCoroutine(EvaluateSlerpPoints(item.gameObject, item.transform.position, path, targetPosition, 0));
        }
        else
        {
            StartCoroutine(EvaluateSlerpPoints(item.gameObject, item.transform.position, parent, 0));
        }
    }

    void SetItemToFistValue(GameObject item)
    {
        dragable itemStatus = dragable.allDragables[item.name];
        item.transform.localScale = itemStatus.FirstScale;
        item.GetComponent<Renderer>().sortingOrder = itemStatus.Layer;
        PauseTool = false;
        itemStatus.status = "wait";
    }
#endregion Undo

#region  Compass
    private Vector2 XLimit;
    private Vector2 YLimit;
    int TimeUseCompass = 0;
    public void ChangeLimited (Vector2 xLimit, Vector2 yLimit)
    {
        XLimit = xLimit; YLimit = yLimit;
// ChangeParticleSize();
        if(IceParticle.gameObject.activeSelf) ChangeParticleSize();
    }
    public void Compass()
    {
        if(CompassArrow.activeSelf) return;
        if(PauseTool) return;
        int compassNum = (int)saveDataJson.GetData("Compass");

        audioManager.PlaySFX("click");
        if(compassNum == 0)
        {
            shopTool.OpenAnimation("Compass");
            return;
        }

        TimeUseCompass = 0;
        CancelInvoke("TimeOutOfUseCompass");
        Invoke("TimeOutOfUseCompass", 15);
        GameObject item = TakeAvailiableItemsToFind();

        if(item == null) return;
        if(mainGame.enabled) mainGame.TurnOffToolTutorial();
        CompassArrow.SetActive(true);
        adsManager.LogEvent($"Compass");
        compassNum--;
        TextComPass.text = $"{compassNum}";
        saveDataJson.SaveData("Compass", compassNum);

        // compassTxt.text = $"{(int)saveDataJson.GetData("Compass")}";
        StartCoroutine(RotationCompass(item));
        
    }

    GameObject TakeAvailiableItemsToFind()
    {
        if(TimeUseCompass == 3) return null;
        string itemName = TakeItemInData();
        GameObject itemToFind = null;
        if(mainGame.enabled)
        {
            foreach(Transform child in ListItemInMap.transform)
            {
                // string childName = child.name.Split(" (")[0];
                if(child.gameObject.activeSelf && dragable.allDragables[child.name].status == "wait" && child.name.Split(" (")[0] == itemName)
                {
                    Transform parent = moveFollowPath.CheckItem(child.name);
                    if(parent != null && parent.childCount == 0)
                    {
                        continue;
                    }
                    itemToFind = child.gameObject;
                    // Debug.Log(itemToFind);
                    break;
                }
            }
        }
        else if(challengeGame.enabled)
        {
            foreach(Transform child in ListItemInMap.transform)
            {
                if(child.gameObject.activeSelf && dragable.allDragables[child.name].status == "wait")
                {
                    if(itemName == "") { itemToFind = child.gameObject; break;}
                    else if(child.name.Split(" (")[0] == itemName)
                    {
                        itemToFind = child.gameObject;
                        break;
                    }
                }
            }
        }
        TimeUseCompass++;
        return itemToFind;
    }

    string TakeItemInData()
    {
        string itemName = "";
        if(mainGame.enabled)
        {
            SaveDataJson.Map mapData = saveDataJson.TakeMapData().map[MapNum];
            for(int i = 0; i < 7; i++)
            {
                Transform child = ListItemCollected.transform.GetChild(i);
                if(child.childCount > 0 && mapData.ItemList.Contains(child.GetChild(0).name.Split(" (")[0]))
                {
                    itemName = child.GetChild(0).name.Split(" (")[0];
                    break;
                }
                else if (i == 0 && child.childCount > 0) itemName = child.GetChild(0).name.Split(" (")[0];
            }

            if(itemName == "")
            {
                foreach(Transform child in ListItemToFind.transform)
                {
                    if(child.gameObject.activeSelf)
                    {
                        itemName = child.name; break;
                    }
                }
            }
        }
        else if(challengeGame.enabled)
        {
            for(int i = 0; i < 7; i++)
            {
                Transform child = ListItemCollected.transform.GetChild(i);
                if(child.childCount > 0)
                {
                    itemName = child.GetChild(0).name.Split(" (")[0];
                    break;
                }
            }
        }
        return itemName;
    }

    IEnumerator RotationCompass (GameObject item)
    {
        UpdateCompassRotation(item);

        yield return new WaitForSeconds(Time.deltaTime);
        if(item == null || !item.activeSelf || dragable.allDragables[item.name].status != "wait")
        {
            // item = TakeRandomItem();
            item = TakeAvailiableItemsToFind();
        }

        if(item == null) CompassArrow.SetActive(false);
        else if(CompassArrow.activeSelf) StartCoroutine(RotationCompass(item));
    }

    private void UpdateCompassRotation(GameObject targetItem)
    {
        CheckLimited(targetItem);
        Vector3 vectorToTarget = targetItem.transform.position - CompassArrow.transform.position;
        int num = 0; //thay đổi theo hướng mũi tên ban đầu của image
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - num;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        CompassArrow.transform.rotation = Quaternion.Slerp(CompassArrow.transform.rotation, q, Time.deltaTime * 20);
    }

    void CheckLimited (GameObject item)
    {
        Vector3 itemPos = item.transform.position;
        float scaleX = canvas.transform.localScale.x * 100;

        Vector3 normalizedDirection = (itemPos - Vector3.zero).normalized;
        Vector3 pointA = itemPos - normalizedDirection * 1.5f * scaleX;

        CompassArrow.transform.position = new Vector3
        (
            x:Mathf.Clamp(value:pointA.x, min:XLimit.y + 1.5f * scaleX, max:XLimit.x - 1.5f * scaleX),
            y:Mathf.Clamp(value:pointA.y, min:YLimit.x + 2.1f * scaleX, max:YLimit.y - 0.9f * scaleX),
            0
        );
    }


    // Hàm tính vị trí điểm A trực tiếp từ một vị trí bất kỳ
    public static Vector2 CalculatePointA(Vector2 currentItemPosition)
    {
        Vector2 directionToOrigin = (Vector2.zero - currentItemPosition).normalized;
        return currentItemPosition + directionToOrigin;
    }

    void TimeOutOfUseCompass()
    {
        CompassArrow.SetActive(false);
    }
#endregion Compass

#region Thunder
    public void UseThunder()
    {
        string nameItem1 = "";
        string nameItem2 = "";
        string nameItem3 = "";
        string childName;
        int count1 = 0 ;
        int count2 = 0 ;
        int count3 = 0 ;
        if(mainGame.enabled)
        {
            SaveDataJson.Map mapData = saveDataJson.TakeMapData().map[MapNum];
            foreach(Transform child in ListItemInMap.transform)
            {
                childName = child.name.Split(" (")[0];
                if(mapData.ItemList.Contains(childName)) continue;

                if(nameItem1 == "") nameItem1 = childName;
                else if(nameItem2 == "" && childName != nameItem1)nameItem2 = childName;
                else if(nameItem3 == "" && childName != nameItem1 && childName != nameItem2)nameItem3 = childName;

                if(childName == nameItem1 && count1 < 3)
                {
                    count1++;
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                else if(childName == nameItem2 && count2 < 3)
                {
                    count2++;
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                else if(childName == nameItem3 && count3 < 3)
                {
                    count3++;
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                if(count1 + count2 + count3 == 9) break;
            }
        }
        else if(challengeGame.enabled)
        {
            ChallengeGame obj = challengeGame.GetComponent<ChallengeGame>();
            foreach(Transform child in ListItemInMap.transform)
            {
                childName = child.name.Split(" (")[0];
                if(!child.gameObject.activeSelf || dragable.allDragables[child.name].status != "wait") continue;
                if(nameItem1 == "") nameItem1 = childName;
                else if(nameItem2 == "" && childName != nameItem1)nameItem2 = childName;
                else if(nameItem3 == "" && childName != nameItem1 && childName != nameItem2)nameItem3 = childName;

                if(childName == nameItem1 && count1 < 3)
                {
                    count1++;
                    obj.SaveFoundItem(child);
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                else if(childName == nameItem2 && count2 < 3)
                {
                    count2++;
                    obj.SaveFoundItem(child);
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                else if(childName == nameItem3 && count3 < 3)
                {
                    count3++;
                    obj.SaveFoundItem(child);
                    StartCoroutine(DeleteItem(child, count1 + count2 + count3));
                }
                if(count1 + count2 + count3 == 9) 
                {
                    // obj.CaculateScoreSlider();
                    obj.CaculateScoreSlider();
                    break;
                }
            }
        }
    }

    IEnumerator DeleteItem(Transform item, int time)
    {
        dragable.allDragables[item.name].status = "found";
        yield return new WaitForSeconds(time * 0.2f);
        // Debug.Log(time);
        SpawnThunder(item);
        StartCoroutine(DestroyItem(item));
    }
    
    IEnumerator DestroyItem(Transform item)
    {
        yield return new WaitForSeconds(2f);

        item.DOKill();
        item.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            item.gameObject.SetActive(false);
            SetItemToFistValue(item.gameObject);
        });
    }

    void SpawnThunder(Transform item)
    {
        int length = Thunder.transform.parent.childCount;
        Transform lightning = null;
        for(int i = 0; i < length; i++)
        {
            Transform child = Thunder.transform.parent.GetChild(i);
            if(!child.gameObject.activeSelf)
            {
                lightning = child;
                break;
            }
        }

        if(lightning == null) lightning = Instantiate(Thunder, Vector3.zero, Quaternion.identity).transform;
        lightning.SetParent(Thunder.transform.parent);
        lightning.gameObject.SetActive(true);
        lightning.position = item.position + new Vector3(0,19,0);
        // Debug.Log(item.name);
        // Debug.Log(item.position);
        // lightning.GetComponent<ParticleSystemRenderer>().sortingOrder = dragable.allDragables[item.name].Layer + 1;
        lightning.GetComponent<ParticleSystem>().Play();
        Invoke("PlayThunderSound", 0.5f);

        StartCoroutine(RemoveParticle(lightning));
    }

    void PlayThunderSound()
    {
        audioManager.PlaySFX("thunder");
    }

    IEnumerator RemoveParticle(Transform lightning)
    {
        yield return new WaitForSeconds(7);
        lightning.gameObject.SetActive(false);
    }
#endregion Thunder

#region Animaion move an object in an arc
        IEnumerator EvaluateSlerpPoints(GameObject obj, Vector3 start, Transform parent, float interpolateAmount) {
        yield return new WaitForSeconds(Time.deltaTime);
        interpolateAmount += Time.deltaTime * 3f;
        obj.transform.position = QuadraticLerp(start, parent.position, parent.position, interpolateAmount);

        if(obj.transform.position != parent.position) StartCoroutine(EvaluateSlerpPoints(obj, start, parent, interpolateAmount));
        else
        {
            obj.transform.SetParent(parent);
            SetItemToFistValue(obj);
            // moveFollowPath.SetItemDirection(obj.transform);
        }
    }

    IEnumerator EvaluateSlerpPoints(GameObject obj, Vector3 start, Vector3 path, Vector3 end, float interpolateAmount) {
        yield return new WaitForSeconds(Time.deltaTime);
        interpolateAmount += Time.deltaTime * 3f;
        obj.transform.position = QuadraticLerp(start, path, end, interpolateAmount);

        if(obj.transform.position != end) StartCoroutine(EvaluateSlerpPoints(obj, start, path, end, interpolateAmount));
        else
        {
            SetItemToFistValue(obj);
        }
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
}

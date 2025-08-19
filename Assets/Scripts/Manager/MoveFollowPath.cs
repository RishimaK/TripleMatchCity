using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

public class MoveFollowPath : MonoBehaviour
{
    SaveDataJson.Map mapData;
    public SaveDataJson saveDataJson;
    Transform ListItemInMap;
    public GameObject MoveFollow;

    private Transform Path;
    private List<Transform> listItemAvailable = new List<Transform>();
    private List<Transform> listParentMove = new List<Transform>();

    void Start()
    {
       
    }

    public void CheckPath(int mapNum)
    {
        mapData = saveDataJson.TakeMapData().map[mapNum];
        listItemAvailable = new List<Transform>();
        listParentMove = new List<Transform>();
        if(mapData.Moving.Length == 0) return;
        Path = gameObject.transform.parent.Find($"MainGame/Map/Map{mapNum}/Path");
        ListItemInMap = gameObject.transform.parent.Find($"MainGame/Map/Map{mapNum}/ListItemToFind");

        TakeListItemToMove();
    }

    Transform TakeParentMoveFollow(Transform item)
    {
        Transform parent = null;
        foreach(Transform child in transform)
        {
            if(!child.gameObject.activeSelf)
            {
                parent = child;
                break;
            }
        }
        if(parent == null)
        {
            parent = Instantiate(MoveFollow , Vector3.zero, Quaternion.identity).transform;
            parent.SetParent(transform);
        }
        listParentMove.Add(parent);
        parent.gameObject.SetActive(true);
        parent.position = item.position;
        item.SetParent(parent);

        parent.name = item.name;
        parent.SetParent(ListItemInMap.transform);

        return parent;
    }

    void TakeListItemToMove()
    {
        int length = mapData.Moving.Length;
        // int listItemLength = ListItemInMap.transform.childCount;

        for(int i = 0; i < length; i++)
        {
            Transform child = ListItemInMap.transform.Find(mapData.Moving[i]);

            listItemAvailable.Add(child);
            Transform path = Path.transform.GetChild(0);
            child.position = path.GetChild(0).position;
            Transform parent = TakeParentMoveFollow(child);
            StartCoroutine(MoVeToPath(path, parent, 1, i * 10));

            // for(int j = 0; j < listItemLength; j++)
            // {
            //     Transform child = ListItemInMap.transform.GetChild(j);

            //     if(child.name == mapData.Moving[i])
            //     {
            //         listItemAvailable.Add(child);
            //         Transform path = Path.transform.GetChild(0);
            //         child.position = path.GetChild(0).position;
            //         Transform parent = TakeParentMoveFollow(child);

            //         StartCoroutine(MoVeToPath(path, parent, 1, count * 10));
            //         count++;
            //         j--;
            //         listItemLength--;
            //         break;
            //     }
            // }
        }
    }

    public void StopMovingParent(Transform child1, Transform child2, Transform child3)
    {
        // if(!listItemAvailable.Contains(child1.transform)) return;
        // Debug.Log(child1.name);
        // Debug.Log(child2.name);
        // Debug.Log(child3.name);

        Transform parent = listParentMove.Find(obj => obj.name == child1.name);
        if(parent != null)
        {   
            parent.DOKill();
            parent.SetParent(transform);
            parent.gameObject.SetActive(false);
        }

        parent = listParentMove.Find(obj => obj.name == child2.name);
        if(parent != null)
        {   
            parent.DOKill();
            parent.SetParent(transform);
            parent.gameObject.SetActive(false);
        }

        parent = listParentMove.Find(obj => obj.name == child3.name);
        if(parent != null)
        {   
            parent.DOKill();
            parent.SetParent(transform);
            parent.gameObject.SetActive(false);
        }
    }

    public Transform CheckItemInList(GameObject item)
    {
        if(!listItemAvailable.Contains(item.transform)) return null;
        Transform parent = listParentMove.Find(obj => obj.name == item.name);
        return parent;
        // item.transform.position = parent.position;
        // item.transform.SetParent(parent);
    }

    public Transform CheckItem(string name)
    {
        if(listItemAvailable.Count == 0) return null;
        Transform parent = listParentMove.Find(obj => obj.name == name);
        return parent;
    }

    public void SetItemDirection (Transform item)
    {
        // Skeleton ske = item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton;
        // if(angle > 90) ske.SetSkin("Car1");
        // else if(angle > 0) ske.SetSkin("Car1_2");
        // else if(angle > -90) ske.SetSkin("Car1_3");
        // else ske.SetSkin("Car1_1");
    }

    IEnumerator MoVeToPath(Transform path, Transform item, int pathNum, float waitTime = 0)
    {
        Vector3 targetPos = path.GetChild(pathNum).position - transform.parent.position;
        Vector3 vectorToTarget = targetPos - (path.GetChild(pathNum - 1).position - transform.parent.position);
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        if(item.childCount > 0)
        {
            Skeleton ske = item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton;
            if(angle > 90) ske.SetSkin("Car1");
            else if(angle > 0) ske.SetSkin("Car1_2");
            else if(angle > -90) ske.SetSkin("Car1_3");
            else ske.SetSkin("Car1_1");
        }

        float speed = 0.7f;
        // float speed = 0.1f;
        float time = Vector3.Distance(
            path.GetChild(pathNum - 1).position - transform.parent.position, targetPos
        ) / speed;
        yield return new WaitForSeconds(waitTime);

        if(item.gameObject.activeSelf)
        {
            item.DOLocalMove(targetPos, time).OnComplete(() => {
                pathNum++;

                if(path.childCount <= pathNum)
                {
                    item.position = path.GetChild(0).position;
                    pathNum = 1;
                }

                StartCoroutine(MoVeToPath(path, item, pathNum));
            });
        }

        // while (item.gameObject.activeSelf)
        // {
        //     Vector3 targetPos = path.GetChild(pathNum).position - transform.parent.position;
        //     Vector3 vectorToTarget = targetPos - (path.GetChild(pathNum - 1).position - transform.parent.position);
        //     float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        //     if(angle > 90) item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1");
        //     else if(angle > 0) item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1_2");
        //     else if(angle > -90) item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1_3");
        //     else item.GetChild(0).GetComponent<SkeletonAnimation>().skeleton.SetSkin("Car1_1");

        //     float speed = 0.7f;
        //     float time = Vector3.Distance(
        //         path.GetChild(pathNum - 1).position - 
        //         transform.parent.position, targetPos
        //     ) / speed;
        //     yield return new WaitForSeconds(waitTime);
        //     Debug.Log("??");
        //     item.DOLocalMove(targetPos, time).OnComplete(() => {
        //         pathNum++;

        //         if(path.childCount <= pathNum)
        //         {
        //             item.position = path.GetChild(0).position;
        //             pathNum = 1;
        //         }

        //         StartCoroutine(MoVeToPath(path, item, pathNum));
        //     });
        // }
    }

    public void Reset()
    {
        if(mapData.Moving.Length == 0) return;
        int num = listParentMove.Count;
        for(int i = 0; i < num; i++)
        {
            Transform child = listParentMove[i];
            if(!child.gameObject.activeSelf) continue;
            child.DOKill();
            child.gameObject.SetActive(false);
            child.transform.SetParent(transform);
            if(child.childCount != 1) continue;
            child.GetChild(0).SetParent(ListItemInMap);
        }
    }
}

using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class dragable : MonoBehaviour
{
    public static Dictionary<string, dragable> allDragables = new Dictionary<string, dragable>();
    public Vector3 FirstPosition;
    public Vector3 FirstScale;
    public int Layer;
    public Spine.Skin InitialSkin = null;

    public string status = "wait";
    /**
        wait: allowe to be touch
        combining: is combining;
        moving: is moving
        found: found and not do anything
        rearranging: is rearranging list collected;
    **/

    public int myVariable;

    void Awake()
    {
        FirstPosition = gameObject.transform.position;
        Layer = gameObject.GetComponent<Renderer>().sortingOrder;
        FirstScale = gameObject.transform.localScale;

        allDragables[gameObject.name] = this;

        string name = gameObject.name;
        if(name.Length >= 3 && name.Substring(0, 3) == "Car")
        {
            if(gameObject.GetComponent<SkeletonAnimation>() != null)
            {
                // Debug.Log(gameObject.GetComponent<SkeletonAnimation>().skeleton.FindSlot("Car1"));
                // Debug.Log(gameObject.GetComponent<SkeletonAnimation>().skeleton.Data);
                InitialSkin = gameObject.GetComponent<SkeletonAnimation>().skeleton.Skin;
            }
            // InitialSkin = gameObject.GetComponent<SkeletonAnimation>().skeleton.Skin;
        }
    }

    void OnDestroy()
    {
        // Xóa khỏi Dictionary khi object bị hủy
        if(allDragables.ContainsKey(gameObject.name))
        {
            allDragables.Remove(gameObject.name);
        }
    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
        // Debug.Log("Touch Start");
        // Debug.Log(collision.gameObject.name);
    // }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     Debug.Log("Touch Stay");
    // }

    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     Debug.Log("Touch End");
    // }


}

// public class dragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
// {
//     public void OnDrag(PointerEventData eventData)
//     {
//         Debug.Log(eventData);
//     }

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         Debug.Log(eventData);
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         Debug.Log(eventData);
//     }

// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZoomAndMove : MonoBehaviour
{
    private Vector3 touchStart;
    public float maxZoom = 3;
    public float minZoom = 15;
    public float speed = 10;
    float targetZoom;

    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }
    void Update()
    {
        cameraController();
    }

    void zoom(float increment)
    {
        targetZoom = Mathf.Clamp(cam.orthographicSize - increment, maxZoom, minZoom);
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
    }

    void cameraController() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference);
        } else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += direction;
        }
        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

// #region GPT
//     public float maxZoom = 100f;
//     public float minZoom = 10f;
//     public float zoomSpeed = 10f;
//     public float moveSpeed = 5f;

//     private Camera cam;
//     private Vector3 lastMousePosition;
//     private float targetZoom;

//     void Start()
//     {
//         cam = Camera.main;
//         if (!cam.orthographic)
//         {
//             Debug.LogWarning("This script is designed for orthographic cameras!");
//         }
//         targetZoom = cam.orthographicSize;
//     }

//     void Update()
//     {
//         HandleZoom();
//         HandleMove();
//     }

//     void HandleZoom()
//     {
//         // Zoom với scroll wheel
//         float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
//         if (scrollWheel != 0)
//         {
//             targetZoom -= scrollWheel * zoomSpeed;
//             targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
//         }

//         // Zoom multi-touch
//         if (Input.touchCount == 2)
//         {
//             Touch touchZero = Input.GetTouch(0);
//             Touch touchOne = Input.GetTouch(1);

//             Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
//             Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

//             float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
//             float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

//             float difference = currentMagnitude - prevMagnitude;
//             targetZoom -= difference * 0.01f;
//             targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
//         }

//         // Smooth zoom
//         cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
//     }

//     void HandleMove()
//     {
//         // Di chuyển camera
//         if (Input.GetMouseButtonDown(0))
//         {
//             lastMousePosition = Input.mousePosition;
//         }

//         if (Input.GetMouseButton(0))
//         {
//             Vector3 delta = cam.ScreenToWorldPoint(Input.mousePosition) - cam.ScreenToWorldPoint(lastMousePosition);
//             transform.Translate(-delta * moveSpeed * Time.deltaTime);
//             lastMousePosition = Input.mousePosition;
//         }
//     }
// #endregion

    // public Camera Camera;
    // public bool Rotate;
    // protected Plane Plane;

    // private void Awake()
    // {
    //     if (Camera == null)
    //         Camera = Camera.main;
    // }

    // private void Update()
    // {

    //     //Update Plane
    //     if (Input.touchCount >= 1)
    //         Plane.SetNormalAndPosition(transform.up, transform.position);

    //     var Delta1 = Vector3.zero;
    //     var Delta2 = Vector3.zero;

    //     //Scroll
    //     if (Input.touchCount >= 1)
    //     {
    //         Delta1 = PlanePositionDelta(Input.GetTouch(0));
    //         if (Input.GetTouch(0).phase == TouchPhase.Moved)
    //             Camera.transform.Translate(Delta1, Space.World);
    //     }

    //     //Pinch
    //     if (Input.touchCount >= 2)
    //     {
    //         var pos1  = PlanePosition(Input.GetTouch(0).position);
    //         var pos2  = PlanePosition(Input.GetTouch(1).position);
    //         var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
    //         var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

    //         //calc zoom
    //         var zoom = Vector3.Distance(pos1, pos2) /
    //                    Vector3.Distance(pos1b, pos2b);

    //         //edge case
    //         if (zoom == 0 || zoom > 10)
    //             return;

    //         //Move cam amount the mid ray
    //         Camera.transform.position = Vector3.LerpUnclamped(pos1, Camera.transform.position, 1 / zoom);

    //         if (Rotate && pos2b != pos2)
    //             Camera.transform.RotateAround(pos1, Plane.normal, Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, Plane.normal));
    //     }

    // }

    // protected Vector3 PlanePositionDelta(Touch touch)
    // {
    //     //not moved
    //     if (touch.phase != TouchPhase.Moved)
    //         return Vector3.zero;

    //     //delta
    //     var rayBefore = Camera.ScreenPointToRay(touch.position - touch.deltaPosition);
    //     var rayNow = Camera.ScreenPointToRay(touch.position);
    //     if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
    //         return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

    //     //not on plane
    //     return Vector3.zero;
    // }

    // protected Vector3 PlanePosition(Vector2 screenPos)
    // {
    //     //position
    //     var rayNow = Camera.ScreenPointToRay(screenPos);
    //     if (Plane.Raycast(rayNow, out var enterNow))
    //         return rayNow.GetPoint(enterNow);

    //     return Vector3.zero;
    // }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawLine(transform.position, transform.position + transform.up);
    // }
}

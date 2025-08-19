using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour
{
#region Variables

    [Header("Touch Control")]
    private bool touchItem = true;
    private bool mapAction = true;
    private bool _isDragActive = false;
    private bool _isScreenChange = false;

    public Camera MainCamera;
    private Vector3 offset;
    private Vector3 offset2;
    private dragable _lastDragged; 
    // private bool _draggMove = false;
    private Vector2 _checkPos;
    private Vector2 XLimit;
    private Vector2 YLimit;

    private Vector3 testPos;
    private Vector3 touchPos;

    private Vector2 _screenPosition;
    private Vector3 _worldPosition;
    private bool moveAllowed;

    private int fistFingerId = 0;

    private float zoomMin = 3;
    private float zoomMax = 15;
    private float fistCamerasize = 15;

    [Header("__________________")]
    private GameObject currentMap;
    public SupportTools supportTools;

    public GameObject canvas;

    public MainGame mainGame;
    public ChallengeGame challengeGame;

    public GameObject GameUI;

    public AudioManager audioManager;

    private Vector2 bodyHeight;

        
#endregion Variables

    void Start()
    {
    }

    public void AllowTouchItem()
    {
        touchItem = true;
    }

    public void StopTouchItem()
    {
        touchItem = false;
    }

    public void AllowMapAction()
    {
        mapAction = true;
    }

    public void StopMapAction()
    {
        mapAction = false;
    }

    void Update()
    {
        // if(Camera.main.orthographicSize < 15) Zoom(-0.4f);
        // if(Camera.main.orthographicSize > 3) Zoom(0.1f);
        if(touchItem) HandleTouchItems();
        if(mapAction) HandleZoomAndMove();
    }

    #region Handle Touch Items
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

        _worldPosition = MainCamera.ScreenToWorldPoint(_screenPosition);
        // if(_isDragActive) Drag();
        // else
        // {

            // RaycastHit2D hit = Physics2D.Raycast(_worldPosition, Vector2.zero);
            // if(hit.collider != null)
            // {
            //     dragable dragable = hit.transform.gameObject.GetComponent<dragable>();
            //     Debug.Log(dragable.GetComponent<dragable>());
            //     if(dragable != null)
            //     {
            //         _lastDragged = dragable;
            //         InitDrag();
            //     }
            // } 
            // // else
            // // {
            // //     if(Input.GetTouch(0).phase == TouchPhase.Ended && !_isScreenChange)
            // //     {
                    
            // //     }
            // // }


            RaycastHit2D[] hit = Physics2D.RaycastAll(_worldPosition, Vector2.zero);

            if(hit.Length > 0)
            {
                dragable dragable = null;

                if(hit.Length == 1) dragable = hit[0].transform.gameObject.GetComponent<dragable>();
                else
                {
                    int highestLayer = 0;
                    int itemLayer = 0;
                    bool checkCircleCollider = true;
                    RaycastHit2D item = hit[0];
                    
                    foreach (RaycastHit2D item2D in hit)
                    {
                        // itemLayer = item2D.transform.GetComponent<Renderer>() != null ? 
                        //     item2D.transform.GetComponent<Renderer>().sortingOrder :
                        //     item2D.transform.GetChild(0).GetComponent<Renderer>().sortingOrder;
                        itemLayer = item2D.transform.GetComponent<Renderer>().sortingOrder;
                        

                        if(item2D.collider is PolygonCollider2D)
                        {
                            if(checkCircleCollider)
                            {
                                checkCircleCollider = false;
                                item = item2D;
                                highestLayer = itemLayer;
                            }
                            else if(highestLayer < itemLayer)
                            {
                                highestLayer = itemLayer;
                                item = item2D;
                            }
                        }
                        else if(checkCircleCollider)
                        {
                            if(highestLayer < itemLayer)
                            {
                                highestLayer = itemLayer;
                                item = item2D;
                            }
                        }
                    }
                    dragable = item.transform.gameObject.GetComponent<dragable>();
                }

                if(dragable != null)
                {
                    _lastDragged = dragable;
                    InitDrag();
                }
            }
        // }
    }

    void InitDrag()
    {
        _isDragActive = true;
        // _draggMove = false;
        // _checkPos = Input.GetTouch(0).position;
    }

    void Drag()
    {   
        // if(_checkPos != Input.GetTouch(0).position) _draggMove = true;
    }

    void Drop()
    {
        bool isInBounds = 
            // _worldPosition.x >= XLimit.x && _worldPosition.x <= XLimit.y &&
            _worldPosition.y >= bodyHeight.x && _worldPosition.y <= bodyHeight.y;
        // check limit to take item

        if(isInBounds && Input.touchCount < 2)
        {
            dragable item = _lastDragged;
            if(!_isScreenChange && item.status == "wait" && item.gameObject.activeSelf)
            {
                audioManager.PlaySFX("choose");
                if(mainGame.enabled) mainGame.AddItemToListItemCollected(item);
                else challengeGame.AddItemToListItemCollected(item);
            }
        }
        if(!mapAction) ResetDragState(); 
    }

    void ResetDragState()
    {
        _isDragActive = false;
        _lastDragged = null;
        // _draggMove = false;
    }
#endregion Handle Touch Items

#region Handle Zoom And Move
    void HandleZoomAndMove()
    {
        if (Input.touchCount == 0) return;
        if(Input.touchCount == 2){
            _isScreenChange = true;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            TakeOffset(touch1, "2");
            if(EventSystem.current != null &&
                (EventSystem.current.IsPointerOverGameObject(touch0.fingerId)
                || EventSystem.current.IsPointerOverGameObject(touch1.fingerId))) return;

            // float zoomDelta = Vector2.Distance(touch0.position, touch1.position) - 
            //     Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

            float zoomDelta = (touch0.position - touch1.position).magnitude - 
                ((touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition)).magnitude; 

            Zoom(zoomDelta * 0.015f);
            if(touch0.phase == TouchPhase.Ended && touch1.phase == TouchPhase.Ended)
            {
                _isScreenChange = false;
                ResetDragState();
            }
        }
        else if(Input.touchCount == 1)
        {
            if(mainGame.enabled) mainGame.TurnOffTutorial();

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
                    testPos = MainCamera.ScreenToWorldPoint(touch.position);
                    TakeOffset(touch);
                    break;
                case TouchPhase.Moved:
                    if(moveAllowed)
                    {
                        Vector3 currentPos = MainCamera.ScreenToWorldPoint(touch.position);
                        // if(Vector3.Distance(testPos, currentPos) >= 0.1f)
                        if(_isScreenChange || (testPos - currentPos).magnitude >= 0.1f)
                        {
                            _isScreenChange = true;

                            currentPos.z = MainCamera.transform.position.z; // Đảm bảo cùng một lớp z
                            transform.position = currentPos - offset;
                            SetLimited();
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    bool isInBounds = _worldPosition.y >= bodyHeight.x && _worldPosition.y <= bodyHeight.y;
                    if(isInBounds && !_isScreenChange && _lastDragged == null) PlayParticleTouchNone();
                    ResetDragState();
                    _isScreenChange = false;
                    break;
            }                
        }
    }

    void TakeOffset(Touch touch, string val = null)
    {
        touchPos = MainCamera.ScreenToWorldPoint(touch.position);
        touchPos.z = MainCamera.transform.position.z; // Đảm bảo cùng một lớp z

        if(val == null) offset = touchPos - transform.position;
        else offset2 = touchPos - transform.position;
    }

    private void Zoom(float increment)
    {
        if(mainGame.enabled) mainGame.TurnOffZoomTutorial();
        float currentCamSize = MainCamera.orthographicSize;
        // MainCamera.GetComponent<ViewportHandler>().ChangeUnitsSize(increment, zoomMin, zoomMax);
        MainCamera.orthographicSize = Mathf.Clamp(value:MainCamera.orthographicSize - increment, zoomMin, zoomMax);
        
        ChangeLimited(currentCamSize);
    }

    Vector2 PixelToUnit(Vector2 pixelPosition)
    {
        return MainCamera.ScreenToWorldPoint(new Vector3(pixelPosition.x, pixelPosition.y, MainCamera.nearClipPlane));
    }

    Vector2 UnitToPixel(Vector2 unitPosition)
    {
        Vector3 worldPosition = new Vector3(unitPosition.x, unitPosition.y, MainCamera.nearClipPlane);
        Vector3 screenPosition = MainCamera.WorldToScreenPoint(worldPosition);
        return new Vector2(screenPosition.x, screenPosition.y);
    }

    void ChangeLimited(float currentCamSize, float val = 0)
    {
        val = val == 0 ? MainCamera.orthographicSize : val;

        RectTransform rectBody = GameUI.transform.Find("Body").GetComponent<RectTransform>();
        Vector2 mapSize = currentMap.GetComponent<SpriteRenderer>().bounds.size;
        float bodyPos = 0.7006567f / 12 * val;
        // float bodyPos = rectBody.position.y;

        XLimit += new Vector2 (-(currentCamSize - val) * MainCamera.aspect, (currentCamSize - val) * MainCamera.aspect);

        float scaleY = 0.0008209766f * val;

        // Debug.Log(rectBody.position.y);
        // Debug.Log(0.7006567f / 12 * 15);
        float uiHeightInUnits = rectBody.rect.y * scaleY;

        // uiHeightInUnits = rectBody.position.y;
        // Debug.Log(bodyPos + " / " + bodyPos *scaleY*100);
        // Debug.Log(rectBody.position.y + " / " + rectBody.position.y * scaleY*100);
        // currentBodyY = rectBody.rect.y;
        supportTools.ChangeLimited(
            new Vector2(-rectBody.rect.x * scaleY, rectBody.rect.x * scaleY), 
            new Vector2(uiHeightInUnits + bodyPos, -uiHeightInUnits + bodyPos)
        );
        // scaleY *= 100;

        YLimit = new Vector2(
            mapSize.y / 2 + uiHeightInUnits + bodyPos,
            -mapSize.y / 2 - uiHeightInUnits + bodyPos
        );

        // YLimit = new Vector2(
        //     mapSize.y / 2 + uiHeightInUnits,
        //     -mapSize.y / 2 - uiHeightInUnits
        // );
        bodyHeight = new Vector2(uiHeightInUnits + bodyPos, -uiHeightInUnits + bodyPos);
        SetLimited();
    }

    void SetLimited()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3
        (
            x:Mathf.Clamp(value:currentPosition.x, min:XLimit.x, max:XLimit.y),
            y:Mathf.Clamp(value:currentPosition.y, min:YLimit.y, max:YLimit.x),
            currentPosition.z
        ); 
    }

    public void SetDefaultLimit(GameObject map = null)
    {
        // MainCamera.orthographicSize = 15;
        if(map.transform.parent.name == "Map1") fistCamerasize = 9;
        else fistCamerasize = 15;
        // MainCamera.orthographicSize = fistCamerasize;
        // float bodyPos = 0.7006567f / 12 * fistCamerasize;

        currentMap = map == null ? currentMap : map;
        Vector2 mapSize = currentMap.GetComponent<SpriteRenderer>().bounds.size;
        RectTransform rectBody = GameUI.transform.Find("Body").GetComponent<RectTransform>();
        // float scaleY = 0.0008209766f * MainCamera.orthographicSize;
        // float uiHeightInUnits = rectBody.rect.y * scaleY;
        float uiWidthInUnits = rectBody.rect.x * canvas.transform.localScale.x;
        float uiHeightInUnits = rectBody.rect.y * canvas.transform.localScale.y;
        bodyHeight = new Vector2(uiHeightInUnits + rectBody.position.y, -uiHeightInUnits + rectBody.position.y);
        // currentBodyY = rectBody.position.y;
        // Debug.Log(rectBody.position.y); //0.8758208
        // Debug.Log(MainCamera.orthographicSize);

        XLimit = new Vector2(-mapSize.x / 2 - uiWidthInUnits, mapSize.x / 2 + uiWidthInUnits);
        YLimit = new Vector2(
            mapSize.y / 2 + uiHeightInUnits + rectBody.position.y,
            -mapSize.y / 2 - uiHeightInUnits + rectBody.position.y
        );

        supportTools.ChangeLimited(new Vector2(-uiWidthInUnits, uiWidthInUnits), 
            new Vector2(uiHeightInUnits + rectBody.position.y, -uiHeightInUnits + rectBody.position.y));
        CheckLimited();
    }

    public void CheckLimited() 
    {
        float currentCamSize = MainCamera.orthographicSize;
        if(currentCamSize != fistCamerasize)
        {
            MainCamera.orthographicSize = fistCamerasize;
            ChangeLimited(currentCamSize);
            // SetDefaultLimit();
        }
        // else supportTools.ChangeParticleSize();
    }

#endregion Handle Zoom And Move
    public void MoveMapToTutorialItem(Transform item)
    {
        touchItem = false;
        // mapAction = false;
        Vector3 mapPos = gameObject.transform.position;
        float xx = mapPos.x - item.position.x;
        float yy = mapPos.y - item.position.y;
        Vector3 newPosition = new Vector3(xx, yy, mapPos.z);

        newPosition = new Vector3
        (
            x:Mathf.Clamp(value:newPosition.x, min:XLimit.x, max:XLimit.y),
            y:Mathf.Clamp(value:newPosition.y, min:YLimit.y, max:YLimit.x),
            newPosition.z
        );

        gameObject.transform.DOMove(newPosition, 0.5f).OnComplete(() => 
        {
            touchItem = true;
            // mapAction = true;
        });
    }


#region ParticleSystem
    public GameObject ClickNonePrefab;
    public GameObject ParticleClick;
    public void PlayParticleTouchNone()
    {
        audioManager.PlaySFX("wrong");
        Touch touch =Input.GetTouch(0);
        Vector3 currentPos = MainCamera.ScreenToWorldPoint(touch.position);
        currentPos.z = 0;

        GameObject clickNone = null;
        foreach (Transform child in ParticleClick.transform)
        {
            if(!child.gameObject.activeSelf)
            {
                clickNone = child.gameObject;
                break;
            }
        }

        if(clickNone == null)
        {
            clickNone = ObjectPoolManager.SpawnObject(ClickNonePrefab, Vector3.zero, Quaternion.identity);
            clickNone.transform.SetParent(ParticleClick.transform);
        } else clickNone.SetActive(true);
        
        clickNone.transform.position = currentPos;
        clickNone.GetComponent<ParticleSystem>().Play();
        StartCoroutine(HideParticle(clickNone));
    }

    IEnumerator HideParticle(GameObject particle)
    {
        yield return new WaitForSeconds(1.5f);
        particle.SetActive(false);
    }
#endregion ParticleSystem
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Touch_Control : MonoBehaviour
{
    [Header("Tap")]
    [Tooltip("The maximum movement for a touch motion to be treated as a tap")]
    public float maxDistanceForTap = 40;
    [Tooltip("The maximum duration for a touch motion to be treated as a tap")]
    public float maxDurationForTap = 0.4f;
    [Header("Desktop debug")]
    [Tooltip("Use the mouse on desktop?")]
    public bool useMouse = true;
    [Tooltip("The simulated pinch speed using the scroll wheel")]
    public float mouseScrollSpeed = 2;
    Vector2 touch0StartPosition;
    Vector2 touch0LastPosition;
    float touch0StartTime;
    bool cameraControlEnabled = true;
    bool canUseMouse;
    /// <summary> Has the player at least one finger on the screen? </summary>
    public bool isTouching { get; private set; }
    public delegate void OnSwipeDelegate(Vector2 pos);
    public static OnSwipeDelegate OnSwipe = pos => { };
    public delegate void OnTapDelegate(Vector2 pos);
    public static OnTapDelegate OnTap = pos => { };
    public delegate void OnTouchReleasedDelegate(Vector2 pos);
    public static OnTouchReleasedDelegate OnTouchReleased = pos => { };
    public delegate void OnHoldDelegate(Vector2 pos);
    public static OnHoldDelegate OnHold = pos => { };
    public delegate void OnTouchDownDelegate();
    public static OnTouchDownDelegate OnTouchDown = () => { };
    public delegate void OnPinchDelegate(Vector2 vector, float prevDistance, float newDistance, Vector2 vectorNormalized);
    public static OnPinchDelegate OnPinch = (vector, distance, newDistance, normalized) => { };
    /// <summary> The point of contact if it exists in Screen space. </summary>
    public Vector2 touchPosition
    {
        get { return touch0LastPosition; }
    }
    private void Start()
    {
        canUseMouse = Application.platform != RuntimePlatform.Android &&
                      Application.platform != RuntimePlatform.IPhonePlayer && Input.mousePresent;
    }
    private void Update()
    {
        if (IsPointerOverUIObject()) return;
#if UNITY_EDITOR
        UpdateWithMouse();
#else 
        UpdateWithTouch();
#endif
    }
    private void UpdateWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touch0StartPosition = Input.mousePosition;
            touch0StartTime = Time.time;
            touch0LastPosition = touch0StartPosition;
            OnTouchDown();
            isTouching = true;
        }
        if (Input.GetMouseButton(0) && isTouching)
        {
            Vector2 move = (Vector2)Input.mousePosition - touch0LastPosition;
            touch0LastPosition = Input.mousePosition;
            if (move != Vector2.zero)
            {
                OnSwipe(move);
            }
            OnHold(move);
        }
        if (Input.GetMouseButtonUp(0) && isTouching)
        {
            if (Time.time - touch0StartTime <= maxDurationForTap
                && Vector2.Distance(Input.mousePosition, touch0StartPosition) <= maxDistanceForTap)
            {
                OnTap(Input.mousePosition);
            }
            else
            {
                OnTouchReleased(Input.mousePosition);
            }
            isTouching = false;
            cameraControlEnabled = true;
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            OnPinch(Input.mousePosition, 1, Input.mouseScrollDelta.y < 0 ? (1 / mouseScrollSpeed) : mouseScrollSpeed,
                Vector2.right);
        }
    }
    void UpdateWithTouch()
    {
        int touchCount = Input.touches.Length;
        if (touchCount == 1)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        touch0StartPosition = touch.position;
                        touch0StartTime = Time.time;
                        touch0LastPosition = touch0StartPosition;
                        OnTouchDown();
                        isTouching = true;
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        touch0LastPosition = touch.position;
                        if (touch.deltaPosition != Vector2.zero && isTouching)
                        {
                            OnSwipe(touch.deltaPosition);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    {
                        if (Time.time - touch0StartTime <= maxDurationForTap
                            && Vector2.Distance(touch.position, touch0StartPosition) <= maxDistanceForTap
                            && isTouching)
                        {
                            OnTap(touch.position);
                        }
                        else
                        {
                            OnTouchReleased(touch.position);
                        }
                        isTouching = false;
                        cameraControlEnabled = true;
                        break;
                    }
                case TouchPhase.Stationary:
                    OnHold(Vector2.zero);
                    break;
                case TouchPhase.Canceled:
                    break;
            }
        }
        else if (touchCount == 2)
        {
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];
            if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended) return;
            isTouching = true;
            float previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition,
                touch1.position - touch1.deltaPosition);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);
            if (previousDistance != currentDistance)
            {
                OnPinch((touch0.position + touch1.position) / 2, previousDistance, currentDistance,
                    (touch1.position - touch0.position).normalized);
            }
        }
        else
        {
            if (isTouching)
            {
                isTouching = false;
            }
            cameraControlEnabled = true;
        }
    }
    /// <summary> Checks if the the current input is over canvas UI </summary>
    public bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

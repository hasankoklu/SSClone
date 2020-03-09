using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public enum SwipeType { None, Right, Left, Up, Down }
    public SwipeType currentSwipe;
    public bool isSwiping;
    public Vector2 firstVector, lastVector;

    public static SwipeManager instance;

    bool isMouseUp;
    bool isMouseDown;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public delegate void OnSwipeDelegate();
    public static OnSwipeDelegate SwipeDelegate;

    public void OnSwipe()
    {
        SwipeDelegate();
    }

    private void Update()
    {
        if (GameManager.instance && GameManager.instance.isGameRunning)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseDown = true;
                isMouseUp = false;
                StartCoroutine(StartSwipeDetector());
            }
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
                isMouseUp = true;
            }
        }
    }

    public IEnumerator StartSwipeDetector(float swipeTime = 0.2f)
    {
        while (!isMouseUp)
        {
            firstVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            yield return new WaitForSeconds(swipeTime);
            lastVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 deltaVector = DetectSwipe(lastVector - firstVector);
            OnSwipe();
            Debug.Log(currentSwipe);
            yield return null;
        }
    }

    Vector2 DetectSwipe(Vector2 deltaVector)
    {
        if (Mathf.Abs(deltaVector.x) > Mathf.Abs(deltaVector.y)) //X Axis
        {
            if (deltaVector.x > 0)
            {
                currentSwipe = SwipeType.Right;
            }
            else if (deltaVector.x < 0)
            {
                currentSwipe = SwipeType.Left;
            }
        }
        else if (Mathf.Abs(deltaVector.x) < Mathf.Abs(deltaVector.y))//Y Axis
        {
            if (deltaVector.y > 0)
            {
                currentSwipe = SwipeType.Up;
            }
            else if (deltaVector.y < 0)
            {
                currentSwipe = SwipeType.Down;
            }
        }
        else
        {
            currentSwipe = SwipeType.None;
        }

        return deltaVector;
    }
}

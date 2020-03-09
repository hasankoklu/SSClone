using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    #region Variables

    [HideInInspector]
    public bool isMoving = false;
    private bool activeCoroutine = false;
    private bool bufferActive;
    private GameManager gameManager;

    private float SmoothLerpFactor;

    #endregion


    #region JourneyBufferClass

    class JourneyBuffer
    {
        public float startPosX;
        public float endPosX;
        public float startPosZ;
        public float endPosZ;
        public SwipeManager.SwipeType direction;
    }

    List<JourneyBuffer> jbList = new List<JourneyBuffer>();
    JourneyBuffer activeJB = new JourneyBuffer();
    JourneyBuffer LastJB = new JourneyBuffer();

    #endregion


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        LastJB.endPosX = this.transform.position.x;
        LastJB.endPosZ = this.transform.position.z;
        StartCoroutine(FinishChecker());
    }

    private void Start()
    {
        SwipeManager.SwipeDelegate += onSwipe;
    }

    private void onSwipe()
    {
        Engine();
    }

    private void Engine()
    {
        if (!gameManager.isGameRunning)
            return;

        if (SwipeManager.instance.currentSwipe == SwipeManager.SwipeType.Right)
        {
            JourneyBuffer jb = new JourneyBuffer();
            jb.startPosX = LastJB.endPosX;
            jb.startPosZ = LastJB.endPosZ;

            List<Vector3> tempPuzzleGOsList_v3 = new List<Vector3>();

            tempPuzzleGOsList_v3 = gameManager.PuzzleGOsList_v3.Where(x => x.z == jb.startPosZ && x.x > jb.startPosX).ToList().OrderByDescending(p => p.x).ToList();
            if (tempPuzzleGOsList_v3.Count > 0)
                jb.endPosX = tempPuzzleGOsList_v3.LastOrDefault().x - 1f;
            else
                jb.endPosX = gameManager.PlaneGOsList_v3.Where(x => x.z == jb.startPosZ && x.x >= jb.startPosX).OrderByDescending(p => p.x).FirstOrDefault().x;

            jb.direction = SwipeManager.SwipeType.Right;
            LastJB.endPosX = jb.endPosX;

            jbList.Add(jb);
        }
        if (SwipeManager.instance.currentSwipe == SwipeManager.SwipeType.Left)
        {
            JourneyBuffer jb = new JourneyBuffer();
            jb.startPosX = LastJB.endPosX;
            jb.startPosZ = LastJB.endPosZ;

            List<Vector3> tempPuzzleGOsList_v3 = new List<Vector3>();

            tempPuzzleGOsList_v3 = gameManager.PuzzleGOsList_v3.Where(x => x.z == jb.startPosZ && x.x < jb.startPosX).ToList().OrderByDescending(p => p.x).ToList();
            if (tempPuzzleGOsList_v3.Count > 0)
                jb.endPosX = tempPuzzleGOsList_v3.LastOrDefault().x + 1f;
            else
                jb.endPosX = gameManager.PlaneGOsList_v3.Where(x => x.z == jb.startPosZ && x.x <= jb.startPosX).OrderByDescending(p => p.x).LastOrDefault().x;

            jb.direction = SwipeManager.SwipeType.Left;
            LastJB.endPosX = jb.endPosX;
            jbList.Add(jb);

        }
        if (SwipeManager.instance.currentSwipe == SwipeManager.SwipeType.Up)
        {
            JourneyBuffer jb = new JourneyBuffer();
            jb.startPosX = LastJB.endPosX;
            jb.startPosZ = LastJB.endPosZ;

            List<Vector3> tempPuzzleGOsList_v3 = new List<Vector3>();

            tempPuzzleGOsList_v3 = gameManager.PuzzleGOsList_v3.Where(x => x.x == jb.startPosX && x.z > jb.startPosZ).ToList().OrderByDescending(p => p.z).ToList();
            if (tempPuzzleGOsList_v3.Count > 0)
                jb.endPosZ = tempPuzzleGOsList_v3.LastOrDefault().z - 1f;
            else
                jb.endPosZ = gameManager.PlaneGOsList_v3.Where(x => x.x == jb.startPosX && x.z >= jb.startPosZ).OrderByDescending(p => p.z).FirstOrDefault().z;

            jb.direction = SwipeManager.SwipeType.Up;
            LastJB.endPosZ = jb.endPosZ;
            jbList.Add(jb);
        }
        if (SwipeManager.instance.currentSwipe == SwipeManager.SwipeType.Down)
        {
            JourneyBuffer jb = new JourneyBuffer();
            jb.startPosX = LastJB.endPosX;
            jb.startPosZ = LastJB.endPosZ;

            List<Vector3> tempPuzzleGOsList_v3 = new List<Vector3>();

            tempPuzzleGOsList_v3 = gameManager.PuzzleGOsList_v3.Where(x => x.x == jb.startPosX && x.z < jb.startPosZ).ToList().OrderByDescending(p => p.z).ToList();
            if (tempPuzzleGOsList_v3.Count > 0)
                jb.endPosZ = tempPuzzleGOsList_v3.LastOrDefault().z + 1f;
            else
                jb.endPosZ = gameManager.PlaneGOsList_v3.Where(x => x.x == jb.startPosX && x.z <= jb.startPosZ).OrderByDescending(p => p.z).LastOrDefault().z;

            jb.direction = SwipeManager.SwipeType.Down;
            LastJB.endPosZ = jb.endPosZ;
            jbList.Add(jb);

        }


        if (SwipeManager.instance.currentSwipe != SwipeManager.SwipeType.None && jbList.Count > 0 && isMoving == false && !activeCoroutine)
            StartCoroutine(PlayerMoving());
    }


    //IEnumerable FindTheMovmentType(string drct)
    //{
    //    JourneyBuffer jb = new JourneyBuffer();
    //    jb.startPosX = LastJB.endPosX;
    //    jb.startPosZ = LastJB.endPosZ;
    //    Debug.Log("Giren : " + jb.startPosX + "GirenZ : " + LastJB.endPosZ);
    //    List<Vector3> tempPuzzleGOsList_v3 = new List<Vector3>();

    //    tempPuzzleGOsList_v3 = gameManager.PuzzleGOsList_v3.Where(x => x.z == jb.startPosZ && x.x > jb.startPosX).ToList().OrderByDescending(p => p.x).ToList();
    //    if (tempPuzzleGOsList_v3.Count > 0)
    //        jb.endPosX = tempPuzzleGOsList_v3.LastOrDefault().x - 1f;
    //    else
    //        jb.endPosX = gameManager.PlaneGOsList_v3.Where(x => x.z == jb.startPosZ && x.x >= jb.startPosX).OrderByDescending(p => p.x).FirstOrDefault().x;

    //    jb.direction = "Right";
    //    LastJB.endPosX = jb.endPosX;
    //    Debug.Log("Giden : " + jb.startPosX + " : " + jb.endPosX);
    //    jbList.Add(jb);
    //    if (jbList.Count > 0 && isMoving == false)
    //        StartCoroutine(PlayerMoving());
    //    yield return true;
    //}

    IEnumerator PlayerMoving()
    {
        activeCoroutine = true;
        while (jbList.Count > 0)
        {
            yield return new WaitForFixedUpdate();
            SmoothLerpFactor = 0;
            isMoving = true;

            activeJB = new JourneyBuffer();
            activeJB = jbList.FirstOrDefault();


            while (isMoving == true && gameManager.isGameRunning)
            {

                if (activeJB.direction == SwipeManager.SwipeType.Right || activeJB.direction == SwipeManager.SwipeType.Left)
                {
                    SmoothLerpFactor += gameManager.playerSpeed * Time.deltaTime;
                    this.transform.position = new Vector3(Mathf.Lerp(activeJB.startPosX, activeJB.endPosX, SmoothLerpFactor), this.transform.position.y, this.transform.position.z);
                    if (this.transform.position.x == activeJB.endPosX)
                    {
                        isMoving = false;
                    }
                }
                else if (activeJB.direction != SwipeManager.SwipeType.None)
                {
                    SmoothLerpFactor += gameManager.playerSpeed * Time.deltaTime;
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Mathf.Lerp(activeJB.startPosZ, activeJB.endPosZ, SmoothLerpFactor));
                    if (this.transform.position.z == activeJB.endPosZ)
                    {
                        isMoving = false;
                    }
                }
                yield return null;
            }
            jbList.Remove(activeJB);
            SmoothLerpFactor = 0;
            yield return null;
        }
        activeCoroutine = false;
        yield return true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Blank")
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Plane" && GameObject.FindGameObjectsWithTag("Plane").Length == 1)
        {
            col.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f);
            gameManager.finalPos = col.gameObject.transform.position;
            col.gameObject.tag = "Untagged";
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Plane")
        {
            StartCoroutine(ChangeColorePlane(col.gameObject));
        }
    }

    IEnumerator ChangeColorePlane(GameObject go)
    {
        go.tag = "Untagged";
        if (activeJB.direction == SwipeManager.SwipeType.Right)
            for (int i = 0; i < (90 / gameManager.planeRotationSpeed); i++)
            {
                yield return new WaitForFixedUpdate();
                go.transform.Rotate(0f, 0f, -gameManager.planeRotationSpeed, 0f);
            }
        else if (activeJB.direction == SwipeManager.SwipeType.Left)
            for (int i = 0; i < (90 / gameManager.planeRotationSpeed); i++)
            {
                yield return new WaitForFixedUpdate();
                go.transform.Rotate(0f, 0f, gameManager.planeRotationSpeed, 0f);
            }

        else if (activeJB.direction == SwipeManager.SwipeType.Up)
            for (int i = 0; i < (90 / gameManager.planeRotationSpeed); i++)
            {
                yield return new WaitForFixedUpdate();
                go.transform.Rotate(gameManager.planeRotationSpeed, 0f, 0f, 0f);
            }
        else if (activeJB.direction == SwipeManager.SwipeType.Down)
            for (int i = 0; i < (90 / gameManager.planeRotationSpeed); i++)
            {
                yield return new WaitForFixedUpdate();
                go.transform.Rotate(-gameManager.planeRotationSpeed, 0f, 0f, 0f);
            }

        go.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f);

        yield return null;
    }

    public IEnumerator FinishChecker()
    {
        while (gameManager.isGameRunning)
        {
            if ((GameObject.FindGameObjectsWithTag("Plane").Length == 0))
            {
                gameManager.GetComponent<GameManager>().FinishTheGame();
                StartCoroutine(FinishAnim());
                yield return true;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator FinishAnim()
    {
        yield return new WaitForEndOfFrame();
        this.transform.position = new Vector3(gameManager.finalPos.x, this.transform.position.y, gameManager.finalPos.z);


        StopAllCoroutines();
    }

}
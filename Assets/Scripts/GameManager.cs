using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public List<GameObject> LevelsGOs = new List<GameObject>();

    #region Variables

    [HideInInspector]
    public int level;
    [HideInInspector]
    public bool isGameRunning = false;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public List<Vector3> PuzzleGOsList_v3 = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> PlaneGOsList_v3 = new List<Vector3>();
    public float playerSpeed;
    public float planeRotationSpeed;

    #endregion

    #region UI

    public GameObject panel;
    public Text info_Text;
    public Button start_Button;
    [HideInInspector]
    public Vector3 finalPos;

    #endregion

    [HideInInspector]
    public GameObject activeLevel;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator LevelPuller()
    {
        if (activeLevel)
            Destroy(activeLevel);
        yield return new WaitForEndOfFrame();
        activeLevel = Instantiate(LevelsGOs[level]);
        yield return new WaitForEndOfFrame();
        DetectObstacle();

        StartCoroutine(CameraFader());
    }

    public void FinishTheGame()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().isMoving = false;
        isGameRunning = false;
        level += 1;
        if (level == LevelsGOs.Count)
        {
            info_Text.text = "LEVEL " + level + "\nFINISHED" + "\nOther Levels Couldn't Design Yet...";
            start_Button.gameObject.SetActive(false);
        }
        else
        {
            info_Text.text = "LEVEL " + level + "\nFINISHED";
        }
        panel.SetActive(true);
    }

    public void Start_ButtonClick()
    {
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        panel.SetActive(false);
        isGameRunning = true;
        yield return StartCoroutine(LevelPuller());
    }

    private void DetectObstacle()
    {
        PuzzleGOsList_v3 = new List<Vector3>();
        PlaneGOsList_v3 = new List<Vector3>();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Puzzle"))
        {
            PuzzleGOsList_v3.Add(go.transform.position);
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Plane"))
        {
            PlaneGOsList_v3.Add(go.transform.position);
        }
        PlaneGOsList_v3.Add(GameObject.FindGameObjectWithTag("Player").transform.position);

    }

    private IEnumerator CameraFader()
    {
        float i = 0;
        while (i < 1)
        {
            yield return new WaitForFixedUpdate();

            i += Time.deltaTime;

            if (level < 4)
                Camera.main.orthographicSize = Mathf.Lerp(10, 6, i);
            else
                Camera.main.orthographicSize = Mathf.Lerp(6, 10, i);
        }
    }

}

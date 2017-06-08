using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    [HideInInspector]
    public bool moving;
    [HideInInspector]
    public float multiplier;
    [HideInInspector]
    public int coinsCollected;
    [HideInInspector]
    public bool goalReached;
    [HideInInspector]
    public float timeSinceGoalReached;
    [HideInInspector]
    public GameObject levelComplete;

    private Canvas canvas;
    private Text coinsCollectedText;
    private GameObject goal;

    private AsyncOperation nextLevel;
    private static bool isNextLevelMenuScreen = false;

    private Vector3 initialPlayerPosition;
    private Vector3 initialCamPosition;
    private Rigidbody playerBody;



    private static void SetNextLevel(ref AsyncOperation nextLevel)
    {
        if (!isNextLevelMenuScreen)
        {
            nextLevel = LoadLevelInBackground((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());

            if (nextLevel == null)
                nextLevel = LoadLevelInBackground(SceneManager.GetActiveScene().name);
        }
        else
        {
            isNextLevelMenuScreen = false;
            Camera.main.cullingMask = 0;
            GameObject.Find("UICanvas").SetActive(false);
            SceneManager.LoadSceneAsync("LevelSelect");
        }
    }

    // Use this for initialization
    void Start () {

        SetNextLevel(ref nextLevel);

        if (Camera.main.cullingMask == 0) return;

        moving = true;
        canvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        coinsCollectedText = canvas.transform.FindChild("CoinsCollected").GetComponent<Text>();
        goal = transform.FindChild("Goal").gameObject;

        levelComplete = GameObject.Find("Level Complete");
        levelComplete.transform.FindChild("ReRun").GetComponent<Button>().onClick.AddListener(() => Respawn());
        levelComplete.transform.FindChild("NextLevel").GetComponent<Button>().onClick.AddListener(() => GotoNextLevel(ref nextLevel));
        levelComplete.transform.FindChild("Back").GetComponent<Button>().onClick.AddListener(() => GotoNextLevel(ref nextLevel, true));

        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        initialPlayerPosition = playerBody.transform.position;
        initialCamPosition = Camera.main.transform.position;
        Respawn();
    }

    void Update () {

        if (Camera.main.cullingMask == 0) return;

        coinsCollectedText.text = "Coins Collected: " + coinsCollected;

        if (goalReached && timeSinceGoalReached > 0.3f)
        {
                levelComplete.SetActive(true);
        }

        if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x >= Camera.main.pixelWidth * 0.66f) && moving)
        {
            multiplier = 1.5f;
            Camera.main.transform.position += Vector3.right * 0.14f;
        }
        else if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x >= Camera.main.pixelWidth * 0.5f) && moving)
        {
            multiplier = 1.0f;
            Camera.main.transform.position += Vector3.right * 0.10f;
        }
        else
        {
            playerBody.AddForce(Vector3.right * 0.008f);
        }

        if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x >= Camera.main.pixelWidth * 0.7) && moving)
        {
            Camera.main.transform.position += 2 * Vector3.right;
        }

        if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x <= Camera.main.pixelWidth - (Camera.main.pixelWidth * 0.7)) && moving)
        {
            Camera.main.transform.position += 2 * Vector3.left;
        }

        if (Camera.main.WorldToScreenPoint(goal.transform.position).x + (Camera.main.pixelWidth * 0.02f) <= Camera.main.pixelWidth)
        {
            moving = false;
        }

        if (moving)
            transform.position += Vector3.left * 0.08f * multiplier;
    }

    private static AsyncOperation LoadLevelInBackground(string level)
    {
        if (Application.CanStreamedLevelBeLoaded(level))
        {
            var loadScene = SceneManager.LoadSceneAsync(level);
            loadScene.allowSceneActivation = false;
            return loadScene;
        }

        return null;
    }

    public static void GotoNextLevel(ref AsyncOperation nextLevel, bool? isMenuScreen = null)
    {
        if (isMenuScreen != null)
            isNextLevelMenuScreen = (bool)isMenuScreen;

        if (nextLevel != null)
            nextLevel.allowSceneActivation = true;
    }

    public void Respawn()
    {
        levelComplete.SetActive(false);
        transform.position = new Vector3(0.6f, -0.5f, -0.4f);
        moving = true;
        goalReached = false;

        var coins = transform.FindChild("Coins").GetComponentsInChildren<CoinController>();
        foreach (var coin in coins)
            coin.GetComponent<Renderer>().enabled = true;

        timeSinceGoalReached = 0;
        coinsCollected = 0;
        multiplier = 0;
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        Camera.main.transform.position = initialCamPosition;
        playerBody.transform.position = initialPlayerPosition;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    [HideInInspector]
    public float multiplier;
    [HideInInspector]
    public float timeSinceGoalReached;

    public const float onGoalReachedDelay = 0.25f;

    private bool moving;
    private int coinsCollected;
    private GameObject levelCompleteUI;
    private Text coinsCollectedUIText;
    private Transform goalTransform;

    private AsyncOperation nextLevel;
    private static bool isNextLevelMenuScreen = false;

    private Vector3 initialPlayerPosition;
    private Vector3 initialCamPosition;
    private Rigidbody playerBody;

    void Start () {

        // Set the level to follow this one
        // This level may be a placeholder
        SetNextLevel(ref nextLevel);

        // Skip initialisation if nothing is being rendered
        // If so, this level is probably a placeholder between scenes
        if (Camera.main.cullingMask == 0)
            return;

        // The world starts moving
        moving = true;

        // Get the UICanvas object
        var canvasUITransform = GameObject.Find("UICanvas").transform;

        // Find the Coins Collected Text
        coinsCollectedUIText = canvasUITransform.FindChild("CoinsCollected").GetComponent<Text>();

        // Find Level Complete Dialog and assign appropiate on click listeners for buttons
        levelCompleteUI = canvasUITransform.FindChild("Level Complete").gameObject;
        levelCompleteUI.transform.FindChild("ReRun").GetComponent<Button>().onClick.AddListener(() => Respawn());
        levelCompleteUI.transform.FindChild("NextLevel").GetComponent<Button>().onClick.AddListener(() => GotoNextLevel(ref nextLevel));
        levelCompleteUI.transform.FindChild("Back").GetComponent<Button>().onClick.AddListener(() => GotoNextLevel(ref nextLevel, true));

        // Find the goal object
        goalTransform = transform.FindChild("Goal");

        // Get the rigidbody attached to the player
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        // Get the position the player and the camera start at
        initialPlayerPosition = playerBody.transform.position;
        initialCamPosition = Camera.main.transform.position;

        // Reset everything to default state
        Respawn();
    }

    void Update () {

        // Don't Update if nothing is being rendered
        // If so, this level is probably a placeholder between scenes
        if (Camera.main.cullingMask == 0)
            return;

        // When the goal has been reached
        // Stop the player moving and animating
        // And show the level complete dialog
        // Don't continue updating after reaching the goal
        if(timeSinceGoalReached > (onGoalReachedDelay + 0.5f + Time.deltaTime))
        {
            return;
        }
        else if (timeSinceGoalReached > onGoalReachedDelay)
        {
            playerBody.velocity = Vector3.zero;
            playerBody.gameObject.GetComponent<Animator>().SetFloat("Forward", 0);
            DisplayCollectedCoins();
            levelCompleteUI.SetActive(true);
        }

        // On Escape or Back being pressed
        // The next level is a menu scene, switch to it
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GotoNextLevel(ref nextLevel, true);
        }

        // Move everything approiately
        MoveLevelPlayerCamera();
    }

    private void MoveLevelPlayerCamera()
    {
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

        if (Camera.main.WorldToScreenPoint(goalTransform.position).x + (Camera.main.pixelWidth * 0.02f) <= Camera.main.pixelWidth)
        {
            moving = false;
        }

        if (moving)
        {
            transform.position += Vector3.left * 0.08f * multiplier;
        }
    }

    public void CollectCoin(int? collectedCoins = null)
    {
        if (collectedCoins != null)
            coinsCollected = (int)collectedCoins;

        coinsCollected++;
        coinsCollectedUIText.text = "Coins Collected: " + coinsCollected;
    }

    public int DisplayCollectedCoins()
    {
        var finalCoinCountText = levelCompleteUI.transform.FindChild("Coins").GetComponent<Text>();
        var allCoinsList = new List<CoinController>(transform.FindChild("Coins").GetComponentsInChildren<CoinController>());

        finalCoinCountText.text = coinsCollected + "/" + allCoinsList.FindAll(i => i.gameObject.activeSelf).Count + " Coins Collected";

        return coinsCollected;
    }

    private static void SetNextLevel(ref AsyncOperation nextLevel)
    {
        System.Func<string, AsyncOperation> LoadLevelInBackground = (string level) =>
        {
            if (Application.CanStreamedLevelBeLoaded(level))
            {
                var loadScene = SceneManager.LoadSceneAsync(level);
                loadScene.allowSceneActivation = false;
                return loadScene;
            }

            return null;
        };

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

    private static void GotoNextLevel(ref AsyncOperation nextLevel, bool? isMenuScreen = null)
    {
        if (isMenuScreen != null)
            isNextLevelMenuScreen = (bool)isMenuScreen;

        if (nextLevel != null)
            nextLevel.allowSceneActivation = true;
    }

    public void Respawn()
    {
        levelCompleteUI.SetActive(false);
        transform.position = new Vector3(0.6f, -0.5f, -0.4f);
        moving = true;

        var coins = transform.FindChild("Coins").GetComponentsInChildren<CoinController>();
        foreach (var coin in coins)
            coin.GetComponent<Renderer>().enabled = true;

        var levelExtras = GameObject.FindObjectsOfType<LevelExtrasType>();

        if(levelExtras != null)
        {
            foreach(var extra in levelExtras)
                extra.Reset();
        }

        CollectCoin(-1);

        timeSinceGoalReached = 0;
        multiplier = 0;
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        Camera.main.transform.position = initialCamPosition;
        playerBody.transform.position = initialPlayerPosition;

    }
}

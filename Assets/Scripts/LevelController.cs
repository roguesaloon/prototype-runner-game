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

    private Canvas canvas;
    private Text coinsCollectedText;
    private GameObject goal;
    private AsyncOperation loadNextLevel;

    private Vector3 initialPlayerPosition;
    private Vector3 initialCamPosition;
    private GameObject levelComplete;
    private Rigidbody playerBody;

    public bool goalReached;
    public float timeSinceGoalReached;

    // Use this for initialization
    void Start () {
        moving = true;
        canvas = GameObject.FindObjectOfType<Canvas>().GetComponent<Canvas>();
        coinsCollectedText = canvas.transform.FindChild("CoinsCollected").GetComponent<Text>();
        goal = transform.FindChild("Goal").gameObject;
        loadNextLevel = LoadNextLevel();

        levelComplete = GameObject.Find("Level Complete");
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        initialPlayerPosition = playerBody.transform.position;
        initialCamPosition = Camera.main.transform.position;
        Respawn();
    }

    void Update () {
        coinsCollectedText.text = "Coins Collected: " + coinsCollected;

        if (goalReached)
        {
            if (timeSinceGoalReached > 0.3f)
            {
                levelComplete.SetActive(true);
                levelComplete.transform.FindChild("Coins").GetComponent<Text>().text = coinsCollected + "/" + new List<CoinController>(transform.FindChild("Coins").GetComponentsInChildren<CoinController>()).FindAll(i => i.gameObject.activeSelf).Count + " Coins Collected";
                levelComplete.transform.FindChild("ReRun").GetComponent<Button>().onClick.AddListener(() => Respawn());
                levelComplete.transform.FindChild("NextLevel").GetComponent<Button>().onClick.AddListener(() => NextLevel());
            }
        }

        if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x >= Camera.main.pixelWidth * 0.66f) && moving)
        {
            multiplier = 1.5f;
            Camera.main.transform.position += Vector3.right * 0.07f;
        }
        else if ((Camera.main.WorldToScreenPoint(playerBody.transform.position).x >= Camera.main.pixelWidth * 0.5f) && moving)
        {
            multiplier = 1.0f;
            Camera.main.transform.position += Vector3.right * 0.05f;
        }
        else
        {
            playerBody.AddForce(Vector3.right * 0.008f);
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        
        if (Camera.main.WorldToScreenPoint(goal.transform.position).x + 12 <= Camera.main.pixelWidth)
        {
            moving = false;
        }

        if(moving)
            transform.position += Vector3.left * 0.08f * multiplier;
	}

    private static AsyncOperation LoadNextLevel()
    {
        var nextScene = (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString();

        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            var loadScene = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
            loadScene.allowSceneActivation = false;
            return loadScene;
        }

        return null;
    }

    public void NextLevel()
    {
        if(loadNextLevel != null)
            loadNextLevel.allowSceneActivation = true;
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

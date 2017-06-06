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

	// Use this for initialization
	void Start () {
        moving = true;
        canvas = GameObject.FindObjectOfType<Canvas>().GetComponent<Canvas>();
        coinsCollectedText = canvas.transform.FindChild("CoinsCollected").GetComponent<Text>();
        goal = transform.FindChild("Goal").gameObject;
        loadNextLevel = LoadNextLevel();
	}

    void Update () {
        coinsCollectedText.text = "Coins Collected: " + coinsCollected;
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
}

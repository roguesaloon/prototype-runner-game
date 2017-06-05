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

	// Use this for initialization
	void Start () {
        moving = true;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        coinsCollectedText = canvas.transform.FindChild("CoinsCollected").GetComponent<Text>();
        goal = transform.FindChild("Goal").gameObject;
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

    public void NextLevel()
    {


        var nextScene = (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString();

        if(nextScene != null && Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}

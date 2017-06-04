using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    public bool moving;
    public float multiplier;
    public int coinsCollected;

    private Canvas canvas;
    private Text coinsCollectedText;

	// Use this for initialization
	void Start () {
        moving = true;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        coinsCollectedText = canvas.transform.FindChild("CoinsCollected").GetComponent<Text>();
	}
	
    void Update () {
        coinsCollectedText.text = "Coins Collected: " + coinsCollected;
    }

	// Update is called once per frame
	void FixedUpdate () {

        if (transform.position.x <= -20)
             moving = false;

        if(moving)
            transform.position += Vector3.left * 0.08f * multiplier;
	}
}

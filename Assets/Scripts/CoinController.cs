﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

    LevelController level;
    new public Renderer renderer;

	// Use this for initialization
	void Start ()
    {

        level = GameObject.Find("Level").GetComponent<LevelController>();

        renderer = GetComponent<Renderer>();


    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!renderer.enabled) return;

        gameObject.transform.Rotate(0, 0, 8);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!renderer.enabled) return;

        if (collider.tag == "Player")
        {
            level.coinsCollected++;
            renderer.enabled = false;
        }
    }
}

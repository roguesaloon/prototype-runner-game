using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquasherController : MonoBehaviour {

    bool retracting;

	// Use this for initialization
	void Start () {
        retracting = false;
	}
	
	// Update is called once per frame
	void Update () {

        // Serious Problem with Level Axes
        // Everything is Rotated by 270 (-90) Degrees
        // Hence Back is Down and Forward is Up
        // To Be Fixed

        if(!retracting)
        {
            transform.localPosition += Vector3.down * 0.2f;

            if (transform.localPosition.y < -2.8f)
                retracting = true;
        }
        else
        {
            transform.localPosition += Vector3.up * 0.05f;

            if (transform.localPosition.y > -0.5f)
                retracting = false;
        }
	}
}

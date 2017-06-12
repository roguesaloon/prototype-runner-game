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

        if(!retracting)
        {
            transform.Translate(transform.TransformDirection(Vector3.down * 0.2f));

            if (transform.TransformDirection(transform.localPosition).y < -2.8f)
                retracting = true;
        }
        else
        {
            transform.Translate(transform.TransformDirection(Vector3.up * 0.05f));

            if (transform.TransformDirection(transform.localPosition).y > -0.5f)
                retracting = false;
        }
	}
}

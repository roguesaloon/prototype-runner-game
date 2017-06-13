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
            transform.Translate(Vector3.back * 0.2f);

            if (Vector3.Dot(transform.localPosition, transform.TransformDirection(Vector3.forward)) < -2.8f)
                retracting = true;
        }
        else
        {
            transform.Translate(Vector3.forward * 0.05f);

            if (Vector3.Dot(transform.localPosition, transform.TransformDirection(Vector3.forward)) > -0.5f)
                retracting = false;
        }
    }
}

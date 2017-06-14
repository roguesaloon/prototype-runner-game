using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawController : LevelExtrasType {

    private bool rotate;
    private Quaternion initialRotation;

    public int rotateTo;
     
	// Use this for initialization
	void Start () {
        rotate = false;
        initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.localRotation.eulerAngles.z < rotateTo)
        {
            rotate = false;
        }

        if (rotate)
        {
            transform.Rotate(0, 0, -0.5f);
        }
	}

    public override void Reset()
    {
        transform.rotation = initialRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rotate = true;
    }
}

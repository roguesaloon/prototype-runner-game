using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawController : MonoBehaviour {

    private bool rotate;
    private Quaternion initialRotation;

	// Use this for initialization
	void Start () {
        rotate = false;
        initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        

        if (rotate)
        {
            transform.Rotate(0, 0, -0.5f);

            if (transform.localRotation.eulerAngles.z < 32)
            {
                rotate = false;
            }
        }
	}

    public void Reset()
    {
        transform.rotation = initialRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rotate = true;
    }
}

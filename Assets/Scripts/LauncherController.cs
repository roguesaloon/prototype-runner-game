using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour {

    Vector3 initialPosition;

	// Use this for initialization
	void Start () {

        initialPosition = transform.position;
		
	}
	
	public void Reset ()
    {
        transform.position = initialPosition;
	}
}

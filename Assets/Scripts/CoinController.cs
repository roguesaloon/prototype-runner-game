using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CoinController : MonoBehaviour {

    LevelController level;
#pragma warning disable 0109
    private new Renderer renderer;
#pragma warning restore 0109

    // Use this for initialization
    void Start ()
    {
        if(GameObject.Find("Level"))
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
            level.CollectCoin();
            renderer.enabled = false;
        }
    }
}

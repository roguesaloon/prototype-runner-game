using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    private bool jumping;       
    private Animator animator;
    private new Rigidbody rigidbody;
    private LevelController level;

    /* Move to Level Class */
    private bool goalReached;
    private float timeSinceGoalReached;
    private Vector3 initialPosition;
    private Vector3 initialCamPosition;
    private GameObject levelComplete;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        level = GameObject.Find("Level").GetComponent<LevelController>();

        /* Move to Level Class */
        levelComplete = GameObject.Find("Level Complete");
        initialPosition = transform.position;
        initialCamPosition = Camera.main.transform.position;
        Respawn();
    }


    private void Update()
    {
        /* Move to Level Class */
        if (goalReached)
        {
            if (timeSinceGoalReached > 0.3f)
            {
                levelComplete.SetActive(true);
                levelComplete.transform.FindChild("Coins").GetComponent<Text>().text = level.coinsCollected + "/" + new List<CoinController>(level.transform.FindChild("Coins").GetComponentsInChildren<CoinController>()).FindAll(i => i.gameObject.activeSelf).Count + " Coins Collected";
                levelComplete.transform.FindChild("ReRun").GetComponent<Button>().onClick.AddListener(() => Respawn());
                levelComplete.transform.FindChild("NextLevel").GetComponent<Button>().onClick.AddListener(() => level.NextLevel());
            }

            return;
        }

        if ((CrossPlatformInputManager.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && !jumping)
        {
            rigidbody.velocity += Vector3.up * 7.5f;
            jumping = true;
        }

        if (!jumping)
        {
            animator.SetFloat("Forward", 1);
            rigidbody.AddForce(Vector3.right * 4);     
        }
        else
        {
            animator.SetFloat("Forward", 0);
            level.multiplier = 0.7f;
        }

        /* Move to Level Class */
        if ((Camera.main.WorldToScreenPoint(transform.position).x >= Camera.main.pixelWidth * 0.66f) && level.moving)
        {
            level.multiplier = 1.5f;
            Camera.main.transform.position += Vector3.right * 0.07f;
        }
        else if ((Camera.main.WorldToScreenPoint(transform.position).x >= Camera.main.pixelWidth * 0.5f) && level.moving)
        {
            level.multiplier = 1.0f;
            Camera.main.transform.position += Vector3.right * 0.05f;
        }
        else
        {
            rigidbody.AddForce(Vector3.right * 0.008f);
        }

        if (rigidbody.velocity.x < 0)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);
        }

        if(rigidbody.velocity.x > 6)
        {
            rigidbody.velocity = new Vector3(6, rigidbody.velocity.y, rigidbody.velocity.z);
        }


    }

    /* Move to Level Class */
    private void Respawn()
    {
        levelComplete.SetActive(false);
        level.transform.position = new Vector3(0.6f, -0.5f, -0.4f);
        level.moving = true;
        goalReached = false;

        var coins = level.transform.FindChild("Coins").GetComponentsInChildren<CoinController>();
        foreach (var coin in coins)
            coin.GetComponent<Renderer>().enabled = true;

        
        timeSinceGoalReached = 0;
        level.coinsCollected = 0;
        level.multiplier = 0;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        Camera.main.transform.position = initialCamPosition;
        transform.position = initialPosition;

    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Goal")
            timeSinceGoalReached += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ground")
           jumping = false;

        if (collision.gameObject.tag == "Goal")
        {
            animator.SetFloat("Forward", 0);
            goalReached = true;
        }

       if(collision.gameObject.tag == "Respawn")
       {
            Respawn();
       }
    }
}

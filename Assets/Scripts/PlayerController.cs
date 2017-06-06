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

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        level = GameObject.Find("Level").GetComponent<LevelController>();
    }

    private void Update()
    {
        if (level.goalReached) return;

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

        if (rigidbody.velocity.x < 0)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);
        }

        if(rigidbody.velocity.x > 6)
        {
            rigidbody.velocity = new Vector3(6, rigidbody.velocity.y, rigidbody.velocity.z);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Goal")
            level.timeSinceGoalReached += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ground")
           jumping = false;

        if (collision.gameObject.tag == "Goal")
        {
            animator.SetFloat("Forward", 0);
            level.goalReached = true;
        }

       if(collision.gameObject.tag == "Respawn")
       {
            level.Respawn();
       }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    private bool jumping;       
    private Animator animator;
#pragma warning disable 0109
    private new Rigidbody rigidbody;
#pragma warning restore 0109
    private LevelController level;

    private void Start()
    {
        // Get object animator/rigidbody
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        // Get reference to the level being played
        level = GameObject.Find("Level").GetComponent<LevelController>();
    }

    private void Update()
    {
        // Don't update if the goal has been reached
        if (level.timeSinceGoalReached > LevelController.onGoalReachedDelay + 0.2f)
            return;

        // Let the player jump if not already jumping
        if ((CrossPlatformInputManager.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && !jumping)
        {
            rigidbody.velocity += Vector3.up * 7.5f;
            jumping = true;
        }

        // Animate and move the player when not jumping
        if (!jumping)
        {
            animator.SetFloat("Forward", 1);
            rigidbody.AddForce(Vector3.right * 4);     
        }
        else    // Disable animation, and slow down level move speed when jumping 
        {
            animator.SetFloat("Forward", 0);
            level.multiplier = 0.7f;
        }

        // Sets lower limits for player velocity
        if (rigidbody.velocity.x < 0)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);
        }

        // Sets upper limits for player velocity
        if (rigidbody.velocity.x > 6)
        {
            rigidbody.velocity = new Vector3(6, rigidbody.velocity.y, rigidbody.velocity.z);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // When goal is reached increase timer, for slight delay
        if(collision.gameObject.tag == "Goal")
            level.timeSinceGoalReached += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trigger actions on tagged collisions


        if(collision.gameObject.tag == "Ground")
            jumping = false;

        if(collision.gameObject.tag == "Launch")
            rigidbody.AddForce(10, 10, 0, ForceMode.VelocityChange);

        if(collision.gameObject.tag == "Respawn")
                level.Respawn();
    }
}

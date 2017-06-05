using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    private bool m_Jumping, m_GoalReached;       
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private LevelController m_Level;
    private float m_TimeSinceGoalReached;
    private GameObject m_LevelComplete;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Level = GameObject.Find("Level").GetComponent<LevelController>();
        m_LevelComplete = GameObject.Find("Level Complete");

        Respawn();
    }


    private void Update()
    {
        if (m_GoalReached)
        {
            if (m_TimeSinceGoalReached > 0.3f)
            {
                m_LevelComplete.SetActive(true);
                m_LevelComplete.transform.FindChild("Coins").GetComponent<Text>().text = m_Level.coinsCollected + "/15 Coins Collected";
                m_LevelComplete.transform.FindChild("ReRun").GetComponent<Button>().onClick.AddListener(() => Respawn());
            }

            return;
        }

        if ((CrossPlatformInputManager.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && !m_Jumping)
        {
            m_Rigidbody.velocity += Vector3.up * 7.5f;
            m_Jumping = true;
        }

        if (!m_Jumping)
        {
            m_Animator.SetFloat("Forward", 1);
            m_Rigidbody.AddForce(Vector3.right * 4);     
        }
        else
        {
            m_Animator.SetFloat("Forward", 0);
            m_Level.multiplier = 0.7f;
        }

        if ((Camera.main.WorldToScreenPoint(transform.position).x >= Camera.main.pixelWidth * 0.5f))
            m_Level.multiplier = 1.0f;
        else
            m_Rigidbody.AddForce(Vector3.right * 0.008f) ;


    }

    private void Respawn()
    {
        m_LevelComplete.SetActive(false);
        m_Level.transform.position = new Vector3(0.6f, -0.5f, -0.4f);
        m_Level.moving = true;
        m_GoalReached = false;

        var coins = m_Level.transform.FindChild("Coins").GetComponentsInChildren<CoinController>();
        foreach (var coin in coins)
            coin.GetComponent<Renderer>().enabled = true;

        m_TimeSinceGoalReached = 0;
        m_Level.coinsCollected = 0;
        m_Rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(-7, -3.5f, -0.5f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Goal")
            m_TimeSinceGoalReached += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ground")
           m_Jumping = false;

        if (collision.gameObject.tag == "Goal")
        {
            m_Animator.SetFloat("Forward", 0);
            m_GoalReached = true;
        }

       if(collision.gameObject.tag == "Respawn")
       {
            Respawn();
       }
    }
}

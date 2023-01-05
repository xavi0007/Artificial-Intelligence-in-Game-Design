using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent; // To store the NPC NavMeshAgent component.
    Animator anim; // To store the Animator component.
    public GameObject player;
    private Transform playerTransform;
    State currentState;

    public string checkpoint_name;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        agent = this.GetComponent<NavMeshAgent>(); // Grab agents NavMeshAgent.
        currentState = new Idle(this.gameObject, agent, playerTransform); // Create our first state.

    }

    public string getState()
    {
        //Debug.Log("Current State = " + currentState.getName());
        return currentState.getName();
    }

    public void setState(string typeState)
    {
        if (typeState == "Pursue")
        {

            currentState = new Pursue(this.gameObject, agent, playerTransform); // Set tank to go pursue
        }

    }

    void Update()
    {
        playerTransform = player.GetComponent<Transform>();
        currentState = currentState.Process(); // ensure correct state is set.
    }
}

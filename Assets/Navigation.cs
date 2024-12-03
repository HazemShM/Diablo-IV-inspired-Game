using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private bool playerInCamp;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerInCamp = false;
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInCamp){
            agent.destination = player.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ha");
        if (other.CompareTag("Paladin"))
        {
            Debug.Log("in");
            playerInCamp = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("ha");
        if (other.CompareTag("Paladin"))
        {
            Debug.Log("out");
            playerInCamp = false;
        }
    }
}
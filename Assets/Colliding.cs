using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliding : MonoBehaviour
{
    public bool playerInCamp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCamp = true;
        }
    }

     private void OnTriggerStay(Collider other) 
     {
         if (other.CompareTag("Player"))
        {
            playerInCamp = true;
        }
     }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCamp = false;
        }
    }
}

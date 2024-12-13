using UnityEngine;

public class Colliding_Minion : MonoBehaviour
{
    public bool playerInCamp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            playerInCamp = true;
            Debug.Log("Player has entered the camp.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            playerInCamp = false;
            Debug.Log("Player has left the camp.");
        }
    }
}

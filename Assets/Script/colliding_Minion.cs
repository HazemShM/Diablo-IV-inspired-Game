using UnityEngine;
using System.Collections;

public class Colliding_Minion : MonoBehaviour
{
    public bool playerInCamp = false;
    public bool cloneInCamp = false;

    private IEnumerator CloneResetTimer()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);
        cloneInCamp = false;
        Debug.Log("Clone has been reset after 5 seconds.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCamp = true;
            Debug.Log("Player has entered the camp.");
        }
        if (other.CompareTag("Clone"))
        {
            cloneInCamp = true;
            Debug.Log("Clone has entered the camp.");
            StartCoroutine(CloneResetTimer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCamp = false;
            Debug.Log("Player has left the camp.");
        }
    }
}

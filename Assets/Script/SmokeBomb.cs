using UnityEngine;
using System.Collections;

public class SmokeBomb : MonoBehaviour
{
    private float stunDuration = 5f; // Duration enemies are stunned
    public string enemyTag = "Enemy"; // Tag for enemy GameObjects

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the Enemy tag
        if (other.CompareTag(enemyTag))
        {
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                StartCoroutine(StunEnemy(enemyRigidbody));
            }
        }
    }

    private IEnumerator StunEnemy(Rigidbody enemy)
    {
        // Save the current velocity of the enemy
        Vector3 originalVelocity = enemy.velocity;

        // Disable the Rigidbody (freezes movement)
        enemy.isKinematic = true;

        Debug.Log($"{enemy.name} stunned for {stunDuration} seconds!");

        yield return new WaitForSeconds(stunDuration);

        // Re-enable the Rigidbody
        enemy.isKinematic = false;
        enemy.velocity = originalVelocity;

        Debug.Log($"{enemy.name} is no longer stunned.");
    }
}

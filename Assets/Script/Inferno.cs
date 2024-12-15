using UnityEngine;
using System.Collections;

public class DamageAbility : MonoBehaviour
{
    public float damageRange = 10f;  // Range to deal damage
    public int initialDamage = 10;   // Initial damage when first instantiated
    public int ongoingDamage = 2;    // Ongoing damage per second
    public float duration = 5f;      // Duration of the ability
    private Collider[] collidersInRange;  // Store colliders in range

    private void Start()
    {
        // Start the coroutine when the ability is instantiated
        StartCoroutine(DealDamageOverTime());
    }

    private IEnumerator DealDamageOverTime()
    {
        // Deal initial damage to all enemies in range
        DealDamage(initialDamage);

        // Wait for 1 second (initial damage already dealt)
        yield return new WaitForSeconds(1f);

        // Ongoing damage every second for the remaining duration
        float elapsedTime = 1f;
        while (elapsedTime < duration)
        {
            // Deal ongoing damage
            DealDamage(ongoingDamage);
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // Destroy the ability object after the duration
        Destroy(gameObject);
    }

    private void DealDamage(int damageAmount)
    {
        // Get all colliders in range within the damage range (assuming it's a trigger)
        collidersInRange = Physics.OverlapSphere(transform.position, damageRange);

        foreach (Collider collider in collidersInRange)
        {
            // Check if the collider is an enemy and is within the trigger area
            if (collider.CompareTag("Enemy"))
            {
                // Get the Enemy script and apply damage
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                    Debug.Log($"Enemy hit! New health: {enemy.health}");
                }
            }
        }
    }
}

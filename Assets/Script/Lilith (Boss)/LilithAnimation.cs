using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] GameObject minions;
    [SerializeField] private int maxMinions = 3; // Maximum number of minions
    [SerializeField] private List<GameObject> activeMinions = new List<GameObject>(); // List to track active minions
    [SerializeField] private string Phase = "Phase1";
    [SerializeField] private GameObject shield; // Shield (Phase 2)
    [SerializeField] private float shieldHealth = 50f; // Shield health (Phase 2)
    [SerializeField] private float bossHealth = 50f; // Boss health in both phases (Phase 1 & 2)
    private bool isShieldActive = false; // Tracks if Lilith's shield is active (Phase 2)

    private bool hasDestroyedMinion = false; //TEST

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
        }
        if (shield != null)
        {
            shield.SetActive(false); // Shield is inactive by default
        }
        SummonMinions();
    }

    void Update()
    {
        SummonMinions();
        animator.SetInteger("ActiveMinions", activeMinions.Count);
        //TEST SummonMinions
        if (!hasDestroyedMinion && activeMinions.Count > 0 && Time.time > 5)
        {
            // Destroy all minions one by one
            while (activeMinions.Count > 0)
            {
                // Get the last minion in the list
                GameObject lastMinion = activeMinions[activeMinions.Count - 1];

                // Destroy the last minion
                Destroy(lastMinion);

                // Remove the minion from the list
                activeMinions.RemoveAt(activeMinions.Count - 1);

                Debug.Log("Minion destroyed. Remaining: " + activeMinions.Count);
            }

            // Set the flag to true so this runs only once
            hasDestroyedMinion = true;
        }

    }

    public void SummonMinions()
    {
        // Check if the activeMinions list is empty
        if (activeMinions.Count == 0)
        {
            // Reset the trigger to ensure it can be used again
            animator.ResetTrigger("Summon");

            // Trigger the summoning animation
            animator.SetTrigger("Summon");

            // Start the coroutine to delay and summon minions
            StartCoroutine(SummonMinionsWithDelay(1.2f)); // 1.2 seconds delay before summoning
        }
        else
        {
            Debug.Log("Minions are already present, summoning skipped.");
        }
    }


    private IEnumerator SummonMinionsWithDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Summon minions after the delay
        if (activeMinions.Count == 0) // Double-check to avoid duplicates
        {
            for (int i = 0; i < maxMinions; i++)
            {
                // Calculate a random position within a 10-unit range from Lilith's position
                Vector3 randomPosition = new Vector3(
                    transform.position.x + Random.Range(-10f, 10f),
                    transform.position.y,
                    transform.position.z + Random.Range(-10f, 10f)
                );

                // Instantiate the minion at the random position
                GameObject minion = Instantiate(minions, randomPosition, Quaternion.identity);
                activeMinions.Add(minion);
            }

            // Update the ActiveMinions parameter in the Animator after all minions are spawned
            animator.SetInteger("ActiveMinions", activeMinions.Count);

            Debug.Log($"{activeMinions.Count} minions summoned!");
        }
    }

}

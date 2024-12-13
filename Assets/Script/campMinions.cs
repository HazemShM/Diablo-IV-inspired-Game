using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class campMinions : MonoBehaviour
{
    public Minion[] enemies; // Array of minions
    private int maxAlertedMinions = 5; // Maximum number of alerted minions
    private List<Minion> nonAggressiveMinions = new List<Minion>();
    int alertedCount = 0; // List of non-aggressive minions

    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            // Assume minions are already instantiated in the scene and assigned to the array
            Minion minion = enemies[i];
            if (minion == null) continue;

            if (alertedCount < maxAlertedMinions && Random.value > 0.3f)
            {
                // Set minion to alerted state
                minion.AlertMinion();
                alertedCount++;
            }
            else
            {
                minion.currentState = Minion.MinionState.NonAggressive;
                nonAggressiveMinions.Add(minion);
            }

            Debug.Log(minion.currentState);
        }

        Debug.Log($"Total alerted minions: {alertedCount}");
    }

    void Update()
    {
        // Monitor for any dead alerted minions and replace them with non-aggressive ones
        ReplaceDeadAlertedMinions();
    }

    private void ReplaceDeadAlertedMinions()
    {
        foreach (Minion minion in enemies)
        {
            if (minion == null) continue;

            if (minion.currentState == Minion.MinionState.Alerted && minion.GetComponent<Enemy>().health <= 0)
            {
                Minion replacement = GetNonAggressiveMinion();
                if (replacement != null)
                {
                    replacement.AlertMinion();
                    nonAggressiveMinions.Remove(replacement);
                    minion.currentState = Minion.MinionState.NonAggressive;
                    alertedCount++;
                }
            }
        }
    }


    private Minion GetNonAggressiveMinion()
    {
        if (nonAggressiveMinions.Count > 0)
        {
            // Return the first non-aggressive minion
            return nonAggressiveMinions[0];
        }
        return null; // No non-aggressive minions available
    }
}

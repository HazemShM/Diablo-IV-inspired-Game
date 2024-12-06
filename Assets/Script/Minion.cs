using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public enum MinionState { NonAggressive, Alerted }
    public MinionState currentState;
    public float health = 20f;
    public Transform player;
    private NavMeshAgent agent;
    private Animator MinionAnimator;
    public float closeDistance = 20f; 
    public float attackRange = 2f;
    private Colliding_Minion campCollider;
    public GameObject campArea;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        campCollider = campArea.GetComponent<Colliding_Minion>(); // Get the CampCollider script
        MinionAnimator = GetComponent<Animator>(); // Get the Animator component

        // Randomly assign state (either NonAggressive or Alerted)
        currentState =  MinionState.Alerted;

        Debug.Log("Minion state is: " + currentState);
    }

    void Update()
    {
        // If the Player is inside the camp area (from the Collider script)
        if (campCollider.playerInCamp)
        {
            if (currentState == MinionState.Alerted)
            {
                // Get the distance between the Minion and the Player
                float distance = Vector3.Distance(transform.position, player.position);

                // If within attack range, stop moving and attack
                if (distance <= attackRange)
                {
                    agent.isStopped = true; // Stop the agent from moving
                    MinionAnimator.SetBool("punch",true); // Trigger the attack animation
                    MinionAnimator.SetBool("run", false); // Ensure the run animation is not playing
                    MinionAnimator.SetBool("idle", false); // Ensure idle is not playing
                    Debug.Log("run: " + MinionAnimator.GetBool("run"));
                    Debug.Log("idle: " + MinionAnimator.GetBool("idle"));
                    Debug.Log("punch: " + MinionAnimator.GetBool("punch"));
                }
                else
                {
                    // Move towards the player if not within attack range
                    agent.isStopped = false;
                    agent.SetDestination(player.position); // Keep moving toward the player
                    MinionAnimator.SetBool("run", true); // Running animation
                    MinionAnimator.SetBool("idle", false);
                    MinionAnimator.SetBool("punch", false);
                    Debug.Log("run: " + MinionAnimator.GetBool("run"));

                }
            }
        }
        else
        {
            // If the Player leaves the camp or is outside the detection range, stop Minion
            agent.isStopped = true;
            MinionAnimator.SetBool("idle", true); // Idle animation when out of range
            MinionAnimator.SetBool("run", false);
            MinionAnimator.SetBool("punch", false);
        }

       
        if (health <= 0 ) 
        {
            Die();
        }
    }
    public void TakeDamage(float damageAmount)
    {
        // All Minions can take damage, regardless of their state
        health -= damageAmount; // Reduce health by damage amount
        Debug.Log("Minion Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death animation or trigger any other logic
        MinionAnimator.SetTrigger("die"); // You can set up a "die" animation here
        agent.isStopped = true; // Stop the agent from moving
        Debug.Log("Minion has died!");
        Destroy(gameObject, 2f);
    }

    public void AlertMinion()
    {
        currentState = MinionState.Alerted; // Switch to Alerted state
        Debug.Log("Minion is now Alerted!");
    }



}

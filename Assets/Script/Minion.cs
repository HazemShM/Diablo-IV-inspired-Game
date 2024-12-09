using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public enum MinionState
    {
        NonAggressive,
        Alerted,
    }

    public MinionState currentState;
    public int attackDamage = 5;
    public float damageInterval = 2f;
    private bool isPlayerInRange = false;
    private Coroutine damageCoroutine;
    private Transform player;
    private NavMeshAgent agent;
    private Animator MinionAnimator;
    public float closeDistance = 20f;
    public float attackRange = 2f;
    private Colliding_Minion campCollider;
    private GameObject campArea;
    private PlayerController playerController;
    GameObject playerObject;

    // Store the minion's original position
    private Vector3 originalPosition;

    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        Debug.Log(colliders);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Camp"))
            {
                campArea = collider.gameObject;
                break;
            }
        }

        agent = GetComponent<NavMeshAgent>();
        campCollider = campArea.GetComponent<Colliding_Minion>();

        if (campCollider == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find its camp!");
        }

        MinionAnimator = GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        currentState = MinionState.Alerted;

        // Store the original position of the minion
        originalPosition = transform.position;

        Debug.Log("Minion state is: " + currentState);
    }

    void Update()
    {
        if (campCollider.playerInCamp)
        {
            if (currentState == MinionState.Alerted)
            {
                float distance = Vector3.Distance(transform.position, player.position);

                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    MinionAnimator.SetBool("punch", true);
                    MinionAnimator.SetBool("run", false);
                    MinionAnimator.SetBool("idle", false);
                }
                else
                {
                    if(GetComponent<Enemy>().health > 0){
                        agent.isStopped = false;
                    }
                    agent.SetDestination(player.position);
                    MinionAnimator.SetBool("run", true);
                    MinionAnimator.SetBool("idle", false);
                    MinionAnimator.SetBool("punch", false);
                }
            }
        }
        else
        {
            // When the player leaves the camp, move back to the original position
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // The minion has reached its original position
                MinionAnimator.SetBool("idle", true);
                MinionAnimator.SetBool("run", false);
                MinionAnimator.SetBool("punch", false);
            }
            else
            {
                if(GetComponent<Enemy>().health > 0){
                    agent.isStopped = false;
                }
                agent.SetDestination(originalPosition); // Move back to the original position
                MinionAnimator.SetBool("run", true);
                MinionAnimator.SetBool("idle", false);
                MinionAnimator.SetBool("punch", false);
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerController = null;
        }
    }

    public void DealDamageToPlayer()
    {
        if (isPlayerInRange && player != null)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log("Minion dealt damage to the player!");
        }
    }

    public void AlertMinion()
    {
        currentState = MinionState.Alerted;
        Debug.Log("Minion is now Alerted!");
    }
}

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
    private bool isTargetInRange = false;
    private Coroutine damageCoroutine;
    private Transform target;
    private NavMeshAgent agent;
    private Animator minionAnimator;
    public float closeDistance = 20f;
    public float attackRange = 2f;
    private Colliding_Minion campCollider;
    private GameObject campArea;
    private PlayerController playerController;
    GameObject playerObject;

    private Vector3 originalPosition;

    void Start()
    {
        // Find the camp area collider
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
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

        minionAnimator = GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        target = playerObject.transform;
        currentState = MinionState.Alerted;

        originalPosition = transform.position;
    }

    void Update()
    {
        if (currentState == MinionState.Alerted)
        {
            if (campCollider.playerInCamp)
            {
                HandleTargeting();
            }
            else
            {
                ReturnToOriginalPosition();
                float distance = Vector3.Distance(transform.position, player.position);
                Vector3 directionToTarget = (player.position - transform.position).normalized;
                directionToTarget.y = 0;
                transform.rotation = Quaternion.LookRotation(directionToTarget);

                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    MinionAnimator.SetBool("punch", true);
                    MinionAnimator.SetBool("run", false);
                    MinionAnimator.SetBool("idle", false);
                }
                else
                {
                    if (GetComponent<Enemy>().health > 0)
                    {
                        agent.isStopped = false;
                    }
                    agent.SetDestination(player.position);
                    MinionAnimator.SetBool("run", true);
                    MinionAnimator.SetBool("idle", false);
                    MinionAnimator.SetBool("punch", false);
                }
            }
        }
    }

    private void HandleTargeting()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > closeDistance)
        {
            ReturnToOriginalPosition();
            return;
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0; // Keep movement flat
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            minionAnimator.SetBool("punch", true);
            minionAnimator.SetBool("run", false);
            minionAnimator.SetBool("idle", false);
        }
        else
        {
            if (GetComponent<Enemy>().health > 0)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(target.position);
            minionAnimator.SetBool("run", true);
            minionAnimator.SetBool("idle", false);
            minionAnimator.SetBool("punch", false);
        }
    }

    private void ReturnToOriginalPosition()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            minionAnimator.SetBool("idle", true);
            minionAnimator.SetBool("run", false);
            minionAnimator.SetBool("punch", false);
        }
        else
        {
            if (GetComponent<Enemy>().health > 0)
            {
                agent.isStopped = false;
                if (GetComponent<Enemy>().health > 0)
                {
                    agent.isStopped = false;
                }
                agent.SetDestination(originalPosition); // Move back to the original position
                MinionAnimator.SetBool("run", true);
                MinionAnimator.SetBool("idle", false);
                MinionAnimator.SetBool("punch", false);
            }
            agent.SetDestination(originalPosition);
            minionAnimator.SetBool("run", true);
            minionAnimator.SetBool("idle", false);
            minionAnimator.SetBool("punch", false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            isTargetInRange = true;
            if (other.CompareTag("Player"))
            {
                playerController = other.GetComponent<PlayerController>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            isTargetInRange = false;
            if (other.CompareTag("Player"))
            {
                playerController = null;
            }
        }
    }

    public void DealDamageToPlayer()
    {
        if (isPlayerInRange && player != null && !playerController.isShieldActive)
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        HandleTargeting();
      }

private void OnEnable()
    {
        GameManager.OnPlayerInstantiated += SetPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerInstantiated -= SetPlayer;
    }

    public void SetPlayer(GameObject player)
    {
        playerObject = player;
        this.player = player.transform;
    }
}

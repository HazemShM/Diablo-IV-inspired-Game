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
    private GameObject targetObject;
    private Transform player;
    private Transform clone;
    private NavMeshAgent agent;
    private Animator MinionAnimator;
    public float closeDistance = 60f;
    public float attackRange = 2f;
    private Colliding_Minion campCollider;
    private GameObject campArea;
    private PlayerController playerController;
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
        campCollider = campArea != null ? campArea.GetComponent<Colliding_Minion>() : null;

        if (campCollider == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find its camp!");
        }

        MinionAnimator = GetComponent<Animator>();

        originalPosition = transform.position;
    }

    void Update()
    {
        UpdateTarget();

        if (campCollider != null && (campCollider.playerInCamp || campCollider.cloneInCamp))
        {
            if (currentState == MinionState.Alerted && target != null)
            {
                HandleTargeting();
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void UpdateTarget()
    {
        GameObject cloneObject = GameObject.FindGameObjectWithTag("Clone");

        if (cloneObject != null)
        {
            clone = cloneObject.transform;
            target = clone;
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
                target = player;
            }
        }
    }

    private void HandleTargeting()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (!campCollider.playerInCamp && !campCollider.cloneInCamp)
        {
            ReturnToOriginalPosition();
            return;
        }

        Vector3 directionToTarget = (target.position - transform.position).normalized;
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
            agent.SetDestination(target.position);
            MinionAnimator.SetBool("run", true);
            MinionAnimator.SetBool("idle", false);
            MinionAnimator.SetBool("punch", false);
        }
    }

    private void ReturnToOriginalPosition()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            MinionAnimator.SetBool("idle", true);
            MinionAnimator.SetBool("run", false);
            MinionAnimator.SetBool("punch", false);
        }
        else
        {
            if (GetComponent<Enemy>().health > 0)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(originalPosition);
            MinionAnimator.SetBool("run", true);
            MinionAnimator.SetBool("idle", false);
            MinionAnimator.SetBool("punch", false);
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
        if (isTargetInRange && playerController != null && !playerController.isShieldActive && !playerController.invincible)
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
        if (player != null)
        {
            this.player = player.transform;
        }
    }
}

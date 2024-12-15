using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    private Transform player;
    private Transform clone;
    private NavMeshAgent agent;
    private Animator animator;
    public float closeDistance = 30f;
    private PlayerController playerController;
    private Colliding_Minion campCollider;
    private GameObject campArea;

    private bool isTargetInRange = false;
    public int attackDamage = 10;

    // New patrol points
    private Vector3[] points;
    private int currentpointIndex = 0;
    Vector3 target;
    private bool wasRunningOnTarget = false;
    GameObject playerObject;
    public bool isAggressive;
    public bool isDead;

    void Start()
    {
        isDead = false;
        isAggressive = false;
        agent = GetComponent<NavMeshAgent>();
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
        animator = GetComponent<Animator>();

        // Generate patrol points
        points = GeneratePoints(transform.position, 10f);
        if (points.Length > 0)
        {
            target = points[currentpointIndex];
            agent.SetDestination(target);
        }
    }

    Vector3[] GeneratePoints(Vector3 center, float offset)
    {
        return new Vector3[]
        {
            center + new Vector3(0, 0, offset),  // North (+Z)
            center + new Vector3(offset, 0, 0),   // East (+X)
            center + new Vector3(0, 0, -offset), // South (-Z)
            center + new Vector3(-offset, 0, 0), // West (-X)
        };
    }

    void Update()
    {
        UpdateTarget();

        if (campCollider.playerInCamp && isAggressive)
        {
            float distance = Vector3.Distance(transform.position, clone != null ? clone.position : player.position);

            if (distance < closeDistance)
            {
                if (animator != null &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.SetBool("attack", true);
                    animator.SetBool("run", false);
                    animator.SetBool("walk", false);
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(clone != null ? clone.position : player.position);
                if (animator != null &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.SetBool("run", true);
                    animator.SetBool("walk", false);
                    animator.SetBool("attack", false);
                }
            }
            wasRunningOnTarget = true;
        }
        else
        {
            if (animator != null &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetBool("walk", true);
                animator.SetBool("run", false);
                animator.SetBool("attack", false);
            }

            if (wasRunningOnTarget)
            {
                agent.SetDestination(target);
                wasRunningOnTarget = false;
            }
            if (Vector3.Distance(transform.position, target) < 1)
            {
                currentpointIndex = (currentpointIndex + 1) % points.Length;
                target = points[currentpointIndex];
                agent.SetDestination(target);
            }
        }
    }

    private void UpdateTarget()
    {
        // Check if a Clone exists and set it as the target
        GameObject cloneObject = GameObject.FindGameObjectWithTag("Clone");

        if (cloneObject != null)
        {
            clone = cloneObject.transform;
        }
        else
        {
            clone = null;
        }

        // If no Clone exists, fallback to Player
        if (clone == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
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
        if (isTargetInRange && player != null && !playerController.isShieldActive && !playerController.invincible)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log("Demon dealt damage to the player!");
        }
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

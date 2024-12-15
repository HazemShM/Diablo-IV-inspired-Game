using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    public float closeDistance = 30f;
    private PlayerController playerController;
    private Colliding_Minion campCollider;
    private GameObject campArea;

    private bool isPlayerInRange = false;
    public int attackDamage = 10;
    //new
    private Vector3[] points;
    private int currentpointIndex = 0;
    Vector3 target;
    private bool wasRunningOnThePlayer = false;
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
        //new
        points = GeneratePoints(transform.position, 10f);
        if (points.Length > 0)
        {
            target = points[currentpointIndex];
            agent.SetDestination(target);
        }
    }

    //new
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
        if (campCollider.playerInCamp && isAggressive)
        {
            float distance = Vector3.Distance(transform.position, player.position);
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
                agent.SetDestination(player.position);
                if (animator != null &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.SetBool("run", true);
                    animator.SetBool("walk", false);
                    animator.SetBool("attack", false);
                }
            }
            wasRunningOnThePlayer = true;
        }
        else
        {
            // new
            // agent.isStopped = true;
            if (animator != null &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetBool("walk", true);
                animator.SetBool("run", false);
                animator.SetBool("attack", false);
            }
            // new
            if (wasRunningOnThePlayer)
            {
                agent.SetDestination(target);
                wasRunningOnThePlayer = false;
            }
            if (Vector3.Distance(transform.position, target) < 1)
            {
                currentpointIndex = (currentpointIndex + 1) % points.Length;
                target = points[currentpointIndex];
                agent.SetDestination(target);
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
        if (isPlayerInRange && player != null && !playerController.isShieldActive)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log("Minion dealt damage to the player!");
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

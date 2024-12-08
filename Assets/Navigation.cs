using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private NavMeshAgent agent; // The NavMeshAgent component
    public GameObject Collider; // Reference to the GameObject with the Colliding script
    private Colliding collidingScript;
    private Animator animator;
     public float closeDistance = 30f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        collidingScript = Collider.GetComponent<Colliding>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (collidingScript.playerInCamp)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closeDistance)
            {   
                if (animator != null && 
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.SetBool("attack", true);
                    animator.SetBool("run", false);
                    animator.SetBool("idle", false);
                }
            }
            else{
                agent.isStopped = false;
                agent.SetDestination(player.position);
                if (animator != null && 
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.SetBool("run", true); 
                    animator.SetBool("idle", false); 
                    animator.SetBool("attack", false);
                }
            }
        }
        else
        {
            agent.isStopped = true;
            if (animator != null && 
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetBool("idle", true);
                animator.SetBool("run", false);
                animator.SetBool("attack", false);
            }
        }
    }
}

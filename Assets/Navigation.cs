using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public GameObject Collider;
    private Colliding collidingScript;
    private Animator animator;
    public float closeDistance = 30f;

    //new
    private Vector3[] points;
    private int currentpointIndex = 0;
    Vector3 target;
    private bool wasRunningOnThePlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        collidingScript = Collider.GetComponent<Colliding>();
        animator = GetComponent<Animator>();
        //new
        points = GeneratePoints(transform.position, 10f);
        if (points.Length > 0)
        {
            target = points[currentpointIndex];
            agent.SetDestination(target);
            Debug.Log("hahahah");
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
                    animator.SetBool("walk", false);
                }
            }
            else{
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
            if(wasRunningOnThePlayer){
                agent.SetDestination(target);
                wasRunningOnThePlayer = false;
            }
            if(Vector3.Distance(transform.position, target) < 1){
                currentpointIndex = (currentpointIndex + 1) % points.Length;
                target = points[currentpointIndex];
                agent.SetDestination(target);
            }
        }
    }
}

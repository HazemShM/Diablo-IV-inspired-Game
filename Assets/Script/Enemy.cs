using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

public class Enemy : MonoBehaviour
{   
    public float health = 20f;
    Animator animator ;
    private NavMeshAgent agent;
    public int xp = 10;

    void Start(){
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update(){
        if (health <= 0)
        {
            Die();
        }
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Minion Health: " + health);
    }

    private void Die()
    {
        agent.isStopped = true;
        animator.SetTrigger("die");
        Destroy(gameObject, 3f);
    }
    public void setHealth(float health){
        this.health = health;
    }
    public void setXp(int xp){
        this.xp = xp;
    }
    public int getXp(){
        return xp;
    }
}

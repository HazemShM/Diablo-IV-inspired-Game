using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

public class Enemy : MonoBehaviour
{   
    public float health, maxHealth = 20f;
    Animator animator ;
    private NavMeshAgent agent;
    GameObject playerObject;
    public int xp = 10;
    bool died = false;
    [SerializeField] FloatingHealthBar healthBar;
    void Start(){
        health = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Awake(){
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }
    void Update(){
        if (health <= 0 && !died)
        {
            Die();
            died = true;
        }
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        healthBar.UpdateHealthBar(health, maxHealth);
        Debug.Log("Minion Health: " + health);
    }

    private void Die()
    {
        agent.isStopped = true;
        playerObject.GetComponent<PlayerController>().GainXP(xp);
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

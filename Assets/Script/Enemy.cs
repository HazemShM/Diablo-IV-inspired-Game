using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

public class Enemy : MonoBehaviour
{
    public float health, maxHealth = 20f;
    Animator animator;
    private NavMeshAgent agent;
    GameObject playerObject;
    public int xp = 10;
    bool died = false;
    Navigation navigation;
    [SerializeField] FloatingHealthBar healthBar;
    void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }
    void Update()
    {
        if (health <= 0 && !died)
        {
            Die();
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
        died = true;
        agent.isStopped = true;
        animator.SetTrigger("die");
        navigation = gameObject.GetComponent<Navigation>();
        if (navigation != null)
        {
            navigation.isDead = true;
        }
        Destroy(gameObject, 2f);
        playerObject.GetComponent<PlayerController>().GainXP(xp);
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
    }

    public void setHealth(float health)
    {
        this.health = health;
    }
    public void setXp(int xp)
    {
        this.xp = xp;
    }
    public int getXp()
    {
        return xp;
    }
}

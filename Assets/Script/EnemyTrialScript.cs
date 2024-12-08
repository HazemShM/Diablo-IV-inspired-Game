using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

public class EnemyTrialScript : MonoBehaviour
{   
    int health = 20;
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }


}

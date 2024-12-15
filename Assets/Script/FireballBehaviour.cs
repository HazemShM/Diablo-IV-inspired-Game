using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class FireballBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.health -= 5;
                Debug.Log("Fireball hit the enemy! Enemy health is now: " + enemy.health);
            }
            Debug.Log("Fireball hit the enemy!");
            Destroy(gameObject);
        }
    }
}

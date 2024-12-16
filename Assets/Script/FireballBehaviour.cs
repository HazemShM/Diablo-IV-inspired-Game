using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class FireballBehavior : MonoBehaviour
{
    public SorcererAbilityManager sorcererAbilityManager;
    public PlayerController playerController;

    void start(){
        playerController = sorcererAbilityManager.GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            LilithAnimation lilith = other.GetComponent<LilithAnimation>();
            if(lilith != null){
                lilith.TakeDamage(5f, playerController);
            }else{
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(5);
                    Debug.Log("Fireball hit the enemy! Enemy health is now: " + enemy.health);
                }
            }
            Debug.Log("Fireball hit the enemy!");
            Destroy(gameObject);
        }
    }
}

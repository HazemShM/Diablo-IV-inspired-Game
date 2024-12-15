using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool enableDamage = true;
    private float damage = 5f;
    private PlayerController playerController;
    private GameObject player;
    public GameObject hitParticle;
    private void Start(){
        Destroy(gameObject,3);
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }   
    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Enemy")){
            if(enableDamage){
                Enemy enemy = other.GetComponent<Enemy>();
                if(enemy){
                    GameObject particleInstance = Instantiate(
                        hitParticle,
                        new Vector3(
                            other.transform.position.x,
                            transform.position.y,
                            other.transform.position.z
                        ),
                        other.transform.rotation
                    );
                    Destroy(particleInstance, 2.0f);
                    enemy.TakeDamage(damage);
                }
                LilithAnimation lilith = other.GetComponent<LilithAnimation>();

                if (lilith != null)
                {
                    if (lilith.activeMinions.Count > 0)
                    {
                        Debug.Log("There are active minions. Kill them first before damaging Lilith!");
                        return;
                    }
                    else if (lilith.isShieldActive)
                    {
                        if (!lilith.isAuraActive)
                        {
                            lilith.shieldHealth -= damage;
                            Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");
                        }
                        if (lilith.shieldHealth <= 0)
                        {
                            StartCoroutine(lilith.CheckShieldDestroyed());
                        }
                        if (lilith.isAuraActive)
                        {
                            Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                            playerController.ReflectDamage((int)damage + 15);
                            lilith.isAuraActive = false;
                            StartCoroutine(lilith.ReflectiveAuraCountdown());
                        }
                    }
                    else
                    {
                        lilith.TakeDamage(damage, playerController);
                        Debug.Log($"Lilith's Health: {lilith.bossHealth}");
                        GameObject particleInstance = Instantiate(
                        hitParticle,
                        new Vector3(
                            lilith.transform.position.x,
                            transform.position.y,
                            lilith.transform.position.z
                        ), lilith.transform.rotation);
                        Destroy(particleInstance, 2.0f);
                    }
                }
            }
            Destroy(transform.GetComponent<Rigidbody>());
            Destroy(gameObject);
        }
    }
}

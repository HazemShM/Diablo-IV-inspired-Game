using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool enableDamage = true;
    private void Start(){
        Destroy(gameObject,3);
    }   
    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Enemy")){
            if(enableDamage){
                Enemy enemy = other.GetComponent<Enemy>();
                if(enemy){
                    enemy.TakeDamage(5);
                }
            }
            Destroy(transform.GetComponent<Rigidbody>());
            Destroy(gameObject);
        }
    }
}

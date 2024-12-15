using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public float speed = 15f;
    public float explosionRadius = 5f;
    public int damage = 15;
    public float lifetime = 5f;

    private Vector3 targetPosition;

    public void Initialize(Vector3 target, int damage)
    {
        targetPosition = target;
        this.damage = damage;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("Explosive detonated!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null && !player.isShieldActive && !player.invincible)
                {
                    player.TakeDamage(damage);
                    Debug.Log("Player took explosive damage!");
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

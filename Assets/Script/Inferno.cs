using UnityEngine;
using System.Collections;

public class DamageAbility : MonoBehaviour
{
    public float damageRange = 10f;
    public int initialDamage = 10;
    public int ongoingDamage = 2;
    public float duration = 5f;
    private Collider[] collidersInRange;
    public SorcererAbilityManager sorcererAbilityManager;
    public PlayerController playerController;

    private void Start()
    {
        playerController = sorcererAbilityManager.GetComponent<PlayerController>();
        StartCoroutine(DealDamageOverTime());
    }

    private IEnumerator DealDamageOverTime()
    {
        playerController = sorcererAbilityManager.GetComponent<PlayerController>();
        DealDamage(initialDamage);

        yield return new WaitForSeconds(1f);

        float elapsedTime = 1f;
        while (elapsedTime < duration)
        {
            DealDamage(ongoingDamage);
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        Destroy(gameObject);
    }

    private void DealDamage(int damageAmount)
    {
        playerController = sorcererAbilityManager.GetComponent<PlayerController>();
        collidersInRange = Physics.OverlapSphere(transform.position, damageRange);

        foreach (Collider collider in collidersInRange)
        {
            if (collider.CompareTag("Enemy"))
            {
                LilithAnimation lilith = collider.GetComponent<LilithAnimation>();
                if(lilith != null){
                    playerController = sorcererAbilityManager.GetComponent<PlayerController>();
                    Debug.Log(playerController);
                    if(playerController != null){
                        lilith.TakeDamage(damageAmount, playerController);
                    }
                }
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                    Debug.Log($"Enemy hit! New health: {enemy.health}");
                }
            }
        }
    }
}

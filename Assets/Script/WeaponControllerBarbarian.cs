using UnityEngine;

public class WeaponControllerBarbarian : MonoBehaviour
{
    public BarbarianAnimation barbarian;
    public GameObject hitParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !barbarian.canAttack)
        {
            if (barbarian.currentAbility.type == AbilityType.WildCard)
            {
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
                Enemy enemy = other.GetComponent<Enemy>();
                Animator enemyAnimator = enemy.GetComponent<Animator>();
                PlayerController playerController = GetComponent<PlayerController>();
                if (enemy != null)
                {
                    enemyAnimator.SetTrigger("hit");
                    enemy.TakeDamage(barbarian.currentAbility.damage); // Apply IronMaelstorm damage
                }
            }
        }
    }
}

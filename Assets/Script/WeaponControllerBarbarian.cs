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
                Minion minion = other.GetComponent<Minion>();
                Animator minionAnimator = minion.GetComponent<Animator>();
                PlayerController playerController = GetComponent<PlayerController>();
                if (minion != null)
                {
                    minionAnimator.SetTrigger("hit");
                    minion.TakeDamage(barbarian.currentAbility.damage); // Apply IronMaelstorm damage

                    if (minion.health <= 0)
                    {
                        playerController.GainXP(10);
                    }
                }
            }
        }
    }
}

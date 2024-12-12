using UnityEngine;

public class WeaponControllerBarbarian : MonoBehaviour
{
    public BarbarianAnimation barbarian;
    public GameObject hitParticle;
    public PlayerController playerController;
    public BoxCollider weaponCollider;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        weaponCollider.enabled = true;
        // Access the PlayerController via the BarbarianAnimation component
        if (barbarian != null)
        {
            playerController = barbarian.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController not found on the same GameObject as BarbarianAnimation!");
            }
            else
            {
                Debug.Log("PlayerController successfully retrieved from BarbarianAnimation.");
            }
        }
        else
        {
            Debug.LogError("BarbarianAnimation component is not assigned in WeaponControllerBarbarian!");
        }
    }

    void Update()
    {
        if (barbarian.weaponColliderEnabled)
        {
            weaponCollider.enabled = true;
        }
        else if (!barbarian.weaponColliderEnabled)
        {
            weaponCollider.enabled = false;
        }
    }

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
                LilithAnimation lilith = other.GetComponent<LilithAnimation>();
                if (enemy != null)
                {
                    Animator enemyAnimator = enemy.GetComponent<Animator>();
                    enemyAnimator.SetTrigger("hit");
                    enemy.TakeDamage(barbarian.currentAbility.damage); // Apply IronMaelstorm damage
                }
                else if (lilith != null)
                {
                    if (lilith.activeMinions.Count > 0)
                    {
                        Debug.Log("kill minions first");
                    }
                    else if (lilith.isShieldActive)
                    {
                        lilith.shieldHealth -= barbarian.currentAbility.damage;
                        Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");

                        if (lilith.shieldHealth <= 0)
                        {
                            lilith.CheckShieldDestroyed();
                        }
                        else
                        {
                            Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                            playerController.ReflectDamage((int)barbarian.currentAbility.damage + 15);

                        }
                    }
                    else
                    {
                        lilith.TakeDamage(barbarian.currentAbility.damage, playerController);
                        Debug.Log($"Lilith's Health: {lilith.bossHealth}");
                    }
                }
            }
        }
    }
}

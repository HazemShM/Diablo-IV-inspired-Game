using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class BarbarianAnimation : MonoBehaviour
{
    [SerializeField]
    private string movementSpeed = "MovementSpeed";
    Animator anim;
    AudioSource ac;
    private Camera cam;
    private NavMeshAgent agent;

    [SerializeField]
    private InputAction selectTarget = new InputAction();
    private Transform selectedTarget;
    private PlayerController playerController;
    private List<Ability> abilities = new List<Ability>();
    public Ability currentAbility;
    public bool canAttack = true;
    bool isUsingAbility = false;
    bool isCharging = false;
    public AudioClip bashAttackSound;
    public AudioClip ironMaelstormAttackSound;
    public AudioClip chargeAttackSound;
    public GameObject hitParticle;

    public void Start()
    {
        cam = Camera.main;
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        Ability basicAbillity = new Ability(AbilityType.Basic, "Bash", KeyCode.Mouse1, 5, 1);
        basicAbillity.unlockAbility();
        abilities.Add(basicAbillity);
        abilities.Add(new Ability(AbilityType.Defensive, "Shield", KeyCode.W, 0, 10));
        abilities.Add(new Ability(AbilityType.WildCard, "Iron Maelstorm", KeyCode.Q, 10, 5));
        abilities.Add(new Ability(AbilityType.Ultimate, "Charge", KeyCode.E, 100, 10));
    }

    public void Update()
    {
        foreach (var ability in abilities)
        {
            if (ability.isOnCooldown)
            {
                ability.cooldownTimer -= Time.deltaTime;
                if (ability.cooldownTimer <= 0 && ability.isOnCooldown)
                {
                    ability.cooldownTimer = 0;
                    ability.isOnCooldown = false;
                    Debug.Log($"{ability.name} is ready!");
                }
            }

            if (Input.GetKeyDown(ability.activationKey) && canAttack)
            {
                if (isUsingAbility)
                {
                    Debug.Log($"{currentAbility.name} is currently active!");
                    continue;
                }
                if (!ability.isOnCooldown)
                {
                    Debug.Log($"{ability.name} is called!");
                    UseAbility(ability);
                }
                else
                {
                    Debug.Log($"{ability.name} is on cooldown!");
                }
            }
        }
        if (isUsingAbility)
        {
            if (currentAbility.type == AbilityType.Basic)
            {
                SelectTarget();
                Bash();
            }
            else if (currentAbility.type == AbilityType.Defensive)
            {
                Debug.Log("Shield");
                isUsingAbility = false;
            }
            else if (currentAbility.type == AbilityType.WildCard)
            {
                IronMaelstorm();
            }
            else if (currentAbility.type == AbilityType.Ultimate)
            {
                Charge();
            }
        }
    }

    void UseAbility(Ability ability)
    {
        currentAbility = ability;
        Debug.Log($"{ability.name} used!");
        ability.isOnCooldown = true;
        ability.cooldownTimer = ability.cooldownTime;
        isUsingAbility = true;
    }

    public void IronMaelstorm()
    {
        canAttack = false;
        isUsingAbility = false;
        anim.SetTrigger("IronMaelstorm");
        ac.PlayOneShot(ironMaelstormAttackSound);
        StartCoroutine(AttackCooldownIronMaelstorm());
    }

    public void Bash()
    {
        if (selectedTarget == null)
        {
            Debug.LogWarning("No target selected!");
            return;
        }

        Vector3 directionToTarget = (selectedTarget.position - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        LilithAnimation lilith = selectedTarget.GetComponent<LilithAnimation>();
        // Animator lilithAnimator = lilith.GetComponent<Animator>();
        Enemy enemy = selectedTarget.GetComponent<Enemy>();
        if (enemy != null)
        {
            GameObject particleInstance = Instantiate(
                hitParticle,
                new Vector3(
                    enemy.transform.position.x,
                    transform.position.y,
                    enemy.transform.position.z
                ),
                enemy.transform.rotation
            );
            Destroy(particleInstance, 2.0f);
            Animator enemyAnimator = enemy.GetComponent<Animator>();
            enemyAnimator.SetTrigger("hit");
            enemy.TakeDamage(currentAbility.damage);
            Debug.Log(enemy.health);
        }
        if (lilith != null)
        {
            if (lilith.activeMinions.Count > 0)
            {
                Debug.Log("There are active minions. Kill them first before damaging Lilith!");
                return;
            }
            else if (lilith.isShieldActive)
            {
                lilith.shieldHealth -= currentAbility.damage;
                Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");

                if (lilith.shieldHealth <= 0)
                {
                    lilith.CheckShieldDestroyed();
                }
                else
                {
                    Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                    playerController.ReflectDamage((int)currentAbility.damage + 15);

                }
            }
            else
            {
                lilith.TakeDamage(currentAbility.damage, playerController);
                Debug.Log($"Lilith's Health: {lilith.bossHealth}");
                GameObject particleInstance = Instantiate(
                hitParticle,
                new Vector3(
                    lilith.transform.position.x,
                    transform.position.y,
                    lilith.transform.position.z
                ),lilith.transform.rotation);
                Destroy(particleInstance, 2.0f);
            }
        }
        canAttack = false;
        anim.SetTrigger("Bash");
        ac.PlayOneShot(bashAttackSound);
        StartCoroutine(AttackCooldownBash());
        selectedTarget = null;
    }


    public void Charge()
    {
        canAttack = false;
        isUsingAbility = false;
        ac.PlayOneShot(chargeAttackSound);
        Debug.Log("Charge ability activated! Use right-click to select the charge target.");
        StartCoroutine(ChargeSequence());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !canAttack)
        {
            if (currentAbility.type == AbilityType.Ultimate && isCharging)
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
                LilithAnimation lilith = other.GetComponent<LilithAnimation>();

                if (lilith != null)
                {
                    if (lilith.isShieldActive)
                    {
                        lilith.shieldHealth -= currentAbility.damage;
                        Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");

                        if (lilith.shieldHealth <= 0)
                        {
                            lilith.CheckShieldDestroyed();
                        }
                        else
                        {
                            Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                            playerController.ReflectDamage((int)currentAbility.damage);
                        }
                    }
                    else
                    {
                        lilith.TakeDamage(currentAbility.damage, playerController);
                    }
                }
            }
        }
        else if (other.CompareTag("Spikes"))
        {
            playerController.TakeDamage(30);
            Debug.Log($"Player hit spikes, -30 hp, Player Health: {playerController.currentHP}");
        }
    }

    private IEnumerator UltDischarge()
    {
        yield return new WaitForSeconds(3);
    }

    private IEnumerator ChargeSequence()
    {
        Vector3? targetPosition = null;
        while (targetPosition == null)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit1, 100, playerController.layerMask))
                {
                    targetPosition = hit1.point;
                    Debug.Log("Target position set: " + targetPosition);
                }
            }
            yield return null;
        }

        Vector3 target = targetPosition.Value;
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target, out hit, 2.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target position is not on a valid NavMesh!");
            anim.SetBool("isCharging", false);
            canAttack = true;
            yield break;
        }

        target = hit.position;
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        agent.isStopped = true;
        anim.SetBool("isCharging", true);
        anim.SetTrigger("Charge");

        float chargeSpeed = 10f;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                chargeSpeed * Time.deltaTime
            );
            isCharging = true;
            yield return null;
        }

        agent.isStopped = false;
        agent.ResetPath();

        anim.SetBool("isCharging", false);
        canAttack = true;
        isCharging = false;
    }

    private IEnumerator AttackCooldownBash()
    {
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }

    private IEnumerator AttackCooldownIronMaelstorm()
    {
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void OnEnable()
    {
        selectTarget.Enable();
    }

    private void OnDisable()
    {
        selectTarget.Disable();
    }

    private void SelectTarget()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, playerController.layerMask))
        {
            float hitDistance = Vector3.Distance(ray.origin, hit.point);
            if (hit.transform.CompareTag("Enemy"))
            {
                selectedTarget = hit.transform;
            }
        }
        isUsingAbility = false;
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat(movementSpeed, speed);
    }
}

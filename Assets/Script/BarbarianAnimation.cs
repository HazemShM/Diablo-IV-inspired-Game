using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    [SerializeField]
    private NavMeshSurface navmesh;
    private PlayerController playerController;
    private List<Ability> abilities = new List<Ability>();
    public Ability currentAbility;
    public bool canAttack = true;
    bool isUsingAbility = false;
    bool isCharging = false;
    public AudioClip shieldSound;
    public AudioClip chargeAttackSound;
    public GameObject hitParticle;
    public bool weaponColliderEnabled;
    bool rebuildNavMesh;
    [SerializeField]
    private GameObject shieldEffectPrefab; // Visual effect for the shield
    private GameObject activeShieldEffect; // Instance of the shield effect
    private float shieldDuration = 3.0f;
    public void Start()
    {
        cam = Camera.main;
        weaponColliderEnabled = true;
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        Ability basicAbillity = new Ability(AbilityType.Basic, "Bash", KeyCode.Mouse1, 5, 1);
        rebuildNavMesh = false;
        basicAbillity.unlockAbility();
        abilities.Add(basicAbillity);
        abilities.Add(new Ability(AbilityType.Defensive, "Shield", KeyCode.W, 0, 10));
        abilities.Add(new Ability(AbilityType.WildCard, "Iron Maelstorm", KeyCode.Q, 10, 5));
        abilities.Add(new Ability(AbilityType.Ultimate, "Charge", KeyCode.E, 100, 10));
    }

    public void Update()
    {
        if (playerController.defensiveUnlock)
        {
            abilities[1].unlockAbility();
        }
        if (playerController.wildcardUnlock)
        {
            abilities[2].unlockAbility();
        }
        if (playerController.ultimateUnlock)
        {
            abilities[3].unlockAbility();
        }
        if (rebuildNavMesh)
        {
            navmesh.BuildNavMesh();
            Debug.Log("rebuild nav mesh");
            rebuildNavMesh = false;
        }
        foreach (var ability in abilities)
        {

            if (ability.type == AbilityType.Basic)
            {
                playerController.basicCooldownText.text = $"{(int)ability.cooldownTimer}";
            }
            else if (ability.type == AbilityType.WildCard)
            {
                playerController.wildcardCooldownText.text = $"{(int)ability.cooldownTimer}";
            }
            else if (ability.type == AbilityType.Defensive)
            {
                playerController.defensiveCooldownText.text = $"{(int)ability.cooldownTimer}";
            }
            else if (ability.type == AbilityType.Ultimate)
            {
                playerController.ultimateCooldownText.text = $"{(int)ability.cooldownTimer}";
            }

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

            if (Input.GetKeyDown(ability.activationKey) && canAttack && ability.unlocked)
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
                Shield();
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
    public void Shield()
    {
        if (playerController.isShieldActive)
        {
            Debug.Log("Shield is already active!");
            return;
        }

        playerController.isShieldActive = true;

        // Play shield activation animation
        anim.SetTrigger("Shield");

        // Activate visual effect
        if (shieldEffectPrefab != null)
        {
            activeShieldEffect = Instantiate(
                shieldEffectPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }

        Debug.Log("Shield activated!");
        isUsingAbility = false;
        ac.PlayOneShot(shieldSound);
        // Start the shield duration coroutine
        StartCoroutine(ShieldDurationCoroutine());
    }

    private IEnumerator ShieldDurationCoroutine()
    {
        yield return new WaitForSeconds(shieldDuration);

        // Deactivate shield after duration
        playerController.isShieldActive = false;
        if (activeShieldEffect != null)
        {
            Destroy(activeShieldEffect);
        }

        Debug.Log("Shield expired!");
    }

    public void IronMaelstorm()
    {
        canAttack = false;
        isUsingAbility = false;
        anim.SetTrigger("IronMaelstorm");
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
                if (!lilith.isAuraActive)
                {
                    lilith.shieldHealth -= currentAbility.damage;
                    Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");
                }
                if (lilith.shieldHealth <= 0)
                {
                    StartCoroutine(lilith.CheckShieldDestroyed());
                }
                if (lilith.isAuraActive)
                {
                    Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                    playerController.ReflectDamage((int)currentAbility.damage + 15);
                    lilith.isAuraActive = false;
                    StartCoroutine(lilith.ReflectiveAuraCountdown());
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
                ), lilith.transform.rotation);
                Destroy(particleInstance, 2.0f);
            }
        }
        canAttack = false;
        anim.SetTrigger("Bash");
        StartCoroutine(AttackCooldownBash());
        selectedTarget = null;
    }


    public void Charge()
    {
        canAttack = false;
        isUsingAbility = false;
        Debug.Log("Charge ability activated! Use right-click to select the charge target.");
        StartCoroutine(ChargeSequence());
    }
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("destroyable") && !canAttack && currentAbility.type == AbilityType.Ultimate)
        {
            Destroy(other.gameObject);
            rebuildNavMesh = true;
        }
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
            if (!canAttack && currentAbility.type == AbilityType.Ultimate)
            {
                weaponColliderEnabled = false;
                LilithAnimation lilith = other.GetComponent<LilithAnimation>();

                if (lilith != null)
                {
                    if (lilith.isShieldActive)
                    {
                        if (!lilith.isAuraActive)
                        {
                            lilith.shieldHealth -= currentAbility.damage;
                            Debug.Log($"Lilith's Shield Health: {lilith.shieldHealth}");
                        }
                        if (lilith.shieldHealth <= 0)
                        {
                            lilith.CheckShieldDestroyed();
                        }
                        else if (lilith.isAuraActive)
                        {
                            Debug.Log("Lilith's shield absorbed the damage! Reflecting damage to player.");
                            playerController.ReflectDamage(20 + 15);
                            lilith.isAuraActive = false;
                            StartCoroutine(lilith.ReflectiveAuraCountdown());
                        }
                    }
                    else
                    {
                        Debug.Log("here");
                        lilith.TakeDamage(20, playerController);
                    }
                }
            }
        }
        else if (other.CompareTag("Spikes"))
        {
            if (!playerController.isShieldActive)
            {
                playerController.TakeDamage(30);
                Debug.Log($"Player hit spikes, -30 hp, Player Health: {playerController.currentHP}");
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            weaponColliderEnabled = true;
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

    public void ApplyChargeDamage()
    {
        if (currentAbility.type == AbilityType.Ultimate && isCharging)
        {
            foreach (Enemy enemy in enemiesInRange)
            {
                if (enemy == null || damagedEnemies.Contains(enemy)) continue; // Skip if already damaged or null

                damagedEnemies.Add(enemy); // Mark enemy as damaged

                Animator enemyAnimator = enemy.GetComponent<Animator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetTrigger("hit");
                }

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

                enemy.TakeDamage(currentAbility.damage);
            }
        }
    }



    private IEnumerator ChargeSequence()
    {
        Vector3? targetPosition = null;

        // Wait for user to select a target position
        while (targetPosition == null)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit1, 100, playerController.layerMask))
                {
                    targetPosition = hit1.point;
                    ac.PlayOneShot(chargeAttackSound);
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
            yield break; // Stop the coroutine if the target is not valid
        }


        target = hit.position;
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        agent.isStopped = true;

        anim.SetBool("isCharging", true);
        anim.SetTrigger("Charge");

        float chargeSpeed = 20f;

        isCharging = true;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                chargeSpeed * Time.deltaTime
            );

            // Apply damage continuously during charge
            ApplyChargeDamage();
            yield return null;
        }

        agent.isStopped = false;
        agent.ResetPath();
        isCharging = false;
        anim.SetBool("isCharging", false);
        canAttack = true;
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
        int layerMask1 = playerController.layerMask & ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out hit, 25, layerMask1))
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

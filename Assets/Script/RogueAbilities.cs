using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RogueAbilities : MonoBehaviour
{
    Animator animator;
    private Camera cam;
    private List<Ability> abilities = new List<Ability>();
    private NavMeshAgent agent;

    [SerializeField]
    GameObject smokePrefab;

    [SerializeField]
    GameObject arrowPrefab;

    [SerializeField]
    GameObject circlePrefab;

    [SerializeField]
    Transform arrowPoint;

    [SerializeField]
    private LayerMask layerMask = new LayerMask();
    bool isUsingAbility = false;
    public Ability currentAbility;
    bool isDashing = false;
    [SerializeField]
    private GameObject hitParticle;
    private PlayerController playerController;
    private AudioSource audioSource;
    public AudioClip arrowSound;
    public AudioClip showerOfArrowsSound;
    public AudioClip dashSound;
    public AudioClip smokeSound;

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Ability basicAbillity = new Ability(AbilityType.Basic, "Arrow", KeyCode.Mouse1, 5, 1);
        basicAbillity.unlockAbility();
        abilities.Add(basicAbillity);
        abilities.Add(new Ability(AbilityType.Defensive, "SmokeBomb", KeyCode.W, 0, 10));
        abilities.Add(new Ability(AbilityType.WildCard, "Dash", KeyCode.Q, 0, 5));
        abilities.Add(new Ability(AbilityType.Ultimate, "ShowerOfArrows", KeyCode.E, 10, 10));
    }

    void Update()
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

        if (playerController.removeCooldown)
        {
            foreach (Ability ability in abilities){
                ability.cooldownTime = 0;
                ability.isOnCooldown = false;
                ability.cooldownTimer = 0;
            }
            
        }else{
            abilities[0].cooldownTime = 1;
            abilities[1].cooldownTime = 10;
            abilities[2].cooldownTime = 5;
            abilities[3].cooldownTime = 10;
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

            if (Input.GetKeyDown(ability.activationKey) && ability.unlocked)
            {
                if (isUsingAbility && ability.type != AbilityType.Defensive)
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
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (currentAbility.type == AbilityType.WildCard)
                {
                    StartCoroutine(Dash());
                    currentAbility.isOnCooldown = true;
                    currentAbility.cooldownTimer = currentAbility.cooldownTime;
                }
                else if (currentAbility.type == AbilityType.Ultimate)
                {
                    ShowerOfArrows();
                    currentAbility.isOnCooldown = true;
                    currentAbility.cooldownTimer = currentAbility.cooldownTime;
                }
            }
        }
    }

    void UseAbility(Ability ability)
    {
        Debug.Log($"{ability.name} used!");        
        if (ability.type == AbilityType.Basic)
        {   
            ability.isOnCooldown = true;
            ability.cooldownTimer = ability.cooldownTime;
            currentAbility = ability;
            isUsingAbility = true;
            Arrow();
        }
        else if (ability.type == AbilityType.Defensive)
        {   
            ability.isOnCooldown = true;
            ability.cooldownTimer = ability.cooldownTime;
            StartCoroutine(SmokeBomb());
        }else{
            currentAbility = ability;
            isUsingAbility = true;
        }
    }

    private void Arrow()
    {   
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, 100,layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // targetPosition = hit.collider.transform.position;
                animator.SetTrigger("Arrow");
                targetPosition = hit.transform.position;
                Debug.Log("Target enemy detected at: " + targetPosition);

                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                directionToTarget.y = 0;
                transform.rotation = Quaternion.LookRotation(directionToTarget);
                // StartCoroutine(SmoothRotateToTarget(targetPosition));

                // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * 2f); 
                GameObject arrow = Instantiate(arrowPrefab, arrowPoint.position, Quaternion.LookRotation(directionToTarget));
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                audioSource.PlayOneShot(arrowSound);
                if (rb != null)
                {
                    rb.AddForce(directionToTarget * 25f, ForceMode.Impulse);
                    Debug.Log("Arrow shot towards: " + targetPosition);
                }
                else
                {
                    Debug.LogError("Arrow prefab is missing a Rigidbody component!");
                    isUsingAbility = false;
                    return;
                }
            }
            else
            {
                Debug.LogWarning("No enemy found.");
            }
        }
        else
        {
            Debug.LogWarning("No valid target detected!");
        }
        isUsingAbility = false;
    }
    private IEnumerator SmoothRotateToTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0; 
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            if (!isUsingAbility) 
            {
                yield break;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            yield return null; 
        }

        transform.rotation = targetRotation;
    }
    private void ShowerOfArrows()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float duration = 3f;
        float radius = 6f;
        int damage = 10;
        float slowDownMultiplier = 0.25f;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Vector3 targetPosition = hit.point;

            Debug.Log($"Shower of Arrows activated at position: {targetPosition}");

            GameObject ring = Instantiate(circlePrefab, targetPosition, Quaternion.identity);
            audioSource.PlayOneShot(showerOfArrowsSound);
            ring.transform.localScale = new Vector3(radius, 1 , radius);
            Destroy(ring, duration);
            Collider[] hitEnemies = Physics.OverlapSphere(targetPosition, radius, layerMask);
            // DebugDrawSphere(targetPosition, radius, Color.red);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    StartCoroutine(ApplySlowEffect(enemy.gameObject, slowDownMultiplier, 3f));
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if(enemyComponent){
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
                        enemyComponent.TakeDamage(damage);
                    }
                    LilithAnimation lilith = enemy.GetComponent<LilithAnimation>();
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
            }
            StartCoroutine(ArrowRainEffect(targetPosition, duration,radius));

           
        }
        else
        {
            Debug.LogWarning("No valid target selected for Shower of Arrows!");
        }

        isUsingAbility = false;
    }

    private IEnumerator ArrowRainEffect(Vector3 position, float effectDuration , float radius)
    {
        float interval = 0.05f;
        float timer = 0;
        while (timer < effectDuration - interval)
        {
            Vector3 randomPos =
                position
                + new Vector3(
                    UnityEngine.Random.Range(-radius, radius),
                    10,
                    UnityEngine.Random.Range(-radius, radius)
                );

            GameObject arrow = Instantiate(
                arrowPrefab,
                randomPos,
                Quaternion.LookRotation(Vector3.down)
            );
            arrow.GetComponent<Arrow>().enableDamage = false;
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.down * 25f, ForceMode.Impulse);
            Destroy(arrow, 2);

            

            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        Debug.Log("Shower of Arrows effect ended.");
    }

    private IEnumerator ApplySlowEffect(GameObject enemy, float slowMultiplier, float duration)
    {   if(enemy==null){
            yield break;
        }
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            float originalSpeed = agent.speed;
            agent.speed *= slowMultiplier;

            yield return new WaitForSeconds(duration);
            if(agent){
                agent.speed = originalSpeed; 
            }
        }
    }

    private IEnumerator SmokeBomb()
    {   
        float stunRadius = 5f; 
        animator.SetTrigger("SmokeBomb");
        yield return new WaitForSeconds(0.5f);

        GameObject smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(smokeSound);

        // smoke.transform.GetChild(0).localScale = new Vector3(stunRadius, 2, stunRadius);
        // smokePrefab.transform.localScale = new Vector3(stunRadius, 2, stunRadius);

        Destroy(smoke, 2);
        Debug.Log("Smoke bomb dropped!");

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, stunRadius, layerMask);

        foreach (Collider enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                GameObject enemy = enemyCollider.gameObject;
                StartCoroutine(StunEnemy(enemy, 5f));
            }
        }
    }
    private IEnumerator StunEnemy(GameObject enemy, float stunDuration)
    {
        Animator enemyAnimator = enemy.GetComponent<Animator>();
        MonoBehaviour[] enemyScripts = enemy.GetComponents<MonoBehaviour>();
        Enemy enemyS = enemy.GetComponent<Enemy>();

        if (enemyAnimator != null)
        {
            enemyAnimator.enabled = false;
        }

        foreach (MonoBehaviour script in enemyScripts)
        {
            script.enabled = false;
        }

        Debug.Log($"{enemy.name} is stunned!");

        yield return new WaitForSeconds(stunDuration);

        if (enemyAnimator != null)
        {
            enemyAnimator.enabled = true;
        }

        foreach (MonoBehaviour script in enemyScripts)
        {
            script.enabled = true;
        }

        Debug.Log($"{enemy.name} is no longer stunned!");
    }

    private IEnumerator Dash()
    {
        if (isDashing)
        {
            yield break;
        }
        isDashing = true;
        Vector3? targetPosition = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit1, 100, playerController.layerMask))
        {
            targetPosition = hit1.point;
            Debug.Log("Target position set: " + targetPosition);
        }
        Vector3 target = targetPosition.Value;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target, out hit, 2.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target position is not on a valid NavMesh!");
            isDashing = false;
            yield break;
        }

        target = hit.position;
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        agent.isStopped = true;

        Animator anim = GetComponent<Animator>();
        anim.SetBool("Dash", true);
        anim.SetTrigger("DashTrigger");
        audioSource.PlayOneShot(dashSound);
        float dashSpeed = playerController.normalSpeed*2;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (!NavMesh.SamplePosition(target, out hit, 2.0f, NavMesh.AllAreas))
            {
                Debug.LogWarning("Target position is no longer reachable!");
                break;
            }
            transform.rotation = Quaternion.LookRotation(directionToTarget);

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                dashSpeed * Time.deltaTime
            );
            if (Vector3.Distance(transform.position, target) < 1)
            {
                anim.SetBool("Dash", false);
            }
            yield return null;
        }
        agent.isStopped = false;
        agent.ResetPath();

        isUsingAbility = false;
        isDashing = false;
    }
}

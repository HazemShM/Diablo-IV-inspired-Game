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
    Ability currentAbility;
    bool isDashing = false;
    [SerializeField]
    private GameObject hitParticle;
    void Start()
    {
        cam = Camera.main;
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

            if (Input.GetKeyDown(ability.activationKey))
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
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (currentAbility.type == AbilityType.WildCard)
                {
                    StartCoroutine(Dash());
                }
                else if (currentAbility.type == AbilityType.Ultimate)
                {
                    ShowerOfArrows();
                }
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
        if (currentAbility.type == AbilityType.Basic)
        {
            Arrow();
        }
        else if (currentAbility.type == AbilityType.Defensive)
        {
            StartCoroutine(SmokeBomb());
        }
    }

    // private void Arrow(){
    //     animator.SetTrigger("Arrow");
    //     GameObject arrow = Instantiate(arrowPrefab, arrowPoint.position, transform.rotation);
    //     arrow.GetComponent<Rigidbody>().AddForce(transform.forward *25f,ForceMode.Impulse);
    //     isUsingAbility = false;
    // }
    // private void Arrow()
    // {
    //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     Vector3 targetPosition;

    //     // Cast a ray to detect the target
    //     if (Physics.Raycast(ray, out hit, 100))
    //     {
    //         Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

    //         if (hit.collider.CompareTag("Enemy"))
    //         {
    //             // Get the target position
    //             targetPosition = hit.point;
    //             Debug.Log("Target enemy detected at: " + targetPosition);

    //             // Trigger the animation to shoot the arrow
    //             animator.SetTrigger("Arrow");

    //             // Smooth rotation of the player towards the target
    //             Vector3 directionToTarget = (targetPosition - transform.position).normalized;
    //             directionToTarget.y = 0; // Make sure the rotation is only on the Y-axis
    //             Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //             transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation

    //             // Instantiate the arrow at the specified position
    //             GameObject arrow = Instantiate(arrowPrefab, arrowPoint.position, Quaternion.LookRotation(directionToTarget));
    //             Rigidbody rb = arrow.GetComponent<Rigidbody>();

    //             // Apply force to the arrow to shoot it
    //             if (rb != null)
    //             {
    //                 rb.AddForce(directionToTarget * 25f, ForceMode.Impulse);
    //                 Debug.Log("Arrow shot towards: " + targetPosition);
    //             }
    //             else
    //             {
    //                 Debug.LogError("Arrow prefab is missing a Rigidbody component!");
    //                 isUsingAbility = false;
    //                 return;
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("No enemy found.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No valid target detected!");
    //     }
        
    //     isUsingAbility = false;
    // }
    private void Arrow()
    {   
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
            if (hit.collider.CompareTag("Enemy"))
            {
                // targetPosition = hit.collider.transform.position;
                animator.SetTrigger("Arrow");
                targetPosition = hit.point;
                Debug.Log("Target enemy detected at: " + targetPosition);

                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                directionToTarget.y = 0;
                // transform.rotation = Quaternion.LookRotation(directionToTarget);
                // StartCoroutine(SmoothRotateToTarget(targetPosition));

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * 2f); 
                GameObject arrow = Instantiate(arrowPrefab, arrowPoint.position, Quaternion.LookRotation(directionToTarget));
                Rigidbody rb = arrow.GetComponent<Rigidbody>();

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
    private void DebugDrawSphere(Vector3 position, float radius, Color color)
    {
        int segments = 36; // Number of segments to approximate the sphere
        float angleIncrement = 360f / segments;

        // Draw circle in X-Z plane
        for (int i = 0; i < segments; i++)
        {
            float angle1 = Mathf.Deg2Rad * i * angleIncrement;
            float angle2 = Mathf.Deg2Rad * (i + 1) * angleIncrement;

            Vector3 point1 = position + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2 = position + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;

            Debug.DrawLine(point1, point2, color, 0.1f); // Line duration is 0.1 seconds
        }
    }

    private void ShowerOfArrows()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float duration = 3f;
        float radius = 3f;
        int damage = 10;
        float slowDownMultiplier = 0.25f;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Vector3 targetPosition = hit.point;

            Debug.Log($"Shower of Arrows activated at position: {targetPosition}");

            GameObject ring = Instantiate(circlePrefab, targetPosition, Quaternion.identity);
            ring.transform.localScale = new Vector3(radius, 1 , radius);
            Destroy(ring, duration);
            Collider[] hitEnemies = Physics.OverlapSphere(targetPosition, radius, layerMask);
            DebugDrawSphere(targetPosition, radius, Color.red);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    StartCoroutine(ApplySlowEffect(enemy.gameObject, slowDownMultiplier, 3f));
                    enemy.GetComponent<Enemy>().TakeDamage(damage);
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
            
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.down * 25f, ForceMode.Impulse);
            Destroy(arrow, 2);

            

            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        Debug.Log("Shower of Arrows effect ended.");
    }

    private IEnumerator ApplySlowEffect(GameObject enemy, float slowMultiplier, float duration)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            float originalSpeed = agent.speed;
            agent.speed *= slowMultiplier;

            yield return new WaitForSeconds(duration);

            agent.speed = originalSpeed;
        }
    }

    private IEnumerator SmokeBomb()
    {
        animator.SetTrigger("SmokeBomb");
        yield return new WaitForSeconds(0.5f);

        GameObject smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, 2);
        Debug.Log("Smoke bomb dropped!");
        isUsingAbility = false;
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
        if (Physics.Raycast(ray, out RaycastHit hit1, 100))
        {
            targetPosition = hit1.point;
            Debug.Log("Target position set: " + targetPosition);
        }
        Vector3 target = targetPosition.Value;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target position is not on a valid NavMesh!");
            isDashing = false;
            yield break; // Stop the coroutine if the target is not valid
        }

        target = hit.position;
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        agent.isStopped = true;

        Animator anim = GetComponent<Animator>();
        anim.SetBool("Dash", true);
        anim.SetTrigger("DashTrigger");

        float dashSpeed = 10f;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (!NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
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

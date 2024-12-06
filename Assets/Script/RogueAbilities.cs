using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RogueAbilities : MonoBehaviour
{   
    
    Animator animator;
    private Camera cam ;
    private List<Ability> abilities = new List<Ability>();
    private NavMeshAgent agent;
    [SerializeField] GameObject smokePrefab;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    bool isUsingAbility = false;
    Ability currentAbility ;
    bool isDashing = false;

    void Start()
    {   
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Ability basicAbillity =new Ability(AbilityType.Basic,"Arrow", KeyCode.Mouse1, 5, 1);
        basicAbillity.unlockAbility();
        abilities.Add(basicAbillity);
        abilities.Add(new Ability(AbilityType.Defensive,"SmokeBomb", KeyCode.W, 0, 10));
        abilities.Add(new Ability(AbilityType.WildCard,"Dash", KeyCode.Q, 0, 5));
        abilities.Add(new Ability(AbilityType.Ultimate,"ShowerOfArrows", KeyCode.E, 10, 10));

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
                if(isUsingAbility){
                    Debug.Log($"{currentAbility.name} is currently active!");
                    continue;
                }
                if(!ability.isOnCooldown){
                    Debug.Log($"{ability.name} is called!");
                    UseAbility(ability);
                }else{
                    Debug.Log($"{ability.name} is on cooldown!");
                }
            }
        }
        if(isUsingAbility){ 
            if(Input.GetKeyDown(KeyCode.Mouse1)){
                if(currentAbility.type == AbilityType.Defensive){
                    SmokeBomb();
                }else if(currentAbility.type == AbilityType.WildCard){
                    StartCoroutine(Dash());
                }else if(currentAbility.type == AbilityType.Ultimate){
                    ShowerOfArrows();
                }
            }
        }

    }
    void UseAbility(Ability ability)
    {   
        currentAbility =ability;
        Debug.Log($"{ability.name} used!");
        ability.isOnCooldown = true;
        ability.cooldownTimer = ability.cooldownTime;
        isUsingAbility = true;
        if(currentAbility.type == AbilityType.Basic){
            Arrow();
        }
    }
    private void Arrow(){
        animator.SetTrigger("Arrow");
        isUsingAbility = false;

    }

    private void SmokeBomb(){
        animator.SetTrigger("SmokeBomb");
        GameObject smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, 2);
        Debug.Log("Smoke bomb dropped!");
        isUsingAbility = false;

    }
    private IEnumerator Dash(){
        if(isDashing){
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

            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }

        agent.isStopped = false;
        agent.ResetPath();

        anim.SetBool("Dash", false);
        isUsingAbility = false;
        isDashing = false;

    }
    private void ShowerOfArrows(){

    }
    


}

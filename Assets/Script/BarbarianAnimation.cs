using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Collections.Generic;
public class BarbarianAnimation : MonoBehaviour
{
    [SerializeField] private string movementSpeed = "MovementSpeed";
    Animator anim;
    AudioSource ac;
    private Camera cam ;
    private NavMeshAgent agent;
    [SerializeField] private InputAction selectTarget = new InputAction();
    private Transform selectedTarget;
    private PlayerController playerController;
    private List<Ability> abilities = new List<Ability>();
    Ability currentAbility ;
    public bool canAttack = true;
    bool isUsingAbility = false;
    public AudioClip bashAttackSound;
    public AudioClip ironMaelstormAttackSound;

    public void Start(){
        cam = Camera.main;
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        Ability basicAbillity =new Ability(AbilityType.Basic,"Bash", KeyCode.Mouse1, 5, 1);
        basicAbillity.unlockAbility();
        abilities.Add(basicAbillity);
        abilities.Add(new Ability(AbilityType.Defensive,"Shield", KeyCode.W, 0, 10));
        abilities.Add(new Ability(AbilityType.WildCard,"Iron Maelstorm", KeyCode.Q, 10, 5));
        abilities.Add(new Ability(AbilityType.Ultimate,"Charge", KeyCode.E, 100, 10));
    }
    public void Update(){


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
                if(currentAbility.type == AbilityType.Basic){
                    SelectTarget();
                    Bash();
                }else if(currentAbility.type == AbilityType.Defensive){
                    Debug.Log("Shield");
                    isUsingAbility = false;
                }else if(currentAbility.type == AbilityType.WildCard){
                    IronMaelstorm();
                }else if(currentAbility.type == AbilityType.Ultimate){
                    Charge();
                }
        }
        // if (Input.GetKeyDown(KeyCode.Space)){
        //     if(canAttack){
        //         IronMaelstorm();
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.E)){
        //    if(canAttack){
        //         Charge();
        //     }
        // }

        // if(selectTarget.WasPressedThisFrame()){
        //     if(canAttack){   
        //         SelectTarget();
        //         Bash();
        //     }
        // }
    }

    void UseAbility(Ability ability)
    {   
        currentAbility =ability;
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
    public void Bash(){
        if (selectedTarget == null)
    {
            Debug.LogWarning("No target selected!");
            return;
    }
        Vector3 directionToTarget = (selectedTarget.position - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

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
    Debug.Log("Charge ability activated! Use right-click to select the charge target.");

    StartCoroutine(ChargeSequence());
}

private IEnumerator ChargeSequence()
{
    Vector3? targetPosition = null;
    while (targetPosition == null)
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit1, 100))
            {
                targetPosition = hit1.point;
                Debug.Log("Target position set: " + targetPosition);
            }
        }
        yield return null;
    }

    Vector3 target = targetPosition.Value;
    NavMeshHit hit;
    if (!NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
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

    float chargeSpeed = 10f;

    while (Vector3.Distance(transform.position, target) > 0.1f)
    {
        if (!NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target position is no longer reachable!");
            break;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, chargeSpeed * Time.deltaTime);
        yield return null;
    }

    agent.isStopped = false;
    agent.ResetPath();

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
        yield return new WaitForSeconds(3.0f);
        canAttack = true;
    }
    private void OnEnable(){
        selectTarget.Enable();
    }
    private void OnDisable(){
        selectTarget.Disable();
    }
    private void SelectTarget(){
        Debug.Log("here");
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1f);
        RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10)){
                float hitDistance = Vector3.Distance(ray.origin, hit.point);
                Debug.Log("Hit distance: " + hitDistance);
                if(hit.transform.CompareTag("Enemy")){
                    selectedTarget = hit.transform;
                     Debug.Log("Hit enemy at distance: " + hitDistance);
                    }
            }
        isUsingAbility = false;
    }   

    public void SetSpeed(float speed){
        anim.SetFloat(movementSpeed, speed);
    }

}

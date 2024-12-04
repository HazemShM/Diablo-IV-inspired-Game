
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.XR;
public class BarbarianAnimation : MonoBehaviour
{
    [SerializeField] private string movementSpeed = "MovementSpeed";
    [SerializeField] private InputAction selectTarget = new InputAction();
    private Transform selectedTarget;
    private PlayerController playerController;
    public bool canAttack = true;
    public GameObject HitParticle;
    public AudioClip bashAttackSound;
    public AudioClip ironMaelstormAttackSound;

    public void Start(){
        playerController = GetComponent<PlayerController>();
    }
    public void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            if(canAttack){
                IronMaelstorm();
            }
        }
        if (Input.GetKeyDown(KeyCode.W)){
            if(canAttack){
                Bash();
            }
        }
        if(Input.GetKeyDown(KeyCode.E)){
           if(canAttack){
                Charge();
            }
        }

        if(selectTarget.WasPressedThisFrame()){
            SelectTarget();
        }
    }

    public void IronMaelstorm()
{   
        canAttack = false;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("IronMaelstorm");
        AudioSource ac = GetComponent<AudioSource>();
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
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Bash");
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(bashAttackSound);
        StartCoroutine(AttackCooldownBash());
    }

    public void Charge()
{
    canAttack = false;
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
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
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
        yield break; // Stop the coroutine if the target is not valid
    }
    
    target = hit.position;
    Vector3 directionToTarget = (target - transform.position).normalized;
    directionToTarget.y = 0;
    transform.rotation = Quaternion.LookRotation(directionToTarget);

    NavMeshAgent agent = playerController.GetComponent<NavMeshAgent>();
    agent.isStopped = true;

    Animator anim = GetComponent<Animator>();
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
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10)){
                if(hit.transform.CompareTag("Enemy")){
                    selectedTarget = hit.transform;
                    Debug.Log("hit enemy");
                    }
            }
    }   

    public void SetSpeed(float speed){
        Animator anim = GetComponent<Animator>();
        anim.SetFloat(movementSpeed, speed);
    }

}

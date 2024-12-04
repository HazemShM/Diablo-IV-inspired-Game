using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{   
    [SerializeField] private InputAction movement = new InputAction();
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private NavMeshAgent agent;
    private Camera cam ;
    public event Action<float> OnSpeedChanged;
    private Animator animator;
    private string movementSpeed = "MovementSpeed";

    private void Start(){
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        OnSpeedChanged += SetSpeed;
        animator = GetComponent<Animator>();
    }

    private void OnEnable(){
        movement.Enable();
    }
    private void OnDisable(){
        movement.Disable();
        OnSpeedChanged -= SetSpeed;
    }

    private void Update(){
        HandleInput();
        OnSpeedChanged?.Invoke(Mathf.Clamp01(agent.velocity.magnitude / agent.speed));
    }

    private void HandleInput(){
        if (movement.ReadValue<float>() == 1){
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100, layerMask)){
                PlayerMove(hit.point);
            }
        }
    }

    private void PlayerMove(Vector3 location)
    {   
        agent.speed = 5;
        agent.SetDestination(location);
    }
     public void SetSpeed(float speed){
        animator.SetFloat(movementSpeed, speed);
    }

}

using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{   
    [SerializeField] private InputAction movement = new InputAction();
    [SerializeField] public LayerMask layerMask = new LayerMask();
    public NavMeshAgent agent = null;
    private Camera cam = null;
    [SerializeField] private BarbarianAnimation barbarianAnimation;
    public event Action<float> OnSpeedChanged;

    private void Start(){
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        OnSpeedChanged += barbarianAnimation.SetSpeed;
    }

    private void OnEnable(){
        movement.Enable();
    }
    private void OnDisable(){
        movement.Disable();
        OnSpeedChanged -= barbarianAnimation.SetSpeed;
    }

    private void Update(){
        if (movement.ReadValue<float>() == 1){
            HandleInput();
        }
        OnSpeedChanged?.Invoke(Mathf.Clamp01(agent.velocity.magnitude / agent.speed));
    }

    public void HandleInput(){
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, layerMask)){
            PlayerMove(hit.point);
        }
    }

    private void PlayerMove(Vector3 location)
    {
        agent.SetDestination(location);
    }
}

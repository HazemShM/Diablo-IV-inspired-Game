using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputAction movement = new InputAction();

    [SerializeField]
    public LayerMask layerMask = new LayerMask();
    private NavMeshAgent agent;
    private Camera cam;
    public event Action<float> OnSpeedChanged;
    private Animator animator;
    private string movementSpeed = "MovementSpeed";
    public int currentLevel = 1;
    public int currentXP = 0;
    public int maxXP = 100; // XP required to level up
    public int maxHP = 100;
    public int currentHP = 100;
    public int abilityPoints = 0;
    bool die = false;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        OnSpeedChanged += SetSpeed;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Player took {damage} damage! Current health: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void GainXP(int xp)
    {
        currentXP += xp;
        Debug.Log($"Gained {xp} XP. Current XP: {currentXP}/{maxXP}");

        while (currentXP >= maxXP && currentLevel < 4)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        abilityPoints++;
        maxHP += 100;
        currentHP = maxHP;
        currentXP -= maxXP;
        maxXP = 100 * currentLevel;

        Debug.Log(
            $"Level up! New Level: {currentLevel}, Ability Points: {abilityPoints}, Max HP: {maxHP}, Current XP: {currentXP}"
        );
    }

    private void Die()
    {
        if (!die)
        {
            Debug.Log("Player has died!");
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("die");
        }
        die = true;
        // Handle player death (disable controls, play animation, etc.)
    }

    private void OnEnable()
    {
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        OnSpeedChanged -= SetSpeed;
    }

    private void Update()
    {
        if (movement.ReadValue<float>() == 1)
        {
            HandleInput();
        }
        OnSpeedChanged?.Invoke(Mathf.Clamp01(agent.velocity.magnitude / agent.speed));
    }

    public void HandleInput()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            PlayerMove(hit.point);
        }
    }

    private void PlayerMove(Vector3 location)
    {
        agent.speed = 5;
        agent.SetDestination(location);
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat(movementSpeed, speed);
    }
}

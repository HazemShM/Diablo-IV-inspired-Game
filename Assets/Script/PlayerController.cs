using MagicPigGames;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public ProgressBar hpbar;
    [SerializeField] public ProgressBar xpbar;
    [SerializeField] public TextMeshProUGUI hpText;
    [SerializeField] public TextMeshProUGUI xpText;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI abilityPointsText;
    [SerializeField] public TextMeshProUGUI healingPotionsText;
    [SerializeField] public TextMeshProUGUI runeFragmentsText;
    [SerializeField] public TextMeshProUGUI basicCooldownText;
    [SerializeField] public TextMeshProUGUI wildcardCooldownText;
    [SerializeField] public TextMeshProUGUI defensiveCooldownText;
    [SerializeField] public TextMeshProUGUI ultimateCooldownText;

    [SerializeField] private InputAction movement = new InputAction();
    [SerializeField] public LayerMask layerMask = new LayerMask();
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
    BarbarianAnimation barbarian;
    RogueAbilities rogue;

    private void Start()
    {   
        hpbar = GameObject.FindWithTag("hpbar")?.GetComponent<HorizontalProgressBar>();
        xpbar = GameObject.FindWithTag("xpbar")?.GetComponent<HorizontalProgressBar>();
        hpText = GameObject.FindWithTag("hpText")?.GetComponent<TextMeshProUGUI>();
        xpText = GameObject.FindWithTag("xpText")?.GetComponent<TextMeshProUGUI>();
        levelText = GameObject.FindWithTag("levelText")?.GetComponent<TextMeshProUGUI>();
        abilityPointsText = GameObject.FindWithTag("abilityPointsText")?.GetComponent<TextMeshProUGUI>();
        healingPotionsText = GameObject.FindWithTag("healingPotionsText")?.GetComponent<TextMeshProUGUI>();
        runeFragmentsText = GameObject.FindWithTag("runeFragmentsText")?.GetComponent<TextMeshProUGUI>();
        basicCooldownText = GameObject.FindWithTag("basicCooldownText")?.GetComponent<TextMeshProUGUI>();
        wildcardCooldownText = GameObject.FindWithTag("wildcardCooldownText")?.GetComponent<TextMeshProUGUI>();
        defensiveCooldownText = GameObject.FindWithTag("defensiveCooldownText")?.GetComponent<TextMeshProUGUI>();
        ultimateCooldownText = GameObject.FindWithTag("ultimateCooldownText")?.GetComponent<TextMeshProUGUI>();
        
        if (hpbar == null) Debug.LogError("HP Bar GameObject not found!");
        if (xpbar == null) Debug.LogError("XP Bar GameObject not found!");
        if (hpText == null) Debug.LogError("HP Text UI not found!");
        if (xpText == null) Debug.LogError("XP Text UI not found!");
        if (levelText == null) Debug.LogError("Level Text UI not found!");
        if (abilityPointsText == null) Debug.LogError("Ability Points Text UI not found!");
        if (healingPotionsText == null) Debug.LogError("Healing Potions Text UI not found!");
        if (runeFragmentsText == null) Debug.LogError("Rune Fragments Text UI not found!");
        if (basicCooldownText == null) Debug.LogError("Basic Cooldown Text UI not found!");
        if (wildcardCooldownText == null) Debug.LogError("Wildcard Cooldown Text UI not found!");
        if (defensiveCooldownText == null) Debug.LogError("Defensive Cooldown Text UI not found!");
        if (ultimateCooldownText == null) Debug.LogError("Ultimate Cooldown Text UI not found!");
        
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        OnSpeedChanged += SetSpeed;
        animator = GetComponent<Animator>();
        updateHP(currentHP);
        updateXP(currentXP);
        levelText.text = $"Level {currentLevel}";
        abilityPointsText.text = $"Ability Points: {abilityPoints}";
        barbarian = GetComponent<BarbarianAnimation>();
        rogue = GetComponent<RogueAbilities>();
    }

        private void Update()
    {
        if (movement.ReadValue<float>() == 1)
        {
            HandleInput();
        }
        OnSpeedChanged?.Invoke(Mathf.Clamp01(agent.velocity.magnitude / agent.speed));
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Player took {damage} damage! Current health: {currentHP}");

        updateHP(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void GainXP(int xp)
    {
        currentXP += xp;
        Debug.Log($"Gained {xp} XP. Current XP: {currentXP}/{maxXP}");

        updateXP(currentXP);

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

        updateHP(currentHP);
        updateXP(currentXP);
        // updateAbilityPoint(abilityPoints);
        // updateLevel(currentLevel);

        levelText.text = $"Level {currentLevel}";
        abilityPointsText.text = $"Ability Points: {abilityPoints}";

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

    public void updateHP(float hp)
    {
        float progress = (float)hp / maxHP;
        hpbar?.SetProgress(progress);
        hpText.text = $"{hp}";
    }

    public void updateXP(float xp)
    {
        float progress = (float)xp / maxXP;
        xpbar?.SetProgress(progress);
        xpText.text = $"{xp}";
    }
    public void updateLevel(int level){
        levelText.text = $"Level {level}";
    }
    public void updateAbilityPoint(int point){
        abilityPointsText.text = $"{point}";
    }
}

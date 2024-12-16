using MagicPigGames;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections;

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
    public int healingPotions = 0;
    public int runeFragments = 0;
    bool die = false;
    bool slowmo = false;
    public bool invincible;
    public bool isShieldActive = false; // Whether the shield is currently active
    public bool removeCooldown;
    public float normalSpeed;
    public bool wildcardUnlock;
    public bool defensiveUnlock;
    public bool ultimateUnlock;
    private AudioSource audioSource;
    public AudioClip runePickUpSound;
    public AudioClip healthPickUpSound;
    public AudioClip healSound;
    public AudioClip hitSound;
    public AudioClip deathSound;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
        healingPotionsText.text = $"Healing Potions: {healingPotions}";
        runeFragmentsText.text = $"Rune Fragments: {runeFragments}";
        normalSpeed = agent.speed;
        invincible = false;
        removeCooldown = false;
    }

    private void Update()
    {   
        if(Input.GetKeyDown(KeyCode.Z)){
            cam.GetComponent<CameraController>().RotateCamera();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Heal();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHP = Mathf.Min(currentHP + 20, maxHP); // Heal but don't exceed maxHP
            updateHP(currentHP);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentHP -= 20;
            updateHP(currentHP);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            abilityPoints++;
        }

        if (Input.GetKeyDown(KeyCode.M) && !slowmo)
        {
            Time.timeScale = 0.5f;
            slowmo = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && slowmo)
        {
            Time.timeScale = 1f;
            slowmo = false;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            invincible = !invincible;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Heal();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            removeCooldown = !removeCooldown;
        }

        if (movement.ReadValue<float>() == 1)
        {
            HandleInput();
        }
        OnSpeedChanged?.Invoke(Mathf.Clamp01(agent.velocity.magnitude / agent.speed));
        updateHP(currentHP);
        updateXP(currentXP);
        levelText.text = $"Level {currentLevel}";
        abilityPointsText.text = $"Ability Points: {abilityPoints}";
        healingPotionsText.text = $"Healing Potions: {healingPotions}";
        runeFragmentsText.text = $"Rune Fragments: {runeFragments}";
        normalSpeed = agent.speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("heal"))
        {
            if (healingPotions < 3)
            {
                healingPotions++;
                Destroy(other.gameObject);
                audioSource.PlayOneShot(healthPickUpSound);
            }

        }
        else if (other.CompareTag("rune"))
        {
            runeFragments++;
            Destroy(other.gameObject);
            audioSource.PlayOneShot(runePickUpSound);
        }
    }

    private void Heal()
    {
        if (currentHP == maxHP)
        {
            Debug.Log("Health is already full. Cannot use a healing potion.");
            return;
        }

        if (healingPotions <= 0)
        {
            Debug.Log("No healing potions available.");
            return;
        }

        int healAmount = Mathf.CeilToInt(maxHP * 0.5f);
        currentHP = Mathf.Min(currentHP + healAmount, maxHP);

        healingPotions--;

        updateHP(currentHP);
        healingPotionsText.text = $"Healing Potions: {healingPotions}";
        audioSource.PlayOneShot(healSound);
        animator.SetTrigger("heal");

        Debug.Log($"Healed for {healAmount} HP. Current HP: {currentHP}. Healing potions left: {healingPotions}");
    }
    public void TakeDamage(int damage)
    {

        if (currentHP > 0)
        {
            currentHP -= damage;
        }
        else
        {
            currentHP = 0;
        }
        Debug.Log($"Player took {damage} damage! Current health: {currentHP}");

        updateHP(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            if (hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }
            animator.SetTrigger("hit");
        }
    }

    public void GainXP(int xp)
    {
        if (currentLevel == 4)
        {
            return;
        }
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
            if (deathSound)
            {
                audioSource.PlayOneShot(deathSound);
            }
        }
        die = true;
        StartCoroutine(DieDelay());
    }
    private IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(6);
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
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500, layerMask))
        {
            PlayerMove(hit.point);
        }
    }

    private void PlayerMove(Vector3 location)
    {
        agent.speed = normalSpeed;
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

    public void ReflectDamage(int reflectedDamage)
    {
        Debug.Log($"Player took {reflectedDamage} reflected damage from Lilith's shield!");
        TakeDamage(reflectedDamage);
    }
    public void updateLevel(int level)
    {
        levelText.text = $"Level {level}";
    }
    public void updateAbilityPoint(int point)
    {
        abilityPointsText.text = $"{point}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LilithAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] GameObject minions;
    [SerializeField] GameObject Bloodspikes;
    [SerializeField] private int maxMinions = 3;
    [SerializeField] GameObject shield; // Shield
    [SerializeField] GameObject DivebombParticle; // Divebomb Particle
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] FloatingShieldScript shieldBar;
    public List<GameObject> activeMinions = new List<GameObject>();
    public int Phase;
    public float shieldHealth = 50f;
    public float bossHealth = 50f;
    public bool isShieldActive = false;
    public bool CanUseReflectiveAura = true;
    public bool isAuraActive = false;
    public bool CanUseBloodySpikes = true; // Because at the start of phase 2, we deploy the shield & deploy the aura with it 
    public bool CanUseSummonMinions = true;
    public bool CanUseDiveBomb = false; // Just like phase 2. Why? Because I can.
    public bool HitbyDiveBomb = false;
    public bool SpikesActivated = false;
    public bool EffectedByPlayerUlt = false;
    public bool TransitioningToPhase2 = false;
    private GameObject shieldInstance;
    private GameObject divebombInstance;
    private GameObject playerObject;
    private Transform player;
    private readonly float diveBombRadius = 10f;
    private readonly float rotationSpeed = 5f;
    public AudioClip BloodSpikesSound;
    public AudioClip DiveBombSound;
    public AudioClip ShieldSound;
    public AudioClip SummonSound;
    public AudioClip Phase1Death;
    public AudioClip Phase2Death;
    AudioSource ac;

    void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        shieldBar = GetComponentInChildren<FloatingShieldScript>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        shieldBar.gameObject.SetActive(false);
        CanUseBloodySpikes = true;
        Phase = 1; // Change this to test phases
        Debug.Log("Phase: " + Phase);
    }
    private void OnEnable()
    {
        GameManager.OnPlayerInstantiated += SetPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerInstantiated -= SetPlayer;
    }

    public void SetPlayer(GameObject player)
    {
        playerObject = player;
        this.player = player.transform;
    }

    void Update()
    {
        if (!player)
        {
            playerObject = GameObject.FindWithTag("Player");
            player = playerObject.transform;
        }
        healthBar.UpdateHealthBar(bossHealth, 50);
        LookAtPlayer();
        UpdateActiveMinions();
        if (Phase == 1)
        {
            if (CanUseSummonMinions && activeMinions.Count == 0)
            {
                SummonMinions();
            }
            else if (activeMinions.Count > 0 && CanUseDiveBomb && !CanUseSummonMinions)
            {
                StartCoroutine(DiveBomb());
            }
        }
        else if (Phase == 2)
        {
            shieldBar.gameObject.SetActive(true);
            shieldBar.UpdateHealthBar(shieldHealth, 50);
            if (!TransitioningToPhase2)
            {
                if (CanUseBloodySpikes && !isAuraActive) // Use Bloody Spikes if available
                {
                    BloodSpikes();
                }
                else if (CanUseReflectiveAura) // Use Reflective Aura if available
                {
                    StartCoroutine(ReflectiveAura());
                }
            }
        }
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        if (directionToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void SummonMinions()
    {
        if (activeMinions.Count == 0)
        {
            animator.SetTrigger("Summon");
            StartCoroutine(SummonMinionsWithDelay(1.2f));
            CanUseSummonMinions = false;
            StartCoroutine(SummonMinionsCountdown());
        }
        else
        {
            Debug.Log("Minions are already present, summoning skipped.");
        }
    }

    private IEnumerator SummonMinionsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (activeMinions.Count == 0)
        {
            ac.PlayOneShot(SummonSound);
            for (int i = 0; i < maxMinions; i++)
            {
                Vector3 randomPosition = new Vector3(
                    transform.position.x + Random.Range(-5f, 5f),
                    transform.position.y + 1,
                    transform.position.z + Random.Range(-5f, 5f)
                );
                GameObject minion = Instantiate(minions, randomPosition, Quaternion.identity);
                activeMinions.Add(minion);
            }
            animator.SetInteger("ActiveMinions", activeMinions.Count);
        }
    }

    public IEnumerator SummonMinionsCountdown()
    {
        CanUseDiveBomb = true;
        yield return new WaitForSeconds(15); // Summon minions charge countdown 
        CanUseSummonMinions = true;
    }

    private void UpdateActiveMinions()
    {
        activeMinions.RemoveAll(minion => minion == null);
        animator.SetInteger("ActiveMinions", activeMinions.Count);
    }

    public IEnumerator DiveBomb()
    {
        CanUseDiveBomb = false;
        animator.SetTrigger("Divebomb");
        yield return new WaitForSeconds(2f);
        Vector3 centerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 spawnPosition = centerPosition + new Vector3(0, 0.2f, 0);
        Quaternion spawnRotation = Quaternion.identity;
        divebombInstance = Instantiate(DivebombParticle, spawnPosition, spawnRotation);
        ac.PlayOneShot(DiveBombSound);
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, diveBombRadius);
        Debug.Log("DiveBomb triggered");
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player") && !HitbyDiveBomb)
            {
                HitbyDiveBomb = true;
                PlayerController playerController = collider.GetComponent<PlayerController>();
                if (playerController != null && !playerController.isShieldActive)
                {
                    playerController.TakeDamage(20);
                    Debug.Log("Player hit by DiveBomb!");
                }
            }
        }
        StartCoroutine(DiveBombCountdown());
    }

    public IEnumerator DiveBombCountdown()
    {
        yield return new WaitForSeconds(1);
        Destroy(divebombInstance);
        yield return new WaitForSeconds(10); // DiveBomb cooldown duration
        HitbyDiveBomb = false;
        CanUseDiveBomb = true;
    }

    public IEnumerator ReflectiveAura()
    {
        animator.SetTrigger("ReflectiveAura");
        Debug.Log("Aura Activated");
        CanUseReflectiveAura = false;
        isAuraActive = true;
        yield return new WaitForSeconds(0.2f);
        ac.PlayOneShot(ShieldSound);
    }

    public IEnumerator ReflectiveAuraCountdown()
    {
        yield return new WaitForSeconds(2);
        //CanUseBloodySpikes = true;
        Debug.Log("Reflective aura cooldown started.");
        yield return new WaitForSeconds(20);
        CanUseReflectiveAura = true;
        Debug.Log("Reflective aura is ready.");
    }

    public IEnumerator CheckShieldDestroyed()
    {
        isShieldActive = false;
        Destroy(shieldInstance);
        Debug.Log("Shield destroyed!");
        yield return new WaitForSeconds(10);
        ActivateShield();
    }

    public void ActivateShield()
    {
        isShieldActive = true;
        shieldBar.gameObject.SetActive(true);
        shieldHealth = 50f;
        Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
        Quaternion spawnRotation = Quaternion.identity;
        shieldInstance = Instantiate(shield, spawnPosition, spawnRotation);
        shieldInstance.transform.SetParent(transform);
    }

    public void BloodSpikes()
    {
        CanUseBloodySpikes = false;
        animator.SetTrigger("BloodSpikes");
        StartCoroutine(SpawnAndAnimateBloodSpikes(0.4f));
    }

    private IEnumerator SpawnAndAnimateBloodSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = transform.position + transform.forward * 4.0f;
        spawnPosition.y = -3;
        GameObject bloodspikes = Instantiate(Bloodspikes, spawnPosition, Quaternion.LookRotation(transform.forward));
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, 1, 1.0f));
        ac.PlayOneShot(BloodSpikesSound);
        yield return new WaitForSeconds(1.8f);
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, -4, 1.0f));
        //ac.PlayOneShot(BloodSpikesSound);
        Destroy(bloodspikes);
        StartCoroutine(BloodySpikesCountdown());
    }

    public IEnumerator BloodySpikesCountdown()
    {
        yield return new WaitForSeconds(10); // Spikes charge countdown 
        CanUseBloodySpikes = true;
    }

    private IEnumerator MoveBloodSpikes(GameObject bloodspikes, float startY, float endY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = bloodspikes.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, endY, startPosition.z);
        while (elapsedTime < duration)
        {
            bloodspikes.transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                elapsedTime / duration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bloodspikes.transform.position = endPosition;
    }

    public void TakeDamage(float damageAmount, PlayerController playerController)
    {
        if (TransitioningToPhase2)
        {
            return;
        }
        else if (isAuraActive)
        {
            isAuraActive = false; // Deactivate the aura
            //CanUseBloodySpikes = true;
            Debug.Log("Aura destroyed! Reflecting damage to the player.");
            StartCoroutine(ReflectiveAuraCountdown()); // Start cooldown
            if (playerController != null && !playerController.isShieldActive)
            {
                playerController.ReflectDamage((int)damageAmount + 15);
                Debug.Log($"Reflected {damageAmount} damage to the player.");
            }
            return;
        }
        else if (isShieldActive)
        {
            shieldHealth -= damageAmount;
            Debug.Log($"Shield health: {shieldHealth}");
            Debug.Log($"Player health: {playerController.hpbar}");
            if (shieldHealth <= 0)
            {
                shieldBar.gameObject.SetActive(false);
                float excessDamage = Mathf.Abs(shieldHealth);
                shieldHealth = 0;
                isShieldActive = false;
                Destroy(shieldInstance);
                Debug.Log("Shield destroyed!");
                if (excessDamage > 0)
                {
                    Debug.Log($"Excess damage of {excessDamage} applied to Lilith.");
                    bossHealth -= excessDamage;
                }
            }
            return;
        }
        bossHealth -= damageAmount;
        Debug.Log($"Lilith's Health: {bossHealth}");
        if (bossHealth > 0)
        {
            animator.SetTrigger("Hit");
        }
        if (bossHealth <= 0)
        {
            if (Phase == 2)
            {
                StartCoroutine(Die());
            }
            else if (Phase == 1)
            {
                StartCoroutine(TransitionToPhase2());
            }
        }
    }


    private IEnumerator Die()
    {
        animator.SetBool("Dead", true);
        ac.PlayOneShot(Phase2Death);
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        SceneManager.LoadScene(5);
    }

    private IEnumerator TransitionToPhase2()
    {
        Phase = 2;
        bossHealth = 50;
        Debug.Log("Transitioning to Phase 2...");
        animator.SetTrigger("Phase1Ended");
        TransitioningToPhase2 = true;
        ac.PlayOneShot(Phase1Death);
        yield return new WaitForSeconds(5);
        TransitioningToPhase2 = false;
        yield return new WaitForSeconds(3.8f);
        ActivateShield();
    }
}
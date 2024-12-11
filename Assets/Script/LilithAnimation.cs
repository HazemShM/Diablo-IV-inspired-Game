using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] GameObject minions;
    [SerializeField] GameObject Bloodspikes;
    [SerializeField] private int maxMinions = 3;
    [SerializeField] GameObject shield;
    public List<GameObject> activeMinions = new List<GameObject>();
    public int Phase;
    public float shieldHealth = 50f;
    public float bossHealth = 50f;
    public bool isShieldActive = false;
    public bool CanUseReflectiveAura = true;
    public bool CanUseBloodySpikes = false; // Because when we start phase 2, we need to deploy the shield (As per PDF)
    public bool CanUseSummonMinions = true;
    public bool CanUseDiveBomb = false; // Just like phase 2. Why? Because I can.
    public bool HitbyDiveBomb = false;
    private GameObject shieldInstance;
    private GameObject playerObject;
    private Transform player;
    private readonly float diveBombRadius = 10f;
    private readonly float rotationSpeed = 5f;
    public AudioClip BloodSpikesSound;
    public AudioClip DiveBombSound;
    public AudioClip ShieldSound;
    public AudioClip SummonSound;
    AudioSource ac;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        animator = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        Phase = 1; // Change this to test phases
        Debug.Log("Forced Phase to: " + Phase);
    }

    void Update()
    {
        LookAtPlayer();

        if (Phase == 1)
        {
            UpdateActiveMinions();
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
            if (CanUseBloodySpikes && !isShieldActive) // Use Bloody Spikes if available
            {
                BloodSpikes();
            }
            else if (CanUseReflectiveAura) // Use Reflective Aura if available
            {
                StartCoroutine(ReflectiveAura());
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
        yield return new WaitForSeconds(30); // Summon minions charge countdown 
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
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, diveBombRadius);
        Debug.Log("DiveBomb triggered!");
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player") && !HitbyDiveBomb)
            {
                HitbyDiveBomb = true;
                PlayerController playerController = collider.GetComponent<PlayerController>();
                if (playerController != null)
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
        yield return new WaitForSeconds(15); // DiveBomb cooldown duration
        HitbyDiveBomb = false;
        CanUseDiveBomb = true;
    }

    public IEnumerator ReflectiveAura()
    {
        CanUseReflectiveAura = false;
        animator.SetTrigger("ReflectiveAura");
        isShieldActive = true;
        shieldHealth = 50f;
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = Quaternion.identity;
        yield return new WaitForSeconds(0.2f);
        ac.PlayOneShot(ShieldSound);
        yield return new WaitForSeconds(1.5f);
        shieldInstance = Instantiate(shield, spawnPosition, spawnRotation);
        shieldInstance.transform.SetParent(transform);
        StartCoroutine(ReflectiveAuraCountdown());
    }

    public IEnumerator ReflectiveAuraCountdown()
    {
        yield return new WaitForSeconds(15); // Shield active duration
        CheckShieldDestroyed();
        CanUseBloodySpikes = true;
        yield return new WaitForSeconds(10); // Shield recharge duration
        CanUseReflectiveAura = true;
    }

    public void CheckShieldDestroyed()
    {
        isShieldActive = false;
        Destroy(shieldInstance);
        Debug.Log("Shield destroyed!");
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
        spawnPosition.y = -4;
        GameObject bloodspikes = Instantiate(Bloodspikes, spawnPosition, Quaternion.LookRotation(transform.forward));
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, 0, 1.0f));
        ac.PlayOneShot(BloodSpikesSound);
        yield return new WaitForSeconds(1.8f);
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, -4, 1.0f));
        ac.PlayOneShot(BloodSpikesSound);
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
        if (isShieldActive)
        {
            shieldHealth -= damageAmount;
            Debug.Log($"Shield health: {shieldHealth}");
            playerController.ReflectDamage(15);
            Debug.Log($"Player health: {playerController.hpbar}");
            if (shieldHealth <= 0)
            {
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
        animator.SetTrigger("Hit");
        if (bossHealth <= 0)
        {
            if (Phase == 2)
            {
                StartCoroutine(Die());
            }
            else if (Phase == 1)
            {
                TransitionToPhase2();
            }
        }
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void TransitionToPhase2()
    {
        Phase = 2;
        bossHealth = 100;
        Debug.Log("Transitioning to Phase 2...");
    }
}
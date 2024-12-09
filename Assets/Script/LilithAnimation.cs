using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    GameObject minions;

    [SerializeField]
    GameObject Bloodspikes;

    [SerializeField]
    private int maxMinions = 3;

    [SerializeField]
    public List<GameObject> activeMinions = new List<GameObject>();

    [SerializeField]
    public string Phase = "Phase1";

    [SerializeField]
    GameObject shield; // Shield (Phase 2)

    [SerializeField]
    public float shieldHealth = 50f; // Shield health (Phase 2)

    [SerializeField]
    public float bossHealth = 50f; // Boss health in both phases (Phase 1 & 2)

    public bool isShieldActive = false; // Tracks if Lilith's shield is active (Phase 2)

    AudioSource ac;
    public AudioClip BloodSpikesSound;
    public AudioClip DiveBombSound;
    public AudioClip ShieldSound;
    public AudioClip SummonSound;

    private GameObject shieldInstance;
    private float diveBombRadius = 10f;

    void Start()
    {
        animator = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        //StartCoroutine(ReflectiveAura());
        //StartCoroutine(DiveBomb());
        SummonMinions();
    }

    void Update()
    {
        if (isShieldActive && shieldHealth <= 0)
        {
            CheckShieldDestroyed();
        }
    }

    public void SummonMinions()
    {
        if (activeMinions.Count == 0)
        {
            animator.SetTrigger("Summon");
            StartCoroutine(SummonMinionsWithDelay(1.2f));
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

    public IEnumerator DiveBomb()
    {
        animator.SetTrigger("Divebomb");
        yield return new WaitForSeconds(1.5f);
        ac.PlayOneShot(DiveBombSound);

        Vector3 centerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, diveBombRadius);
        Debug.Log("DiveBomb triggered!");

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerController playerController = collider.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(20);
                    Debug.Log("Player hit by DiveBomb!");
                }
            }
        }
    }

    public IEnumerator ReflectiveAura()
    {
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

        // Update position every frame while shield is active
        while (isShieldActive)
        {
            if (shieldInstance != null)
            {
                Vector3 shieldPosition = shieldInstance.transform.position;
                shieldInstance.transform.position = new Vector3(transform.position.x, shieldPosition.y, transform.position.z);
            }
            yield return null; // Wait for the next frame
        }
    }


    public void CheckShieldDestroyed()
    {
        isShieldActive = false;
        Destroy(shieldInstance);
        Debug.Log("Shield destroyed!");
    }

    public void BloodSpikes()
    {
        animator.SetTrigger("BloodSpikes");
        StartCoroutine(SpawnAndAnimateBloodSpikes(0.4f));
    }

    private IEnumerator SpawnAndAnimateBloodSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = transform.position + transform.forward * 5.0f;
        spawnPosition.y = -4;
        GameObject bloodspikes = Instantiate(
            Bloodspikes,
            spawnPosition,
            Quaternion.LookRotation(transform.forward)
        );

        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, 0, 1.0f));
        ac.PlayOneShot(BloodSpikesSound);
        yield return new WaitForSeconds(1.8f);
        Destroy(bloodspikes);
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
            if (Phase == "Phase2")
            {
                Die();
            }
            else if (Phase == "Phase1")
            {
                TransitionToPhase2();
            }
        }
    }
    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }

    private void TransitionToPhase2()
    {
        Phase = "Phase2";
        bossHealth = 100;
        Debug.Log("Transitioning to Phase 2...");
    }
}

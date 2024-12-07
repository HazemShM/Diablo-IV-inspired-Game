using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] GameObject minions;
    [SerializeField] GameObject Bloodspikes;
    [SerializeField] private int maxMinions = 3; // Maximum number of minions
    [SerializeField] private List<GameObject> activeMinions = new List<GameObject>(); // List to track active minions
    [SerializeField] private string Phase = "Phase1";
    [SerializeField] GameObject shield; // Shield (Phase 2)
    [SerializeField] private float shieldHealth = 50f; // Shield health (Phase 2)
    [SerializeField] private float bossHealth = 50f; // Boss health in both phases (Phase 1 & 2)
    private bool isShieldActive = false; // Tracks if Lilith's shield is active (Phase 2)
    AudioSource ac;
    public AudioClip BloodSpikesSound;
    public AudioClip DiveBombSound;
    public AudioClip ShieldSound;
    public AudioClip SummonSound;

    private bool hasDestroyedMinion = false; //TEST

    void Start()
    {
        animator = GetComponent<Animator>();
        ac = GetComponent<AudioSource>();
        StartCoroutine(ReflectiveAura());
        // StartCoroutine(DiveBomb());
        // SummonMinions();
        // BloodSpikes();
    }
    void Update()
    {
        // SummonMinions();  // Shouldn't be put in Update() (As per TA Abdelrahman)
    }
    public void SummonMinions() // Needs better logic here 
    {
        // Check if the activeMinions list is empty
        if (activeMinions.Count == 0)
        {
            animator.SetTrigger("Summon");
            StartCoroutine(SummonMinionsWithDelay(1.2f)); // 1.2 seconds delay before summoning (to match with the end of the animation)
        }
        else
        {
            Debug.Log("Minions are already present, summoning skipped.");
        }
    }
    private IEnumerator SummonMinionsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // wait for the delay (1.2 seconds)
        // Summon minions after the delay
        if (activeMinions.Count == 0) // Double-check to avoid duplicates
        {
            ac.PlayOneShot(SummonSound);
            for (int i = 0; i < maxMinions; i++)
            {
                // Calculate a random position within a 5-unit range from Lilith's position
                Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y + 1, transform.position.z + Random.Range(-5f, 5f));
                GameObject minion = Instantiate(minions, randomPosition, Quaternion.identity);
                activeMinions.Add(minion);
            }
            animator.SetInteger("ActiveMinions", 3);

            Debug.Log($"{activeMinions.Count} minions summoned!");
        }
    }
    public IEnumerator DiveBomb()
    {
        animator.SetTrigger("Divebomb");
        yield return new WaitForSeconds(1.5f);
        ac.PlayOneShot(DiveBombSound);
    }
    public IEnumerator ReflectiveAura()
    {
        animator.SetTrigger("ReflectiveAura");
        isShieldActive = true;
        Vector3 spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Quaternion spawnRotation = Quaternion.identity; // No rotation
        yield return new WaitForSeconds(0.2f);
        ac.PlayOneShot(ShieldSound);
        yield return new WaitForSeconds(1.5f);
        GameObject bloodspikesInstance = Instantiate(shield, spawnPosition, spawnRotation);
    }
    public void BloodSpikes()
    {
        animator.SetTrigger("BloodSpikes");
        StartCoroutine(SpawnAndAnimateBloodSpikes(0.4f));
    }
    private IEnumerator SpawnAndAnimateBloodSpikes(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = transform.position + transform.forward * 5.0f; // 5 units in front of Lilith
        spawnPosition.y = -4;
        GameObject bloodspikes = Instantiate(Bloodspikes, spawnPosition, Quaternion.LookRotation(transform.forward));
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, -4, 0, 1.0f));
        ac.PlayOneShot(BloodSpikesSound);
        yield return new WaitForSeconds(1.8f);
        ac.PlayOneShot(BloodSpikesSound);
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveBloodSpikes(bloodspikes, 0, -4, 1.0f));
        
        Destroy(bloodspikes);
    }
    private IEnumerator MoveBloodSpikes(GameObject bloodspikes, float startY, float endY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = bloodspikes.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, endY, startPosition.z);
        while (elapsedTime < duration)
        {
            bloodspikes.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bloodspikes.transform.position = endPosition;
    }
}
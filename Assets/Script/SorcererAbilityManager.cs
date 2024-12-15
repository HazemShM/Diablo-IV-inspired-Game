using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class SorcererAbilityManager : MonoBehaviour
{
    private PlayerController playerController;
    public GameObject clonePrefab;
    public GameObject fireballPrefab;
    public GameObject infernoPrefab;
    public float cloneDuration = 5f;
    public float cloneCooldown = 10f;
    public float fireballCooldown = 1f;
    public float teleportCooldown = 10f;
    public float infernoCooldown = 15f;
    public float infernoDuration = 5f;

    private float nextCloneTime = 0f;
    private float nextFireballTime = 0f;
    private float nextTeleportTime = 0f;
    private float nextInfernoTime = 0f;
    private string activeAbility = null;

    Animator animator;

    AudioSource audioSource;
    public AudioClip infernoSound;
    public AudioClip fireballSound;
    public AudioClip cloneSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextCloneTime)
        {
            nextCloneTime = Time.time + cloneCooldown;
            activeAbility = "Clone";
            PlaySound(cloneSound);
            StartCoroutine(CastClone());
        }
        else if (Input.GetKeyDown(KeyCode.W) && Time.time >= nextTeleportTime)
        {
            nextTeleportTime = Time.time + teleportCooldown;
            activeAbility = "Teleport";
            StartCoroutine(CastTeleport());
        }
        else if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextInfernoTime)
        {
            nextInfernoTime = Time.time + infernoCooldown;
            activeAbility = "Inferno";
            PlaySound(infernoSound);
            StartCoroutine(CastInferno());
        }
        else if (Input.GetMouseButtonDown(1) && activeAbility == null && Time.time >= nextFireballTime)
        {
            nextFireballTime = Time.time + fireballCooldown;
            PlaySound(fireballSound);
            StartCoroutine(CastFireball());
        }
    }

    private IEnumerator WaitForMouseClick(System.Action<Vector3> callback)
    {
        while (!Input.GetMouseButtonDown(1))
        {
            yield return null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, playerController.layerMask))
        {
            callback(hit.point);
        }
        else
        {
            callback(Vector3.zero);
        }
    }

    private IEnumerator CastTeleport()
    {
        //animator.SetTrigger("CastTeleport");
        activeAbility = "Teleport";
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        if (!clickCompleted || targetPos == Vector3.zero)
        {
            yield break;
        }

        transform.position = targetPos;
        GetComponent<NavMeshAgent>().SetDestination(targetPos);
        Time.timeScale = 1f;
        activeAbility = null;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator CastClone()
    {
        //animator.SetTrigger("CastClone");
        activeAbility = "Clone";
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        if (clickCompleted && targetPos != Vector3.zero)
        {
            GameObject clone = Instantiate(clonePrefab, targetPos, Quaternion.identity);
            clone.tag = "Clone";
            Time.timeScale = 1f;
            clone.GetComponent<PlayerController>().enabled = false;
            clone.GetComponent<SorcererAbilityManager>().enabled = false;
            NotifyEnemiesAboutClone(clone);

            yield return new WaitForSeconds(cloneDuration);

            Destroy(clone);
            NotifyEnemiesTargetPlayer();
        }

        Time.timeScale = 1f;
        activeAbility = null;
        yield return new WaitForSeconds(0.5f);
    }

    private void NotifyEnemiesAboutClone(GameObject clone)
    {
        Minion[] minions = FindObjectsOfType<Minion>();
        foreach (Minion minion in minions)
        {
            minion.SetTarget(clone.transform);
        }
    }

    private void NotifyEnemiesTargetPlayer()
    {
        Minion[] minions = FindObjectsOfType<Minion>();
        foreach (Minion minion in minions)
        {
            minion.SetTarget(this.transform);
        }
    }


    private IEnumerator CastFireball()
    {
        activeAbility = "Fireball";
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetPos = hit.point;
                    clickCompleted = true;
                    break;
                }
            }
        }));

        if (!clickCompleted || targetPos == Vector3.zero)
        {
            Time.timeScale = 1f;
            activeAbility = null;
            Debug.Log("Fireball ability canceled: No valid target.");
            yield break;
        }

        Vector3 playerPos = transform.position;
        playerPos.y += 3f;
        targetPos = new Vector3(targetPos.x, playerPos.y, targetPos.z);

        animator.SetTrigger("Fireball");
        GameObject fireball = Instantiate(fireballPrefab, playerPos, Quaternion.identity);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody attached to fireballPrefab!");
            activeAbility = null;
            yield break;
        }

        Vector3 direction = (targetPos - playerPos).normalized;
        rb.velocity = direction * 10f;
        activeAbility = null;
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator CastInferno()
    {
        activeAbility = "Inferno";
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;
        animator.SetTrigger("CastSpell");

        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        if (!clickCompleted || targetPos == Vector3.zero)
        {
            yield break;
        }
        targetPos.y = 0;

        GameObject inferno = Instantiate(infernoPrefab, targetPos, Quaternion.identity);
        Destroy(inferno, infernoDuration);
        Time.timeScale = 1f;
        activeAbility = null;
        yield return new WaitForSeconds(0.5f);
    }
}

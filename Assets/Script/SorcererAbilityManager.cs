using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class SorcererAbilityManager : MonoBehaviour
{
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

    Animator animator;

    private void Start(){
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextCloneTime)
        {
            nextCloneTime = Time.time + cloneCooldown;
            StartCoroutine(CastClone());
        }
        else if (Input.GetKeyDown(KeyCode.W) && Time.time >= nextTeleportTime)
        {
            nextTeleportTime = Time.time + teleportCooldown;
            StartCoroutine(CastTeleport());
        }
        else if (Input.GetKeyDown(KeyCode.R) && Time.time >= nextFireballTime)
        {
            nextFireballTime = Time.time + fireballCooldown;
            StartCoroutine(CastFireball());
        }
        else if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextInfernoTime)
        {
            nextInfernoTime = Time.time + infernoCooldown;
            StartCoroutine(CastInferno());
        }
    }

    private IEnumerator WaitForMouseClick(System.Action<Vector3> callback)
    {
        while (!Input.GetMouseButtonDown(1)) // Wait for right mouse click
        {
            yield return null; // Wait for the next frame
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            callback(hit.point); // Call the callback with the clicked position
        }
        else
        {
            callback(Vector3.zero); // Return zero if no valid position
        }
    }

    private IEnumerator CastTeleport()
    {
        //animator.SetTrigger("CastTeleport");

        // Wait for the player to right-click and get the target position
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        // Start waiting for the mouse click
        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        // If no valid position, cancel ability
        if (!clickCompleted || targetPos == Vector3.zero)
        {
            yield break;
        }

        // Teleport the player to the clicked position
        transform.position = targetPos;
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f); // Allow the animation to finish
    }

    private IEnumerator CastClone()
    {
        //animator.SetTrigger("CastClone");

        // Wait for the player to right-click and get the position
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        // Start waiting for the mouse click
        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        // If no valid position, cancel ability
        if (!clickCompleted || targetPos == Vector3.zero)
        {
            yield break;
        }

        GameObject clone = Instantiate(clonePrefab, targetPos, Quaternion.identity);
        Destroy(clone, cloneDuration);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f); // Allow animation to finish
    }

    private IEnumerator CastFireball()
    {
        // Wait for the player to right-click and get the position
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;

        // Start waiting for the mouse click
        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Check if the clicked object has the "Enemy" tag
                if (hitInfo.collider.CompareTag("Enemy"))
                {
                    targetPos = hitInfo.point;
                    clickCompleted = true;
                }
            }
        }));
        animator.SetTrigger("Fireball");
        // If no valid position or no enemy clicked, cancel ability
        if (!clickCompleted || targetPos == Vector3.zero)
        {
            Time.timeScale = 1f; // Ensure the game time resumes
            yield break;
        }

        Vector3 playerPos = new Vector3(transform.position.x, 3f, transform.position.z);
        GameObject fireball = Instantiate(fireballPrefab, playerPos, Quaternion.identity);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        targetPos.y = 3f; // Adjust the height of the target position
        Vector3 direction = (targetPos - playerPos);
        rb.velocity = direction.normalized * 10f; // Adjust speed as needed
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);
    }


    private IEnumerator CastInferno()
    {
        // Wait for the player to right-click and get the position
        Vector3 targetPos = Vector3.zero;
        bool clickCompleted = false;
        Time.timeScale = 0.1f;
        animator.SetTrigger("CastSpell");

        // Start waiting for the mouse click
        yield return StartCoroutine(WaitForMouseClick(pos =>
        {
            targetPos = pos;
            clickCompleted = true;
        }));

        // If no valid position, cancel ability
        if (!clickCompleted || targetPos == Vector3.zero)
        {
            yield break;
        }

        GameObject inferno = Instantiate(infernoPrefab, targetPos, Quaternion.identity);
        Destroy(inferno, infernoDuration);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);
    }
}

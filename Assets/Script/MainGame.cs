using System.Collections;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] GameObject barbarianPrefab;
    [SerializeField] GameObject sorcererPrefab;
    [SerializeField] GameObject roguePrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject bossPortal;
    PlayerController playerController;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        bool playerFound = false;
        bool wildcardUnlock = false;
        bool defensiveUnlock = false;
        bool ultimateUnlock = false;
        int currentLevel = 1;
        int currentXP = 0;
        int maxXP = 100; 
        int maxHP = 100;
        int currentHP = 100;
        int abilityPoints = 0;
        int healingPotions = 0;
        int runeFragments = 0;
        if(player){
            wildcardUnlock = player.GetComponent<PlayerController>().wildcardUnlock;
            defensiveUnlock = player.GetComponent<PlayerController>().defensiveUnlock;
            ultimateUnlock = player.GetComponent<PlayerController>().ultimateUnlock;
            currentLevel = player.GetComponent<PlayerController>().currentLevel;
            currentXP =player.GetComponent<PlayerController>().currentXP;
            maxXP = player.GetComponent<PlayerController>().maxXP;
            maxHP = player.GetComponent<PlayerController>().maxHP;
            currentHP = player.GetComponent<PlayerController>().currentHP;
            abilityPoints = player.GetComponent<PlayerController>().abilityPoints;
            healingPotions = player.GetComponent<PlayerController>().healingPotions;
            runeFragments = player.GetComponent<PlayerController>().runeFragments;
            PlayerPersistence.instance = null;
            Destroy(player);
            player = null;
            playerFound = true;
        }
        switch (GameManager.SelectedCharacter)
        {   
            case "Barbarian":
                player = Instantiate(barbarianPrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                playerController = player.GetComponent<PlayerController>();
                break;
            case "Sorcerer":

                player = Instantiate(sorcererPrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                playerController = player.GetComponent<PlayerController>();
                break;
            case "Rogue":
                player = Instantiate(roguePrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                playerController = player.GetComponent<PlayerController>();
                break;
            default:
                Debug.LogError("No character selected!");
                break;
        }
        if(playerFound){
            player.GetComponent<PlayerController>().currentLevel = currentLevel;
            player.GetComponent<PlayerController>().currentXP = currentXP;
            player.GetComponent<PlayerController>().maxXP = maxXP;
            player.GetComponent<PlayerController>().maxHP = maxHP;
            player.GetComponent<PlayerController>().currentHP = currentHP;
            player.GetComponent<PlayerController>().abilityPoints = abilityPoints;
            player.GetComponent<PlayerController>().healingPotions = healingPotions;
            player.GetComponent<PlayerController>().runeFragments = runeFragments;
            player.GetComponent<PlayerController>().wildcardUnlock = wildcardUnlock;
            player.GetComponent<PlayerController>().defensiveUnlock = defensiveUnlock;
            player.GetComponent<PlayerController>().ultimateUnlock = ultimateUnlock;
            playerFound = false;
            StartCoroutine(UnlockAbilityUI(wildcardUnlock,defensiveUnlock ,ultimateUnlock,0.5f));
        }else{
            UnlockMaxLevels();
        }          
    }
    public void UnlockMaxLevels(){
        if(!GameManager.increaseLevels){
            return;
        }
        GameObject player = GameObject.FindWithTag("Player");
        bool wildcardUnlock = true;
        bool defensiveUnlock = true;
        bool ultimateUnlock = true;
        int currentLevel = 4;
        int currentXP = 0;
        int maxXP = 600; 
        int maxHP = 400;
        int currentHP = 400;
        int abilityPoints = 0;
        int healingPotions = 0;
        int runeFragments = 0;
        player.GetComponent<PlayerController>().currentLevel = currentLevel;
        player.GetComponent<PlayerController>().currentXP = currentXP;
        player.GetComponent<PlayerController>().maxXP = maxXP;
        player.GetComponent<PlayerController>().maxHP = maxHP;
        player.GetComponent<PlayerController>().currentHP = currentHP;
        player.GetComponent<PlayerController>().abilityPoints = abilityPoints;
        player.GetComponent<PlayerController>().healingPotions = healingPotions;
        player.GetComponent<PlayerController>().runeFragments = runeFragments;
        player.GetComponent<PlayerController>().wildcardUnlock = wildcardUnlock;
        player.GetComponent<PlayerController>().defensiveUnlock = defensiveUnlock;
        player.GetComponent<PlayerController>().ultimateUnlock = ultimateUnlock;
        StartCoroutine(UnlockAbilityUI(wildcardUnlock,defensiveUnlock ,ultimateUnlock,0));

    }
    private IEnumerator UnlockAbilityUI(bool wildcard, bool defensive , bool ultimate,float delay)
    {
        yield return new WaitForSeconds(delay);
        UnlockAbilities[] unlockAbilities = FindObjectsOfType<UnlockAbilities>();

        if (unlockAbilities.Length > 0)
        {
            GameObject firstObject = unlockAbilities[0].gameObject;
            unlockAbilities[0].unlockSpecificAbilities(wildcard,defensive ,ultimate);
        }
        else
        {
            Debug.Log("No objects found with MySpecificScript.");
        }
    }

    void Update(){
        if(playerController && playerController.runeFragments >= 3){
            if (bossPortal != null && !bossPortal.activeSelf)
            {
                bossPortal.SetActive(true);
            }
        }
    }
}

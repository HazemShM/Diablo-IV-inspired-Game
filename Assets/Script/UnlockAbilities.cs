using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAbilities : MonoBehaviour
{
    PlayerController playerController ;
    public void Start(){
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void WildcardAbility(){
        if(playerController.abilityPoints > 0){
            playerController.abilityPoints--;
            playerController.wildcardUnlock = true;
            Debug.Log("Wildcard Ability Unlocked");
        }
        
    }
    public void UltimateAbility(){
        if(playerController.abilityPoints > 0){
            playerController.abilityPoints--;
            playerController.ultimateUnlock = true;
            Debug.Log("Ultimate Ability Unlocked");
        }
        
    }
    public void DefensiveAbility(){
        if(playerController.abilityPoints > 0){
            playerController.abilityPoints--;
            playerController.defensiveUnlock = true;
            Debug.Log("Defensive Ability Unlocked");
        }
    }
}

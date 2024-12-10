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
        Debug.Log("Wildcard Ability Unlocked");
        
    }
    public void UltimateAbility(){
        Debug.Log("Ultimate Ability Unlocked");
        
    }
    public void DefensiveAbility(){
        Debug.Log("Defensive Ability Unlocked");
    }
}

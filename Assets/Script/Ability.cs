using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{   
    
    public AbilityType type;
    public string name;      
    public KeyCode activationKey;
    public float cooldownTime;
    public float cooldownTimer;
    public bool isOnCooldown;
    public float damage;
    public bool unlocked;
    
    public Ability(AbilityType type, string name, KeyCode activationKey,float damage, float cooldownTime)
    {   
        this.type = type;
        this.name = name;
        this.activationKey = activationKey;
        this.cooldownTime = cooldownTime;
        this.cooldownTimer = 0f;
        this.isOnCooldown = false;
        this.damage = damage;
        this.unlocked = false;
    }
    public void unlockAbility(){
        this.unlocked = true;
    }
}

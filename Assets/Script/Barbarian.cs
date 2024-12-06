using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Barbarian : MonoBehaviour
{
    [SerializeField] BarbarianAnimation agentAnimation;
    [SerializeField] private PlayerController movement;
     
    private void Start(){
        movement.OnSpeedChanged += agentAnimation.SetSpeed;
        agentAnimation.SetSpeed(0);
    }
}

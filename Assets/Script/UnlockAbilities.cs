using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnlockAbilities : MonoBehaviour
{
    PlayerController playerController;
    public Button wildcardButton;
    public Button ultimateButton;
    public Button defensiveButton;
    public void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        SetButtonAlpha(wildcardButton, 125);
        SetButtonAlpha(ultimateButton, 125);
        SetButtonAlpha(defensiveButton, 125);
    }
    public void Update(){
        if (Input.GetKeyDown(KeyCode.U))
        {
            playerController.wildcardUnlock = true;
            playerController.defensiveUnlock = true;
            playerController.ultimateUnlock = true;
            SetButtonUnlocked(wildcardButton);
            SetButtonUnlocked(ultimateButton);
            SetButtonUnlocked(defensiveButton);
        }
    }
    public void WildcardAbility()
    {
        Debug.Log("Wildcard button clicked");
        if (playerController.abilityPoints > 0)
        {
            playerController.abilityPoints--;
            playerController.wildcardUnlock = true;
            Debug.Log("Wildcard Ability Unlocked");
            SetButtonUnlocked(wildcardButton);
        }

    }
    public void UltimateAbility()
    {
        if (playerController.abilityPoints > 0)
        {
            playerController.abilityPoints--;
            playerController.ultimateUnlock = true;
            Debug.Log("Ultimate Ability Unlocked");
            SetButtonUnlocked(ultimateButton);
        }

    }
    public void DefensiveAbility()
    {
        if (playerController.abilityPoints > 0)
        {
            playerController.abilityPoints--;
            playerController.defensiveUnlock = true;
            Debug.Log("Defensive Ability Unlocked");
            SetButtonUnlocked(defensiveButton);
        }
    }
    private void SetButtonAlpha(Button button, byte alpha)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha / 255f;
            buttonImage.color = color;
        }
    }

    private void SetButtonUnlocked(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 1f;
            buttonImage.color = color;
            button.interactable = false;
        }
    }
}

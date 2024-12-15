using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharactersSelect : MonoBehaviour
{
    [SerializeField] TMP_Text description_text;
    private string barbarian;
    private string sorcerer;
    private string rogue;

    private string chosen;

    // Start is called before the first frame update
    void Start()
    {
        barbarian = "The Barbarian is known for wielding an axe. Here are the four key abilities:\r\n" +
            "Bash: A basic ability that swings the axe to attack an enemy, dealing 5 damage with a 1-second cooldown.\r\n" +
            "Shield: A defensive ability that instantly protects the Barbarian from all incoming attacks for 3 seconds, with a 10-second cooldown.\r\n" +
            "Iron Maelstorm: A wild card ability that instantly whirls the axe in a circular range, dealing 10 damage to surrounding enemies, with a 5-second cooldown.\r\n" +
            "Charge: An ultimate ability where the Barbarian rushes to a selected position, instantly killing enemies or dealing 20 damage to a Boss, with a 10-second cooldown.";
        sorcerer = "The Sorcerer uses fire magic to attack. Here are the main abilities:\r\n" +
            "Fireball: A basic ability that shoots a ball of fire at a selected enemy, dealing 5 damage, with a cooldown of 1 second.\r\n" +
            "Teleport: A defensive ability that allows the Sorcerer to instantly move to a selected walkable position to evade enemies, with a cooldown of 10 seconds.\r\n" +
            "Clone: A wild card ability that creates an immobile decoy of the Sorcerer for 5 seconds, which explodes to deal 10 damage, with a cooldown of 10 seconds.\r\n" +
            "Inferno: An ultimate ability that creates a ring of flames at a selected position, dealing 10 initial damage and 2 damage per second to enemies inside for 5 seconds, with a cooldown of 15 seconds.";
        rogue = "The Rogue is skilled in mobility and precise attacks. Here are the Rogue's four key abilities:\r\n" +
            "Arrow: A basic ability that shoots an arrow at a selected enemy, dealing 5 damage, with a 1-second cooldown.\r\n" +
            "Smoke Bomb: A defensive ability that stuns enemies in a medium range for 5 seconds, making them unable to move or attack, with a 10-second cooldown.\r\n" +
            "Dash: A wild card ability that allows the Rogue to quickly move to a selected walkable position, doubling their speed, with a 5-second cooldown.\r\n" +
            "Shower of Arrows: An ultimate ability that creates a medium ring of arrows, dealing 10 damage to enemies and slowing them to a quarter of their speed for 3 seconds, with a cooldown of 10 seconds.";

        onBarbarianSelect();
    }

    public void onBarbarianSelect()
    {
        description_text.SetText(barbarian);
        GameManager.SelectedCharacter = "Barbarian"; // Save selection
    }

    public void onSorcererSelect()
    {
        description_text.SetText(sorcerer);
        GameManager.SelectedCharacter = "Sorcerer"; // Save selection
    }

    public void onRogueSelect()
    {
        description_text.SetText(rogue);
        GameManager.SelectedCharacter = "Rogue"; // Save selection
    }
}

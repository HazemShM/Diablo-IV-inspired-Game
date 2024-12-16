using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsSelect : MonoBehaviour
{
    [SerializeField] TMP_Text description_text;
    [SerializeField] TMP_Text title_text;

    private bool teamcreds;

    private string teamnames;
    private string assets;

    // Start is called before the first frame update
    void Start()
    {
        teamcreds = true;

        teamnames = "Name:                                                                  Role:\n" +
            "Ziad Yasser Fawzy Atteya Gafar                          Boss\n" +
            "Marwan Khalil Ahmed Khalil                                 UI Design\n" +
            "Maryam Mohamed Alsayed Ahmed                      UI Design\n" +
            "Seifeldin Refaat Hussein Ahmed                          Sorcerer\n" +
            "Yahia Mohamed Ahmed Ali Eid                             Barbarian\n" +
            "Hazem Sherif Mahmoud Abdo Mohamed             Rogue\n" +
            "Roaa Mahmoud                                   Minion\n" +
            "Ahmed Sherif Hamed Mohamed Hassan              Demon";
        assets = "https://assetstore.unity.com/packages/3d/environments/landscapes/terrain-sample-asset-pack-145808\n" +
            "https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325\n" +
            "https://assetstore.unity.com/packages/3d/props/potions-coin-and-box-of-pandora-pack-71778\n" +
            "https://assetstore.unity.com/packages/3d/environments/historic/lowpoly-medieval-buildings-58289\n" +
            "https://assetstore.unity.com/packages/2d/gui/icons/2d-skills-icon-set-handpainted-210622\n" +
            "https://assetstore.unity.com/packages/3d/arrow-shield-92886\n" +
            "https://assetstore.unity.com/packages/3d/props/free-healing-item-including-c-script-275780";
    }

    // Update is called once per frame
    void Update()
    {
        if (teamcreds)
        {
            description_text.SetText(teamnames);
            title_text.SetText("Assets");
        }
        else
        {
            description_text.SetText(assets);
            title_text.SetText("Team");
        }
    }

    public void OnToggleButton()
    {
        teamcreds = !teamcreds;
    }
}

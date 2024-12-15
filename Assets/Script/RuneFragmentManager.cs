using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RuneFragmentManager : MonoBehaviour
{
    campMinions campMinions;
    demonControll demonControll;
    public GameObject runeFragment;
    // Start is called before the first frame update
    void Start()
    {
        campMinions = GetComponent<campMinions>();
        demonControll = GetComponent<demonControll>();
    }

    // Update is called once per frame
    void Update()
    {
        if (campMinions.enemies.All(e => e == null) && demonControll.enemies.All(e => e == null))
        {
            if (runeFragment != null)
            {
                runeFragment.SetActive(true);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class demonControll : MonoBehaviour
{
    public Navigation[] enemies;
    private int AgEnemyIndex;

    void Start()
    {
        AgEnemyIndex = Random.Range(0, 2);
    }

    void Update()
    {
        int enemiesDeadCount = 0;
        foreach (Navigation enemy in enemies)
        {
            if (enemy.isDead)
            {
                enemiesDeadCount++;
            }
        }
        if (enemiesDeadCount == 3)
        {
            return;
        }

        if (enemies[AgEnemyIndex].isDead)
        {
            AgEnemyIndex = Random.Range(0, 2);
        }
        if (!enemies[AgEnemyIndex].isAggressive)
        {
            enemies[AgEnemyIndex].isAggressive = true;
        }
    }

}

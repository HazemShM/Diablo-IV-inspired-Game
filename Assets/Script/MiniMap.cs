using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    Transform player;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Maintain the minimap camera's height
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}

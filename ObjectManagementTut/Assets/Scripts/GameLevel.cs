using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private SpawnZone spawnZone;

    void Start () 
    {
        Game.gameInstance.SpawnZoneOfLevel = spawnZone;
    }
}

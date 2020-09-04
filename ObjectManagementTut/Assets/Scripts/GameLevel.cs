using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private SpawnZone spawnZone;

    public void Start () 
    {
        Game.GameInstance.SpawnZoneOfLevel = spawnZone;
    }
}

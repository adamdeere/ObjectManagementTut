﻿using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField] private SpawnZone[] spawnZones;
    [SerializeField] private bool sequential;
    [SerializeField] private bool overrideConfig;
    private int _nextSequentialIndex;
    public override Vector3 SpawnPoint 
    {
        get 
        {
            int index;
            if (sequential) 
            {
                index = _nextSequentialIndex++;
                if (_nextSequentialIndex >= spawnZones.Length) 
                {
                    _nextSequentialIndex = 0;
                }
            }
            else 
            {
                index = Random.Range(0, spawnZones.Length);
            }
            return spawnZones[index].SpawnPoint;
        }
    }
    
    public override void SpawnShape () 
    {
        if (overrideConfig) 
             base.SpawnShape();
        
        else 
        {
            int index;
            if (sequential) 
            {
                index = _nextSequentialIndex++;
                if (_nextSequentialIndex >= spawnZones.Length) 
                {
                    _nextSequentialIndex = 0;
                }
            }
            else 
            { 
                index = Random.Range(0, spawnZones.Length);
            }
             spawnZones[index].SpawnShape();
        }
    }
    public override void Save (GameDataWriter writer) 
    {
        writer.Write(_nextSequentialIndex);
    }

    public override void Load (GameDataReader reader) 
    {
        _nextSequentialIndex = reader.ReadInt();
    }
}

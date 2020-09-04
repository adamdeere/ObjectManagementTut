using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class SpawnZone : MonoBehaviour
{
    
    public abstract Vector3 SpawnPoint { get; }

    
}

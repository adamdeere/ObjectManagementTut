using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private bool surfaceOnly;
    public Vector3 SpawnPoint => transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}

using UnityEngine;

namespace Spawn_Zones
{
    public class SphereSpawn : SpawnZone
    {
        [SerializeField] private bool surfaceOnly;
   

        public override Vector3 SpawnPoint => transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
        }
    }
}

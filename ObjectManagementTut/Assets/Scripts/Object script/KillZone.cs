using UnityEngine;

namespace Object_script
{
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private float dyingDuration;
        
        void OnDrawGizmos () 
        {
            Gizmos.color = new Color(0.28f, 1f, 0.19f);
            Gizmos.matrix = transform.localToWorldMatrix;
            var c = GetComponent<Collider>();
            var b = c as BoxCollider;
            if (b != null) 
            {
                Gizmos.DrawWireCube(b.center, b.size);
                return;
            }
            var s = c as SphereCollider;
            if (s == null) return;
            Gizmos.DrawWireSphere(s.center, s.radius);
            return;
        }
        
        public void OnTriggerEnter (Collider other) 
        {
            var shapeComponent = other.GetComponent<IKillable>();
            var shape = other.GetComponent<Shape>().IsMarkedAsDying;
            
            if (dyingDuration <= 0f) 
                shapeComponent?.Kill();
            
            else if (!shape)
                shapeComponent?.AddInterface(dyingDuration);
            
        }
    }
}

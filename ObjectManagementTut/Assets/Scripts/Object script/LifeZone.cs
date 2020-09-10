using Shape_behaviours;
using UnityEngine;

namespace Object_script
{
    public class LifeZone : MonoBehaviour
    {
        void OnDrawGizmos () 
        {
            Gizmos.color = new Color(1f, 0.39f, 0.55f);
            Gizmos.matrix = transform.localToWorldMatrix;
            var c = GetComponent<Collider>();
            var b = c as BoxCollider;
            if (b != null) 
            {
                Gizmos.DrawWireCube(b.center, b.size);
                return;
            }
            var s = c as SphereCollider;
            if (s != null) 
            {
                Gizmos.DrawWireSphere(s.center, s.radius);
                return;
            }
        }
        
        [SerializeField] private float dyingDuration;
        void OnTriggerExit (Collider other)
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

using Shape_behaviours;
using UnityEngine;

namespace Object_script
{
    public class LifeZone : MonoBehaviour
    {
        [SerializeField] private float dyingDuration;
        void OnTriggerExit (Collider other)
        {
            var shapeComponent = other.GetComponent<IKillable>();
            var shape = other.GetComponent<Shape>();
            
            if (dyingDuration <= 0f) 
                shapeComponent?.Kill();
            
            else if (!shape.IsMarkedAsDying)
                shapeComponent?.AddInterface(dyingDuration);
            
        }
    }
}

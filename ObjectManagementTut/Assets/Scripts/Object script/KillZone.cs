using UnityEngine;

namespace Object_script
{
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private float dyingDuration;
        public void OnTriggerEnter (Collider other) 
        {
            IKillable shapeComponent = other.GetComponent<IKillable>();
            Shape shape = other.GetComponent<Shape>();
            if (dyingDuration <= 0f) 
            {
                shapeComponent?.Kill();
            }
            else if (!shape.IsMarkedAsDying)
            {
                shapeComponent?.AddInterface(dyingDuration);
            }
        }
    }
}

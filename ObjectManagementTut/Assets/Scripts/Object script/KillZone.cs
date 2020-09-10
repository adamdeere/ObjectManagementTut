using UnityEngine;

namespace Object_script
{
    public class KillZone : MonoBehaviour
    {
        public void OnTriggerEnter (Collider other) 
        {
            var shape = other.GetComponent<IKillable>();
            shape?.Kill();
        }
    }
}

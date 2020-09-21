using Save_scripts;
using UnityEngine;

namespace Object_script
{
    public class RotatingObject : PersistableObject
    {
        [SerializeField] private Vector3 angularVelocity;

        public void FixedUpdate () 
        {
            transform.Rotate(angularVelocity * Time.deltaTime);
        }
    }
}

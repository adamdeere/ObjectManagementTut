using UnityEngine;

public class RotatingObject : PersistableObject
{
    [SerializeField] private Vector3 angularVelocity;

    public void FixedUpdate () 
    {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }
}

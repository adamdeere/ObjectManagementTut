using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct FloatRange 
{

    public float min, max;
    public float RandomValueInRange => Random.Range(min, max);
}

[System.Serializable]
public struct SpawnConfiguration 
{

    public enum MovementDirection 
    {
        Forward,
        Upward,
        Outward,
        Random
    }

    public MovementDirection movementDirection;

    public FloatRange speed;
    
    public FloatRange angularSpeed;

    public FloatRange scale;
}
public abstract class SpawnZone : PersistableObject
{
    
    public abstract Vector3 SpawnPoint { get; }
    [SerializeField] private SpawnConfiguration spawnConfig;
   
    public virtual void ConfigureSpawn (Shape shape) 
    {
        var t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        shape.SetColour(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f, valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f));
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        
        Vector3 direction;
        switch (spawnConfig.movementDirection) {
            case SpawnConfiguration.MovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.MovementDirection.Outward:
                direction = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = transform.forward;
                break;
        }
        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange;
    }
}

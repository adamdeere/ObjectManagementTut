using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct ColorRangeHSV 
{
     [FloatRangeSlider(0f, 1f)]
    public FloatRange hue, saturation, value;

    public Color RandomInRange => Random.ColorHSV(hue.min, hue.max, saturation.min, saturation.max, value.min, value.max, 1f, 1f);
}


[System.Serializable]
public struct FloatRange 
{

    public float min, max;
    public float RandomValueInRange => Random.Range(min, max);
}

[System.Serializable]
public struct SpawnConfiguration 
{
    public bool uniformColor;
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
  
    public ColorRangeHSV color;
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
        if (spawnConfig.uniformColor) 
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else 
        {
            for (int i = 0; i < shape.ColorCount; i++) 
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
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

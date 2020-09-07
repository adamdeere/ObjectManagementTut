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
    
    public ShapeFactory[] factories;
    
    public MovementDirection movementDirection;

    public FloatRange speed;
    
    public FloatRange angularSpeed;

    public FloatRange scale;
  
    public ColorRangeHSV color;
    
    public MovementDirection oscillationDirection;

    public FloatRange oscillationAmplitude;

    public FloatRange oscillationFrequency;
}
public abstract class SpawnZone : PersistableObject
{
    
    public abstract Vector3 SpawnPoint { get; }
    [SerializeField] private SpawnConfiguration spawnConfig;
   
    public virtual Shape SpawnShape () 
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
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
            var i = 0;
            for (; i < shape.ColorCount; i++) 
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f)
        {
            var rotation = shape.AddBehavior<RotationBehaviour>(); 
            rotation.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        }
       
        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0f)
        {
            var movement = shape.AddBehavior<MovementBehaviour>();
            movement.Velocity = GetDirectionVector(spawnConfig.movementDirection, t) * speed;
          
        }
        SetupOscillation(shape);
        return shape;
    }
    
    private Vector3 GetDirectionVector (SpawnConfiguration.MovementDirection direction, Transform t) 
    {
        switch (direction) 
        {
            case SpawnConfiguration.MovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.MovementDirection.Outward:
                return (t.localPosition - transform.position).normalized;
            case SpawnConfiguration.MovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return transform.forward;
        }
    }
    
    private void SetupOscillation (Shape shape) 
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0f || frequency == 0f) 
        {
            return;
        }
        var oscillation = shape.AddBehavior<OcscillationShapeBehaviour>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }
}

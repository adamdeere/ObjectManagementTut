using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LifecycleShapeBehavior : ShapeBehaviour
{
    private Vector3 _originalScale;
    private float _adultDuration, _dyingDuration, _dyingAge;
    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Growing;

    public void Initialize (Shape shape, float growingDuration, float adultDuration, float dyingDuration) 
    {
        _adultDuration = adultDuration;
        _dyingDuration = dyingDuration;
        _dyingAge = growingDuration + adultDuration;
		
        if (growingDuration > 0f) {
            shape.AddBehavior<GrowingShapeBehavior>().Initialize(
                shape, growingDuration
            );
        }
    }
    
    public override bool GameUpdate (Shape shape) 
    {
        float dyingDuration = shape.Age - _dyingAge;
        if (dyingDuration < _dyingDuration) 
        {
            float s = 1f - dyingDuration / _dyingDuration;
            s = (3f - 2f * s) * s * s;
            shape.transform.localScale = s * _originalScale;
            return true;
        }
        shape.Die();
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_originalScale);
        writer.Write(_dyingDuration);
        writer.Write(_dyingAge);
    }

    public override void Load(GameDataReader reader)
    {
        _originalScale = reader.ReadVector();
        _dyingDuration = reader.ReadFloat();
        _dyingAge = reader.ReadFloat();
    }

    public override void Recycle () 
    {
        ShapeBehaviourPool<LifecycleShapeBehavior>.Reclaim(this);
    }
}

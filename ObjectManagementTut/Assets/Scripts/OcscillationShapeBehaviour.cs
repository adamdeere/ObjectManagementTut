using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class OcscillationShapeBehaviour  : ShapeBehaviour
{
    
    public Vector3 Offset { get; set; }
	
    public float Frequency { get; set; }
    float _previousOscillation;
    public override void GameUpdate(Shape shape)
    {
        float oscillation = Mathf.Sin(2f * Mathf.PI * Frequency * shape.Age);
        shape.transform.localPosition += oscillation * Offset;
        _previousOscillation = oscillation;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Offset);
        writer.Write(Frequency);
        writer.Write(_previousOscillation);
    }

    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector();
        Frequency = reader.ReadFloat();
        _previousOscillation = reader.ReadFloat();
    }

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Oscillation;

    public override void Recycle()
    {
        _previousOscillation = 0f;
        ShapeBehaviourPool<OcscillationShapeBehaviour>.Reclaim(this);
    }
}

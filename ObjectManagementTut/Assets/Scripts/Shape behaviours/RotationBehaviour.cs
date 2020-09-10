﻿using Object_script;
using UnityEngine;

public sealed class RotationBehaviour : ShapeBehaviour
{
    public Vector3 AngularVelocity { get; set; }

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.Rotate(AngularVelocity * Time.deltaTime);
        return true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(AngularVelocity);
    }

    public override void Load(GameDataReader reader)
    {
        AngularVelocity = reader.ReadVector();

    }

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Rotation;
    
    public override void Recycle()
    {
        ShapeBehaviourPool<RotationBehaviour>.Reclaim(this);
    }
}

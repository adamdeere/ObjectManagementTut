﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RotationBehaviour : ShapeBehaviour
{
    public Vector3 AngularVelocity { get; set; }

    public override void GameUpdate(Shape shape)
    {
        shape.transform.Rotate(AngularVelocity * Time.deltaTime);
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

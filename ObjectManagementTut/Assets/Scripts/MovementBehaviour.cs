using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MovementBehaviour : ShapeBehaviour
{
    private readonly ShapeBehaviorType _behaviorType;
    public Vector3 Velocity { get; set; }

    public override void GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector();
    }

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Movement;
    public override void Recycle()
    {
        ShapeBehaviourPool<MovementBehaviour>.Reclaim(this);
    }
}

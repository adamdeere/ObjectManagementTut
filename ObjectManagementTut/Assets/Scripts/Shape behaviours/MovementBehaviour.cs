using Object_script;
using UnityEngine;

public sealed class MovementBehaviour : ShapeBehaviour
{
    private readonly ShapeBehaviorType _behaviorType;
    public Vector3 Velocity { get; set; }

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
        return true;
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

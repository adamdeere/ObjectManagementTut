using UnityEngine;

public sealed class SatelliteShapeBehavior : ShapeBehaviour
{
    private Shape _focalShape;

    private float _frequency;

    private Vector3 _cosOffset, _sinOffset;
    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {
        _focalShape = focalShape;
        this._frequency = frequency;
        _cosOffset = Vector3.right;
        _sinOffset = Vector3.forward;
        _cosOffset *= radius;
        _sinOffset *= radius;
        GameUpdate(shape);
    }
    public override void GameUpdate(Shape shape)
    {
        var t = 2f * Mathf.PI * _frequency * shape.Age;
        shape.transform.localPosition = _focalShape.transform.localPosition + _cosOffset * Mathf.Cos(t) + _sinOffset * Mathf.Sin(t);
    }

    public override void Save(GameDataWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public override void Load(GameDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Satellite;
    public override void Recycle()
    {
        ShapeBehaviourPool<SatelliteShapeBehavior>.Reclaim(this);
    }
}

using UnityEngine;

public sealed class SatelliteShapeBehavior : ShapeBehaviour
{
    private ShapeInstance _focalShape;

    private float _frequency;

    private Vector3 _cosOffset, _sinOffset;
    public void Initialize(Shape shape, Shape focalShape, float radius, float frequency)
    {
        _focalShape = focalShape; 
        _frequency = frequency;
        var orbitAxis = Random.onUnitSphere;
        _cosOffset = Vector3.right;
        do 
        {
            _cosOffset = Vector3.Cross(orbitAxis, Random.onUnitSphere).normalized;
        }
        while (_cosOffset.sqrMagnitude < 0.1f);
        _sinOffset =Vector3.Cross(_cosOffset, orbitAxis);
        
        _cosOffset *= radius;
        _sinOffset *= radius;
       
        shape.AddBehavior<RotationBehaviour>().AngularVelocity = -360f * _frequency * shape.transform.InverseTransformDirection(orbitAxis);
     
        GameUpdate(shape);
    }
    public override bool GameUpdate(Shape shape)
    {
        if (_focalShape.IsValid)
        {
            var t = 2f * Mathf.PI * _frequency * shape.Age;
            shape.transform.localPosition = _focalShape.Shape.transform.localPosition + _cosOffset * Mathf.Cos(t) + _sinOffset * Mathf.Sin(t);
            return true;
        }

        return false;
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

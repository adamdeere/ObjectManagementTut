using Level_scripts;
using Object_script;
using Save_scripts;
using UnityEngine;

namespace Shape_behaviours
{
    public sealed class SatelliteShapeBehavior : ShapeBehaviour
    {
        private ShapeInstance _focalShape;

        private float _frequency;

        private Vector3 _cosOffset, _sinOffset, _previousPosition;
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
            _previousPosition = shape.transform.localPosition;
        }
        public override bool GameUpdate(Shape shape)
        {
            if (_focalShape.IsValid)
            {
                var t = 2f * Mathf.PI * _frequency * shape.Age;
                _previousPosition = shape.transform.localPosition;
                shape.transform.localPosition = _focalShape.Shape.transform.localPosition + _cosOffset * Mathf.Cos(t) + _sinOffset * Mathf.Sin(t);
                return true;
            }
            shape.AddBehavior<MovementBehaviour>().Velocity = (shape.transform.localPosition - _previousPosition) / Time.deltaTime;
            return false;
        }

        public override void Save(GameDataWriter writer)
        {
            writer.Write(_focalShape);
            writer.Write(_frequency);
            writer.Write(_cosOffset);
            writer.Write(_sinOffset);
            writer.Write(_previousPosition);
        }

        public override void Load(GameDataReader reader)
        {
            _focalShape = reader.ReadShapeInstance();
            _frequency = reader.ReadFloat();
            _cosOffset = reader.ReadVector();
            _sinOffset = reader.ReadVector();
            _previousPosition = reader.ReadVector();
        }
        public override void ResolveShapeInstances () 
        {
            _focalShape.Resolve();
        }
        public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Satellite;
        public override void Recycle()
        {
            ShapeBehaviourPool<SatelliteShapeBehavior>.Reclaim(this);
        }
    }
}

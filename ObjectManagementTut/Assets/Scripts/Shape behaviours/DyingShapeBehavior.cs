using Level_scripts;
using Object_script;
using Save_scripts;
using UnityEngine;

namespace Shape_behaviours
{
    public sealed class DyingShapeBehavior : ShapeBehaviour
    {
        private Vector3 _originalScale;
        private float _duration, _dyingAge;
        public override ShapeBehaviorType BehaviorType => ShapeBehaviorType.Growing;

        public void Initialize (Shape shape, float duration) 
        {
            _originalScale = shape.transform.localScale;
            _duration = duration;
            _dyingAge = shape.Age;
            shape.MarkAsDying();
        }
    
        public override bool GameUpdate (Shape shape) 
        {
            float dyingDuration = shape.Age - _dyingAge;
            if (dyingDuration < _duration) 
            {
                float s = 1f - dyingDuration / _duration;
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
            writer.Write(_duration);
            writer.Write(_dyingAge);
        }

        public override void Load(GameDataReader reader)
        {
            _originalScale = reader.ReadVector();
            _duration = reader.ReadFloat();
            _dyingAge = reader.ReadFloat();
        }

        public override void Recycle () 
        {
            ShapeBehaviourPool<DyingShapeBehavior>.Reclaim(this);
        }
    }
}

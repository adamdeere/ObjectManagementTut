using System;
using JetBrains.Annotations;
using Object_script;
using Save_scripts;
using Shape_behaviours;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Spawn_Zones
{
    [Serializable]
    public struct IntRange 
    {
        public int min, max;
        public int RandomValueInRange => Random.Range(min, max + 1);
    }

    [Serializable]
    public struct ColorRangeHSV 
    {
        [FloatRangeSlider(0f, 1f)]
        public FloatRange hue, saturation, value;

        public Color RandomInRange => Random.ColorHSV(hue.min, hue.max, saturation.min, saturation.max, value.min, value.max, 1f, 1f);
    }

    [Serializable]
    public struct FloatRange 
    {
        public float min, max;
        public float RandomValueInRange => Random.Range(min, max);
    }
    [Serializable]
    public struct SatelliteConfiguration 
    {
        public IntRange amount;
    
        [FormerlySerializedAs("RelativeScale")] [FloatRangeSlider(0.1f, 1f)]
        public FloatRange relativeScale;

        [FormerlySerializedAs("OrbitRadius")] public FloatRange orbitRadius;

        [FormerlySerializedAs("OrbitFrequency")] public FloatRange orbitFrequency;
    
        public bool uniformLifecycles;
   
    }
    [Serializable]
    public struct SpawnConfiguration 
    {
        public bool uniformColor;
        public enum MovementDirection 
        {
            Forward,
            Upward,
            Outward,
            Random
        }
    
        public ShapeFactory[] factories;
    
        public MovementDirection movementDirection;

        public FloatRange speed;
    
        public FloatRange angularSpeed;

        public FloatRange scale;
  
        public ColorRangeHSV color;
    
        public MovementDirection oscillationDirection;

        public FloatRange oscillationAmplitude;

        public FloatRange oscillationFrequency;

        public SatelliteConfiguration satellite;
    
        [Serializable]
        public struct LifecycleConfiguration 
        {

            [FloatRangeSlider(0f, 2f)]
            public FloatRange growingDuration;
        
            [FloatRangeSlider(0f, 100f)]
            public FloatRange adultDuration;
        
            [FloatRangeSlider(0f, 2f)]
            public FloatRange dyingDuration;
        
       
        
            public Vector3 RandomDurations => new Vector3(growingDuration.RandomValueInRange, adultDuration.RandomValueInRange, dyingDuration.RandomValueInRange);
        }

        public LifecycleConfiguration lifecycle;
    }


    public abstract class SpawnZone : PersistableObject
    {
    
        public abstract Vector3 SpawnPoint { get; }
        [SerializeField] private SpawnConfiguration spawnConfig;
   
        [SerializeField, Range(0f, 50f)] private float spawnSpeed;
    
        private float _spawnProgress;
    
        public void FixedUpdate () 
        {
            _spawnProgress += Time.deltaTime * spawnSpeed;
            while (_spawnProgress >= 1f) 
            {
                _spawnProgress -= 1f;
                SpawnShape();
            }
        }
        public override void Save (GameDataWriter writer) 
        {
            writer.Write(_spawnProgress);
        }

        public override void Load (GameDataReader reader) 
        {
            _spawnProgress = reader.ReadFloat();
        }
        public virtual void SpawnShape () 
        {
            int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
            Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
            var t = shape.transform;
            t.localPosition = SpawnPoint;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
            SetupColor(shape);
            float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
            if (angularSpeed != 0f)
            {
                var rotation = shape.AddBehavior<RotationBehaviour>(); 
                rotation.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
            }
       
            float speed = spawnConfig.speed.RandomValueInRange;
            if (speed != 0f)
            {
                var movement = shape.AddBehavior<MovementBehaviour>();
                movement.Velocity = GetDirectionVector(spawnConfig.movementDirection, t) * speed;
          
            } 
            var lifecycleDurations = spawnConfig.lifecycle.RandomDurations;
            //  SetupOscillation(shape);
            var satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;
            for (int i = 0; i < satelliteCount; i++) 
            {
                CreateSatelliteFor(shape, spawnConfig.satellite.uniformLifecycles ? lifecycleDurations : spawnConfig.lifecycle.RandomDurations);
            }
      
            SetupLifecycle(shape, lifecycleDurations);
      
        }
    
        private Vector3 GetDirectionVector (SpawnConfiguration.MovementDirection direction, Transform t) 
        {
            switch (direction) 
            {
                case SpawnConfiguration.MovementDirection.Upward:
                    return transform.up;
                case SpawnConfiguration.MovementDirection.Outward:
                    return (t.localPosition - transform.position).normalized;
                case SpawnConfiguration.MovementDirection.Random:
                    return Random.onUnitSphere;
                case SpawnConfiguration.MovementDirection.Forward:
                    return transform.forward;
                default:
                    return transform.forward;
            }
        }
    
        private void SetupOscillation (Shape shape) 
        {
            var amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
            var frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
            if (amplitude == 0f || frequency == 0f) 
            {
                return;
            }
            var oscillation = shape.AddBehavior<OcscillationShapeBehaviour>();
            oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
            oscillation.Frequency = frequency;
        }
    
        private void CreateSatelliteFor ([NotNull] Shape focalShape,  Vector3 durations) 
        {
            SetupColor(focalShape);
            if (focalShape == null) throw new ArgumentNullException(nameof(focalShape));
            var factoryIndex = Random.Range(0, spawnConfig.factories.Length);
            var shape = spawnConfig.factories[factoryIndex].GetRandom();
            var t = shape.transform;
            t.localRotation = Random.rotation;
            t.localScale = focalShape.transform.localScale * spawnConfig.satellite.relativeScale.RandomValueInRange;
            SetupColor(shape);
            shape.AddBehavior<SatelliteShapeBehavior>().Initialize(shape, focalShape, spawnConfig.satellite.orbitRadius.RandomValueInRange, spawnConfig.satellite.orbitFrequency.RandomValueInRange);
            SetupLifecycle(shape, durations);
        }
        void SetupColor (Shape shape) 
        {
            if (spawnConfig.uniformColor) 
            {
                shape.SetColor(spawnConfig.color.RandomInRange);
            }
            else 
            {
                for (int i = 0; i < shape.ColorCount; i++) 
                {
                    shape.SetColor(spawnConfig.color.RandomInRange, i);
                }
            }
        }
    
        void SetupLifecycle (Shape shape,  Vector3 durations) 
        {
            if (durations.x > 0f) 
            {
                if (durations.y > 0f || durations.z > 0f) 
                {
                    shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape, durations.x, durations.y, durations.z);
                }
                else 
                {
                    shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, durations.x);
                }
            }
            else if (durations.y > 0f) 
            {
                shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape, durations.x, durations.y, durations.z);
            }
            else if (durations.z > 0f) 
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, durations.z);
            }
        }
    }
}
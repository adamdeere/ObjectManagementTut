using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShapeInstance 
{
    public Shape Shape { get; private set; }
    
    private int _instanceIdOrSaveIndex;
    
    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        _instanceIdOrSaveIndex = shape.InstanceId;
    }
    public ShapeInstance (int saveIndex) 
    {
        Shape = null;
        _instanceIdOrSaveIndex = saveIndex;
    }
    
    public void Resolve () 
    {
        if (_instanceIdOrSaveIndex >= 0) 
        {
            Shape = Game.Instance.GetShape(_instanceIdOrSaveIndex);
            _instanceIdOrSaveIndex = Shape.InstanceId;
        }
    }
    public bool IsValid => Shape && _instanceIdOrSaveIndex == Shape.InstanceId;
}

public class Shape : PersistableObject
{
   
    public static implicit operator ShapeInstance (Shape shape) 
    {
        return new ShapeInstance(shape);
    }
    [SerializeField] private MeshRenderer[] _meshRenderers;
    private Color[] _color;
    static int _colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock _sharedPropertyBlock;

    int _shapeId = int.MinValue;
    public int MaterialId { get; private set; }
    public int ShapeId 
    {
        get => _shapeId;
        set 
        {
            if (_shapeId == int.MinValue && value != int.MinValue) 
                _shapeId = value;
            else 
                Debug.LogError("Not allowed to change shapeId.");
        }
    }
    
    public int InstanceId { get; private set; }
    public float Age { get; private set; }
    
    public int SaveIndex { get; set; }
    
    List<ShapeBehaviour> _behaviorList = new List<ShapeBehaviour>();

    public int ColorCount => _color.Length;
    
    private ShapeFactory _originFactory;
    
    
    
    private void Awake()
    {
        _color = new Color[_meshRenderers.Length];
    }
    
    public T AddBehavior<T> () where T : ShapeBehaviour, new ()
    {
        var behavior = ShapeBehaviourPool<T>.Get();
        _behaviorList.Add(behavior);
        return behavior;
    }
    public ShapeFactory OriginFactory 
    {
        get => _originFactory;
        set 
        {
            if (_originFactory == null) 
            {
                _originFactory = value;
            }
            else {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }
    public void SetMaterial (Material material, int materialId)
    {
        foreach (var t in _meshRenderers)
        {
            t.material = material;
        }

        MaterialId = materialId;
    }
   
    public void SetColor (Color color, int index) 
    {
        if (_sharedPropertyBlock == null) {
            _sharedPropertyBlock = new MaterialPropertyBlock();
        }
        _sharedPropertyBlock.SetColor(_colorPropertyId, color);
        _color[index] = color;
        _meshRenderers[index].SetPropertyBlock(_sharedPropertyBlock);
    }
    public void SetColor (Color color) 
    {
        if (_sharedPropertyBlock == null) 
        {
            _sharedPropertyBlock = new MaterialPropertyBlock();
        }
        _sharedPropertyBlock.SetColor(_colorPropertyId, color);
        for (int i = 0; i < _meshRenderers.Length; i++) 
        {
            _color[i] = color;
            _meshRenderers[i].SetPropertyBlock(_sharedPropertyBlock);
        }
    }
    public override void Save (GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_color.Length);
        foreach (var t in _color)
        {
            writer.Write(t);
        }
        writer.Write(Age);
        writer.Write(_behaviorList.Count);
        foreach (var t in _behaviorList)
        {
            writer.Write((int)t.BehaviorType);
            t.Save(writer);
        }
    }

    public override void Load (GameDataReader reader) 
    {
        base.Load(reader);
        
        if (reader.Version >= 5) 
        {
            LoadColors(reader);
        }
        else 
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        if (reader.Version >= 6) 
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++) 
            {
                ShapeBehaviour behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                _behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if (reader.Version >= 4) 
        {
            AddBehavior<RotationBehaviour>().AngularVelocity = reader.ReadVector();
            AddBehavior<MovementBehaviour>().Velocity = reader.ReadVector();
        }
    }

    public void GameUpdate ()
    {
        Age += Time.deltaTime;
        for (int i = 0; i < _behaviorList.Count; i++) 
        {
            if (!_behaviorList[i].GameUpdate(this)) 
            {
                _behaviorList[i].Recycle();
                _behaviorList.RemoveAt(i--);
            }
        }
    }
    void LoadColors (GameDataReader reader) 
    {
        var count = reader.ReadInt();
        var max = count <= _color.Length ? count : _color.Length;
        var i = 0;
        for (; i < max; i++) 
        {
            SetColor(reader.ReadColor(), i);
        }
        if (count > _color.Length) 
        {
            for (; i < count; i++) {
                reader.ReadColor();
            }
        }
        else if (count < _color.Length) {
            for (; i < _color.Length; i++) {
                SetColor(Color.white, i);
            }
        }
    }
    public void ResolveShapeInstances ()
    {
        foreach (var t in _behaviorList)
        {
            t.ResolveShapeInstances();
        }
    }
    public void Recycle () 
    {
        Age = 0f;
        InstanceId += 1;
        foreach (var t in _behaviorList)
        {
            t.Recycle();
        }
        _behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }
    public void Die () 
    {
        Game.Instance.Kill(this);
    }

}

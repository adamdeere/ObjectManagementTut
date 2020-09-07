using System;
using UnityEngine;

public class Shape : PersistableObject
{
    [SerializeField] private MeshRenderer[] _meshRenderers;
    private Color[] _color;
    static int _colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock _sharedPropertyBlock;
    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }

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

    public int ColorCount => _color.Length;
    
    private ShapeFactory _originFactory;

    private void Awake()
    {
        _color = new Color[_meshRenderers.Length];
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
        //this.color = color;
        if (_sharedPropertyBlock == null) 
        {
            _sharedPropertyBlock = new MaterialPropertyBlock();
        }
        _sharedPropertyBlock.SetColor(_colorPropertyId, color);
        for (int i = 0; i < _meshRenderers.Length; i++) {
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
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
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
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;
    }

    public void GameUpdate () 
    {
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
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
    
    public void Recycle () 
    {
        OriginFactory.Reclaim(this);
    }

}

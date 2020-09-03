using UnityEngine;

public class Shape : PersistableObject
{
    private MeshRenderer _meshRenderer;
    private Color _color;
    static int _colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock _sharedPropertyBlock;
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

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
    
    public void SetMaterial (Material material, int materialId) 
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }

    public void SetColour(Color colour)
    {
        _color = colour;
        if (_sharedPropertyBlock == null) {
            _sharedPropertyBlock = new MaterialPropertyBlock();
        }
        _sharedPropertyBlock.SetColor(_colorPropertyId, _color);
        GetComponent<MeshRenderer>().SetPropertyBlock(_sharedPropertyBlock);
    }
    
    public override void Save (GameDataWriter writer) {
        base.Save(writer);
        writer.Write(_color);
    }

    public override void Load (GameDataReader reader) {
        base.Load(reader);
        SetColour(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}

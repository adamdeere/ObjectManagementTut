﻿using System;
using UnityEngine;

public class Shape : PersistableObject
{
    [SerializeField] private MeshRenderer[] _meshRenderers;
    private Color _color;
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
    
    public void SetMaterial (Material material, int materialId)
    {
        foreach (var t in _meshRenderers)
        {
            t.material = material;
        }

        MaterialId = materialId;
    }
   
    public void SetColour(Color colour)
    {
        _color = colour;
        if (_sharedPropertyBlock == null) 
        {
            _sharedPropertyBlock = new MaterialPropertyBlock();
        }
        _sharedPropertyBlock.SetColor(_colorPropertyId, _color);
        foreach (var t in _meshRenderers)
        {
            t.SetPropertyBlock(_sharedPropertyBlock);
        }

    }
    
    public override void Save (GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_color);
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load (GameDataReader reader) 
    {
        base.Load(reader);
        SetColour(reader.Version > 0 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector() : Vector3.zero;
    }

    public void GameUpdate () 
    {
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
    }
}

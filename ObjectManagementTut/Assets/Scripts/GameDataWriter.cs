using System.IO;
using UnityEngine;
public class GameDataWriter 
{
    BinaryWriter _writer;
    
    public GameDataWriter (BinaryWriter writer) 
    {
        _writer = writer;
    }
    
    public void Write (float value) 
    {
        _writer.Write(value);
    }

    public void Write (int value) 
    {
        _writer.Write(value);
    }
    
    public void Write (Quaternion value) 
    {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
        _writer.Write(value.w);
    }
	
    public void Write (Vector3 value) 
    {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
    }
}

public class GameDataReader
{
    private readonly BinaryReader _reader;
    
    public GameDataReader (BinaryReader reader) 
    {
        _reader = reader;
    }
    
    public float ReadFloat ()
    {
        return _reader.ReadSingle();
       
    }

    public int ReadInt ()
    {
        return _reader.ReadInt32();
    }
    
    public Quaternion ReadQuaternion ()
    {
        Quaternion value;
        value.x = _reader.ReadSingle();
        value.y = _reader.ReadSingle();
        value.z = _reader.ReadSingle();
        value.w = _reader.ReadSingle();

        return value;
    }
	
    public Vector3 ReadVector ()
    {
        Vector3 value;
        value.x = _reader.ReadSingle();
        value.y = _reader.ReadSingle();
        value.z = _reader.ReadSingle();

        return value;
    }
}

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
    public void Write(Random.State value) 
    {
        _writer.Write(JsonUtility.ToJson(value));
    }
    public void Write (Color value) 
    {
        _writer.Write(value.r);
        _writer.Write(value.g);
        _writer.Write(value.b);
        _writer.Write(value.a);
    }
}
public class GameDataReader
{
    private readonly BinaryReader _reader;
    public int Version { get; }
    
    public GameDataReader (BinaryReader reader, int version) 
    {
        _reader = reader;
        Version = version;
    }
    public Random.State ReadRandomState ()
    {
        return JsonUtility.FromJson<Random.State>(_reader.ReadString());
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
    public Color ReadColor () {
        Color value;
        value.r = _reader.ReadSingle();
        value.g = _reader.ReadSingle();
        value.b = _reader.ReadSingle();
        value.a = _reader.ReadSingle();
        return value;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : PersistableObject
{
    [SerializeField] private ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveGame = KeyCode.S;
    
    [FormerlySerializedAs("LoadGame")] public KeyCode loadGame = KeyCode.L;
    private List<Shape> _ShapeList;

    [SerializeField] private PersistentStorage storage;
   
    private string _savePath;

    public static int SaveVersion { get; } = 1;

    // Start is called before the first frame update
    public void Start()
    { 
        _ShapeList = new List<Shape>();
    }

    public void Update () 
    {
        if (Input.GetKeyDown(createKey)) 
        {
           CreateObject();
        }
        else if (Input.GetKeyDown(newGameKey)) 
        {
            NewGame();
        }
        if (Input.GetKeyDown(saveGame)) 
        {
           storage.Save(this, SaveVersion);
        }
        else if (Input.GetKeyDown(loadGame)) 
        {
            NewGame();
            storage.Load(this);
        }
    }

    private void CreateObject ()
    {
        var instance = shapeFactory.GetRandom();
        var t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColour(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f
        ));
        _ShapeList.Add(instance);
    }

    private void NewGame()
    {
        foreach (var item in _ShapeList)
        {
            Destroy(item.gameObject);
        }
        _ShapeList.Clear();
    }
    
    public override void Save (GameDataWriter writer) 
    {
        writer.Write(_ShapeList.Count);
        for (int i = 0; i < _ShapeList.Count; i++) 
        {
            writer.Write(_ShapeList[i].ShapeId);
            writer.Write(_ShapeList[i].MaterialId);
            _ShapeList[i].Save(writer);
        }
    }
    public override void Load (GameDataReader reader) 
    {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();
        if (version > SaveVersion) 
        {
            Debug.Log("Unsupported future save version " + version);
            return;
        }
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            _ShapeList.Add(instance);
        }
    }
}



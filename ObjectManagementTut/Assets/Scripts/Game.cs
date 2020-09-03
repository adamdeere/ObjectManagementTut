using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : PersistableObject
{
    [SerializeField] private PersistableObject prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveGame = KeyCode.S;
    
    [FormerlySerializedAs("LoadGame")] public KeyCode loadGame = KeyCode.L;
    private List<PersistableObject> _objectList;

    [SerializeField] private PersistentStorage storage;
   
    private string _savePath;
    // Start is called before the first frame update
    public void Start()
    { 
        _objectList = new List<PersistableObject>();
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
           storage.Save(this);
        }
        else if (Input.GetKeyDown(loadGame)) 
        {
            NewGame();
            storage.Load(this);
        }
    }

    private void CreateObject () 
    {
        var o = Instantiate(prefab);
        var t = o.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        
        _objectList.Add(o);
    }

    private void NewGame()
    {
        foreach (var item in _objectList)
        {
            Destroy(item.gameObject);
        }
        _objectList.Clear();
    }
    
    public override void Save (GameDataWriter writer) 
    {
        writer.Write(_objectList.Count);
        for (int i = 0; i < _objectList.Count; i++) 
        {
            _objectList[i].Save(writer);
        }
    }
    public override void Load (GameDataReader reader) 
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) 
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            _objectList.Add(o);
        }
    }
}



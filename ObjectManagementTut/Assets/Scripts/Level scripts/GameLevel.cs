using Spawn_Zones;
using UnityEngine;

public class GameLevel : PersistableObject
{
    [SerializeField] private SpawnZone spawnZone;
    [SerializeField] private PersistableObject[] persistentObjects;
    [SerializeField] private int populationLimit;
    public static GameLevel Current { get; private set; }

    public int PopulationLimit => populationLimit;

    public void OnEnable () 
    {
       
        Current = this;
        if (persistentObjects == null) 
            persistentObjects = new PersistableObject[0];
        
    }
    public void SpawnShape () 
    {
         spawnZone.SpawnShape();
    }
    
    public override void Save (GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        foreach (var t in persistentObjects)
        {
            t.Save(writer);
        }
    }

    public override void Load (GameDataReader reader) 
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++) 
        {
            persistentObjects[i].Load(reader);
        }
    }
}

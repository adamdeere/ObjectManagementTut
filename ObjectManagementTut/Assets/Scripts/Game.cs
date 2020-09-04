using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using System.Collections;
using Random = UnityEngine.Random;

public class Game : PersistableObject
{
    public static Game gameInstance;
    [SerializeField] private ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveGame = KeyCode.S;
    public KeyCode destroyKey = KeyCode.X;
    [FormerlySerializedAs("LoadGame")] public KeyCode loadGame = KeyCode.L;
    private List<Shape> _ShapeList;

    [SerializeField] private PersistentStorage storage;
    public SpawnZone SpawnZoneOfLevel { get; set; }
    private string _savePath;
    public int levelCount;
    public static int SaveVersion { get; } = 2;
    public float CreationSpeed { get; set; }
    private int _loadedLevelBuildIndex;
    public float DestructionSpeed { get; set; }
    float _creationProgress, _destructionProgress;

    private void OnEnable()
    {
        if (gameInstance == null)
            gameInstance = this;
      
    }

    // Start is called before the first frame update
    public void Start()
    { 
        _ShapeList = new List<Shape>();
        if (Application.isEditor) 
        {
           
            for (int i = 0; i < SceneManager.sceneCount; i++) 
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Two")) 
                {
                    SceneManager.SetActiveScene(loadedScene);
                    _loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }
        StartCoroutine(LoadLevel(levelCount));
    }

    public void Update () 
    {
        #region all keyboard inputs
        if (Input.GetKeyDown(createKey)) 
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey)) 
        {
            NewGame();
        }
        else if (Input.GetKeyDown(saveGame)) 
        {
            storage.Save(this, SaveVersion);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else if (Input.GetKeyDown(loadGame)) 
        {
            NewGame();
            storage.Load(this);
        }
        else 
        {
            for (int i = 1; i <= levelCount; i++) 
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) 
                {
                    NewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        #endregion
        
        _creationProgress += Time.deltaTime * CreationSpeed;
        while (_creationProgress >= 1f) 
        {
            _creationProgress -= 1f;
            CreateShape();
        }
        _destructionProgress += Time.deltaTime * DestructionSpeed;
        while (_destructionProgress >= 1f) 
        {
            _destructionProgress -= 1f;
            DestroyShape();
        }
        
    }

    
    IEnumerator LoadLevel (int levelIndex) 
    {
        enabled = false;
        if (_loadedLevelBuildIndex > 0) 
            yield return SceneManager.UnloadSceneAsync(_loadedLevelBuildIndex);
        
        yield return SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelIndex));
        _loadedLevelBuildIndex = levelIndex;
        enabled = true;
    }
    private void CreateShape ()
    {
        var instance = shapeFactory.GetRandom();
        var t = instance.transform;
        t.localPosition = SpawnZoneOfLevel.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColour(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f,  valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f));
        _ShapeList.Add(instance);
    }

    private void NewGame()
    {
        foreach (var item in _ShapeList)
        {
            
            shapeFactory.Reclaim(item);
        }
        _ShapeList.Clear();
    }
    
    public override void Save (GameDataWriter writer) 
    {
        writer.Write(_ShapeList.Count);
        writer.Write(_loadedLevelBuildIndex);
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
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
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
    void DestroyShape ()
    {
        if (_ShapeList.Count <= 0) return;
        
        var index = Random.Range(0, _ShapeList.Count);
        shapeFactory.Reclaim(_ShapeList[index]);
        var lastIndex = _ShapeList.Count - 1;
        _ShapeList[index] = _ShapeList[lastIndex];
        _ShapeList.RemoveAt(lastIndex);
    }
}



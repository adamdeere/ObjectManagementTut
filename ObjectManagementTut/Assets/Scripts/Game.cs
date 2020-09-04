using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using System.Collections;
using Random = UnityEngine.Random;

public class Game : PersistableObject
{
    [SerializeField] private ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveGame = KeyCode.S;
    public KeyCode destroyKey = KeyCode.X;
    [FormerlySerializedAs("LoadGame")] public KeyCode loadGame = KeyCode.L;
    private List<Shape> _shapeList;

    [SerializeField] private PersistentStorage storage;
  
    private string _savePath;
    public int levelCount;
    public static int SaveVersion { get; } = 3;
    public float CreationSpeed { get; set; }
    private int _loadedLevelBuildIndex;
    public float DestructionSpeed { get; set; }
    float _creationProgress, _destructionProgress;
    private Random.State _mainRandomState;



    [FormerlySerializedAs("ReseedOnLoad")] [SerializeField] private bool reseedOnLoad;

    // Start is called before the first frame update
    public void Start()
    { 
        _mainRandomState = Random.state;
        _shapeList = new List<Shape>();
        // if (Application.isEditor) 
        // {
        //    
        //     for (int i = 0; i < SceneManager.sceneCount; i++) 
        //     {
        //         Scene loadedScene = SceneManager.GetSceneAt(i);
        //         if (loadedScene.name.Contains("One")) 
        //         {
        //             SceneManager.SetActiveScene(loadedScene);
        //             _loadedLevelBuildIndex = loadedScene.buildIndex;
        //             return;
        //         }
        //     }
        // }

        NewGame();
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
            StartCoroutine(LoadLevel(_loadedLevelBuildIndex));
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
    private void CreateShape ()
    {
        var instance = shapeFactory.GetRandom();
        var t = instance.transform;
        t.localPosition = GameLevel.Current.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColour(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f,  valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f));
        _shapeList.Add(instance);
    }

    private void NewGame()
    {
        Random.state = _mainRandomState;
        int seed = Random.Range(0, int.MaxValue);
        _mainRandomState = Random.state;
        Random.InitState(seed);
        foreach (var item in _shapeList)
        { 
            shapeFactory.Reclaim(item);
        }
        _shapeList.Clear();
    }
    
    public override void Save (GameDataWriter writer) 
    {
        writer.Write(_shapeList.Count);
        writer.Write(Random.state);
        writer.Write(_loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
        foreach (var t in _shapeList)
        {
            writer.Write(t.ShapeId);
            writer.Write(t.MaterialId);
            t.Save(writer);
        }
    }
    
    public override void Load (GameDataReader reader) 
    {
        int version = reader.Version;
        if (version > SaveVersion) 
        {
            Debug.Log("Unsupported future save version " + version);
            return;
        }
        StartCoroutine(LoadGame(reader));
       
    }
    void DestroyShape ()
    {
        if (_shapeList.Count <= 0) return;
        
        var index = Random.Range(0, _shapeList.Count);
        shapeFactory.Reclaim(_shapeList[index]);
        var lastIndex = _shapeList.Count - 1;
        _shapeList[index] = _shapeList[lastIndex];
        _shapeList.RemoveAt(lastIndex);
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
    
    IEnumerator LoadGame (GameDataReader reader) 
    {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3) 
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad) 
            {
                Random.state = state;
            }
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3) 
        { 
            GameLevel.Current.Load(reader);
        }
        for (int i = 0; i < count; i++) 
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            _shapeList.Add(instance);
        }
    }
}



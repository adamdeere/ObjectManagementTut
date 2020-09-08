using UnityEngine;

public abstract class ShapeBehaviour
#if UNITY_EDITOR
    : ScriptableObject
#endif
{
#if UNITY_EDITOR
    void OnEnable () 
    {
        if (IsReclaimed) 
            Recycle();
        
    }

    public bool IsReclaimed { get; set; }
#endif
    public abstract bool GameUpdate(Shape shape);
    
    public abstract void Save (GameDataWriter writer);

    public abstract void Load (GameDataReader reader);
    
    public abstract ShapeBehaviorType BehaviorType { get; }
    
    public abstract void Recycle ();
    
    public virtual void ResolveShapeInstances () {}

}

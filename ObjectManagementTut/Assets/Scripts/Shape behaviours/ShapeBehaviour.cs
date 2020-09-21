using Level_scripts;
using Object_script;
using Save_scripts;
using UnityEngine;

namespace Shape_behaviours
{
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
}

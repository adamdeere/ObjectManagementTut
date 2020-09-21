using System.Collections.Generic;
using UnityEngine;

namespace Shape_behaviours
{
    public static class ShapeBehaviourPool<T> where T : ShapeBehaviour, new() 
    {  
        static Stack<T> _stack = new Stack<T>();
    
        public static T Get () 
        {
            if (_stack.Count > 0) 
            {
                T behavior = _stack.Pop();
#if UNITY_EDITOR
                behavior.IsReclaimed = false;
#endif
                return behavior;
            }
#if UNITY_EDITOR
            return ScriptableObject.CreateInstance<T>();
#else
		return new T();
#endif
        }

        public static void Reclaim (T behavior) 
        {
#if UNITY_EDITOR
            behavior.IsReclaimed = true;
#endif

            _stack.Push(behavior);
        }
    }
}

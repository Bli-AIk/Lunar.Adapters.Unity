using Lunar.Interfaces;
using LunarGameObject = Lunar.GameObject; 

namespace Lunar.Adapters.Unity
{
    public class GameObjectAdapter : IGameObject
    {
        public GameObjectAdapter(UnityEngine.GameObject gameObject)
        {
            GameObject = new LunarGameObject(gameObject);
        }
        public LunarGameObject GameObject { get; }
        
        internal UnityEngine.GameObject NativeGameObject => (UnityEngine.GameObject)GameObject.BaseGameObject;
        
    }
}
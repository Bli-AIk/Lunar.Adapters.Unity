using Lunar.Core.ECS.Components;

namespace Lunar.Adapters.Unity.Utils
{
    public static class UnityConversionUtils
    {
        public static bool TryParseToUnity(this GameObjectComponent gameObject,
            out UnityEngine.GameObject unityGameObject)
        {
            unityGameObject = gameObject.GameObjectHandle.NativeGameObject as UnityEngine.GameObject;

            return unityGameObject;

        }


        public static bool TryParseToUnity(this SpriteComponent sprite,
            out UnityEngine.SpriteRenderer unitySpriteRenderer)
        {
            if (sprite.Sprite != null)
            {
                unitySpriteRenderer = sprite.Sprite.NativeSprite as UnityEngine.SpriteRenderer;

                return unitySpriteRenderer;
            }

            unitySpriteRenderer = null;
            return false;
        }
        
        
    }
}
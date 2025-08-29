using System;
using Lunar.Components;

namespace Lunar.Adapters.Unity
{
    public static class UnityConversionUtils
    {
        public static bool TryParseToUnity(this GameObjectComponent gameObject,
            out UnityEngine.GameObject unityGameObject)
        {
            unityGameObject = gameObject.GameObject.BaseGameObject as UnityEngine.GameObject;

            return unityGameObject;
        }


        public static bool TryParseToUnity(this SpriteComponent sprite,
            out UnityEngine.SpriteRenderer unitySpriteRenderer)
        {
            unitySpriteRenderer = sprite.Sprite.BaseSprite as UnityEngine.SpriteRenderer;

            return unitySpriteRenderer;
        }
        
        
    }
}
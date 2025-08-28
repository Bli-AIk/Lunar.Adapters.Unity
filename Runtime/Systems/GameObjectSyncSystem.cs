using System;
using Arch.Core;
using Lunar.Components;
using Lunar.Systems;
using UnityEngine;

namespace Lunar.Adapters.Unity.Systems
{
    public class GameObjectSyncSystem : GameObjectSyncSystemBase
    {
        public GameObjectSyncSystem(World world) : base(world) { }

        protected override void SyncTransform(GameObjectComponent gameObject, PositionComponent position)
        {
            if (TryGetUnityGameObject(gameObject, out var unityGameObject))
            {
                unityGameObject.transform.position = new Vector3(position.X, position.Y, position.Z);
            }
        }

        protected override void SyncName(GameObjectComponent gameObject, NameComponent name)
        {
            if (TryGetUnityGameObject(gameObject, out var unityGameObject))
            {
                unityGameObject.name = name.Name;
            }
        }

        private static bool TryGetUnityGameObject(GameObjectComponent gameObject,
            out UnityEngine.GameObject unityGameObject)
        {
            unityGameObject = gameObject.GameObject.BaseGameObject as UnityEngine.GameObject;

            return unityGameObject;
        }
    }
}
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

        protected override void SyncTransform(GameObjectComponent gameObject, TransformComponent transform)
        {
            if (TryGetUnityGameObject(gameObject, out var unityGameObject))
            {
                unityGameObject.transform.position =
                    new Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
                unityGameObject.transform.rotation =
                    new Quaternion(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, transform.Rotation.W);
                unityGameObject.transform.localScale =
                    new Vector3(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);
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
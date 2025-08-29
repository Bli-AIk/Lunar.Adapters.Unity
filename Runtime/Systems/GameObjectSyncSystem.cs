using System;
using Arch.Core;
using Lunar.Adapters.Unity.Utils;
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
            if (!gameObject.TryParseToUnity(out var unityGameObject))
            {
                return;
            }

            unityGameObject.transform.position =
                new Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
            unityGameObject.transform.rotation =
                new Quaternion(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, transform.Rotation.W);
            unityGameObject.transform.localScale =
                new Vector3(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);
        }

        protected override void SyncName(GameObjectComponent gameObject, NameComponent name)
        {
            if (gameObject.TryParseToUnity(out var unityGameObject))
            {
                unityGameObject.name = name.Name;
            }
        }
    }
}
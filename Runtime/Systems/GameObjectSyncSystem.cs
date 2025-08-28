using Arch.Core;
using Lunar.Components;
using Lunar.Systems;
using UnityEngine;

namespace Lunar.Adapters.Unity.Systems
{
    public class GameObjectSyncSystem : GameObjectSyncSystemBase
    {
        public GameObjectSyncSystem(World world) : base(world) { }
        protected override void ApplyTransform(GameObjectComponent gameObject, PositionComponent position)
        {
            var unityGameObject = gameObject.GameObject.BaseGameObject as UnityEngine.GameObject;

            if (unityGameObject)
            {
                unityGameObject.transform.position = new Vector3(position.X, position.Y, position.Z);
            }
        }
    }
}
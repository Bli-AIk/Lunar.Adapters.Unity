using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Adapters.Unity.Systems;
using Lunar.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Lunar.Adapters.Unity
{
    /// <summary>
    ///     Unity specific implementation of pure C# logic
    /// </summary>
    internal class GameControllerImplementation : GameControllerBase
    {
        private readonly ObjectPool<UnityEngine.GameObject> _gameObjectPool;
        private readonly Transform _parent;

        public GameControllerImplementation(ObjectPool<UnityEngine.GameObject> pool, Transform parent)
        {
            _gameObjectPool = pool;
            _parent = parent;
        }

        protected override Group<float> CreateSystems(World world)
        {
            return new Group<float>("MainGroup",
                new GameObjectSyncSystem(world),
                new DebugCreateObjectSystem(world)
            );
        }

        protected override void SetEvents(World world)
        {
            world.SubscribeComponentAdded((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
            {
                if (!entity.Has<PositionComponent>())
                {
                    entity.Add<PositionComponent>();
                }

                if (gameObjectComponent.GameObject != null)
                {
                    return;
                }

                var unityGameObject = _gameObjectPool.Get();
                unityGameObject.transform.SetParent(_parent);
                entity.Set(new GameObjectComponent(new GameObject(unityGameObject)));
            });

            world.SubscribeComponentRemoved((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
            {
                if (gameObjectComponent.GameObject != null)
                {
                    _gameObjectPool.Release(gameObjectComponent.GameObject.BaseGameObject as UnityEngine.GameObject);
                }
            });
        }
    }
}
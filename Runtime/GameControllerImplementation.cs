using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Adapters.Unity.Systems;
using Lunar.Adapters.Unity.Utils;
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

        public GameControllerImplementation(
            ObjectPool<UnityEngine.GameObject> pool,
            Transform parent)
        {
            _gameObjectPool = pool;
            _parent = parent;
        }

        protected override Group<float> CreateSystems(World world)
        {
            var serviceRegistry = GameController.Services;
            return new Group<float>("MainGroup",
                new DebugCreateObjectSystem(world),
                new GameObjectSyncSystem(world),
                new SpriteSyncSystem(world, serviceRegistry.Resources, serviceRegistry.Logger)
            );
        }

        protected override void SetEvents(World world)
        {
            world.SubscribeComponentAdded((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
            {
                if (!entity.Has<TransformComponent>())
                {
                    entity.Add(new TransformComponent());
                }

                if (gameObjectComponent.GameObject != null)
                {
                    return;
                }

                var unityGameObject = _gameObjectPool.Get();
                unityGameObject.transform.SetParent(_parent);
                entity.Set(new GameObjectComponent(new GameObject(unityGameObject)));
            });


            world.SubscribeComponentAdded((in Entity entity, ref SpriteComponent spriteComponent) =>
            {
                if (!entity.TryGet<GameObjectComponent>(out var gameObject))
                {
                    gameObject = new GameObjectComponent();
                    entity.Add(gameObject);
                }

                if (!gameObject.TryParseToUnity(out var unityGameObject))
                {
                    return;
                }

                spriteComponent.Sprite = unityGameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer)
                    ? new Sprite(spriteRenderer)
                    : new Sprite(unityGameObject.AddComponent<SpriteRenderer>());
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
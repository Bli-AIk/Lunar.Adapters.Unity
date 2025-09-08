using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Adapters.Unity.Systems;
using Lunar.Adapters.Unity.Utils;
using Lunar.Core.Base;
using Lunar.Core.ECS;
using Lunar.Core.ECS.Components;
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
                new SpriteSyncSystem(world, serviceRegistry.Resources, serviceRegistry.Logger),
                new SpriteCleanSystem(world)
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

                if (gameObjectComponent.GameObjectBase != null)
                {
                    return;
                }

                var unityGameObject = _gameObjectPool.Get();
                unityGameObject.transform.SetParent(_parent);
                entity.Set(new GameObjectComponent(new GameObjectBase(unityGameObject)));
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

                if (unityGameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    spriteRenderer.enabled = true;
                    spriteComponent.Sprite = new SpriteBase(spriteRenderer);
                }
                else
                {
                    spriteComponent.Sprite = new SpriteBase(unityGameObject.AddComponent<SpriteRenderer>());
                }
            });

            world.SubscribeComponentRemoved((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
            {
                if (gameObjectComponent.GameObjectBase != null)
                {
                    _gameObjectPool.Release(gameObjectComponent.GameObjectBase.BaseGameObject as UnityEngine.GameObject);
                }
            });
        }
    }
}
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Adapters.Unity.Systems;
using Lunar.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Lunar.Adapters.Unity
{
    public class GameController : MonoBehaviour
    {
        private ObjectPool<UnityEngine.GameObject> _gameObjectPool;
        private World _mainWorld;
        private Group<float> _systems;

        private void Awake()
        {
            SetUpGameObjectPool();

            _mainWorld = World.Create();

            _systems = new Group<float>("MainGroup",
                new RenderingSystem(_mainWorld)
            );

            SetEvents();

            _systems.Initialize();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _systems.BeforeUpdate(in deltaTime);
            _systems.Update(in deltaTime);

            //Test

            if (Input.GetKeyDown(UnityEngine.KeyCode.A))
            {
                _mainWorld.Create(new GameObjectComponent());
            }


            if (Input.GetKeyDown(UnityEngine.KeyCode.Q))
            {
                var query = new QueryDescription().WithAll<GameObjectComponent>();
                _mainWorld.Query(in query, (Entity entity,
                    ref GameObjectComponent gameObjectComponent) =>
                {
                    entity.Remove<GameObjectComponent>();
                });
            }
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            // _physicsSystems.Update(in fixedDeltaTime);
        }

        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            _systems.AfterUpdate(in deltaTime);
        }

        private void OnDestroy()
        {
            _systems.Dispose();
            _mainWorld.Dispose();
            _gameObjectPool.Clear();
        }

        private void SetUpGameObjectPool()
        {
            _gameObjectPool = new ObjectPool<UnityEngine.GameObject>(
                () => new UnityEngine.GameObject(),
                gameObjectItem => gameObjectItem.SetActive(true),
                gameObjectItem => gameObjectItem.SetActive(false),
                Destroy,
                false,
                20,
                200
            );
        }

        private void SetEvents()
        {
            _mainWorld.SubscribeComponentAdded((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
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
                unityGameObject.transform.SetParent(transform);

                entity.Set(new GameObjectComponent(new GameObject(unityGameObject)));
            });

            _mainWorld.SubscribeComponentRemoved((in Entity entity, ref GameObjectComponent gameObjectComponent) =>
            {
                if (gameObjectComponent.GameObject != null)
                {
                    _gameObjectPool.Release(gameObjectComponent.GameObject.BaseGameObject as UnityEngine.GameObject);
                }
            });
        }
    }
}
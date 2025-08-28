using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;
using UnityEngine.Pool;
using Lunar.Adapters.Unity.Systems;
using Lunar.Components;

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

        private void SetUpGameObjectPool()
        {
            _gameObjectPool = new ObjectPool<UnityEngine.GameObject>(
                createFunc: () => new UnityEngine.GameObject(),
                actionOnGet: gameObjectItem => gameObjectItem.SetActive(true),
                actionOnRelease: gameObjectItem => gameObjectItem.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: false,
                defaultCapacity: 20,
                maxSize: 200
            );
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
                RemoveObject();
            }

            return;

            void RemoveObject()
            {
                var query = new QueryDescription().WithAll<GameObjectComponent>();
                _mainWorld.Query(in query, (Entity entity, 
                    ref GameObjectComponent gameObjectComponent) =>
                {
                    if (gameObjectComponent.GameObject != null)
                    {
                        _gameObjectPool.Release(gameObjectComponent.GameObject.BaseGameObject as UnityEngine.GameObject);
                    }

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
        }
    }
}
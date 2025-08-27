using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Adapters.Unity.Systems;
using Lunar.Components;
using UnityEngine;

namespace Lunar.Adapters.Unity
{
    public class GameController : MonoBehaviour
    {
        private World _mainWorld;
        private Group<float> _systems;

        private void Awake()
        {
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

                var nativeGameObject = new UnityEngine.GameObject();
                gameObjectComponent.GameObject = new GameObject(nativeGameObject);
            });
        }
    }
}
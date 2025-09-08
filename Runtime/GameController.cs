using System;
using System.Collections.Generic;
using Lunar.Core.Base;
using UnityEngine;
using UnityEngine.Pool;

namespace Lunar.Adapters.Unity
{
    public class GameController : MonoBehaviour
    {
        private GameControllerImplementation _controller;
        private ObjectPool<UnityEngine.GameObject> _gameObjectPool;
        public static ServiceRegistry Services { get; private set; }

        /// <summary>
        ///     External replacement service factory
        /// </summary>
        public static Func<ServiceRegistry> ServicesFactory { get; set; } = DefaultServicesFactory;

        private void Awake()
        {
            SetUpGameObjectPool();

            Services = ServicesFactory();

            _controller = new GameControllerImplementation(_gameObjectPool, transform);
            _controller.Initialize();
        }

        private void Update()
        {
            _controller.Update(Time.deltaTime);
        }

        private void LateUpdate()
        {
            _controller.LateUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _controller.Dispose();
            _gameObjectPool.Clear();
        }

        private void SetUpGameObjectPool()
        {
            _gameObjectPool = new ObjectPool<UnityEngine.GameObject>(
                () => new UnityEngine.GameObject(),
                go => go.SetActive(true),
                go => go.SetActive(false),
                Destroy,
                false,
                20,
                200
            );
        }

        /// <summary>
        ///     Default service configuration
        /// </summary>
        public static ServiceRegistry DefaultServicesFactory()
        {
            return new ServiceRegistry
            {
                Resources = new ResourcesAdapter(),
                ResourcesAsync = new ResourcesAdapter(),
                InputActions = new InputActionsAdapter(
                    new InputAdapter(),
                    new Dictionary<string, KeyCodeBase[]>
                    {
                        ["Play"] = new[] { KeyCodeBase.Space },
                        ["Pause"] = new[] { KeyCodeBase.P },
                        ["Resume"] = new[] { KeyCodeBase.R },
                        ["Cancel"] = new[] { KeyCodeBase.C }
                    }),
                Logger = new DebugLogAdapter()
            };
        }
    }
}
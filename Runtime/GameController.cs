using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Lunar.Adapters.Unity
{
    public class GameController : MonoBehaviour
    {
        private GameControllerImplementation _controller;
        private ObjectPool<UnityEngine.GameObject> _gameObjectPool;
        public static ServiceRegistry Services { get; private set; }

        private void Awake()
        {
            SetUpGameObjectPool();

            SetupServices();

            _controller = new GameControllerImplementation(_gameObjectPool, transform);

            _controller.Initialize();
        }

        private static void SetupServices()
        {
            Services = new ServiceRegistry
            {
                Resources = new ResourcesAdapter(),
                ResourcesAsync = new ResourcesAdapter(),
                InputActions = new InputActionsAdapter(
                    new InputAdapter(),
                    new Dictionary<string, KeyCode[]>
                    {
                        ["Play"] = new[] { KeyCode.Space },
                        ["Pause"] = new[] { KeyCode.P },
                        ["Resume"] = new[] { KeyCode.R },
                        ["Cancel"] = new[] { KeyCode.C }
                    }),
                Logger = new DebugLogAdapter()
            };
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
    }
}
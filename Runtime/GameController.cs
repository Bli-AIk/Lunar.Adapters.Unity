using UnityEngine;
using UnityEngine.Pool;

namespace Lunar.Adapters.Unity
{
    public class GameController : MonoBehaviour
    {
        private GameControllerImplementation _controller;
        private ObjectPool<UnityEngine.GameObject> _gameObjectPool;

        private void Awake()
        {
            SetUpGameObjectPool();
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
    }
}
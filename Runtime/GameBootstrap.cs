namespace Lunar.Adapters.Unity
{
    using UnityEngine;

    public abstract class GameBootstrap : MonoBehaviour
    {
        public void Awake()
        {
            GameController.ServicesFactory = ServiceRegistry;

            if (!gameObject.GetComponent<GameController>())
            {
                gameObject.AddComponent<GameController>();
            }
        
            DontDestroyOnLoad(gameObject); 
        }

        public abstract ServiceRegistry ServiceRegistry();
    }

}
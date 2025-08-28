using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Lunar.Components;
using UnityEngine;

namespace Lunar.Adapters.Unity.Systems
{
    public class DebugCreateObjectSystem : BaseSystem<World, float>
    {
        public DebugCreateObjectSystem(World world) : base(world) { }

        public override void Initialize()
        {
            Debug.Log("DebugCreateObjectSystem Initialized!");
            
        }

        public override void Update(in float deltaTime)
        {
            if (Input.GetKeyDown(UnityEngine.KeyCode.A))
            {
                var entity = World.Create(new GameObjectComponent());
                Debug.Log($"Created {entity}");
                entity.Add(new NameComponent($"Lunar Entity: {entity.Id}"));
            }


            if (Input.GetKeyDown(UnityEngine.KeyCode.Q))
            {
                var query = new QueryDescription().WithAll<GameObjectComponent>();
                World.Query(in query, (Entity entity,
                    ref GameObjectComponent gameObjectComponent) =>
                {
                    World.Remove<GameObjectComponent>(entity);
                    Debug.Log($"Remove {entity}");
                });
                
            }
        }
    }
}
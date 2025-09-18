using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;

using Lunar.Core.ECS.Components;
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
                if (Input.GetKey(UnityEngine.KeyCode.LeftControl))
                {
                    Debug.Log($"Add SpriteComponent: {entity.Id}");
                    entity.Add(new SpriteComponent("A"));
                }
            }


            if (Input.GetKeyDown(UnityEngine.KeyCode.D))
            {
                var query = new QueryDescription().WithAll<GameObjectComponent>();
                World.Query(in query, (Entity entity,
                    ref GameObjectComponent gameObjectComponent) =>
                {
                    World.Remove<GameObjectComponent>(entity);
                    Debug.Log($"Remove {entity}");
                });
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.W))
            {
                var queryS = new QueryDescription().WithAll<SpriteComponent>();
                World.Query(in queryS, (Entity entity,
                    ref SpriteComponent sprite) =>
                {
                    Debug.Log($"{entity} have {sprite}");
                });
            }


            if (Input.GetKeyDown(UnityEngine.KeyCode.C))
            {
                var query = new QueryDescription().WithAll<SpriteComponent>();
                World.Query(in query, (Entity entity,
                    ref SpriteComponent spriteComponent) =>
                {
                    spriteComponent.Path = spriteComponent.Path switch
                    {
                        "A" => "B",
                        "B" => "A",
                        _ => spriteComponent.Path
                    };
                });
            }
        }
    }
}
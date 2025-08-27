using Arch.Core;
using Arch.System;
using Lunar.Components;

namespace Lunar.Adapters.Unity.Systems
{
    public class RenderingSystem : BaseSystem<World, float>
    {
        public RenderingSystem(World world) : base(world) { }

        public override void Update(in float deltaTime)
        {
            var gameObjectQuery = new QueryDescription().WithAll<GameObjectComponent, PositionComponent>();
            World.Query(in gameObjectQuery,
                (Entity entity, ref GameObjectComponent lunarGameObject, ref PositionComponent position) =>
                {
                    
                });
        }
    }
}
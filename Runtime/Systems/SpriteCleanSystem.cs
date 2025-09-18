using Arch.Core;
using Lunar.Adapters.Unity.Utils;
using Lunar.Components;
using Lunar.Systems;
using UnityEngine;

namespace Lunar.Adapters.Unity.Systems
{
    public class SpriteCleanSystem : SpriteCleanSystemBase
    {
        public SpriteCleanSystem(World world) : base(world) { }
        protected override void CleanSprite(SpriteComponent sprite)
        {
            if (!sprite.TryParseToUnity(out var spriteRenderer))
            {
                return;
            }

            spriteRenderer.sprite = null;
            spriteRenderer.color = Color.white;
            spriteRenderer.enabled = false;
        }
    }
}
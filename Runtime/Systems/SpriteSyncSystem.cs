using System;
using Arch.Core;
using JetBrains.Annotations;
using Lunar.Adapters.Unity.Utils;
using Lunar.Components;
using Lunar.Systems;
using UnityEngine;

namespace Lunar.Adapters.Unity.Systems
{
    public class SpriteSyncSystem : SpriteSyncSystemBase
    {
        public SpriteSyncSystem(World world) : base(world) { }
        protected override void SyncSprite(SpriteComponent sprite)
        {
            if (!sprite.TryParseToUnity(out var unitySpriteRenderer))
            {
                return;
            }
            //unitySpriteRenderer.sprite =
            
        }
    }
}
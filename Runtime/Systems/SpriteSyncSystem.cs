#nullable enable
using System;
using System.Threading.Tasks;
using Arch.Core;
using Lunar.Adapters.Unity.Utils;

using Lunar.Core.Base.Interfaces;
using Lunar.Core.ECS.Components;
using Lunar.Core.ECS.Systems;


namespace Lunar.Adapters.Unity.Systems
{
    public class SpriteSyncSystem : SpriteSyncSystemBase
    {
        private readonly ILogger? _logger;
        private readonly IResources? _resources;
        private readonly IResourcesAsync? _resourcesAsync;

        public SpriteSyncSystem(World world, IResourcesAsync resourcesAsync, ILogger? logger = null)
            : base(world)
        {
            _resourcesAsync = resourcesAsync ?? throw new ArgumentNullException(nameof(resourcesAsync));
            _logger = logger;
        }

        public SpriteSyncSystem(World world, IResources resources, ILogger? logger = null)
            : base(world)
        {
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
            _logger = logger;
        }


        protected override async Task<SpriteComponent> SyncSprite(SpriteComponent sprite)
        {
            try
            {
                if (sprite.Path == sprite.LastLoadPath || !sprite.TryParseToUnity(out var unitySpriteRenderer))
                {
                    return sprite;
                }

                if (_resourcesAsync != null)
                {
                    unitySpriteRenderer.sprite = await _resourcesAsync.LoadAsync<UnityEngine.Sprite>(sprite.Path);
                }
                else if (_resources != null)
                {
                    unitySpriteRenderer.sprite = _resources.Load<UnityEngine.Sprite>(sprite.Path);
                }
                else
                {
                    throw new ArgumentNullException(
                        $"Both _resourcesAsync and _resources are null in SpriteSyncSystem. Cannot load sprite at path: '{sprite.Path}'");
                }

                sprite.SetLastLoadPath(sprite.Path);
                return sprite;
            }
            catch (Exception e)
            {
                _logger?.LogError($"[SyncSprite] Failed to load sprite at path: {sprite.Path}\n{e}");
                return sprite;
            }
        }
    }
}
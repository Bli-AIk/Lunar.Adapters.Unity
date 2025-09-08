using Lunar.Core.Base.Interfaces;

namespace Lunar.Adapters.Unity
{
    public class ServiceRegistry
    {
        public IResources Resources { get; set; }
        public IResourcesAsync ResourcesAsync { get; set; }
        public IInputActions InputActions { get; set; }
        public ILogger Logger { get; set; }
    }
}
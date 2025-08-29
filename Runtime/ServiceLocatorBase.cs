using Lunar.Interfaces;

namespace Lunar.Adapters.Unity
{
    public interface IServiceLocator
    {
        IResources Resources { get; }
        //IResourcesAsync ResourcesAsync { get; }
        IInputActions InputActions { get; }
        ILogger Debug { get; }
    }
}
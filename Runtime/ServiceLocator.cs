using System.Collections.Generic;
using Lunar.Interfaces;

namespace Lunar.Adapters.Unity
{
    public class ServiceLocator : IServiceLocator
    {
        private ServiceLocator() { }

        public static IServiceLocator Instance { get; } = new ServiceLocator();

        public IResources Resources { get; } = new ResourcesAdapter();
        //public IResourcesAsync ResourcesAsync { get; } = new AddressablesAdapter();

        public IInputActions InputActions { get; } = new InputActionsAdapter(new InputAdapter(),
            new Dictionary<string, KeyCode[]>
            {
                ["Play"] = new[] { KeyCode.Space },
                ["Pause"] = new[] { KeyCode.P },
                ["Resume"] = new[] { KeyCode.R },
                ["Cancel"] = new[] { KeyCode.C }
            });

        public ILogger Debug { get; } = new DebugLogAdapter();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Interfaces;

namespace Lunar.Adapters.Unity
{
    public class InputActionsAdapter : IInputActions
    {
        public Dictionary<string, KeyCode[]> Bindings { get; }
        public IInput Input { get; }

        public InputActionsAdapter(IInput input, Dictionary<string, KeyCode[]> bindings)
        {
            Input = input;
            Bindings = bindings;
        }

        public bool SetBinding(string action, params KeyCode[] keys)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return false;
            }

            if (keys.Length == 0)
            {
                return false;
            }

            if (keys.Any(key => !Enum.IsDefined(typeof(KeyCode), key)))
            {
                return false;
            }

            var copy = keys.ToArray();
            Bindings[action] = copy;
            return true;
        }

        public bool RemoveBinding(string action)
        {
            return Bindings.Remove(action);
        }

        public void ClearBindings()
        {
            Bindings.Clear();
        }

        public bool GetActionDown(string action)
        {
            return Bindings.TryGetValue(action, out var keys) && keys.Any(key => Input.GetKeyDown(key));
        }

        public bool GetAction(string action)
        {
            return Bindings.TryGetValue(action, out var keys) && keys.Any(key => Input.GetKey(key));
        }

        public bool GetActionUp(string action)
        {
            return Bindings.TryGetValue(action, out var keys) && keys.Any(key => Input.GetKeyUp(key));
        }
    }
}
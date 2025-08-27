using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lunar.Interfaces;
using UnityEngine;
using System.Linq;

namespace Lunar.Adapters.Unity
{
    public class InputAdapter : IInput
    {
        public bool GetKeyDown(KeyCode keycode)
        {
            return Input.GetKeyDown(keycode.ToUnity());
        }

        public bool GetKey(KeyCode keycode)
        {
            return Input.GetKey(keycode.ToUnity());
        }

        public bool GetKeyUp(KeyCode keycode)
        {
            return Input.GetKeyUp(keycode.ToUnity());
        }
    }

    public static class KeyCodeConverter
    {
        private static readonly ConcurrentDictionary<KeyCode, UnityEngine.KeyCode> Cache = new();

        // If some key names are inconsistent on both sides, they can be overwritten here uniformly
        private static readonly Dictionary<KeyCode, UnityEngine.KeyCode> Overrides
            = new()
            {
                // [KeyCode.SomeLunarName] = UnityEngine.KeyCode.SomeDifferentUnityName,
            };

        public static UnityEngine.KeyCode ToUnity(this KeyCode key)
        {
            if (Overrides.TryGetValue(key, out var overridden))
            {
                return overridden;
            }

            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            if (!Enum.TryParse<UnityEngine.KeyCode>(key.ToString(), out var unityKey))
            {
                throw new InvalidOperationException($"Cannot map Lunar.KeyCode '{key}' to UnityEngine.KeyCode.");
            }

            Cache[key] = unityKey;
            return unityKey;

        }
    }
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
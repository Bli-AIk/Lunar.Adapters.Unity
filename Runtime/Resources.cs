using System;
using Lunar.Interfaces;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Lunar.Adapters.Unity
{
    public class Resources : IResources
    {
        public T Load<T>(string path)
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be loaded through Resources.Load.");
            }

            return (T)(object)UnityEngine.Resources.Load(path, typeof(T));
        }

        public void Release<T>(T resources)
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be released through Resources.UnloadAsset.");
            }

            UnityEngine.Resources.UnloadAsset(resources as Object);
        }
    }

    public class Addressables : IResources
    {
        [Obsolete("Obsolete")]
        public T Load<T>(string key)
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be loaded through Addressables.LoadAsset.");
            }
            return UnityEngine.AddressableAssets.Addressables.LoadAsset<T>(key).WaitForCompletion();
        } 

        public void Release<T>(T resources)
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be released through Resources.UnloadAsset.");
            }

            UnityEngine.AddressableAssets.Addressables.Release(resources as Object);
        }
    }
}
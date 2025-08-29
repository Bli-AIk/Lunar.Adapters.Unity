using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunar.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lunar.Adapters.Unity
{
    public static class ResourcesUtility
    {
        public static void ValidateUnityObjectType<T>()
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be loaded.");
            }
        }

        public static void ValidateUnityObjectType<T>(string methodName)
        {
            if (!typeof(Object).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Type {typeof(T)} is not UnityEngine.Object and cannot be loaded through {methodName}.");
            }
        }
    }

    public class ResourcesAdapter : IResources, IResourcesAsync
    {
        public T Load<T>(string path)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.Load");
            return (T)(object)Resources.Load(path, typeof(T));
        }

        public IEnumerable<T> LoadAll<T>(string path)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.LoadAll");
            return Resources.LoadAll(path, typeof(T)).Cast<T>();
        }

        public IEnumerable<T> LoadAll<T>(IEnumerable<string> paths)
        {
            return paths.Select(Load<T>);
        }

        public void Release<T>(T resource)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.UnloadAsset");
            Resources.UnloadAsset(resource as Object);
        }

        public Task<T> LoadAsync<T>(string path, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> LoadAllAsync<T>(string path, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }
    }
}
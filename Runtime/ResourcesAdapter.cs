using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunar.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

    public class ResourcesAdapter : IResources
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

        public void Release<T>(T resources)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.UnloadAsset");
            Resources.UnloadAsset(resources as Object);
        }
    }

    public class AddressablesAdapter : IResources, IResourcesAsync
    {
        [Obsolete("Obsolete")]
        public T Load<T>(string key)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAsset");
            return Addressables.LoadAsset<T>(key).WaitForCompletion();
        }

        [Obsolete("Obsolete")]
        public IEnumerable<T> LoadAll<T>(string path)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return LoadAll<T>(path, null);
        }


        [Obsolete("Obsolete")]
        public IEnumerable<T> LoadAll<T>(IEnumerable<string> paths)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return LoadAll<T>(paths, null);
        }

        public void Release<T>(T resources)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.Release");
            Addressables.Release(resources as Object);
        }

        public Task<T> LoadAsync<T>(string path, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var req = Addressables.LoadAssetAsync<T>(path);
            CancellationTokenRegistration ctr = default;

            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() => tcs.TrySetCanceled(ct));
            }

            req.Completed += handle =>
            {
                try
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        tcs.TrySetResult(handle.Result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Failed to load asset: {path}"));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };


            return tcs.Task;
        }

        public Task<IEnumerable<T>> LoadAllAsync<T>(string path, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");

            var tcs = new TaskCompletionSource<IEnumerable<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var req = Addressables.LoadAssetsAsync<T>(path, null);
            CancellationTokenRegistration ctr = default;

            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() => tcs.TrySetCanceled(ct));
            }

            req.Completed += handle =>
            {
                try
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        tcs.TrySetResult(handle.Result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Failed to load asset: {path}"));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };


            return tcs.Task;
        }

        public Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");

            var tcs = new TaskCompletionSource<IEnumerable<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var req = Addressables.LoadAssetsAsync<T>(paths, null);
            CancellationTokenRegistration ctr = default;

            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() => tcs.TrySetCanceled(ct));
            }

            req.Completed += handle =>
            {
                try
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        tcs.TrySetResult(handle.Result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Failed to load asset: {paths}"));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };


            return tcs.Task;
        }


        [Obsolete("Obsolete")]
        public static IEnumerable<T> LoadAll<T>(string path, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return Addressables.LoadAssets(path, callback).WaitForCompletion();
        }

        [Obsolete("Obsolete")]
        public static IEnumerable<T> LoadAll<T>(IEnumerable<string> paths, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return Addressables.LoadAssets(paths, callback).WaitForCompletion();
        }
    }
}
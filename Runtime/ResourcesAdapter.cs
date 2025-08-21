using System;
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

        public void Release<T>(T resources)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.UnloadAsset");
            Resources.UnloadAsset(resources as Object);
        }

        
        public Task<T> LoadAsync<T>(string path, CancellationToken ct = default)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.LoadAsync");

            if (ct.IsCancellationRequested)
            {
                return Task.FromCanceled<T>(ct);
            }

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var req = Resources.LoadAsync(path, typeof(T));
            CancellationTokenRegistration ctr = default;

            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() => tcs.TrySetCanceled(ct));
            }

            req.completed += _ =>
            {
                try
                {
                    if (req.asset is T t)
                    {
                        tcs.TrySetResult(t);
                    }
                    else
                    {
                        tcs.TrySetResult(default);
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };

            return tcs.Task;
        }
    }

    public class AddressablesAdapter : IResources
    {
        [Obsolete("Obsolete")]
        public T Load<T>(string key)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAsset");
            return Addressables.LoadAsset<T>(key).WaitForCompletion();
        }

        public void Release<T>(T resources)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.Release");
            Addressables.Release(resources as Object);
        }

        public Task<T> LoadAsync<T>(string key, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");
            
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var req = Addressables.LoadAssetAsync<T>(key);
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
                        tcs.TrySetException(new Exception($"Failed to load asset: {key}"));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };


            return tcs.Task;
        }
    }
}
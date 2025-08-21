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
        [Obsolete("Use LoadAsync instead.")]
        public T Load<T>(string key)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAsset");
            return Addressables.LoadAsset<T>(key).WaitForCompletion();
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public IEnumerable<T> LoadAll<T>(string path)
        {
            return LoadAll<T>(path, null);
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public IEnumerable<T> LoadAll<T>(IEnumerable<string> paths)
        {
            return LoadAll<T>(paths, null);
        }

        public void Release<T>(T resource)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.Release");
            if (resource is Object obj)
            {
                Addressables.Release(obj);
            }
        }

        public Task<T> LoadAsync<T>(string path, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");
            var handle = Addressables.LoadAssetAsync<T>(path);
            return HandleAsyncOperation(handle, path, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(paths, null, Addressables.MergeMode.None, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(path, null, Addressables.MergeMode.None, ct);
        }


        public static async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            Action<T> callback,
            Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetsAsync");
            var handle = Addressables.LoadAssetsAsync(path, callback, mergeMode);
            return await HandleAsyncOperation(handle, path, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(path, null, mergeMode, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            Action<T> callback,
            CancellationToken ct = new())
        {
            return await LoadAllAsync(path, callback, Addressables.MergeMode.None, ct);
        }


        public async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(paths, null, mergeMode, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            Action<T> callback,
            CancellationToken ct = new())
        {
            return await LoadAllAsync(paths, callback, Addressables.MergeMode.None, ct);
        }

        public static async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            Action<T> callback,
            Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetsAsync");
            var handle = Addressables.LoadAssetsAsync(paths, callback, mergeMode);
            return await HandleAsyncOperation(handle, paths, ct);
        }

        /// <summary>
        ///     A private helper method to convert an AsyncOperationHandle into a Task.
        ///     This encapsulates the logic for handling completion, cancellation, and exceptions.
        /// </summary>
        /// <param name="handle">The Addressables async operation handle.</param>
        /// <param name="key">The key or path used for loading, for exception messages.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <typeparam name="TResult">The result type of the operation.</typeparam>
        /// <returns>A Task representing the async operation.</returns>
        private static Task<TResult> HandleAsyncOperation<TResult>(AsyncOperationHandle<TResult> handle,
            object key,
            CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);

            CancellationTokenRegistration ctr = default;
            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() =>
                {
                    Addressables.Release(handle);
                    tcs.TrySetCanceled(ct);
                });
            }

            handle.Completed += h =>
            {
                try
                {
                    if (h.Status == AsyncOperationStatus.Succeeded)
                    {
                        tcs.TrySetResult(h.Result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Failed to load asset with key: {key}. Status: {h.Status}",
                            h.OperationException));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };

            return tcs.Task;
        }


        [Obsolete("Use LoadAllAsync instead.")]
        public static IEnumerable<T> LoadAll<T>(string path, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return Addressables.LoadAssets(path, callback).WaitForCompletion();
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public static IEnumerable<T> LoadAll<T>(IEnumerable<string> paths, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return Addressables.LoadAssets(paths, callback).WaitForCompletion();
        }
    }
}
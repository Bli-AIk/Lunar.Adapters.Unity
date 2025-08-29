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


        public Task<IEnumerable<T>> LoadAllAsync<T>(string path, CancellationToken ct = default)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Resources.LoadAll");

            if (ct.IsCancellationRequested)
            {
                return Task.FromCanceled<IEnumerable<T>>(ct);
            }

            var tcs = new TaskCompletionSource<IEnumerable<T>>(TaskCreationOptions.RunContinuationsAsynchronously);

            Task.Run(() =>
            {
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(ct);
                        return;
                    }

                    var assets = Resources.LoadAll(path, typeof(T)).Cast<T>();
                    tcs.TrySetResult(assets);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, ct);

            return tcs.Task;
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths, CancellationToken ct = default)
        {
            var results = new List<T>();

            foreach (var path in paths)
            {
                ct.ThrowIfCancellationRequested();

                var asset = await LoadAsync<T>(path, ct).ConfigureAwait(false);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }

            return results;
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lunar.Adapters.Unity.Tests
{
    public class ResourcesUtilityTests
    {
        [Test]
        public void ValidateUnityObjectType_ThrowsForNonUnityType_NoMethodName()
        {
            var ex = Assert.Throws<InvalidOperationException>(ResourcesUtility.ValidateUnityObjectType<int>);
            StringAssert.Contains("System.Int32", ex.Message);
            StringAssert.Contains("cannot be loaded", ex.Message);
        }

        [Test]
        public void ValidateUnityObjectType_ThrowsForNonUnityType_WithMethodName()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                ResourcesUtility.ValidateUnityObjectType<int>("MyMethod");
            });
            StringAssert.Contains("MyMethod", ex.Message);
            StringAssert.Contains("cannot be loaded through MyMethod", ex.Message);
        }

        [Test]
        public void ValidateUnityObjectType_AllowsUnityObjectType()
        {
            // Should not throw for UnityEngine.Object derived types
            Assert.DoesNotThrow(ResourcesUtility.ValidateUnityObjectType<TextAsset>);
            Assert.DoesNotThrow(() => ResourcesUtility.ValidateUnityObjectType<GameObject>("LoadGameObject"));
        }
    }

    public class ResourcesAdapterTests
    {
        private ResourcesAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _adapter = new ResourcesAdapter();
        }

        [Test]
        public void Load_TextAsset_ReturnsAsset()
        {
            // Ensure you have TestAssets/one.txt in Resources
            var asset = _adapter.Load<TextAsset>("TestAssets/one");
            Assert.IsNotNull(asset, "TextAsset 'TestAssets/one' should be found in Resources/TestAssets/one.txt");
            Assert.AreEqual("ONE", asset.text.Trim());
        }

        [Test]
        public void LoadAll_ByFolder_ReturnsAll()
        {
            // Ensure TestAssets/one.txt and TestAssets/two.txt exist
            var all = _adapter.LoadAll<TextAsset>("TestAssets").ToList();
            Assert.IsNotNull(all);
            // expect at least two that we placed
            Assert.IsTrue(all.Count >= 2,
                $"Expected at least 2 TextAssets under Resources/TestAssets but got {all.Count}");
            var texts = all.Select(t => t.text.Trim()).ToList();
            CollectionAssert.Contains(texts, "ONE");
            CollectionAssert.Contains(texts, "TWO");
        }

        [Test]
        public void LoadAll_ByPathsEnumerable_ReturnsEach()
        {
            var paths = new[] { "TestAssets/one", "TestAssets/two" };
            var items = _adapter.LoadAll<TextAsset>(paths).ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual("ONE", items[0].text.Trim());
            Assert.AreEqual("TWO", items[1].text.Trim());
        }

        [Test]
        public void Release_TextAsset_DoesNotThrow()
        {
            var asset = _adapter.Load<TextAsset>("TestAssets/one");
            Assert.IsNotNull(asset);
            Assert.DoesNotThrow(() => _adapter.Release(asset));
            // Resources.UnloadAsset doesn't provide a return; we just ensure Release doesn't throw
        }
    }

    public class AddressablesAdapterTypeValidationTests
    {
        private AddressablesAdapter _addrAdapter;

        [SetUp]
        public void SetUp()
        {
            _addrAdapter = new AddressablesAdapter();
        }

        [Test]
        public void LoadAsync_ThrowsImmediately_ForNonUnityType()
        {
            // Should throw before any Addressables call because of type validation
            Assert.Throws<InvalidOperationException>(() =>
            {
                // We just call the method; it should synchronously throw
                var _ = _addrAdapter.LoadAsync<int>("someKey");
            });
        }

        [Test]
        public void LoadAllAsync_StringPath_ThrowsImmediately_ForNonUnityType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                // static overload also validates type right away
                var task = AddressablesAdapter.LoadAllAsync<int>("somePath", null, Addressables.MergeMode.None,
                    CancellationToken.None);
            });
        }

        [Test]
        [Obsolete("Obsolete")]
        public void Obsolete_LoadAll_Static_ThrowsImmediately_ForNonUnityType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = AddressablesAdapter.LoadAll<int>("somePath", null);
            });
        }
    }
}
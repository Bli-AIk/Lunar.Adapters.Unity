using System;
using System.Collections.Generic;
using Lunar.Core.Base;
using NUnit.Framework;

namespace Lunar.Adapters.Unity.Tests
{
    [TestFixture]
    public class KeyCodeConverterTests
    {
        [Test]
        public void All_LunarKeyCodes_MapTo_UnityEngineKeyCode()
        {
            var failed = new List<string>();

            foreach (KeyCodeBase lunarKey in Enum.GetValues(typeof(KeyCodeBase)))
            {
                try
                {
                    var unityKey = lunarKey.ToUnity();
                    _ = (int)unityKey;
                }
                catch (InvalidOperationException)
                {
                    failed.Add(lunarKey.ToString());
                }
                catch (Exception ex)
                {
                    failed.Add($"{lunarKey} (threw {ex.GetType().Name}: {ex.Message})");
                }
            }

            Assert.IsEmpty(failed, 
                $"以下 Lunar.KeyCode 未能映射到 UnityEngine.KeyCode，请在 KeyCodeConverter 的 overrides 中补上或修正命名: {string.Join(", ", failed)}");
        }
    }
}
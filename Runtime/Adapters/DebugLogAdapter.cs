using System;
using UnityEngine;
using ILogger = Lunar.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Lunar.Adapters.Unity
{
    public class DebugLogAdapter : ILogger
    {
        public void Log(object message, object context = null)
        {
            #if UNITY_EDITOR
            RunDebugLogger(Debug.Log, Debug.Log, message, context);
            #endif
        }

        public void LogWarning(object message, object context = null)
        {
            #if UNITY_EDITOR
            RunDebugLogger(Debug.LogWarning, Debug.LogWarning, message, context);
            #endif
        }

        public void LogError(object message, object context = null)
        {
            #if UNITY_EDITOR
            RunDebugLogger(Debug.LogError, Debug.LogError, message, context);
            #endif
        }

        private static void RunDebugLogger(
            Action<object> logNoContext,
            Action<object, Object> logWithContext,
            object message,
            object context)
        {
            if (context is not Object obj)
            {
                logNoContext(message);
            }
            else
            {
                logWithContext(message, obj);
            }
        }
    }
}
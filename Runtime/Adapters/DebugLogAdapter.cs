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
            RunDebugLogger(Debug.Log, Debug.Log, message, context);
        }

        public void LogWarning(object message, object context = null)
        {
            RunDebugLogger(Debug.LogWarning, Debug.LogWarning, message, context);
        }

        public void LogError(object message, object context = null)
        {
            RunDebugLogger(Debug.LogError, Debug.LogError, message, context);
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
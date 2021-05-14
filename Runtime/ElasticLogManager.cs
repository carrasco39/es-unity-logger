using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace ESUnityLogger
{
    public class ElasticLogManager
    {
        static LogDelivery elasticSearchDelivery;

        static bool isInitialized = false;
        
        static Guid sessionId;
        
        static string appVersion;

        private static float messageIndex = 0;
        static LogCache logCache;

        static ElasticLogManager()
        {
            appVersion = Application.version;
        }

        public static void Initialize(Guid sessionId, ElasticConfig config = null)
        {
            try
            {
                if (config == null)
                    config = DefaultConfig();
                
                elasticSearchDelivery = new LogDelivery(config);

                ElasticLogManager.sessionId = sessionId;

                CreateSession(true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Dispose();
            }

            Application.quitting += Dispose;
            Application.logMessageReceivedThreaded += LogCallback;
            isInitialized = true;
        }

        private static ElasticConfig DefaultConfig()
        {
            return new ElasticConfig
            {

                indexPrefix = "log",
                indexSuffixPattern = "yyyy-MM-dd",
                address = "http://localhost:9200",
                user = "user",
                password = "12345",
                transformer = DefaultTransformer
            };
        }

        public static ElasticReturn DefaultTransformer(LogCacheMessage cache)
        {
            CustomElasticReturn returnMessage = new CustomElasticReturn();

            returnMessage.level = cache.type.ToString();
            returnMessage.message = cache.message;
            returnMessage.timestamp = cache.timestamp;

            string stack = cache.stack;
            
            if (cache.type != LogType.Error && cache.type != LogType.Exception && stack.Contains('\n'))
                stack = stack.Split('\n') [1];

            returnMessage.fields = new Fields()
            {
                AppVersion = appVersion,
                Stack = stack,
                SessionId = sessionId.ToString(),
                MessageIndex = messageIndex++,
                Application = "Unity",
                Source = "Default"
            };

            return returnMessage;
        }

        public static void Dispose()
        {
            isInitialized = false;
            DisposeSession(logCache);
            
            elasticSearchDelivery.Dispose();
            Application.quitting -= Dispose;
            Application.logMessageReceived -= LogCallback;
        }

        static void CreateSession(bool rethrowInternalExceptions = false)
        {
            try
            {
                logCache = new LogCache();
            }
            catch (Exception e)
            {
                if (rethrowInternalExceptions)
                    throw e;

                Debug.LogException(e);
            }
        }

        static void DisposeSession(LogCache sessionCache)
        {
            try
            {
                sessionCache.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        static void LogCallback(string condition, string stackTrace, LogType type)
        {
            string concat = condition + stackTrace;
            logCache.AddLog(type, concat, condition, stackTrace);
        }

        [Preserve]
        class LogCache
        {
            public readonly Dictionary<LogType, int> TypeCount;
            public readonly Dictionary<int, LogCounter> CachedLogs;
            
            public LogCache()
            {
                TypeCount = new Dictionary<LogType, int>();
                CachedLogs = new Dictionary<int, LogCounter>();
            }

            public void AddLog(LogType type, string log, string condition, string stackTrace)
            {
                int logHash = log.GetHashCode();

                if (TypeCount.ContainsKey(type))
                    TypeCount[type]++;
                else
                    TypeCount[type] = 1;

                if (!CachedLogs.TryGetValue(logHash, out LogCounter cache))
                {
                    cache = new LogCounter(logHash, type, log);
                    CachedLogs[logHash] = cache;
                    
                    elasticSearchDelivery.SendLog(type, condition, stackTrace);
                }
                else
                    cache.Increment();
            }

            public void Dispose()
            {
                try
                {
                    elasticSearchDelivery.Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        [Preserve]
        struct LogCounter
        {
            public readonly int Hash;
            public readonly LogType Type;
            public readonly string Log;
            public int Count { get; private set; }

            public LogCounter(int hash, LogType type, string log)
            {
                Hash = hash;
                Type = type;
                Log = log;
                Count = 1;
            }

            public void Increment()
            {
                Count++;
            }
        }
    }
}
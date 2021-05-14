using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ESUnityLogger
{
    public class LogDelivery
    {
        private ElasticConfig _elasticConfig;
        private List<LogCacheMessage> logList = new List<LogCacheMessage>();
        private CancellationTokenSource cancellationTokenSource;
        //private Client elasticClient;
        private bool includeLogs = false;

        public LogDelivery(ElasticConfig elasticConfig)
        {
            _elasticConfig = elasticConfig;
                
            if (_elasticConfig.transformer == null)
                _elasticConfig.transformer = DefaultTransformer;
            
            cancellationTokenSource = new CancellationTokenSource();
            
           ESCallerService.Setup(_elasticConfig);

#if DEBUG
            includeLogs = true;
#endif

            Task.Run(CheckLogs, cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            FlushMessages();
            cancellationTokenSource.Cancel();
        }

        public async Task CheckLogs()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(_elasticConfig.flushInterval));
                try
                {
                    await FlushMessages();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public void SendLog(LogType type, string message, string stack)
        {
            LogCacheMessage logCacheItem = new LogCacheMessage(type, message, stack);

            lock (logList)
            {
                logList.Add(logCacheItem);
            }

            if(_elasticConfig.bufferLimit > 0 && logList.Count >= _elasticConfig.bufferLimit)
                FlushMessages();
        }

        private async Task FlushMessages()
        {
            List<ElasticReturn> bulkMessages = new List<ElasticReturn>();

            lock (logList)
            {
                foreach (LogCacheMessage logCache in logList)
                {
                    ElasticReturn elasticReturn = _elasticConfig.transformer(logCache);
                    
                    if(includeLogs || logCache.type == LogType.Error || logCache.type == LogType.Exception)
                        bulkMessages.Add(elasticReturn);
                }

                if (bulkMessages.Count <= 0)
                    return;

                logList.Clear();
            }

            ElasticBulkMessage elasticBulkMessage = new ElasticBulkMessage()
            {
                messages = bulkMessages,
                index = _elasticConfig.index
            };

            await ElasticsearchSendMessages(elasticBulkMessage);
        }

        private async Task ElasticsearchSendMessages(ElasticBulkMessage message)
        {
            try
            {
                await ESCallerService.BulkAsync(message);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError($"Exception while emitting periodic batch from {this}: {ex}");
            }
        }

        private ElasticReturn DefaultTransformer(LogCacheMessage cache)
        {
            DefaultElasticReturn returnMessage = new DefaultElasticReturn();

            returnMessage.level = cache.type.ToString();
            returnMessage.message = cache.message;
            returnMessage.timestamp = cache.timestamp;

            return returnMessage;
        }
    }
}
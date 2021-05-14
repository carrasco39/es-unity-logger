using System;
using UnityEngine;

namespace ESUnityLogger
{
    public class LogCacheMessage
    {
        public LogType type;
        public string message;
        public string stack;
        
        public string timestamp;

        public LogCacheMessage(LogType type, string message, string stack)
        {
            this.type = type;
            this.message = message;
            this.stack = stack;
            
            timestamp = DateTime.Now.ToUniversalTime()
                .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}
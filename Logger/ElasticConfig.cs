using System;

namespace ESUnityLogger
{
    public class ElasticConfig
    {
        public string indexPrefix = "logs";
        public string indexSuffixPattern = "YYYY.MM.DD";
        public Func<LogCacheMessage, ElasticReturn> transformer;
        public float flushInterval = 2000;
        public int bufferLimit = 0;
        public string address;
        public string user;
        public string password;

        public string index => indexPrefix + "-" + DateTime.Now.ToUniversalTime().ToString(indexSuffixPattern);
    }
}
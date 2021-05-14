using System.Collections.Generic;

namespace ESUnityLogger
{
    public class ElasticBulkMessage
    {
        public string index;
        public List<ElasticReturn> messages;
    }
}
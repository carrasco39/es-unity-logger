using Newtonsoft.Json;

namespace ESUnityLogger
{
    
    [JsonObject]

    public abstract class ElasticReturn
    {
        [JsonProperty(PropertyName = "@timestamp")]
        public string timestamp;
        [JsonProperty]
        public string message;
        [JsonProperty]
        public string level;
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
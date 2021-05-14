namespace ESUnityLogger
{
    public class CustomElasticReturn : ElasticReturn
    {
        public Fields fields;
    }

    public class Fields
    {
        public string Application;
        public string Source;
        public string Stack;
        public string SessionId;
        public string AppVersion;
        public float MessageIndex;
        public string SystemInfo;
    }
}
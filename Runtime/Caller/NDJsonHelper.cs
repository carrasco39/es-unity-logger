using System.Collections.Generic;
using Newtonsoft.Json;

namespace ESUnityLogger
{
    public static class NDJsonHelper
    {
        public static string ToNDJson(params object[] items)
        {
            string result = "";
            foreach (var item in items)
            {
                result += JsonConvert.SerializeObject(item);

                result += "\n";
            }

            return result;
        }
        
        public static string ToNDJson<T>(IEnumerable<T> items)
        {
            string result = "";
            foreach (var item in items)
            {
                result += JsonConvert.SerializeObject(item);

                
                result += "\n";
            }

            //result = result.Remove(result.Length - 1);
            return result;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ESUnityLogger
{
    public class ESCallerService
    {
        private static ESCallerService instance;

        private UnityCaller caller;
        public static void Setup(ElasticConfig config)
        {
            instance = new ESCallerService();
            instance.caller = new UnityCaller();
            instance.caller.Setup(new Uri(config.address),new BasicAuthentication(config.user,config.password));
        }

        public static async UniTask<bool> BulkAsync(ElasticBulkMessage msg)
        {
            string indexJson = "{\"index\": {\"_index\": \"" + msg.index + "\"}}\n";
            string json = indexJson + NDJsonHelper.ToNDJson(msg.messages);

            await UniTask.SwitchToMainThread();
            var response = await instance.caller.PostNDJson($"{msg.index}/_bulk", json);

            if (!response.IsSuccess)
            {
                Debug.LogError(response.Error);
            }
            return response.IsSuccess;
        }
    }
}
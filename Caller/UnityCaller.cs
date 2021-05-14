using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ESUnityLogger
{
    public interface IResponse
    {
        bool IsSuccess { get; }
        string Error { get; }
    }
    
    public class UnityCaller : ICaller
    {
        Uri baseUrl;
        IAuthentication auth; 
        public void Setup(Uri uri,IAuthentication authentication)
        {
            baseUrl = uri;
            auth = authentication;
        }
        
        public async UniTask<IResponse> PostNDJson(string path, string ndjson)
        {
            UnityWebRequest request = new UnityWebRequest(baseUrl+path, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(ndjson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("AUTHORIZATION", auth.ToString());
            request.SetRequestHeader("Content-Type", "application/json");
            
            try
            {
                var response = await request.SendWebRequest();
                
                return new BasicUnityResponse(response);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
    }
}
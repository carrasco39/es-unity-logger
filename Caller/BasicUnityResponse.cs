using UnityEngine.Networking;

namespace ESUnityLogger
{
    public class BasicUnityResponse : IResponse
    {
        private bool isSuccess;
        private string error;
        public BasicUnityResponse(UnityWebRequest request)
        {
            isSuccess = request.result == UnityWebRequest.Result.Success;
            error = request.error;
        }

        public bool IsSuccess => isSuccess;
        public string Error { get; }
    }
}
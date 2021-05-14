using System;
using Cysharp.Threading.Tasks;

namespace ESUnityLogger
{
    public interface ICaller
    {
        void Setup(Uri uri, IAuthentication authentication);
        UniTask<IResponse> PostNDJson(string path, string ndjson);
    }
}
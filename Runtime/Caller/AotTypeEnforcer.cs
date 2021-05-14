using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace ESUnityLogger
{
    public class AotTypeEnforcer : MonoBehaviour
    {
        void Awake()
        {
            AotHelper.EnsureType<StringEnumConverter>();
        }
    }
}
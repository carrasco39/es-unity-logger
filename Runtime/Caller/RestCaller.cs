// using System;
// using System.IO;
// using System.Net;
// using System.Text;
// using System.Threading;
// using Cysharp.Threading.Tasks;
// using Newtonsoft.Json;
// using RestSharp;
// using RestSharp.Authenticators;
// using UnityEngine;
//
//
// namespace Gunstars.Logging.RestCaller
// {
//     public static class RestCaller
//     {
//         static RestClient client;
//
//         public static void Setup(Uri uri, IAuthenticator authenticator)
//         {
//             client = new RestClient(uri);
//             client.Authenticator = authenticator;
//             
//             client.ConfigureWebRequest((r) =>
//             {
//                 r.ServicePoint.Expect100Continue = false;
//                 r.KeepAlive = true;
//             });
//             
//             ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//         }
//
//         public static async UniTask<RestResponse> PostNDJson(string path, string ndjson)
//         {
//             RestRequest request = new RestRequest(path, Method.POST);
//             request.AdvancedResponseWriter += (input, response) => response.RawBytes = ReadAsBytes(input);
//             try
//             {
//                 request.AddParameter("application/json", ndjson, ParameterType.RequestBody);
//
//                 RestResponse response = await client.PostAsync<RestResponse>(request);
//                 return response;
//             }
//             catch (Exception e)
//             {
//                 Debug.Log(e);
//                 return null;
//             }
//         }
//         
//         private static byte[] ReadAsBytes(Stream input)
//         {
//             var buffer = new byte[16 * 1024];
//
//             using (var ms = new MemoryStream())
//             {
//                 int read;
//                 try
//                 {
//                     while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
//                     { ms.Write(buffer, 0, read); }
//
//                     return ms.ToArray();
//                 }
//                 catch (WebException ex)
//                 { return Encoding.UTF8.GetBytes(ex.Message); }
//             };
//         }
//     }
// }

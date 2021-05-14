using System;
using System.Text;

namespace ESUnityLogger
{
    public class BasicAuthentication : IAuthentication
    {
        string user;
        string pass;
        public BasicAuthentication(string user, string pass)
        {
            this.user = user;
            this.pass = pass;
        }

        public override string ToString()
        {
            string auth = $"{user}:{pass}";
            auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            return auth;
        }
    }
}
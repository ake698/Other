using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace URL
{
    class Utils
    {
        public static HttpWebRequest GetCommonRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Mobile Safari/537.36";
            request.Method = "GET";
            request.Accept = "application/json, text/plain, */*";
            return request;
        }


    }
}

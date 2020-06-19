using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace URL
{
    public class CommonRequest
    {
        private CookieContainer cookie;
        public CommonRequest()
        {
            cookie = new CookieContainer();
        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public JObject HttpPost(string url, Dictionary<string, string> dic)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Mobile Safari/537.36";
            request.Method = "POST";
            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            SetHeaderValue(request.Headers, "Host", "passport.weibo.cn");
            SetHeaderValue(request.Headers, "Origin", "https://passport.weibo.cn");
            SetHeaderValue(request.Headers, "Referer", "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=https://m.weibo.cn/");
            request.CookieContainer = cookie;
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                //return JavaScriptConvert.SerializeObject(reader.ReadToEnd());
                //return reader.ReadToEnd();
                string result = reader.ReadToEnd();
                var a = JsonConvert.SerializeObject(result);
                JObject b;
                try
                {
                    b = (JObject)JsonConvert.DeserializeObject(result);
                }
                catch (JsonReaderException)
                {
                    return null;
                }
                return b;
            }
        }


        public JObject HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Mobile Safari/537.36";
            request.Method = "POST";
            request.Accept = "application/json, text/plain, */*";
            request.CookieContainer = cookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                //return JavaScriptConvert.SerializeObject(reader.ReadToEnd());
                //return reader.ReadToEnd();
                string result = reader.ReadToEnd();
                var a = JsonConvert.SerializeObject(result);
                JObject b;
                try
                {
                    b = (JObject)JsonConvert.DeserializeObject(result);
                }
                catch (JsonReaderException)
                {
                    return null;
                }

                return b;
            }
        }
    }
}

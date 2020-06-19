using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace URL
{
    public class Login
    {
        private string username;
        private string password;
        public Login(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public CommonRequest LoginPost()
        {
            CommonRequest request = new CommonRequest();
            var loginDic = PopulateParam();
            var response = request.HttpPost("https://passport.weibo.cn/sso/login", loginDic);
            Debug.WriteLine(response);
            return request;
            
        }

        public Dictionary<string,string> PopulateParam()
        {
            var loginDic = new Dictionary<string, string>();
            loginDic.Add("username", this.username);
            loginDic.Add("password", this.password);
            loginDic.Add("savestate", "1");
            loginDic.Add("r", "https://m.weibo.cn/");
            loginDic.Add("ec", "0");
            loginDic.Add("entry", "mweibo");
            loginDic.Add("mainpageflag", "1");
            return loginDic;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace URL
{
    public class Login
    {
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
            loginDic.Add("username", "15873884096");
            loginDic.Add("password", "aa3611698");
            loginDic.Add("savestate", "1");
            loginDic.Add("r", "https://m.weibo.cn/");
            loginDic.Add("ec", "0");
            loginDic.Add("entry", "mweibo");
            loginDic.Add("mainpageflag", "1");
            return loginDic;
        }

    }
}

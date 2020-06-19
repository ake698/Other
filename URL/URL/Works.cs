using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace URL
{
    
    class Works
    {
        //更新按钮委托
        public delegate void AsyncSetButton(int type);
        public AsyncSetButton setButton;
        //消息提醒委托
        public delegate void AsyncMessageTip(string msg);
        public AsyncMessageTip messageTip;
        private string uid;
        private CommonRequest request;
        //不能小于2 对比天数
        private readonly int days = 30;
        //文件生成目录
        private readonly string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //文件名字
        private readonly string WBFILE = "WBURL" + DateTime.Now.ToString("yyyymmddhhmmss") + ".txt";
        private readonly string PLFILE = "PLURL" + DateTime.Now.ToString("yyyymmddhhmmss") + ".txt";
        private readonly string baseUrl = "https://m.weibo.cn/";
        public string username;
        public string password;

        public Works(string uid)
        {
            this.uid = uid;
            this.baseUrl = "https://m.weibo.cn/" + uid + '/';
            //this.request = new CommonRequest();
            
        }

        public void Start()
        {
            setButton(2);
            bool loginFlag = this.Login();
            if (!loginFlag)
            {
                setButton(1);
                return;
            }
            string more = GetContainerId(uid);
            List<string> ids = GetAllWB(more);
            GetCommentUrl(ids);
            #region 评论获取测试
            //List<string> ids = new List<string>();
            //ids.Add("4512401191748679");
            //GetCommentUrl(ids);
            #endregion
            setButton(1);
            Debug.WriteLine("done");
        }


        /// 获取用户containId 用于获取所有微博
        private string GetContainerId(string uid)
        {
            string url = "https://m.weibo.cn/profile/info?uid=" + uid;
            var result = request.HttpGet(url);
            string more = result["data"]["more"].ToString();
            more = more.Replace("/p/", "");
            return more;
        }
        /// <summary>
        /// 获取所有微博
        /// </summary>
        /// <param name="more"></param>
        /// <returns></returns>
        private List<string> GetAllWB(string more)
        {

            List<string> ids = new List<string>();
            //用于设置循环获取微博
            bool flag = true;
            int page = 1;
            int count = 0;
            while (flag)
            {
                string url = "https://m.weibo.cn/api/container/getIndex?&page_type=03&page=" + page
     + "&containerid=" + more;
                Debug.WriteLine(url);
                JObject result;
                try
                {
                    result = request.HttpGet(url);
                }
                catch (WebException)
                {
                    Debug.WriteLine("请求超时，等待中...");
                    Thread.Sleep(10000);
                    continue;
                }
                
                var cards = result["data"]["cards"];
                //最后一条数据不为9代表无数据了
                if (cards.Last["card_type"].ToString() != "9")
                {
                    Debug.WriteLine("无数据！！");
                    flag = false;
                    break;
                }
                foreach (var card in cards)
                {
                    if (card["card_type"].ToString() != "9")
                    {
                        Debug.WriteLine("非正常数据!");
                        continue;
                    }
                    //判断是否为置顶
                    string time = card["mblog"]["created_at"].ToString();
                    flag = TimeComparison(time);
                    if (card["mblog"]["ad_state"] != null)
                    {
                        //为置顶
                        flag = true;
                        Debug.WriteLine("此微博为置顶");
                    }
                    string id = card["mblog"]["id"].ToString();
                    if (flag)
                    {
                        ids.Add(id);
                    }
                    else
                    {
                        Debug.WriteLine("时间超过...");
                        //设置时间  超过5条则关闭
                        count++;
                        if (count > 5) break;
                        else
                        {
                            continue;
                        }

                    }
                    Debug.WriteLine(time + "天的数据");
                    Debug.WriteLine(card["mblog"]["id"].ToString());
                    Debug.WriteLine(card["mblog"]["text"].ToString());
                    Debug.WriteLine("----------------");
                }
                Thread.Sleep(1200);
                page++;
            }

            Thread.Sleep(2000);
            return ids;
        }

        //时间判断
        private bool TimeComparison(string time)
        {
            var t = time.Split('-');
            if (t.Length > 2) return false;
            if (t.Length < 2) return true;
            DateTime now = DateTime.Now;
            string blogTimeStr = now.Year.ToString() + t[0] + t[1];
            DateTime blogTime = DateTime.ParseExact(blogTimeStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

            TimeSpan ts1 = new TimeSpan(now.Ticks);
            TimeSpan ts2 = new TimeSpan(blogTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            Debug.WriteLine(ts.Days.ToString());
            if (int.Parse(ts.Days.ToString()) > days) return false;
            return true;
        }

        //获取所有微博内容信息
        private void GetCommentUrl(List<string> ids)
        {
            //写所有的微博
            FileStream wbFile = new FileStream(dir + WBFILE, FileMode.Create, FileAccess.Write);
            StreamWriter wbWrite = new StreamWriter(wbFile);
            //写所有的内评
            FileStream plFile = new FileStream(dir + PLFILE, FileMode.Create, FileAccess.Write);
            StreamWriter plWrite = new StreamWriter(plFile);
            //遍历所有的微博
            foreach (string id in ids)
            {
                //处理微博评论内容
                HandleWBAllComments(id, wbWrite, plWrite);
                //处理完一条微博休息1s
                Thread.Sleep(3000);
            }

            plWrite.Close();
            plFile.Close();
            wbWrite.Close();
            wbFile.Close();
        }
        /// <summary>
        /// 获取一条微博的所有评论信息
        /// </summary>
        /// <param name="id">微博id</param>
        private void HandleWBAllComments(string id, StreamWriter wbWrite, StreamWriter plWrite)
        {
            bool hasNext = true;
            Debug.WriteLine("获取 https://m.weibo.cn/detail/" + id + " 微博评论");
            string url = "https://m.weibo.cn/comments/hotflow?id=" + id + "&mid=" + id + "&max_id_type=0";
            JObject result;
            try
            {
                result = request.HttpGet(url);
            }
            catch (WebException)
            {
                Debug.WriteLine("访问微博过频繁，休息会...");
                Thread.Sleep(10000);
                return;
            }
            
            //评论页数
            int max = int.Parse(result["data"]["max"].ToString());
            //当前页数
            int current = 1;
            Debug.WriteLine("共有" + max + "页");
            while (current <= max && current<= 2) 
            {
                Debug.WriteLine(url);
                CommentDataHandle(id, result, wbWrite, plWrite);
                if (result["ok"].ToString() == "0")
                {
                    //无评论
                    hasNext = false;
                    Debug.WriteLine("此微博评论到此为止");
                    continue;
                }
                else
                {
                    //存在下一页评论
                    //开始访问下一页
                    //max = result["data"]["max"].ToString();
                    string max_id = result["data"]["max_id"].ToString();
                    url = "https://m.weibo.cn/comments/hotflow?id=" + id
                    + "&mid=" + id
                    + "&max_id=" + max_id
                    + "&max_id_type=0";
                    result = request.HttpGet(url);
                    try
                    {
                        var checkdata = result["data"]["data"];
                    }
                    catch (NullReferenceException)
                    {
                        Debug.WriteLine("取值异常，休息片刻..");
                        Debug.WriteLine(result);
                        Thread.Sleep(5000);
                        result = request.HttpGet(url);
                    }
                    current++;
                    Thread.Sleep(3000);
                }
                Debug.WriteLine("评论" + (current-1).ToString() + "页,hasNext:"+hasNext.ToString());
            }
        }


        private void CommentDataHandle(string id, JObject data, StreamWriter write, StreamWriter write2)
        {
            Debug.WriteLine("开始处理评论");
            string wbUrl = this.baseUrl + id;
            write.WriteLine(wbUrl);
            Debug.WriteLine(wbUrl);
            foreach (var comment in data["data"]["data"])
            {
                string rootMid = comment["mid"].ToString();
                //需要判断此评论是否为用户自己的评论
                string userId = comment["user"]["id"].ToString();
                //Debug.WriteLine(comment["text"]);
                if (userId.Equals(uid))
                {
                    string rootResult = wbUrl + "||" + rootMid;
                    Debug.WriteLine("一级评论" + rootResult);
                    write2.WriteLine(rootResult);
                }
                //二级评论 也就是评论的回复内容
                string secondComment = comment["comments"].ToString();
                if (secondComment != "false")
                {
                    //还存在子评论
                    foreach (var child in comment["comments"])
                    {
                        string childUserId = child["user"]["id"].ToString();
                        //判断回复评论是否为作者本身
                        if (childUserId.Equals(uid))
                        {
                            string childMid = child["mid"].ToString();
                            string childResult = wbUrl + "||" + childMid;
                            Debug.WriteLine("二级评论" + childResult);
                            write2.WriteLine(childResult);
                        }

                    }

                }
            }
        }


        //登录
        public bool Login()
        {
            Login login = new Login(this.username,this.password);
            this.request = login.LoginPost();
            var result = request.HttpGet("https://m.weibo.cn/api/remind/unread");
            Debug.WriteLine(result);
            var code = result["ok"].ToString();
            if(code == "1")
            {
                Debug.WriteLine("success");
                return true;
            }
            else
            {
                messageTip("登录失败！");
                Debug.WriteLine("error");
                return false;
            }
        }




    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace URL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //不能小于2
        private readonly int days = 30;
        private readonly string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private string baseUrl = "";
        private string uid;
        private readonly string fileName = "WBURL" + DateTime.Now.ToString("yyyymmddhhmmss") + ".txt";
        private readonly string fileName2 = "PLURL" + DateTime.Now.ToString("yyyymmddhhmmss") + ".txt";
        //给任务程使用
        private delegate void SetButoon(int x);
        private SetButoon setButoon;
        //主线程
        private delegate void AsynSetButton(int x);



        private void button1_Click(object sender, EventArgs e)
        {
            this.uid = this.textBox1.Text;
            this.baseUrl = "https://m.weibo.cn/" + uid + "/";
            this.setButoon = SetButtonStatus;

            //bool check = CheckUid(uid);
            //if (check)
            //{
            //    SetButtonStatus(2);
            //    string more = GetContainerId(uid);
            //    List<string> ids = GetAllWB(more);
            //    GetCommentUrl(ids);
            Thread thread = new Thread(new ThreadStart(Start));
            thread.Start();
            //SetButtonStatus(1);
        }

        private void Start()
        {
            setButoon(2);
            bool check = CheckUid(uid);
            if (check)
            {
                //SetButtonStatus(2);
                string more = GetContainerId(uid);
                List<string> ids = GetAllWB(more);
                GetCommentUrl(ids);
            }
            setButoon(1);
        }

        private bool CheckUid(string uid)
        {
            string url = "https://m.weibo.cn/profile/info?uid=" + uid;
            JObject result = HttpGet(url);
            if (result == null)
            {
                MessageTip("没有此用户！！");
                return false;
            }
            return true;
        }

        private JObject HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Mobile Safari/537.36";
            request.Method = "GET";
            request.Accept = "application/json, text/plain, */*";
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

        private string GetContainerId(string uid)
        {
            string url = "https://m.weibo.cn/profile/info?uid=" + uid;
            JObject result = HttpGet(url);
            string more = result["data"]["more"].ToString();
            more = more.Replace("/p/", "");
            return more;
        }
        ///p/2304133820903337_-_WEIBO_SECOND_PROFILE_WEIBO
        ///

        private List<string> GetAllWB(string more)
        {

            List<string> ids = new List<string>();
            bool flag = true;
            int page = 1;
            int count = 0;
            while (flag)
            {
                string url = "https://m.weibo.cn/api/container/getIndex?&page_type=03&page=" + page
     + "&containerid=" + more;
                Debug.WriteLine(url);
                var result = HttpGet(url);
                var cards = result["data"]["cards"];
                if(cards.Last["card_type"].ToString() != "9")
                {
                    Debug.WriteLine("无数据！！");
                    //return ids;
                    flag = false;
                    break;
                }
                foreach (var card in cards)
                {
                    if (card["card_type"].ToString() != "9")
                    {
                        Debug.WriteLine("第一条数据！");
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
                        Debug.WriteLine(flag);
                        //设置时间  超过3条则关闭
                        count++;
                        if (count > 5) break; 
                        else
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                    }
                    Debug.WriteLine(time);
                    Debug.WriteLine(card["mblog"]["id"].ToString());
                    Debug.WriteLine(card["mblog"]["text"].ToString());
                    Debug.WriteLine("----------------");
                }
                Thread.Sleep(1000);
                page++;
            }

            Thread.Sleep(1500);
            return ids;
        }

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
            FileStream file = new FileStream(dir + fileName, FileMode.Create, FileAccess.Write);
            //写所有的微博
            StreamWriter write = new StreamWriter(file);
            FileStream file2 = new FileStream(dir + fileName2, FileMode.Create, FileAccess.Write);
            //写所有的内评
            StreamWriter write2 = new StreamWriter(file2);

            foreach (string id in ids)
            {
                //通过第一页来获取评论信息
                Dictionary<string, string>  info = GetCommentInfo(id, write, write2);
                //没有评论的存在
                if (info["success"] == "400") continue;
                int max = int.Parse(info["max"]);
                Debug.WriteLine("此微博存在评论页数" + max);
                string max_id = info["max_id"];
                //处理分页评论
                for(int i = 1; i < max; i++)
                {
                    //无下一页
                    if (max_id == "0") break;
                    Thread.Sleep(200);
                    string url = "https://m.weibo.cn/comments/hotflow?id=" + id
                    + "&mid=" + id
                    + "&max_id=" + max_id
                    + "&max_id_type=0";
                    var result = HttpGet(url);
                    //Debug.WriteLine(url);
                    //Debug.WriteLine(result);
                    if(result != null && result["data"] != null)
                    {
                        //获取下一页的id
                        Debug.WriteLine("此微博ID" + id);
                        Debug.WriteLine(url);
                        max_id = result["data"]["max_id"].ToString();
                        CommentDataHandle(id, result, write, write2);
                    }
                    else
                    {
                        Debug.WriteLine("此页评论无法加载." + i);
                    }
                    
                }
            }
            
            write.Close();
            file.Close();
            write2.Close();
            file2.Close();
        }

        private Dictionary<string, string> GetCommentInfo(string id, StreamWriter write, StreamWriter write2)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            string success = "400";
            string url = "https://m.weibo.cn/comments/hotflow?id=" + id + "&mid=" + id + "&max_id_type=0";
            Debug.WriteLine(url);
            var result = HttpGet(url);
            if(result["data"] != null)
            {
                string max = result["data"]["max"].ToString();
                string max_id = result["data"]["max_id"].ToString();
                info.Add("max", max);
                info.Add("max_id", max_id);
                //success = "200";
                //存在评论
                CommentDataHandle(id, result, write, write2);
            }
            else
            {
                
                write.WriteLine(this.baseUrl + id);
                Debug.WriteLine("这篇微博没有评论");
            }
            info.Add("success", success);
            return info;
        }

        private void CommentDataHandle(string id, JObject data, StreamWriter write, StreamWriter write2)
        {
            string wbUrl = this.baseUrl + id;
            write.WriteLine(wbUrl);
            foreach (var comment in data["data"]["data"])
            {
                string rootMid = comment["mid"].ToString();
                //需要判断此评论是否为用户自己的评论
                string userId = comment["user"]["id"].ToString();
                if (userId.Equals(uid))
                {
                    string rootResult = wbUrl + "||" + rootMid;
                    write2.WriteLine(rootResult);
                }
                //二级评论 也就是评论的回复内容
                string secondComment = comment["comments"].ToString();
                if (secondComment != "false")
                {
                    //还存在子评论
                    foreach(var child in comment["comments"])
                    {
                        string childUserId = child["user"]["id"].ToString();
                        //判断回复评论是否为作者本身
                        if (childUserId.Equals(uid))
                        {
                            string childMid = child["mid"].ToString();
                            string childResult = wbUrl + "||" + childMid;
                            write2.WriteLine(childResult);
                        }
                        
                    }
                    
                }
            }
        }

        //private void SetButtonStatus(int type)
        //{
        //    if(type == 1)
        //    {
        //        this.start.Text = "开始";
        //        this.start.Enabled = true;
        //    }
        //    else
        //    {
        //        this.start.Text = "运行中";
        //        this.start.Enabled = false;
        //    }
        //}

        private void SetButtonStatus(int type)
        {
            this.Invoke(new AsynSetButton(delegate (int s)
            {
                if (type == 1)
                {
                    this.start.Text = "开始";
                    this.start.Enabled = true;
                }
                else
                {
                    this.start.Text = "运行中";
                    this.start.Enabled = false;
                }
            }), type);

        }

        private void MessageTip(string message)
        {
            MessageBox.Show(message, "信息提示");
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int kc = e.KeyChar;
            if ((kc < 48 || kc > 57) && kc != 8)
                e.Handled = true;
        }
    }
}

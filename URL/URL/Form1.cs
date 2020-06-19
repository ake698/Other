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

        

        private string uid;
        //主线程
        private delegate void AsynSetButton(int x);
        private delegate void AsynMessageTip(string msg);
        public CommonRequest request;



        private void button1_Click(object sender, EventArgs e)
        {
            string username = this.textBox2.Text;
            string password = this.textBox3.Text;

            this.SetButtonStatus(2);
            this.uid = this.textBox1.Text;
            bool check = CheckUid(uid);
            if (!check)
            {
                SetButtonStatus(1);
                return;
            }

            Works works = new Works(uid)
            {
                setButton = this.SetButtonStatus,
                messageTip = this.SetMessageTip,
                username = username,
                password = password
            };
            Thread thread = new Thread(new ThreadStart(works.Start));
            thread.IsBackground = true;
            thread.Start();

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

        private void SetMessageTip(string message)
        {
            this.Invoke(new AsynMessageTip(delegate (string msg)
            {
                MessageBox.Show(message, "信息提示");
            }), message);
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


        public bool Login(string username, string password)
        {
            Login login = new Login(username, password);
            this.request = login.LoginPost();
            var result = request.HttpGet("https://m.weibo.cn/api/remind/unread");
            Debug.WriteLine(result);
            var code = result["ok"].ToString();
            if (code == "1")
            {
                Debug.WriteLine("success");
                return true;
            }
            else
            {
                Debug.WriteLine("error");
                return false;
            }
        }
    }
}

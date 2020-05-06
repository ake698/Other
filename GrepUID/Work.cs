using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GrepUID
{
    class Work
    {
        public delegate void AsyncUpdateButton(string text, bool status);
        public AsyncUpdateButton updateButton;
        private string File1;
        private List<string> Uids;
        private bool identity = false;
        private int count;
        public Work(string file1)
        {
            this.File1 = file1;
            count = 0;
        }

        public void Start()
        {
            updateButton("运行中", false);
            Thread.Sleep(1000);
            this.HandlerFile();
            updateButton("开始", true);
        }

        //private void GetUrls()
        //{
        //    Urls = new List<string>();
        //    FileStream file = new FileStream(this.File1, FileMode.Open, FileAccess.Read);
        //    StreamReader reader = new StreamReader(file);
        //    string line;
        //    while ((line = reader.ReadLine()) != null)
        //    {
        //        Debug.WriteLine(line);
        //        Urls.Add(line);
        //    }
        //    reader.Close();
        //    file.Close();
        //}



        public void HandlerFile()
        {
            Uids = new List<string>();
            //this.GetUrls();
            FileStream file = new FileStream(this.File1, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
            FileStream writeFile = new FileStream(System.Environment.CurrentDirectory + "/" + "uid" + fileName + ".txt", FileMode.Create);
            FileStream writeFile2 = new FileStream(System.Environment.CurrentDirectory + "/" + "mail" + fileName + ".txt", FileMode.Create);
            StreamWriter write = new StreamWriter(writeFile);
            StreamWriter write2 = new StreamWriter(writeFile2);
            string line;
            string[] result;
            while((line = reader.ReadLine()) != null)
            {
                //Debug.WriteLine(line);
                result = this.HandlerLine(line);
                if (result[0] == "none") continue;
                if (!Uids.Contains(result[0]))
                //if (ContainsUrl(result[0]))
                {
                    if(result[1] == "F")
                    {
                        write.WriteLine(line);
                    }
                    else
                    {
                        write2.WriteLine(line);
                    }
                    
                    Uids.Add(result[0]);
                 
                }
                count++;
                Debug.WriteLine(count);

            }

            reader.Close();
            file.Close();

            write.Close();
            writeFile.Close();

            write2.Close();
            writeFile2.Close();
        }

        public bool ContainsUrl(string url)
        {
            foreach(string u in Uids)
            {
                if (u.Trim().Equals(url.Trim())) return false; //包含
            }
            return true;
        }

        public string IsMail(string username)
        {
            if (username.Contains("@")) return "T";
            return "F";
        }

        //id ismail
        public string[] HandlerLine(string line)
        {
            string[] arrs;
            string id;
            //string secren;
            string isMail = "F";
            if (line.IndexOf("----") > -1)
            {
                //Debug.WriteLine("第二种");
                line = line.Replace("----", "*");
                arrs = line.Split('*');
                if (arrs.Length < 4) return new string[] { "none", "" };
                id = arrs[2];
                isMail = this.IsMail(arrs[0]);
                //secren = arrs[0] + "----" + arrs[1] + "----" + arrs[2];
            }
            else
            {
                id = "";
                //secren = "";
                if(identity== true)
                {
                    Debug.WriteLine("第一种");
                    arrs = line.Split('|');
                    //if (arrs.Length < 7) return new string[] { "none", "" };
                    id = arrs[5];
                    //secren = "";
                }
                
            }
            //string url = "https://weibo.com/u/" + id;
            return new string[] { id, isMail };
        }


    }
}

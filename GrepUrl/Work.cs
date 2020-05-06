using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GrepUrl
{
    class Work
    {
        public delegate void AsyncUpdateButton(string text, bool status);
        public AsyncUpdateButton updateButton;
        private string File1;
        private string File2;
        private List<string> Urls;
        public Work(string file1,string file2)
        {
            this.File1 = file1;
            this.File2 = file2;
        }

        public void Start()
        {
            updateButton("运行中", false);
            Thread.Sleep(1000);
            this.HandlerFile();
            updateButton("选择文件", true);
        }

        private void GetUrls()
        {
            Urls = new List<string>();
            FileStream file = new FileStream(this.File2, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Debug.WriteLine(line);
                Urls.Add(line);
            }
            reader.Close();
            file.Close();
        }



        public void HandlerFile()
        {
            this.GetUrls();
            FileStream file = new FileStream(this.File1, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
            FileStream writeFile = new FileStream(System.Environment.CurrentDirectory + "/" + "grep" + fileName + ".txt", FileMode.Create);
            StreamWriter write = new StreamWriter(writeFile);
            string line;
            string[] result;
            while((line = reader.ReadLine()) != null)
            {
                Debug.WriteLine(line);
                result = this.HandlerLine(line);
                if (result[0] == "none") continue;

                if (ContainsUrl(result[0])) write.WriteLine(result[1]);
            }

            reader.Close();
            file.Close();

            write.Close();
            writeFile.Close();
        }

        public bool ContainsUrl(string url)
        {
            foreach(string u in Urls)
            {
                if (u.Trim().Equals(url.Trim())) return true;
            }
            return false;
        }

        public string[] HandlerLine(string line)
        {
            string[] arrs;
            string id;
            string secren;
            if (line.IndexOf("----") > -1)
            {
                Debug.WriteLine("第二种");
                line = line.Replace("----", "*");
                arrs = line.Split('*');
                if (arrs.Length < 4) return new string[] { "none","" };
                id = arrs[2];
                secren = arrs[0] + "----" + arrs[1] + "----" + arrs[2];
            }
            else
            {
                Debug.WriteLine("第一种");
                arrs = line.Split('|');
                if (arrs.Length < 7) return new string[] { "none", "" };
                id = arrs[5];
                secren = "";
            }
            string url = "https://weibo.com/u/" + id;
            return new string[] { url,secren};
        }


    }
}

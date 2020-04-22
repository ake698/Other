using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SinaUrl
{
    class Work
    {
        public delegate void AsyncUpdateButton(string text, bool status, bool hasFile);
        public AsyncUpdateButton updateButton;
        private string File;
        public Work(string file)
        {
            this.File = file;
        }

        public void Start()
        {
            updateButton("运行中", false, true);
            Thread.Sleep(1000);
            this.HandlerFile();
            updateButton("选择文件", true, false);
        }

        public void HandlerFile()
        {
            FileStream file = new FileStream(this.File, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
            FileStream writeFile = new FileStream(System.Environment.CurrentDirectory + "/" + "url" + fileName + ".txt", FileMode.Create);
            StreamWriter write = new StreamWriter(writeFile);
            string line;
            string url;
            while((line = reader.ReadLine()) != null)
            {
                Debug.WriteLine(line);
                url = this.HandlerLine(line);
                if (url == "none") continue;
                write.WriteLine(url);
            }

            reader.Close();
            file.Close();

            write.Close();
            writeFile.Close();
        }

        public string HandlerLine(string line)
        {
            string[] arrs;
            string id;
            if (line.IndexOf("----") > -1)
            {
                Debug.WriteLine("第二种");
                line = line.Replace("----", "*");
                arrs = line.Split('*');
                if (arrs.Length < 4) return "none";
                id = arrs[2];
            }
            else
            {
                Debug.WriteLine("第一种");
                arrs = line.Split('|');
                if (arrs.Length < 7) return "none";
                id = arrs[5];
            }
            string url = "https://weibo.com/u/" + id;
            return url;
        }


    }
}

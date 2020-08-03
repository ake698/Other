using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace MailGroup
{
    public class Work
    {
        public delegate void AsyncUpdateButton(string text, bool status);
        public AsyncUpdateButton updateButton;
        private string File1;

        public Work(string file1)
        {
            this.File1 = file1;
        }

        public void Start()
        {
            updateButton("运行中", false);
            Thread.Sleep(2000);
            this.HandlerFile();
            updateButton("开始", true);
        }

        private void HandlerFile()
        {
            FileStream file = new FileStream(this.File1, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
            var fileWriterStreams = Util.GetFileStreamWriterDic(fileName);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var username = SpliteLine(line);
                USERTYPE type = Util.GetUserType(username);
                fileWriterStreams[type].WriteLine(line);
            }
            Util.CloseStreams();
        }

        public string SpliteLine(string line)
        {
            if (line.IndexOf("----") > -1)
            {
                line = line.Replace("----", "*");
                var arrs = line.Split('*');
                var username = arrs[0];
                return username;
            }
            return null;
        }

    }
}

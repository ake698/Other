using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace GrepMobilePhone
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
            Thread.Sleep(1000);
            this.HandlerFile();
            updateButton("开始", true);
        }
        public void HandlerFile()
        {
            FileStream file = new FileStream(this.File1, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
            FileStream mailFile = new FileStream(System.Environment.CurrentDirectory + "/" + "mail" + fileName + ".txt", FileMode.Create);
            FileStream phoneFile = new FileStream(System.Environment.CurrentDirectory + "/" + "phone" + fileName + ".txt", FileMode.Create);
            StreamWriter mailWrite = new StreamWriter(mailFile);
            StreamWriter phoneWrite = new StreamWriter(phoneFile);
            string line;
            bool result;
            while ((line = reader.ReadLine()) != null)
            {
                result = this.IsMail(line);
                if (result)
                {
                    mailWrite.WriteLine(line);
                }
                else
                {
                    phoneWrite.WriteLine(line);
                }

            }

            reader.Close();
            file.Close();

            mailWrite.Close();
            mailFile.Close();

            phoneWrite.Close();
            phoneFile.Close();
        }


        public bool IsMail(string line)
        {
            string[] arrs;
            string username;
            if (line.IndexOf("----") > -1)
            {
                //Debug.WriteLine("第1种");
                line = line.Replace("----", "*");
                arrs = line.Split('*');
                if (arrs.Length < 3) return false;
                username = arrs[0];
                if (username.Contains("@")) return true;
            }
            return false;
        }

    }
}


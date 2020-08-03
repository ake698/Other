using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InsertEmoji
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool HasFile = false;
        private readonly string commonStr = "[表情]";

        private bool FileDialog(TextBox textBox)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*txt*)|*.txt*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;//返回文件的完整路径            
                textBox.Text = file;
                this.button2.Enabled = true;
                return true;
            }
            return false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            HasFile = this.FileDialog(this.textBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var type = this.comboBox1.SelectedIndex;
            Debug.WriteLine(type);
            var countText = this.textBox2.Text ?? "1";
            int count = int.Parse(countText);
            Debug.WriteLine(count);
            this.Work(type, count);
        }

        private void Work(int type = 1, int count = 1)
        {
            FileStream file = new FileStream(this.textBox1.Text, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, System.Text.Encoding.Default);
            string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");

            FileStream writeFile = new FileStream(System.Environment.CurrentDirectory + "/" + "emoji" + fileName + ".txt", FileMode.Create);
            StreamWriter write = new StreamWriter(writeFile, System.Text.Encoding.Default);

            string commonStrs = this.SetCommonStrs(count);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length < 1) continue;
                line = this.RegexReplace(line);
                string result = this.SetNewString(type, commonStrs, line);
                write.WriteLine(result);
            }
            reader.Close();
            file.Close();

            write.Close();
            writeFile.Close();
        }

        private string RegexReplace(string line)
        {
            Regex r = new Regex(@"\[.*?\]");
            string result = r.Replace(line, "");
            return result;
        }

        private string SetCommonStrs(int count)
        {
            string commonStrs = null;
            for (int i = 0; i < count; i++)
            {
                commonStrs += commonStr;
            }
            return commonStrs;
        }

        private string SetNewString(int type, string commonStrs,string line)
        {
            string temp;
            if (type == 0)
            {
                temp = commonStrs + line + commonStrs;
            }
            else
            {
                temp = line;
                Random random = new Random();
                int max = temp.Length;
                int min = max <= 1 ? 0 :1;
                int insertIndex = random.Next(min,max);
                temp = temp.Insert(insertIndex, commonStrs);
            }
            return temp;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            int kc = e.KeyChar;
            if ((kc < 48 || kc > 57) && kc != 8)
                e.Handled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 1;
            this.textBox2.Text = "1";
        }
    }
}

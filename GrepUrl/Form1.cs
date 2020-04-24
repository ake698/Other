using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GrepUrl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        delegate void UpdateButtonDelegate(Button button,string text, bool status);
        UpdateButtonDelegate updateButton;

        //判断是否选择了文件
        private bool HasFile1 = false;
        private bool HasFile2 = false;
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
                return true;
                //Console.WriteLine(file);
            }
            return false;
        }

        public void UpdateButton(Button button,string text, bool status)
        {
            button.Enabled = status;
            button.Text = text;
        }
        public void AsyncUpdate(string text, bool status)
        {
            this.Invoke(updateButton,this.button3, text, status);
            this.Invoke(updateButton, this.button1, "选择", status);
            this.Invoke(updateButton, this.button2, "选择", status);
           
        }

        private void EnableButton3()
        {
            if (HasFile1 && HasFile2) this.button3.Enabled=true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            HasFile1 =  this.FileDialog(this.textBox1);
            this.EnableButton3();
        }

     
        private void button2_Click(object sender, EventArgs e)
        {
            HasFile2 = this.FileDialog(this.textBox2);
            this.EnableButton3();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Work work = new Work(this.textBox1.Text,this.textBox2.Text);
            work.updateButton = this.AsyncUpdate;
            Thread thread = new Thread(new ThreadStart(work.Start));
            thread.IsBackground = true;
            thread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.updateButton = UpdateButton;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GrepMobilePhone
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool HasFile = false;
        delegate void UpdateButtonDelegate(Button button, string text, bool status);
        UpdateButtonDelegate updateButton;

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
                UpdateButton(button1,"开始", true);
                return true;
            }
            return false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!HasFile)
            {
                HasFile = this.FileDialog(this.textBox1);
            }
            else
            {
                
                Work work = new Work(this.textBox1.Text);
                work.updateButton = this.AsyncUpdate;
                Thread thread = new Thread(new ThreadStart(work.Start));
                thread.IsBackground = true;
                thread.Start();
            }
            
            
        }

        public void UpdateButton(Button button, string text, bool status)
        {
            button.Enabled = status;
            button.Text = text;
        }
        public void AsyncUpdate(string text, bool status)
        {
            this.Invoke(updateButton, this.button1, text, status);

        }


        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.updateButton = UpdateButton;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MailGroup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //判断是否选择了文件
        private bool HasFile = false;

        delegate void UpdateButtonDelegate(Button button, string text, bool status);
        UpdateButtonDelegate updateButton;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!HasFile)
            {
                //没有选择文件  弹出文件选择
                this.FileDialog();
            }
            else
            {
                //直接开始操作流程。
                Console.WriteLine("start");
                Work work = new Work(this.textBox1.Text);
                work.updateButton = this.UpdateAsync;
                Thread thread = new Thread(new ThreadStart(work.Start));
                thread.IsBackground = true;
                thread.Start();
            }
        }


        private void FileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*txt*)|*.txt*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;//返回文件的完整路径            
                this.textBox1.Text = file;
                this.HasFile = true;
                UpdateButton(button1, "开始", true);
                //Console.WriteLine(file);
            }
        }

        public void UpdateButton(Button button, string text, bool status)
        {
            button.Enabled = status;
            button.Text = text;
        }

        public void UpdateAsync(string text, bool status)
        {
            this.Invoke(updateButton, this.button1, text, status);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.updateButton = UpdateButton;
        }
    }
}

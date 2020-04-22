using System;
using System.Threading;
using System.Windows.Forms;

namespace SinaUrl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        delegate void UpdateButtonDe(string text, bool status, bool hasFile);
        //delegate void AsyncUpdateButton(string text, bool status);
        //AsyncUpdateButton asyncUpdateButton;
        UpdateButtonDe updateButton;

        //判断是否选择了文件
        private bool HasFile = false;
        

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
                work.updateButton = this.AsyncUpdate;
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
                updateButton("开始",true, HasFile);
                //Console.WriteLine(file);
            }
        }


        public void UpdateButton(string text, bool status, bool hasFile)
        {
            this.button1.Enabled = status;
            this.button1.Text = text;
            this.HasFile = hasFile;
        }

        public void AsyncUpdate(string text, bool status, bool hasFile)
        {
            this.Invoke(updateButton,text,status,hasFile);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.updateButton = UpdateButton;
            //this.asyncUpdateButton = AsyncUpdate;
        }
    }
}

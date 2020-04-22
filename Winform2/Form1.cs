using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winform2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        delegate void AsynUpdateUI(int step);
        delegate void EnableButtonD();
        EnableButtonD enable;
        AsynUpdateUI update;

        private void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = 20;
            this.progressBar1.Value = 0;
            Work work = new Work();
            //委托
            work.update = UpdateProgress;
            work.finished = Finish;
            //
            enable = EnableButton;
            update = AddProgress;

            Thread thread = new Thread(new ParameterizedThreadStart(work.WorkStart));
            thread.IsBackground = true;
            thread.Start(20);
            //this.button1.Enabled = false;
            this.enable();

        }

        

        
        public void UpdateProgress(int step)
        {
            if (InvokeRequired)
            {
                //this.Invoke(new AsynUpdateUI(
                ////    delegate (int s)
                ////{
                ////    this.progressBar1.Value += s;
                ////}
                //AddProgress
                //), step);
                this.Invoke(update, 1);
            }
            else
            {
                this.progressBar1.Value += step;

            }
        }

        public void AddProgress(int s)
        {
            this.progressBar1.Value += s;
        }

        public void Finish()
        {
            MessageBox.Show("完成！");
            this.Invoke(enable);
        }

        public void EnableButton()
        {
            this.button1.Enabled = true;
        }
    }
}

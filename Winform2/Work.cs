using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winform2
{
    class Work
    {
        public delegate void UpdateUI(int step);
        public delegate void Finished();

        public UpdateUI update;
        public Finished finished;

        public void WorkStart(object count)
        {
            for (int i = 0; i < (int)count; i++)
            {
                update(1);
                Thread.Sleep(100);
            }
            finished();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Utils
{
    public class TaskRunner
    {
        private Task Task;
        public Action DoWork { set; get; }
        public TaskRunner()
        {
            Task = new Task(DoWork);
        }
        private void doWork(object state)
        {
            if (DoWork != null)
                DoWork();
        }
    }
}

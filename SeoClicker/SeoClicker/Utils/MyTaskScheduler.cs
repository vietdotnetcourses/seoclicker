using System;
using System.Threading;
using ThreadTimer = System.Threading.Timer;

namespace SeoClicker.Utils
{
    public class MyTaskScheduler
    {
        private ThreadTimer _threadTimer; //keep it alive
        public Action DoWork { set; get; }
        private long _startAfter = 0;
        private long _interval = 0;
        public int Id { get; set; }
        public MyTaskScheduler(long startAfter, long interval)
        {
            _startAfter = startAfter;
            _interval = interval;
        }
        public void Start()
        {
            _threadTimer = new ThreadTimer(doWork, null, Timeout.Infinite, 1000);
            _threadTimer.Change(_startAfter * 1000, _interval * 100);
        }

        public void Stop()
        {
            if (_threadTimer != null)
                _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void doWork(object state)
        {
            if (DoWork != null)
                DoWork();
        }
    }
}
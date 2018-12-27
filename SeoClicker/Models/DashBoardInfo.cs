using SeoClicker.Utils;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SeoClicker.Models
{
    public class DashBoardInfo : INotifyPropertyChanged
    {

        AsyncObservableCollection<ClickerThreadInfo> _threadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
        AsyncObservableCollection<string> _logs = new AsyncObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public AsyncObservableCollection<ClickerThreadInfo> ThreadInfos
        {
            get { return _threadInfos; }
            set
            {
                _threadInfos = value;
                notifyPropertyChanged("ThreadInfos");
            }
        }

        [JsonIgnore]
        public AsyncObservableCollection<string> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                notifyPropertyChanged("Logs");
            }
        }
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

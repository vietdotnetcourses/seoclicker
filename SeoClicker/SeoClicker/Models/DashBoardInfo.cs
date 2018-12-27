using SeoClicker.Utils;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SeoClicker.Models
{
    public class DashBoardInfo : INotifyPropertyChanged
    {

        AsyncObservableCollection<ClickerThreadInfo> _threadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
        AsyncObservableCollection<string> _logs = new AsyncObservableCollection<string>();
        string _resultMessage;
        string _spinnerVisibility;
        bool _isEnabled;

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



        public string ResultMessage
        {
            get { return _resultMessage; }
            set
            {
                _resultMessage = value;
                notifyPropertyChanged("ResultMessage");
            }
        }

        public string SpinnerVisibility
        {
            get { return _spinnerVisibility; }
            set
            {
                _spinnerVisibility = value;
                notifyPropertyChanged("SpinnerVisibility");
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
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                notifyPropertyChanged("IsEnabled");
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

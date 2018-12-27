using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class TaskSettings : INotifyPropertyChanged
    {
        int _totalRequest;
        int _loadTime;
        int _numberOfThreads;
        bool _isEnabled;
        public int TotalRequest
        {
            get
            {
                return _totalRequest;
            }
            set
            {
                _totalRequest = value;
                notifyPropertyChanged("TotalRequest");
            }
        }

        public int LoadTime
        {
            get
            {
                return _loadTime;
            }
            set
            {
                _loadTime = value;
                notifyPropertyChanged("LoadTime");
            }
        }
        public int NumberOfThreads
        {
            get
            {
                return _numberOfThreads;
            }
            set
            {
                _numberOfThreads = value;
                notifyPropertyChanged("NumberOfThreads");
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
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

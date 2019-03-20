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
        int _loadCount;
        int _numberOfThreads;
        bool _clearResultFiles;
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

        public int LoadCount
        {
            get
            {
                return _loadCount;
            }
            set
            {
                _loadCount = value;
                notifyPropertyChanged("LoadCount");
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

        public bool ClearResultFiles
        {
            get
            {
                return _clearResultFiles;
            }
            set
            {
                _clearResultFiles = value;
                notifyPropertyChanged("ClearResultFiles");
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

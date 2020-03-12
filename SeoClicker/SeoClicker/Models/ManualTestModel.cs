using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class ManualTestModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _url;
        string _geo;
        string _device;
        string _log;
        List<SelectionItem> _devices;

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
                notifyPropertyChanged("Url");
            }
        }
        public string Geo
        {
            get
            {
                return _geo;
            }
            set
            {
                _geo = value;
                notifyPropertyChanged("Geo");
            }
        }
        public string Device
        {
            get
            {
                return _device;
            }
            set
            {
                _device = value;
                notifyPropertyChanged("Device");
            }
        }

        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
                notifyPropertyChanged("Log");
            }
        }

        public List<SelectionItem> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                notifyPropertyChanged("Devices");
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

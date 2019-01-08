using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class DataServerSettings : INotifyPropertyChanged
    {
        string _getDataApiLink;
        string _reportApiLink;
        int _urlCount;

        public string GetDataApiLink
        {
            get
            {
                return _getDataApiLink;
            }
            set
            {
                _getDataApiLink = value;
                notifyPropertyChanged("GetDataApiLink");
            }
        }


        public string ReportApiLink
        {
            get
            {
                return _reportApiLink;
            }
            set
            {
                _reportApiLink = value;
                notifyPropertyChanged("ReportApiLink");
            }
        }

        public int UrlCount
        {
            get
            {
                return _urlCount;
            }
            set
            {
                _urlCount = value;
                notifyPropertyChanged("UrlCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

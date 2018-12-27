using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class ClickerThreadInfo: INotifyPropertyChanged
    {
        int _order;
        string _info;
        string _url;
        string _geo;
        public int Order
        {
            get { return _order; }
            set
            {
                _order = value;
                notifyPropertyChanged("Order");
            }
        }

        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                notifyPropertyChanged("Info");
            }
        }
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                notifyPropertyChanged("Url");
            }
        }
        public string Geo
        {
            get { return _geo; }
            set
            {
                _geo = value;
                notifyPropertyChanged("Geo");
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

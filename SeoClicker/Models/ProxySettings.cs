using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class ProxySettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _proxyZone;
        string _userName;
        string _password;     
        int _port;
        List<SelectionItem> _route;
        List<SelectionItem> _superProxy;
        List<SelectionItem> _userAgent;
        List<SelectionItem> _dnsResolution;

        public string ProxyZone
        {
            get
            {
                return _proxyZone;
            }
            set
            {
                _proxyZone = value;
                notifyPropertyChanged("ProxyZone");
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                notifyPropertyChanged("Port");
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                notifyPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                notifyPropertyChanged("Password");
            }
        }

        public List<SelectionItem> UserAgent
        {
            get
            {
                return _userAgent;
            }
            set
            {
                _userAgent = value;
                notifyPropertyChanged("UserAgent");
            }
        }

        public List<SelectionItem> Route
        {
            get
            {
                return _route;
            }
            set
            {
                _route = value;
                notifyPropertyChanged("Route");
            }
        }

        public List<SelectionItem> SuperProxy
        {
            get
            {
                return _superProxy;
            }
            set
            {
                _superProxy = value;
                notifyPropertyChanged("SuperProxy");
            }
        }
        public List<SelectionItem> DNSResolution
        {
            get
            {
                return _dnsResolution;
            }
            set
            {
                _dnsResolution = value;
                notifyPropertyChanged("DNSResolution");
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

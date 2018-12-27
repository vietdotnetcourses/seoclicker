using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SeoClicker.Utils;
using SeoClicker.Models;
using System.Collections.Generic;
using SeoClicker.Helpers;
using System.Linq;

namespace SeoClicker.ViewModels
{
    public class MainWindowViewModel
    {
        readonly List<SimpleTaskScheduler> _taskSchedulers = new List<SimpleTaskScheduler>();
        private RequestWorker RequestWorker;
        public MainWindowViewModel()
        {
            setupData();
            setupCommands();
            manageAppExit();
            initTaskScheduler();
        }

        public DelegateCommand<string> DoStart { set; get; }
        public DelegateCommand<string> DoStop { set; get; }
        public DelegateCommand<string> DoClearLogs { get; set; }
        public DelegateCommand<string> DoSaveSettings { get; set; }
        public ProxySettings ProxySettings { get; set; }
        public DataServerSettings DataServerSettings { get; set; }
        public TaskSettings TaskSettings { get; set; }    
        public DashBoardInfo DashBoardInfo { get; set; }
        bool canDoStart(string data)
        {
            return false;
        }

        // Private Methods (14)
        void currentExit(object sender, ExitEventArgs e)
        {
            exit();
        }

        void currentSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            exit();
        }



        void doStart(string data)
        {
            DashBoardInfo.ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>();
            DashBoardInfo.Logs = new AsyncObservableCollection<string>();
            var proxyRoute = ProxySettings.Route.FirstOrDefault(x => x.IsSelected)?.Value ?? "pass_dyn";          
            var superProxy = ProxySettings.SuperProxy.FirstOrDefault(x => x.IsSelected)?.Value ?? ".zproxy.lum-superproxy.io";
            var dnsResolution = ProxySettings.DNSResolution.FirstOrDefault(x => x.IsSelected)?.Value ?? "-session-";
            var dataItem = DataHelper.LoadData();
            var targetUri = dataItem.url;
            var userAgent = ProxySettings.UserAgent.FirstOrDefault(x => x.IsSelected)?.Value ?? "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            var geo = !string.IsNullOrWhiteSpace(dataItem.geo) ? dataItem.geo : "us";
            var clientSettings = new ClientSettings
            {
                Country = geo,
                DNSResolution = dnsResolution,
                Password = ProxySettings.Password,
                Port = ProxySettings.Port,
                Route = proxyRoute,
                SuperProxy = superProxy,
                TargetUrl = targetUri,
                UserAgent = userAgent,
                UserName = ProxySettings.UserName,
                Zone = ProxySettings.ProxyZone
            };
            RequestWorker = new RequestWorker(TaskSettings.NumberOfThreads, TaskSettings.TotalRequest, TaskSettings.LoadTime, clientSettings, DashBoardInfo);
            RequestWorker.DoWork();
            TaskSettings.IsEnabled = false;
        }
        void doStop(string data)
        {
            RequestWorker.Stop();
            TaskSettings.IsEnabled = true;
        }

        void doSaveSettings(string data)
        {
            var settings = new Settings
            {
                DataServerSettings = DataServerSettings,
                ProxySettings = ProxySettings,
                TaskSettings = TaskSettings
            };
            SettingsHelper.SaveSettings(settings);
        }

        void doClearLogs(string data)
        {

        }

        private void initTaskScheduler()
        {

        }

        private void exit()
        {
            foreach(var scheduler in _taskSchedulers)
            {
                scheduler.Stop();
            }
           
        }

 
        private void manageAppExit()
        {
            Application.Current.Exit += currentExit;
            Application.Current.SessionEnding += currentSessionEnding;
        }

    
        private void setupCommands()
        {
            DoStart = new DelegateCommand<string>(doStart, data => true);
            DoStop = new DelegateCommand<string>(doStop, data => true);
            DoSaveSettings = new DelegateCommand<string>(doSaveSettings, data => true);
            DoClearLogs = new DelegateCommand<string>(doClearLogs, data => true);
        }

        private void setupData()
        {
            var settings = SettingsHelper.LoadSettings();
            ProxySettings = settings.ProxySettings;
            DataServerSettings = settings.DataServerSettings;
            TaskSettings = settings.TaskSettings;
            DashBoardInfo = new DashBoardInfo
            {
                ThreadInfos = new AsyncObservableCollection<ClickerThreadInfo>(),
                Logs = new AsyncObservableCollection<string>()
            };


        }
    }
}
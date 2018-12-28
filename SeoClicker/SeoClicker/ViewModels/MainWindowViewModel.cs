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
using System.Net;
using System.Threading;

namespace SeoClicker.ViewModels
{
    public class MainWindowViewModel
    {
        public RequestWorker RequestWorker { get; set; }
        public MainWindowViewModel()
        {
            setupData();
            setupCommands();
            manageAppExit();
            initTaskScheduler();
        }
        public string SpinnerImagePath { get; set; }
        public DelegateCommand<string> DoStart { set; get; }
        public DelegateCommand<string> DoStop { set; get; }
        public DelegateCommand<string> DoClearLogs { get; set; }
        public DelegateCommand<string> DoSaveSettings { get; set; }
        public ProxySettings ProxySettings { get; set; }
        public DataServerSettings DataServerSettings { get; set; }
        public TaskSettings TaskSettings { get; set; }
        //  public DashBoardInfo DashBoardInfo { get; set; }
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

            var proxyRoute = ProxySettings.Route.FirstOrDefault(x => x.IsSelected)?.Value ?? "pass_dyn";
            var superProxy = ProxySettings.SuperProxy.FirstOrDefault(x => x.IsSelected)?.Value ?? "zproxy.lum-superproxy.io";
            var dnsResolution = ProxySettings.DNSResolution.FirstOrDefault(x => x.IsSelected)?.Value ?? "-session-";
            var dataItem = DataHelper.LoadData();

            //Clear files .txt inside ~/Results folder
            if (TaskSettings.ClearResultFiles)
            {
                DataHelper.DeleteResultsFolder();
            }

            RequestWorker.SpinnerVisibility = "Visible";
            RequestWorker.ResultMessage = "";
            RequestWorker.ThreadInfos.Clear();
            RequestWorker.Logs.Clear();
            RequestWorker.IsEnabled = false;


            // foreach (var item in dataItems)
            // {

          //  var item = dataItems.First();
            
            var targetUri = dataItem.url;
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
                Device = dataItem.device,
                UserName = ProxySettings.UserName,
                Zone = ProxySettings.ProxyZone,
                Credential = new NetworkCredential($"lum-customer-{ProxySettings.UserName}-zone-{ProxySettings.ProxyZone}-route_err-{proxyRoute}-country-{geo}", ProxySettings.Password),
                Timeout = Timeout.Infinite,
                NumberOfThread = TaskSettings.NumberOfThreads,
                RequestNumber = dataItem.clickCount,
                IpChangeRequestNumber = 1
            };
            RequestWorker.ClientSettings = clientSettings;
            RequestWorker.ConfigureTask();
            RequestWorker.DoWork();
            // }


        }
        void doStop(string data)
        {
            RequestWorker.Stop();

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
            RequestWorker.Logs.Clear();
        }

        private void initTaskScheduler()
        {

        }

        private void exit()
        {
            RequestWorker.Stop();

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

            RequestWorker = new RequestWorker();
            SpinnerImagePath = DataHelper.GetSpinnerImagePath();

        }
    }
}
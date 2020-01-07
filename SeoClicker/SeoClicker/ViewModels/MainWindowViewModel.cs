using System.Windows;
using SeoClicker.Utils;
using SeoClicker.Models;
using SeoClicker.Helpers;
using System.Linq;
using System.Threading;
using System;

namespace SeoClicker.ViewModels
{
    public class MainWindowViewModel
    {
        public RequestTaskRunner RequestWorker { get; set; }

        public MainWindowViewModel()
        {
            setupData();
            setupCommands();
            manageAppExit();
        }
        public string SpinnerImagePath { get; set; }
        public DelegateCommand<string> DoStart { set; get; }
        public DelegateCommand<string> DoStop { set; get; }
        public DelegateCommand<string> DoClearLogs { get; set; }
        public DelegateCommand<string> DoSaveSettings { get; set; }
        public ProxySettings ProxySettings { get; set; }
        public DataServerSettings DataServerSettings { get; set; }
        public TaskSettings TaskSettings { get; set; }

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
            if (Designer.IsInDesignModeStatic)
                return;

            RequestWorker.SpinnerVisibility = "Visible";
            RequestWorker.ResultMessage = "";
            RequestWorker.ThreadInfos.Clear();
            RequestWorker.Logs = "";
            RequestWorker.IsEnabled = false;
            var clientSettings = new ClientSettings
            (
               ProxySettings.ProxyZone,
               ProxySettings.UserName,
               ProxySettings.Password,
               Timeout.Infinite,
               TaskSettings.NumberOfThreads > 20 ? 20 : TaskSettings.NumberOfThreads,
               DataServerSettings.GetDataApiLink,
               DataServerSettings.UrlCount,
               TaskSettings.LoadCount != 0 ? TaskSettings.LoadCount : 10,
               TaskSettings.LoadTime != 0 ? TaskSettings.LoadTime : 5,
               TaskSettings.ClearResultFiles
            );
            // try
            // {
            RequestWorker.ClientSettings = clientSettings;
            RequestWorker.ConfigureTask().Start(); ;

            // }
            //catch (Exception ex)
            //{
            //   RequestWorker.Logs = ex.Message;
            //}

        }
        void doStop(string data)
        {
            RequestWorker.Stop();
            RequestWorker.ResultMessage = "";
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
            RequestWorker.Logs = "";
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

            RequestWorker = new RequestTaskRunner();
            SpinnerImagePath = DataHelper.GetSpinnerImagePath();

        }
    }
}
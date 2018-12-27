using Microsoft.Win32;
using System.Windows.Forms;

namespace SeoClicker.Utils
{
    public static class RunOnWindowsStartup
    {
        public static void Do()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.SetValue("SeoClicker", Application.ExecutablePath);
            }
        }

        public static void Undo()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
            {
                rkApp.DeleteValue("SeoClicker", false);
            }
        }
    }
}
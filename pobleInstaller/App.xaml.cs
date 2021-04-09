using HandyControl.Themes;
using IWshRuntimeLibrary;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace pobleInstaller
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CreateShortCut();
            var boot = new Bootstrapper();
            boot.Run();
        }
        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x19;



        string ICON_FILE = ConfigurationManager.AppSettings.Get("IconFile");
        string ProjectName = ConfigurationManager.AppSettings.Get("ProjectName");

        internal async void CreateDesktopShortcutForAllUsers()
        {
            string sProduct = "pobleInstaller";

            StringBuilder allUserProfile = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, allUserProfile, CSIDL_COMMON_DESKTOPDIRECTORY, false);
            //The above API call returns: C:UsersPublicDesktop 
            string settingsLink = Path.Combine(allUserProfile.ToString(), $"{ProjectName}.lnk");

            if (!System.IO.File.Exists(settingsLink))
            {
                //Create All Users Desktop Shortcut for Application Settings
                WshShell shellClass = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shellClass.CreateShortcut(settingsLink);
                shortcut.TargetPath = String.Format(@"{0}\{1}.exe", Directory.GetCurrentDirectory(), sProduct);
                shortcut.IconLocation = String.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), ICON_FILE);
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                //shortcut.Arguments = "arg1 arg2";
                shortcut.Description = "poble updater";
                shortcut.Save();
            }
                       
        }

        internal async void CreateStartupShortcut()
        {
            string sProduct = "pobleInstaller";

            StringBuilder allUserProfile = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, allUserProfile, CSIDL_COMMON_DESKTOPDIRECTORY, false);
            //The above API call returns: C:UsersPublicDesktop 
            string settingsLink = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), $"{ProjectName}.lnk");

            if (!System.IO.File.Exists(settingsLink))
            {
                //Create All Users Desktop Shortcut for Application Settings
                WshShell shellClass = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shellClass.CreateShortcut(settingsLink);
                shortcut.TargetPath = String.Format(@"{0}\{1}.exe", Directory.GetCurrentDirectory(), sProduct);
                shortcut.IconLocation = String.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), ICON_FILE);
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                //shortcut.Arguments = "arg1 arg2";
                shortcut.Description = "poble updater";
                shortcut.Save();
            }
        }

        internal void CreateShortCut()
        {
            try
            {
                
                CreateDesktopShortcutForAllUsers();
                CreateStartupShortcut();
            }
            catch(Exception ex)
            {

            }
        }


        internal void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        internal void UpdateAccent(Brush accent)
        {
            if (ThemeManager.Current.AccentColor != accent)
            {
                ThemeManager.Current.AccentColor = accent;
            }
        }
    }
}

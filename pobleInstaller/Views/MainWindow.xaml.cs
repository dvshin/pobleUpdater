using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibGit2Sharp;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using pobleInstaller.ViewModels;
using System.Reflection;
using System.Configuration;

namespace pobleInstaller.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ((App)Application.Current).UpdateTheme(ApplicationTheme.Dark);

            string MAIN_PAGE_URI = ConfigurationManager.AppSettings.Get("MainUri");
            wbMain.Source = new Uri(MAIN_PAGE_URI);
            HideScriptErrors(wbMain, true);
            //wbMain.Source = new Uri( "https://sites.google.com/posnet.com.au/poble-update/home");
            

        }


        /// <summary>
        /// java script error pop up disable
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="Hide"></param>
        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser)
                .GetField("_axIWebBrowser2",
                          BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember(
                "Silent", BindingFlags.SetProperty, null, objComWebBrowser,
                new object[] { Hide });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        

        

    }
}

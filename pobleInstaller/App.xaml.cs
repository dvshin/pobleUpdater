using HandyControl.Themes;
using System;
using System.IO;
using System.Reflection;
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
        internal async void CreateShortCut()
        {
            try
            {
                Assembly code = Assembly.GetExecutingAssembly();

                string sCompany = "POSNET";
                string sProduct = "pobleInstaller";

                //if (Attribute.IsDefined(code, typeof(AssemblyCompanyAttribute)))
                //{
                //    AssemblyCompanyAttribute oCompany = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyCompanyAttribute));
                //    sCompany = oCompany.Company;
                //}

                //if (Attribute.IsDefined(code, typeof(AssemblyProductAttribute)))
                //{
                //    AssemblyProductAttribute oProduct = (AssemblyProductAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyProductAttribute));
                //    sProduct = oProduct.Product;
                //}

                if (sCompany != string.Empty && sProduct != string.Empty)
                {
                    string sShortcutPath = string.Empty;
                    sShortcutPath = String.Format(@"{0}\{1}.exe", Directory.GetCurrentDirectory(),  sProduct);

                    string sDesktopPath = string.Empty;
                    sDesktopPath = String.Format(@"{0}\{1}.exe", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), sProduct);

                    if(!File.Exists(sDesktopPath))
                    {
                        System.IO.File.Copy(sShortcutPath, sDesktopPath, true);
                    }
                    

                    //copy to Start up folder
                    string sStartUpPath = String.Format(@"{0}\{1}.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup), sProduct);

                    if (!File.Exists(sStartUpPath))
                    {
                        System.IO.File.Copy(sShortcutPath, sStartUpPath, true);
                    }
                    
                }
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

using LibGit2Sharp;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace pobleInstaller.ViewModels
{
    public enum UPDATE_STATUS {Install,Update,Launch };
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Poble Installer";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public bool CanInstall { get {
                //return MainStatus == UPDATE_STATUS.Install || MainStatus == UPDATE_STATUS.Update;
                return MainStatus == UPDATE_STATUS.Install;
            } }

        private bool _showProgressBar =false;
        public bool ShowProgressBar
        {
            get
            {
                //return MainStatus == UPDATE_STATUS.Install || MainStatus == UPDATE_STATUS.Update;
                return _showProgressBar;
            }
            set
            {
                SetProperty(ref _showProgressBar, value);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ShowProgressBar)));
            }
        }

        bool canInstall()
        {
            return CanInstall;
        }

        /// <summary>
        /// check update status
        /// </summary>
        private UPDATE_STATUS _main_Status;
        public UPDATE_STATUS MainStatus
        {
            get { return _main_Status; }
            set { 
                SetProperty(ref _main_Status, value);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(MainStatus)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanLaunch)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanInstall)));
            }
        }


        /// <summary>
        /// prop for launch button enable
        /// </summary>
        public bool CanLaunch
        {
            get { return _main_Status != UPDATE_STATUS.Install; }
        }

      

        
        
        /// <summary>
        /// progress bar value
        /// </summary>
        private int _currentProgress = 0;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set { 
                SetProperty(ref _currentProgress, value);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentProgress)));
            }
        }

        /// <summary>
        /// progressbar total value
        /// </summary>
        private int _totalProgress = 100;
        public int TotalProgress
        {
            get { return _totalProgress; }
            private set 
            {   
                SetProperty(ref _totalProgress, value);
                
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(TotalProgress)));
            }
        }

        private string _updateStatus;
        public string UpdateStatus { get
            { return _updateStatus; } 
            
            set {
                SetProperty(ref _updateStatus, value);

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(UpdateStatus)));
            } }


        /// <summary>
        /// local variable
        /// </summary>       
        string gitPath;
        string POBLE_EXE;
        
        string GIT_URI = ConfigurationManager.AppSettings.Get("GitUri");
        string GIT_PATH = ConfigurationManager.AppSettings.Get("GitPath");
        string GIT_BRANCH = ConfigurationManager.AppSettings.Get("GitBranch");
        string EXE_FILE = ConfigurationManager.AppSettings.Get("ExeFile");


        

        public MainWindowViewModel()
        {
            InstallOrUpdateCommand = new DelegateCommand(InstallOrUpdate, canInstall).ObservesProperty(() => MainStatus);
            LaunchCommand = new DelegateCommand(Launch).ObservesProperty(() => CanLaunch);
            gitPath = Directory.GetCurrentDirectory() + GIT_PATH;
            POBLE_EXE = string.Format(".{0}/{1}", GIT_PATH, EXE_FILE);


            ///check update
            CheckUpdateStatus();
        }

        /// <summary>
        /// command for install or update
        /// </summary>
        public DelegateCommand InstallOrUpdateCommand { get; private set; }

        internal async Task OpenPortAsync()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string path = string.Format("{0}/{1}", currentDir, "open.cmd");

            string[] lines =
            {
                string.Format( "netsh advfirewall firewall add rule name=\"{0}\" dir=in action=allow program=\"{1}\" enable=yes","poble", string.Format("{0}{1}\\{2}", currentDir,GIT_PATH, EXE_FILE)),
                "\n netsh advfirewall firewall add rule name =\"posHub\" dir =in action = allow protocol = TCP localport = *"
            };

            await File.WriteAllLinesAsync("open.cmd", lines);

            Process process = new Process();
            process.StartInfo.FileName = "open.cmd";
            process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            //process.StandardInput.WriteLine(string.Format(@"netsh advfirewall firewall add rule name =""posHub"" dir =in action = allow protocol = TCP localport = {0}",port));
            //process.StandardInput.Flush();
            //process.StandardInput.Close();
            //process.WaitForExit();
            //Debug.WriteLine(process.StandardOutput.ReadToEnd());
        }

        async void InstallOrUpdate()
        {
            if (MainStatus == UPDATE_STATUS.Install)
            {
                var co = new CloneOptions();

                ShowProgressBar = true;
                co.OnTransferProgress = TransferProgress;

                Task.Run(() =>
                {
                    //co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "Username", Password = "Password" };
                    Repository.Clone(GIT_URI, gitPath, co);
                    MainStatus = UPDATE_STATUS.Launch;
                });

                await OpenPortAsync();
            }
            else
            {
                pullUpdateFromRemote();
            }
        }

        internal void pullUpdateFromRemote()
        {
            if (MainStatus != UPDATE_STATUS.Install)
            {
                using (var repo = new Repository(gitPath))
                {

                    repo.Reset(ResetMode.Hard);

                    // Credential information to fetch
                    LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                    options.FetchOptions = new FetchOptions();

                    
                    //options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                    //    (url, usernameFromUrl, types) =>
                    //        new UsernamePasswordCredentials()
                    //        {
                    //            Username = USERNAME,
                    //            Password = PASSWORD
                    //        });

                    // User information to create a merge commit
                    var signature = new LibGit2Sharp.Signature(
                        new Identity("updater", "updater@posnet.com.au"), DateTimeOffset.Now);

                    options.FetchOptions.OnTransferProgress = TransferProgress;

                    // Pull
                    Task.Run(() =>
                    {
                        Commands.Pull(repo, signature, options);
                        MainStatus = UPDATE_STATUS.Launch;
                    }).Wait();
                }
            }
        }

        /// <summary>
        /// execute poble
        /// </summary>
        public DelegateCommand LaunchCommand { get; private set; }

        void Launch()
        {
            ///run program
            Process process = new Process();
            process.StartInfo.FileName = POBLE_EXE;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Verb = "runas";
            process.Start();
        }


        //git progress for progressbar
        public bool TransferProgress(TransferProgress progress)
        {

            CurrentProgress = progress.ReceivedObjects;
            TotalProgress = progress.TotalObjects;

            if (CurrentProgress >= TotalProgress)
            {
                UpdateStatus = "Please wait";

            }
            else
            {
                UpdateStatus = $"Objects: {progress.ReceivedObjects} of {progress.TotalObjects}, Bytes: {progress.ReceivedBytes}";
            }


            
            return true;
        }

        /// <summary>
        /// check update status
        /// if exe path has no file then update
        /// </summary>
        private async void CheckUpdateStatus()
        {
            if (!File.Exists(POBLE_EXE))
            {
                MainStatus = UPDATE_STATUS.Install;
            }
            else
            {

                MainStatus = UPDATE_STATUS.Update;
                ///change to auto update
                pullUpdateFromRemote();
                //MainStatus = UPDATE_STATUS.Launch;






                //using (var repo = new Repository(gitPath))
                //{
                //    //var lstRef = repo.Network.ListReferences(gitPath);

                //    var status = repo.RetrieveStatus();
                //    var selBranch = repo.Branches[GIT_BRANCH];
                //    if (selBranch == null)
                //    {
                //        //no main branch?
                //    }

                //    Branch curBranch = Commands.Checkout(repo, selBranch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
                //    var remote = repo.Network.Remotes[GIT_BRANCH];
                //    if (remote == null)
                //    {
                //        //no main on remote?
                //    }


                    

                //    // Check if update is available
                //    if (curBranch.TrackingDetails.BehindBy > 0)
                //    {
                //        //update found
                //        MainStatus = UPDATE_STATUS.Update;
                //    }
                //    else
                //    {
                //        MainStatus = UPDATE_STATUS.Launch;
                //    }
                //}





            }
        }






    }




}

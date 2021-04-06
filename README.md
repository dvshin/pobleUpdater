# pobleUpdater
Simple github updater for WPF windows app.

Use github as your deployment server.

modify your config for your app.


    <add key="MainUri" value="https://posnet.com.au" />
    <add key="GitUri" value="https://github.com/poble-pos/poble-win.git" />
    <add key="GitPath" value="/git" />
    <add key="GitBranch" value="main" />
    <add key="ExeFile" value="poble.exe" />  
    
    
    MainUri is your update news page link
    GitUri is your github repo
    GitPath is subfolder of this program
    ExeFile is your main launching app file.
    
    
    Framework dotnet 5
    
    handyControls(3.3.3)
    LibGit2Sharp(0.26.2)
    Prism.Dryloc(8.0.0.1909)
    
    
    

using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace WindowsService123
{
    public partial class Service1 : ServiceBase
    {

        private const string path1 = @"C:\Folder1";
        private const string path2 = @"C:\Folder2";

        private FileSystemWatcher watcher1, watcher2;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                watcher1 = new FileSystemWatcher(path1)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };

                watcher1.Changed += DirectoryChanged;
                watcher1.Created += DirectoryChanged;
                watcher1.Deleted += DirectoryChanged;
                watcher1.Renamed += DirectoryChanged;


                watcher2 = new FileSystemWatcher(path2)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };


                watcher2.Changed += DirectoryChanged2;
                watcher2.Created += DirectoryChanged2;
                watcher2.Deleted += DirectoryChanged2;
                watcher2.Renamed += DirectoryChanged2;

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var dest = Path.Combine(path2, e.Name);
                if (File.Exists(dest))
                {
                    File.Delete(e.FullPath);
                }
                else
                {
                    File.Move(e.FullPath, dest);
                }

                if (!EventLog.SourceExists("myeventlog"))
                { 
                    EventLog.CreateEventSource("myeventlog", "myeventlog"); 
                }
                EventLog log = new EventLog("myeventlog");
                log.Source = "myeventlog";
                var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                File.AppendAllText($"{serviceLocation}\\log.txt", $"{e.ChangeType} - {e.FullPath} \n");
                log.WriteEntry($"{e.ChangeType} - {e.FullPath} \n", EventLogEntryType.Information);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

   

        private void DirectoryChanged2(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (!EventLog.SourceExists("myeventlog"))
                {
                    EventLog.CreateEventSource("myeventlog", "myeventlog");
                }
                EventLog log = new EventLog("myeventlog");
                log.Source = "myeventlog";
                var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                File.AppendAllText($"{serviceLocation}\\log.txt", $"{e.ChangeType} - {e.FullPath} \n");
                log.WriteEntry($"{e.ChangeType} - {e.FullPath} \n", EventLogEntryType.Information);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); 
            }
        }


        protected override void OnStop()
        {
        }
    }
}

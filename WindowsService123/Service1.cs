using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsService123
{
    public partial class Service1 : ServiceBase
    {

        private const string path1 = @"C:\Folder1";
        private const string path2 = @"C:\Folder2";

        FileSystemWatcher watcher1, watcher2;
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
            catch (Exception) 
            { 
                throw; 
            }
        }

        private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                string dest = Path.Combine(path2, e.Name);
                if (File.Exists(dest))
                {
                    File.Delete(e.FullPath);
                }
                else
                {
                    File.Move(e.FullPath, dest);
                }

                if (!EventLog.SourceExists("myeventlog"))
                    System.Diagnostics.EventLog.CreateEventSource("myeventlog", "myeventlog");
                EventLog log = new EventLog("myeventlog");
                log.Source = "myeventlog";
                var msg = $"{e.ChangeType} - {e.FullPath} \n";
                var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                File.AppendAllText($"{serviceLocation}\\log.txt", msg);
                log.WriteEntry(msg, EventLogEntryType.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }


        private void DirectoryChanged2(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (!EventLog.SourceExists("myeventlog"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("myeventlog", "myeventlog");
                }

                EventLog log = new EventLog("myeventlog");
                log.Source = "myeventlog";
                var msg = $"{e.ChangeType} - {e.FullPath} \n";
                var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                File.AppendAllText($"{serviceLocation}\\log.txt", msg);
                log.WriteEntry(msg, EventLogEntryType.Information);
            }
            catch (Exception) { throw; }
        }

        protected override void OnStop()
        {
        }
    }
}

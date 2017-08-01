using System;
using System.Configuration;
using System.IO;
using Host.Contract;

namespace Plugin.FileWatcher
{
    public class FileWatcherService : PluginService
    {
        private FileSystemWatcher _fileSystemWatcher;

        public override void OnStart(string[] args)
        {
            string directory = ConfigurationManager.AppSettings["directory"];

            if (string.IsNullOrEmpty(directory))
            {
                throw new Exception("The directory is not configured.");
            }

            if (!Directory.Exists(directory))
            {
                throw new Exception($"The directory {directory} does not exist.");
            }

            _fileSystemWatcher = new FileSystemWatcher(directory);
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Created += OnChanged;
            _fileSystemWatcher.Deleted += OnChanged;
            _fileSystemWatcher.Renamed += OnRenamed;

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public override void OnStop()
        {
            if (_fileSystemWatcher != null && _fileSystemWatcher.EnableRaisingEvents)
            {
                _fileSystemWatcher.Dispose();
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }
    }
}

using System;
using System.Configuration;
using System.IO;
using Host.Contract;
using Host.Contract.Log;

namespace Plugin.FileWatcher
{
    public class FileWatcherService : PluginService
    {
        private FileSystemWatcher _fileSystemWatcher;
        private ILogger _logger;

        public override void OnStart(ILogger logger)
        {
            _logger = logger;

            try
            {
                var directory = GetTargetDirectory();

                _fileSystemWatcher = new FileSystemWatcher(directory);
                _fileSystemWatcher.Changed += OnChanged;
                _fileSystemWatcher.Created += OnChanged;
                _fileSystemWatcher.Deleted += OnChanged;
                _fileSystemWatcher.Renamed += OnRenamed;

                _fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                _logger.Write(e.Message);
            }
        }

        private string GetTargetDirectory()
        {
            string directory = ConfigurationManager.AppSettings["directory"];

            if (string.IsNullOrEmpty(directory))
            {
                throw new Exception("The directory is not configured.");
            }

            if (string.IsNullOrEmpty(Path.GetPathRoot(directory)) || Path.GetPathRoot(directory) == @"\")
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            }

            if (!Directory.Exists(directory))
            {
                if ((ConfigurationManager.AppSettings["createDirectory"] != null) &&
                    (ConfigurationManager.AppSettings["createDirectory"].ToLower() == "true"))
                {
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    throw new Exception($"The directory {directory} does not exist.");
                }
            }
            return directory;
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
            _logger.Write("File: " + e.FullPath + " " + e.ChangeType);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            _logger.Write($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }
    }
}

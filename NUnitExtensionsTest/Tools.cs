namespace NUnit.Framework
{
    using System;
    using System.IO;
    using System.Threading;

    public static class Tools
    {
        public static void DeleteFile(string fileName)
        {
            if (!File.Exists(fileName)) return;

            string watchPath = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(watchPath)) watchPath = Environment.CurrentDirectory;

            using (FileSystemWatcher watcher = new FileSystemWatcher(watchPath))
            using (ManualResetEvent deleteEvent = new ManualResetEvent(false)) {
                watcher.EnableRaisingEvents = true;
                watcher.Deleted += (s, e) => {
                    deleteEvent.Set();
                };
                File.Delete(fileName);
                if (!deleteEvent.WaitOne(5000)) {
                    string message = string.Format("Can't delete file: {0}", fileName);
                    throw new IOException(message);
                }
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            // Delete each individual file
            string[] files = GetFiles(path);
            foreach (string file in files) {
                DeleteFile(file);
            }

            // Copy the subdirectories
            string[] dirs = GetDirectories(path);
            foreach (string dir in dirs) {
                string nextDir = Path.Combine(path, Path.GetFileName(dir));
                DeleteDirectory(nextDir);
                Directory.Delete(nextDir);
            }
        }

        private static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        private static string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        }
    }
}


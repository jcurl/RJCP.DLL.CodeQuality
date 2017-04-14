namespace NUnit.Framework
{
    using System;
    using System.IO;

    public static class Tools
    {
        public static bool DeleteFile(string path)
        {
            int attempts = 4;
            bool first = true;
            while (attempts > 0 && File.Exists(path)) {
                if (!first) System.Threading.Thread.Sleep(100);
                File.Delete(path);
                --attempts;
                first = false;
            }
            return !File.Exists(path);
        }

        public static bool DeleteDirectory(string path)
        {
            int attempts = 4;
            bool first = true;
            while (attempts > 0 && Directory.Exists(path)) {
                if (!first) System.Threading.Thread.Sleep(100);
                try {
                    Directory.Delete(path, true);
                } catch (DirectoryNotFoundException) {
                    /* We ignore this case, and retry */
                }
                --attempts;
                first = false;
            }
            return !Directory.Exists(path);
        }
    }
}


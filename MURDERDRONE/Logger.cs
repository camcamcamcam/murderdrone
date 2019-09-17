using System;
using System.IO;

namespace MURDERDRONE
{
    public static class Logger
    {

        public static void Log(string message)
        {
            string filePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
               "Log",
               "peepeepoopoo.txt");
            TextWriter sw = new StreamWriter(filePath, true);
            sw.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
            sw.Close();
        }
    }
}

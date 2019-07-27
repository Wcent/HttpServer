using System;
using System.IO;
using System.Text;

namespace Common
{
    class Logger {
        static string directory = "log";
        static string file = "log/" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        static FileStream fileStream;
        private string name;

        private Logger(string name)
        {
            this.name = name;
        }

        static public Logger build(string name)
        {
            return new Logger(name);
        }

        public void log(string message)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                fileStream = File.Open(file, FileMode.Append);

                // fileStream.Position = fileStream.Length;
                string logMark = string.Format("\n\r********{0}: {1}********\n\r", name, DateTime.Now.ToString());
                byte[] markBytes = Encoding.UTF8.GetBytes(logMark);
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                fileStream.Write(markBytes, 0, markBytes.Length);
                fileStream.Write(messageBytes, 0, messageBytes.Length);
                fileStream.Write(markBytes, 0, markBytes.Length);
                fileStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("写日志异常：{0}", ex.ToString());
                Console.WriteLine("日志消息：{0}", message);
            }
            finally
            {
               fileStream.Close();
            }
        }
    }
}
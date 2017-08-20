using System;
using System.IO;
using System.Reflection;

namespace ISO_8583_MessageSandbox
{
    public static class Log
    {
        private static string LogFilePath
        {
            get
            {
                var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return currentLocation != null
                    ? $"{LogFilePath}\\ISO_8583_MessageSandbox_log.txt"
                    : "c:\\ISO_8583_MessageSandbox_log.txt";
            }
        }

        static Log()
        {
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath);
            }
        }

        public static void Write(string message)
        {
            using (var streamWriter = File.AppendText(LogFilePath))
            {
                streamWriter.Write(message);
            }
        }

        public static void WriteLine(string message)
        {
            using (var streamWriter = File.AppendText(LogFilePath))
            {
                streamWriter.WriteLine(message);
            }
        }

        public static void WriteBytes(byte[] message)
        {
            using (var streamWriter = File.AppendText(LogFilePath))
            {
                for (var i = 0; i < message.Length; i++)
                {
                    streamWriter.WriteLine("message[{0}] = {1}", i, message[i]);
                }
            }
        }

        public static void WriteHex(byte[] message)
        {
            var hex = BitConverter.ToString(message).Replace("-", string.Empty);

            using (var streamWriter = File.AppendText(LogFilePath))
            {
                streamWriter.WriteLine(hex);
            }
        }

        private static string ByteArrayToHex(byte[] byteArray)
        {
            var hex = BitConverter.ToString(byteArray);
            return hex; //.Replace("-", "");

            //StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            //foreach (byte b in byteArray)
            //    hex.AppendFormat("{0:x2}", b);
            //return hex.ToString();
        }
    }
}
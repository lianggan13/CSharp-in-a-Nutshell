﻿using log4net;
using log4net.Config;
using System.Reflection;

namespace Utilities
{
    public class LogHelper
    {
        //private static readonly ILog ConsoleLog = LogManager.GetLogger("ColoredLogConsoleAppender");

        static LogHelper()
        {
            string ConfigDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "Config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(ConfigDir, "log4net.config")));
        }

        private static readonly ILog FileLog = LogManager.GetLogger("RollingLogFileAppender");

        //public static void ConsoleInfo(string text) => ConsoleLog.Info(text);

        public static void Info(string text) => FileLog.Info(text);

        public static void Info(string text, Exception ex) => FileLog.Info(text, ex);

        public static void Warn(string text) => FileLog.Warn(text);

        public static void Warn(string text, Exception ex) => FileLog.Warn(text, ex);

        public static void Error(string text) => FileLog.Error(text);

        public static void Error(string text, Exception ex) => FileLog.Error(text, ex);

        public static void Fatal(string text) => FileLog.Fatal(text);

        public static void Fatal(string text, Exception ex) => FileLog.Fatal(text, ex);
    }
}
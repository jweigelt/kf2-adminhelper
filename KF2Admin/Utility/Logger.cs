/* 
 * This file is part of kf2 adminhelper.
 * 
 * SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * kf2 adminhelper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with kf2 adminhelper.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.IO;

namespace KF2Admin.Utility
{
    enum LogLevel
    {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Banner = 4
    }

    static class Logger
    {
        public static bool LogToFile { get; set; } = false;
        public static LogLevel MinLevel { get; set; } = LogLevel.Verbose;
        public static string LogFile { get; set; } = "/log.txt";

        public static void Log(string message, LogLevel logLevel, params string[] args)
        {
            if (logLevel < MinLevel && logLevel != LogLevel.Banner) return;
            message = string.Format(message, args);

            if (logLevel == LogLevel.Banner)
            {
                Console.WriteLine(message);
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Verbose:
                    message = "DEBUG | " + message;
                    break;
                case LogLevel.Info:
                    message = "INFO  | " + message;
                    break;
                case LogLevel.Warning:
                    message = "WARN  | " + message;
                    break;
                case LogLevel.Error:
                    message = "ERROR | " + message;
                    break;
            }

            message = "[" + DateTime.Now.ToString() + "] " + message;

            Console.WriteLine(message);

            if (LogToFile)
            {
                File.AppendAllText(LogFile, message);
            }
        }
    }
}
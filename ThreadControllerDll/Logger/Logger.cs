using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThreadControllerDll.OtherComponents.LoggerTasks
{
    /// <summary>
    /// Logger class used to assist in the debugging and maintenance of the program
    /// </summary>
    public class Logger
    {
#pragma warning disable CS0162 // Unreachable code detected For debugging purposes. Setting the logger level as a constant can cause an unreachable code warning
        private readonly Level _level = Level.ERROR;

        public Logger(Level level)
        {
            _level = level;
        }

        public enum Level
        {
            INFO,
            DEBUG,
            PROCCESS,
            ERROR
        }

        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="message">Message to be written</param>
        /// <param name="logLevel">The log level or severity of the message</param>
        public void Log(string message, Level logLevel)
        {
            switch (_level)
            {
                case Level.ERROR:
                    if (logLevel == Level.ERROR)
                        Console.WriteLine(message);
                    break;

                case Level.PROCCESS:
                    if (logLevel == Level.ERROR || logLevel == Level.PROCCESS)
                        Console.WriteLine(message);
                    break;

                case Level.INFO:
                    if (logLevel == Level.ERROR || logLevel == Level.PROCCESS || logLevel == Level.INFO)
                        Console.WriteLine(message);
                    break;

                case Level.DEBUG:
                    if (logLevel == Level.ERROR || logLevel == Level.PROCCESS || logLevel == Level.INFO || logLevel == Level.DEBUG)
                        Console.WriteLine(message);
                    break;
            }
        }

#pragma warning restore CS0162 // Unreachable code detected For debugging purposes
    }
}
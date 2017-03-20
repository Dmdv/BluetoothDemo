using JetBrains.Annotations;
using NLog;

namespace BluetoothDemo
{
    [UsedImplicitly]
    internal class FileLogger : ILog
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Log(string message)
        {
            Logger.Info(message);
        }
    }


    public interface ILog
    {
        void Log(string message);
    }
}
namespace Auction_Website.BLL.IServices
{
    public interface ILoggerService
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
        void LogError(Exception exception);
        void Log(string logType, string logData);
    }
}
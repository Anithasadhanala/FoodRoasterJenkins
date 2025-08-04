using Serilog;
using Serilog.Context;

namespace FoodRoasterServer.Services
{
    public class AuditService : IAuditService
    {
        private readonly Serilog.ILogger _auditLogger;

        public AuditService()
        {
            _auditLogger = new LoggerConfiguration()
                .WriteTo.File("Logs/audit.log",
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public void Track(string message)
        {
            using (LogContext.PushProperty("LogType", "Audit"))
            {
                _auditLogger.Information(message);
            }
        }
    }
}

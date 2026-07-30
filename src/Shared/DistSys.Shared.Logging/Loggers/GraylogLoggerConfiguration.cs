using Serilog.Events;

namespace DistSys.Shared.Logging.Loggers;

public class GraylogLoggerConfiguration
{
    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public LogEventLevel MinimumLevel { get; set; }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Publisher.Console.Configurations;

public static class LoggerRegister
{
  public static void RegisterLoggers(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    var defaultLogLevel = LogLevel.Information;
    // if (!string.IsNullOrWhiteSpace(configuration.GetSection("Logging:LogLevel:Default").Value))
    //   Enum.TryParse(configuration.GetSection("Logging:LogLevel:Default").Value, true, out defaultLogLevel);
    System.Console.Out.WriteLine($"Console:LogLevel:Default: {defaultLogLevel}");
    serviceCollection.AddLogging(loggingBuilder => loggingBuilder
      .SetMinimumLevel(defaultLogLevel));
  }
}
// See https://aka.ms/new-console-template for more information

using Consumer.Console.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("MassTransit Sub");

var hostBuilder = new HostBuilder()
  .ConfigureHostConfiguration(configHost =>
    configHost.AddEnvironmentVariables("ASPNETCORE_")
  )
  .ConfigureServices((hostContext, services) =>
  {
    services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(45)); // Wait for graceful shutdown.
    services.RegisterLoggers(hostContext.Configuration);
    services.RegisterMassTransit(hostContext.Configuration);
  })
  .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
  {
    loggingBuilder.AddConfiguration(hostBuilderContext.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
  });

await hostBuilder.RunConsoleAsync();
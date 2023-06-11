// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Publisher.Console.Configurations;
using Publisher.Console.Services;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Events.V1;

Console.WriteLine("MassTransit Pub");

var hostBuilder = new HostBuilder()
  .ConfigureHostConfiguration(configHost =>
    configHost.AddEnvironmentVariables("ASPNETCORE_")
  )
  .ConfigureServices((hostContext, services) =>
  {
    services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(45)); // Wait for graceful shutdown.
    services.RegisterLoggers(hostContext.Configuration);
    services.RegisterMassTransit(hostContext.Configuration);
    services.RegisterServices();
  })
  .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
  {
    loggingBuilder.AddConfiguration(hostBuilderContext.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
  });

var app = hostBuilder.Build();
await MainMenu(app);
await app.RunAsync();

async Task MainMenu(IHost appBuilerHost)
{
  var scopedFactory = appBuilerHost.Services.GetService<IServiceScopeFactory>();

  using var scope = scopedFactory!.CreateScope();
  var service = scope.ServiceProvider.GetService<ITObjectService>();

  if (service is null) return;
  
  string? inputKey;
  
  do
  {
    Menu();
    inputKey = Console.ReadLine();
    switch (inputKey)
    {
      case "1":
      {
        var correlationId = Guid.NewGuid();
        await service.SendCreateTObjectCommand(new CreateTObjectCommand { ObjectName = "object", CorrelationId = correlationId });
        await service.PublishTObjectCreatedEvent(new TObjectCreatedEvent { ObjectName = "object", CreatedDateTime = DateTime.UtcNow, CorrelationId = correlationId });
        break;
      }
      case "2":
        for (var i = 0; i < 10; i++)
        {
          var correlationId = Guid.NewGuid();
          await service.SendCreateTObjectCommand(new CreateTObjectCommand { ObjectName = $"object-{i+1}", CorrelationId = correlationId });
          await service.PublishTObjectCreatedEvent(new TObjectCreatedEvent { ObjectName = $"object-{i+1}", CreatedDateTime = DateTime.UtcNow , CorrelationId = correlationId});
        }
        break;
      case "3":
        await service.PublishTObjectCreatedEvent(new TObjectCreatedEvent { ObjectName = "object", CreatedDateTime = DateTime.UtcNow, CorrelationId = Guid.NewGuid() });
        break;
      case "4":
        for (var i = 0; i < 10; i++)
        {
          await service.PublishTObjectCreatedEvent(new TObjectCreatedEvent { ObjectName = $"object-{i+1}", CreatedDateTime = DateTime.UtcNow, CorrelationId = Guid.NewGuid() });
        }
        break;
      default:
        Console.WriteLine("choose wisely!");
        break;
    }
    
  } while (inputKey?.ToLower() != "q");
  Console.WriteLine("Bye");
  Environment.Exit(0);
}

void Menu()
{
  Console.WriteLine("************ Menu ************");
  Console.WriteLine("exit : q || Q ");
  Console.WriteLine("1- Send Command");
  Console.WriteLine("2- BulkSend Command (10)");
  Console.WriteLine("3- Published Event");
  Console.WriteLine("4- BulkPublished Event (10)");
}
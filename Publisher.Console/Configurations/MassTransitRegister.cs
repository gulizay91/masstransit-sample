using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Publisher.Console.Configurations;

public static class MassTransitRegister
{
  // ps: come from config
  private const string busHost = "amqps://localhost:5672/";
  private const string busUsername = "guest";
  private const string busPassword = "guest";

  public static void RegisterMassTransit(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddMassTransit(x =>
    {
      // init bus
      x.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(new Uri(busHost), a =>
        {
          a.Username(busUsername);
          a.Password(busPassword);
        });
      });
    });
  }
}
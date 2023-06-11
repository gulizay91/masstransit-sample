using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Publisher.RabbitMq.Configurations;

public static class MassTransitRegister
{
  // ps: come from config
  private const string BusHost = "amqps://localhost:5672/";
  private const string BusUsername = "guest";
  private const string BusPassword = "guest";

  public static void RegisterMassTransit(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddMassTransit(x =>
    {
      // init bus
      x.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(new Uri(BusHost), a =>
        {
          a.Username(BusUsername);
          a.Password(BusPassword);
        });
      });
    });
  }
}
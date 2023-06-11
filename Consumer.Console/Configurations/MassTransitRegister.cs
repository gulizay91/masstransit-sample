using Consumer.Console.Consumers;
using Consumer.Console.Consumers.Observers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Contracts.Constants;

namespace Consumer.Console.Configurations;

public static class MassTransitRegister
{
  // ps: come from config
  private const string busHost = "amqps://localhost:5672/";
  private const string busUsername = "guest";
  private const string busPassword = "guest";

  public static void RegisterMassTransit(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    // useful documentation https://masstransit-project.com/
    // lots of great example and scenarios https://www.youtube.com/user/PhatBoyG
    serviceCollection.AddMassTransit(x =>
    {
      // add consumer to bus
      x.AddConsumer<CreateTObjectCommandConsumer>();
      x.AddConsumer<TObjectCreatedEventConsumer>();


      // init bus
      x.UsingRabbitMq((context, cfg) =>
      {
        // observe pre - post - fault states of consumer
        cfg.ConnectConsumeObserver(new ConsumeObserver());
        cfg.Host(new Uri(busHost), a =>
        {
          a.Username(busUsername);
          a.Password(busPassword);
        });

        cfg.ReceiveEndpoint(QueueNames.CreateTObjectCommandConsumerQueueName, ep =>
        {
          ep.AutoDelete = false;

          ep.Durable = true;

          ep.ExchangeType = ExchangeType.Fanout;

          ep.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromMilliseconds(1000)); });

          ep.PrefetchCount = 10; // fetch limit

          // circuit breaker
          ep.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

          ep.ConfigureConsumer<CreateTObjectCommandConsumer>(context);
        });


        cfg.ReceiveEndpoint(QueueNames.TObjectCreatedEventConsumerQueueName, ep =>
        {
          ep.AutoDelete = false;

          ep.Durable = true;

          ep.ExchangeType = ExchangeType.Fanout;

          ep.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromMilliseconds(1000)); });

          ep.PrefetchCount = 10; // fetch limit
          
          ep.UseRateLimit(1, TimeSpan.FromSeconds(5)); // throttling consumer
          
          // circuit breaker
          ep.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

          ep.ConfigureConsumer<TObjectCreatedEventConsumer>(context);
        });
      });
    });

  }
}
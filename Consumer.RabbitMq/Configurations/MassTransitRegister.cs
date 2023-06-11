using System.Data;
using Consumer.RabbitMq.Consumers;
using Consumer.RabbitMq.Consumers.Observers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RabbitMQ.Client;
using Shared.Contracts.Constants;

namespace Consumer.RabbitMq.Configurations;

public static class MassTransitRegister
{
  // ps: come from config
  private const string BusHost = "amqps://localhost:5672/";
  private const string BusUsername = "guest";
  private const string BusPassword = "guest";

  public static void RegisterMassTransit(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddQuartz(q =>
    {
      q.SchedulerName = "MassTransit-Scheduler";
      q.SchedulerId = "AUTO";
      q.UseMicrosoftDependencyInjectionJobFactory();
      q.UseTimeZoneConverter();
      q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });
    });
    serviceCollection.AddQuartzHostedService(options =>
    {
      options.StartDelay = TimeSpan.FromSeconds(5);
      options.WaitForJobsToComplete = true;
    });

    // useful documentation https://masstransit-project.com/
    // lots of great example and scenarios https://www.youtube.com/user/PhatBoyG
    serviceCollection.AddMassTransit(x =>
    {
      // consumer scheduler
      var schedulerEndpoint = new Uri($"queue:{QueueNames.DeafultSchedulerQueueName}");
      x.AddMessageScheduler(schedulerEndpoint);

      // for consume scheduler queue
      x.AddQuartzConsumers();

      // add consumer to bus
      x.AddConsumer<CreateObjectCommandConsumer>();
      x.AddConsumer<ObjectCreatedEventConsumer>();
      x.AddConsumer<UpdateObjectCommandConsumer>();

      x.AddConsumer<SchedulerObjectConsumer>();

      // init bus
      x.UsingRabbitMq((context, cfg) =>
      {
        // To schedule messages from a consumer, use any of the ConsumeContext extension methods, such as ScheduleSend, to schedule messages
        cfg.UseMessageScheduler(schedulerEndpoint);

        // temporary solution
        cfg.UseInMemoryScheduler(q =>
        {
          q.QueueName = QueueNames.DeafultSchedulerQueueName;
          q.SchedulerFactory = context.GetService<ISchedulerFactory>()!;
          //q.JobFactory = context.GetService<IJobFactory>();
          //q.StartScheduler = false;
        });

        // observe pre - post - fault states of consumer
        cfg.ConnectConsumeObserver(new ConsumeObserver());

        cfg.Host(new Uri(BusHost), a =>
        {
          a.Username(BusUsername);
          a.Password(BusPassword);
        });

        cfg.ReceiveEndpoint(QueueNames.CreateObjectCommandConsumerQueueName, ep =>
        {
          ep.AutoDelete = false;

          ep.Durable = true;

          ep.ExchangeType = ExchangeType.Fanout;

          ep.UseMessageRetry(r =>
          {
            r.Interval(3, TimeSpan.FromMilliseconds(1000));
            r.Ignore<DuplicateNameException>(); // we dont need this message retry because that obviously nonsense
          });

          ep.PrefetchCount = 10; // fetch limit

          // circuit breaker
          ep.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

          ep.ConfigureConsumer<CreateObjectCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNames.ObjectCreatedEventConsumerQueueName, ep =>
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

          ep.ConfigureConsumer<ObjectCreatedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNames.UpdateObjectCommandConsumerQueueName, ep =>
        {
          ep.AutoDelete = false;

          ep.Durable = true;

          ep.ExchangeType = ExchangeType.Fanout;

          // ep.UseScheduledRedelivery(r =>
          // {
          //   r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3));
          // });
          ep.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromMilliseconds(1000)); });

          ep.PrefetchCount = 10; // fetch limit

          // circuit breaker
          ep.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

          ep.ConfigureConsumer<UpdateObjectCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNames.SchedulerObjectConsumerQueueName, ep =>
        {
          ep.AutoDelete = false;

          ep.Durable = true;

          ep.ExchangeType = ExchangeType.Fanout;

          ep.UseMessageRetry(r =>
          {
            r.Interval(3, TimeSpan.FromMilliseconds(1000));
            r.Ignore<TimeoutException>(); // we dont need this message retry because this message's end must be a death-letter queue
          });

          ep.PrefetchCount = 10; // fetch limit

          // circuit breaker
          ep.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

          ep.ConfigureConsumer<SchedulerObjectConsumer>(context);
        });
      });
    });
  }
}
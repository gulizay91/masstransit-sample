using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.V1;

namespace Consumer.RabbitMq.Consumers;

public class ObjectCreatedEventConsumer : IConsumer<ObjectCreatedEvent>
{
  private readonly ILogger<ObjectCreatedEventConsumer> _logger;

  public ObjectCreatedEventConsumer(ILogger<ObjectCreatedEventConsumer> logger)
  {
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<ObjectCreatedEvent> context)
  {
    _logger.LogInformation(
      $"{nameof(ObjectCreatedEvent)} received at {DateTime.UtcNow} => {context.Message.ObjectName} - {context.Message.CreatedDateTime} - {context.Message.CorrelationId}");
    await Task.CompletedTask;
  }
}
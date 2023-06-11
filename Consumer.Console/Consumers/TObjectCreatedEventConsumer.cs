using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.V1;

namespace Consumer.Console.Consumers;

public class TObjectCreatedEventConsumer : IConsumer<TObjectCreatedEvent>
{
  private readonly ILogger<TObjectCreatedEventConsumer> _logger;
  public TObjectCreatedEventConsumer(ILogger<TObjectCreatedEventConsumer> logger)
  {
    _logger = logger;
  }
  public async Task Consume(ConsumeContext<TObjectCreatedEvent> context)
  {
    _logger.LogInformation($"{nameof(TObjectCreatedEvent)} received at {DateTime.UtcNow} => {context.Message.ObjectName} - {context.Message.CreatedDateTime} - {context.Message.CorrelationId}");
    await Task.CompletedTask;
  }
}
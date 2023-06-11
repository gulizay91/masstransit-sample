using System.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Events.V1;

namespace Consumer.RabbitMq.Consumers;

public class CreateObjectCommandConsumer : IConsumer<CreateObjectCommand>
{
  private readonly ILogger<CreateObjectCommandConsumer> _logger;

  public CreateObjectCommandConsumer(ILogger<CreateObjectCommandConsumer> logger)
  {
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<CreateObjectCommand> context)
  {
    if (context.Message.ObjectName.Contains("1")) // just for exception filter
    {
      var exception = new DuplicateNameException($"{context.Message.ObjectName} duplicated.");
      _logger.LogError("This exception not retry: {exceptionMessage}", exception.Message);
      throw exception;
    }

    if (context.Message.ObjectName.Contains("2") && context.GetRetryAttempt() < 1) // retry
    {
      _logger.LogWarning("This exception is going to retry: {ObjectName} , {DateTimeUtcNow}",
        context.Message.ObjectName, DateTime.UtcNow);
      throw new ConcurrencyException();
    }

    _logger.LogInformation(
      $"{nameof(CreateObjectCommand)} received at {DateTime.UtcNow}, RetryAttempt: {context.GetRetryAttempt()} => {context.Message.ObjectName} - {context.Message.CorrelationId}");

    await context.Publish(new ObjectCreatedEvent
    {
      ObjectName = context.Message.ObjectName, CreatedDateTime = DateTime.UtcNow,
      CorrelationId = context.Message.CorrelationId
    }, context.CancellationToken);

    await Task.CompletedTask;
  }
}
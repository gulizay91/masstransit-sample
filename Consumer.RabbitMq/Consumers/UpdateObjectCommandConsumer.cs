using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Constants;
using Shared.Contracts.Events.V1;

namespace Consumer.RabbitMq.Consumers;

public class UpdateObjectCommandConsumer : IConsumer<UpdateObjectCommand>
{
  private readonly ILogger<UpdateObjectCommandConsumer> _logger;

  public UpdateObjectCommandConsumer(ILogger<UpdateObjectCommandConsumer> logger)
  {
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<UpdateObjectCommand> context)
  {
    // manual scheduled
    if (context.Message.ReplaceObjectName.Contains("scheduled") && !context.Message.ReplaceObjectName.Contains("/"))
    {
      var sendAddress = new Uri($"queue:{QueueNames.SchedulerObjectConsumerQueueName}");
      var sendEndpoint = await context.GetSendEndpoint(sendAddress);
      await sendEndpoint.Send(
        new ScheduleUpdateObjectCommand
        {
          DeliveryTime = DateTime.UtcNow.AddMinutes(2),
          ReplaceObjectName = context.Message.ReplaceObjectName,
          CorrelationId = context.Message.CorrelationId
        });
    }

    _logger.LogInformation(
      $"{nameof(UpdateObjectCommand)} received at {DateTime.UtcNow} => {context.Message.ReplaceObjectName} - {context.Message.CorrelationId}");

    await context.Publish(new ObjectUpdatedEvent
    {
      OldObjectName = context.Message.ReplaceObjectName.Replace("new", string.Empty),
      ReplaceObjectName = context.Message.ReplaceObjectName, UpdatedDateTime = DateTime.UtcNow,
      CorrelationId = context.Message.CorrelationId
    }, context.CancellationToken);

    await Task.CompletedTask;
  }
}
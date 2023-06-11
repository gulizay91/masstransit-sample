using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Constants;

namespace Consumer.RabbitMq.Consumers;

public class SchedulerObjectConsumer :
    IConsumer<ScheduleUpdateObjectCommand>
  //IConsumer<ScheduleUpdateObjectEvent>
{
  private readonly ILogger<SchedulerObjectConsumer> _logger;

  public SchedulerObjectConsumer(ILogger<SchedulerObjectConsumer> logger)
  {
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<ScheduleUpdateObjectCommand> context)
  {
    if (DateTime.UtcNow >= new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 0, 0))
    {
      var exception = new TimeoutException($"This message has timed out! {JsonSerializer.Serialize(context.Message)}");
      _logger.LogError(
        $"{nameof(ScheduleUpdateObjectCommand)} dead at {DateTime.UtcNow} => {exception.Message}");
      throw exception;
    }

    var sendAddress = new Uri($"queue:{QueueNames.UpdateObjectCommandConsumerQueueName}");
    await context.ScheduleSend<UpdateObjectCommand>(sendAddress,
      context.Message.DeliveryTime,
      new UpdateObjectCommand
      {
        ReplaceObjectName = context.Message.ReplaceObjectName + $"/{context.Message.DeliveryTime}",
        CorrelationId = context.Message.CorrelationId
      });

    _logger.LogInformation(
      $"{nameof(ScheduleUpdateObjectCommand)} received at {DateTime.UtcNow} => {context.Message.ReplaceObjectName} - {context.Message.CorrelationId}");

    await Task.CompletedTask;
  }
}
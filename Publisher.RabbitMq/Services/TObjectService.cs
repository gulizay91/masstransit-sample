using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Constants;
using Shared.Contracts.Events.V1;

namespace Publisher.RabbitMq.Services;

public class ObjectService : IObjectService
{
  private static readonly TimeSpan MessagePublishCancellationTokenTimeout = TimeSpan.FromSeconds(20);
  private readonly IBus _bus;
  private readonly ILogger<ObjectService> _logger;

  public ObjectService(IBus bus, ILogger<ObjectService> logger)
  {
    _logger = logger;
    _bus = bus;
  }

  public async Task PublishObjectCreatedEvent(ObjectCreatedEvent messageEvent)
  {
    var cancelationToken = new CancellationTokenSource(MessagePublishCancellationTokenTimeout);
    await _bus.Publish(messageEvent, cancelationToken.Token);
    _logger.LogInformation(
      $"Published Event Message for {nameof(ObjectCreatedEvent)}, Message: {JsonSerializer.Serialize(messageEvent)}");
  }

  public async Task SendCreateObjectCommand(CreateObjectCommand messageCommand)
  {
    var sendEndpoint =
      await _bus.GetSendEndpoint(
        new Uri($"queue:{QueueNames.CreateObjectCommandConsumerQueueName}"));

    await sendEndpoint.Send(messageCommand);
    _logger.LogInformation(
      $"Send Command Message for {nameof(CreateObjectCommand)}, Message: {JsonSerializer.Serialize(messageCommand)}");
  }

  public async Task PublishObjectUpdatedEvent(ObjectUpdatedEvent messageEvent)
  {
    var cancelationToken = new CancellationTokenSource(MessagePublishCancellationTokenTimeout);
    await _bus.Publish(messageEvent, cancelationToken.Token);
    _logger.LogInformation(
      $"Published Event Message for {nameof(ObjectUpdatedEvent)}, Message: {JsonSerializer.Serialize(messageEvent)}");
  }

  public async Task SendUpdateObjectCommand(UpdateObjectCommand messageCommand)
  {
    var sendEndpoint =
      await _bus.GetSendEndpoint(
        new Uri($"queue:{QueueNames.UpdateObjectCommandConsumerQueueName}"));

    await sendEndpoint.Send(messageCommand);
    _logger.LogInformation(
      $"Send Command Message for {nameof(UpdateObjectCommand)}, Message: {JsonSerializer.Serialize(messageCommand)}");
  }
}
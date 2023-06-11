using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;
using Shared.Contracts.Constants;
using Shared.Contracts.Events.V1;

namespace Publisher.Console.Services;

public class TObjectService : ITObjectService
{
  private static readonly TimeSpan MessagePublishCancellationTokenTimeout = TimeSpan.FromSeconds(20);
  private readonly IBus _bus;
  private readonly ILogger<TObjectService> _logger;

  public TObjectService(IBus bus, ILogger<TObjectService> logger)
  {
    _logger = logger;
    _bus = bus;
  }
  
  public async Task PublishTObjectCreatedEvent(TObjectCreatedEvent messageEvent)
  {
    var cancelationToken = new CancellationTokenSource(MessagePublishCancellationTokenTimeout);
    await _bus.Publish(messageEvent, cancelationToken.Token);
    _logger.LogInformation(
      $"Published Event Message for {nameof(TObjectCreatedEvent)}, Message: {JsonSerializer.Serialize(messageEvent)}");
  }

  public async Task SendCreateTObjectCommand(CreateTObjectCommand messageCommand)
  {
    var sendEndpoint =
      await _bus.GetSendEndpoint(
        new Uri($"queue:{QueueNames.CreateTObjectCommandConsumerQueueName}"));

    await sendEndpoint.Send(messageCommand);
    _logger.LogInformation(
      $"Send Command Message for {nameof(CreateTObjectCommand)}, Message: {JsonSerializer.Serialize(messageCommand)}");
  }
}
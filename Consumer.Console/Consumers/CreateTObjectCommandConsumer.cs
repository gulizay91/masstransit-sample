using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Commands.V1;

namespace Consumer.Console.Consumers;

public class CreateTObjectCommandConsumer : IConsumer<CreateTObjectCommand>
{
  private readonly ILogger<CreateTObjectCommandConsumer> _logger;
  public CreateTObjectCommandConsumer(ILogger<CreateTObjectCommandConsumer> logger)
  {
    _logger = logger;
  }
  public async Task Consume(ConsumeContext<CreateTObjectCommand> context)
  {
    _logger.LogInformation($"{nameof(CreateTObjectCommand)} received at {DateTime.UtcNow} => {context.Message.ObjectName} - {context.Message.CorrelationId}");
    await Task.CompletedTask;
  }
}
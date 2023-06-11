using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumer.Console.Consumers.Observers;

public class ConsumeObserver : IConsumeObserver
{
  public ConsumeObserver()
  {
  }

  public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
  {
    await System.Console.Out.WriteLineAsync(
      $"TEventPre:{context.Message.GetType().Name}:{JsonSerializer.Serialize(context.Message)}");
    await Task.CompletedTask;
  }

  public async Task PostConsume<T>(ConsumeContext<T> context) where T : class
  {
    await System.Console.Out.WriteLineAsync(
      $"TEventPost:{context.Message.GetType().Name}:{JsonSerializer.Serialize(context.Message)}");
    await Task.CompletedTask;
  }
  public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
  {
    await System.Console.Out.WriteLineAsync(
      $"TEventFault:{context.Message.GetType().Name}:{JsonSerializer.Serialize(context.Message)}");
    await Task.CompletedTask;
  }
}
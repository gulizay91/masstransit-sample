using Shared.Contracts.Common;

namespace Shared.Contracts.Events.V1;

public class TObjectCreatedEvent : IEvent
{
  public Guid CorrelationId { get; set; }
  public string ObjectName { get; init; }
  public DateTime CreatedDateTime { get; init; }
}
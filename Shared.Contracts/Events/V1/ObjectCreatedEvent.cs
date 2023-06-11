using Shared.Contracts.Common;

namespace Shared.Contracts.Events.V1;

public record ObjectCreatedEvent : IEvent
{
  public required string ObjectName { get; init; }
  public required DateTime CreatedDateTime { get; init; }
  public Guid CorrelationId { get; set; }
}
using Shared.Contracts.Common;

namespace Shared.Contracts.Events.V1;

public record ObjectUpdatedEvent : IEvent
{
  public required string OldObjectName { get; init; }
  public required string ReplaceObjectName { get; init; }
  public required DateTime UpdatedDateTime { get; init; }
  public Guid CorrelationId { get; set; }
}
using Shared.Contracts.Common;

namespace Shared.Contracts.Commands.V1;

public record UpdateObjectCommand : ICommand
{
  public required string ReplaceObjectName { get; init; }
  public Guid CorrelationId { get; set; }
}
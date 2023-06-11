using Shared.Contracts.Common;

namespace Shared.Contracts.Commands.V1;

public record CreateObjectCommand : ICommand
{
  public required string ObjectName { get; init; }
  public Guid CorrelationId { get; set; }
}
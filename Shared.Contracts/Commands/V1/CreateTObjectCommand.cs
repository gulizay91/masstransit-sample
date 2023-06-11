using Shared.Contracts.Common;

namespace Shared.Contracts.Commands.V1;

public class CreateTObjectCommand : ICommand
{
  public Guid CorrelationId { get; set; }
  public string ObjectName { get; init; }
}
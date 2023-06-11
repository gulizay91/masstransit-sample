namespace Shared.Contracts.Commands.V1;

public record ScheduleUpdateObjectCommand : UpdateObjectCommand
{
  public DateTime DeliveryTime { get; init; }
}
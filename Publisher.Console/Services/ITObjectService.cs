using Shared.Contracts.Commands.V1;
using Shared.Contracts.Events.V1;

namespace Publisher.Console.Services;

public interface ITObjectService
{
  Task PublishTObjectCreatedEvent(TObjectCreatedEvent messageEvent);
  Task SendCreateTObjectCommand(CreateTObjectCommand messageCommand);
}
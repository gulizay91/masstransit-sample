using Shared.Contracts.Commands.V1;
using Shared.Contracts.Events.V1;

namespace Publisher.RabbitMq.Services;

public interface IObjectService
{
  Task PublishObjectCreatedEvent(ObjectCreatedEvent messageEvent);
  Task SendCreateObjectCommand(CreateObjectCommand messageCommand);

  Task PublishObjectUpdatedEvent(ObjectUpdatedEvent messageEvent);
  Task SendUpdateObjectCommand(UpdateObjectCommand messageCommand);
}
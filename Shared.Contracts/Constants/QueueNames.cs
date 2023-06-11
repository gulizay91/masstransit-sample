namespace Shared.Contracts.Constants;

public static class QueueNames
{
  public const string DeafultSchedulerQueueName = "scheduler";
  public const string SchedulerObjectConsumerQueueName = "SchedulerObject";

  public const string CreateObjectCommandConsumerQueueName = "CreateObject";
  public const string ObjectCreatedEventConsumerQueueName = "ObjectCreated";

  public const string UpdateObjectCommandConsumerQueueName = "UpdateObject";
  public const string ObjectUpdatedEventConsumerQueueName = "ObjectUpdated";
}
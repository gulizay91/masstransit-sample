using Microsoft.Extensions.DependencyInjection;
using Publisher.RabbitMq.Services;

namespace Publisher.RabbitMq.Configurations;

public static class ServicesRegister
{
  public static void RegisterServices(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddScoped<IObjectService, ObjectService>();
  }
}
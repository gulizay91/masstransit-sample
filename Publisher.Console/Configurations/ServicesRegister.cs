using Microsoft.Extensions.DependencyInjection;
using Publisher.Console.Services;

namespace Publisher.Console.Configurations;

public static class ServicesRegister
{
  public static void RegisterServices(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddScoped<ITObjectService, TObjectService>();
  }
}
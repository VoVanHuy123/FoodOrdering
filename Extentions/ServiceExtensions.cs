using System.Reflection;

namespace FoodOrdering.Extentions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var servicesTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service") && t.IsClass);

            foreach (var implementation in servicesTypes)
            {
                var interfaceType = implementation
                    .GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{implementation.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementation);
                }
                else
                {
                    services.AddScoped(implementation);
                }
            }
        }
    }
}
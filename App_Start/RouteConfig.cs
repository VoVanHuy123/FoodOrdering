using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
namespace FoodOrdering.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}

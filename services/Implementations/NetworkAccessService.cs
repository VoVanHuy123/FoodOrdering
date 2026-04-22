using FoodOrdering.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

namespace FoodOrdering.Services.Implementations
{
    public class NetworkAccessService
    {
        private readonly IConfiguration _configuration;
        private readonly string[] _configFilePaths;

        public NetworkAccessService(IConfiguration configuration)
        {
            _configuration = configuration;
            _configFilePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json")
            };
        }

        public NetworkAccessSettings GetSettings()
        {
            return _configuration.GetSection("NetworkAccess").Get<NetworkAccessSettings>() ?? new NetworkAccessSettings();
        }

        public void UpdateSettings(NetworkAccessSettings settings)
        {
            foreach (var filePath in _configFilePaths)
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();

                    config["NetworkAccess"] = new Dictionary<string, string>
                    {
                        ["RestaurantIp"] = settings.RestaurantIp,
                        ["SubnetMask"] = settings.SubnetMask
                    };

                    var updatedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, updatedJson);
                }
            }
        }
    }
}

using System.Reflection;
using OpenTelemetry.Resources;

namespace Toess.WebApi;

public static class ConfigureServiceExtension
{
    public static ResourceBuilder ConfigureOtelService(this ResourceBuilder resourceBuilder)
    {
        return resourceBuilder.AddService(
                serviceNamespace: "TOESS",
                serviceName: AppDomain.CurrentDomain.FriendlyName,
                serviceVersion: Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(new Dictionary<string, object>
            {
                // Add any desired resource attributes here
                ["deployment.environment"] = "development"
            });
    }
}
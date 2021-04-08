using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StreamClient.Services;

namespace StreamClient.Data
{
    public static class GrpcExtensions
    {
        public static IServiceCollection AddGrpcClient(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_HOST");
            var port = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_PORT");
            var protocol = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_PROTOCOL");

            services.AddTransient(sp => GrpcChannel.ForAddress($"{protocol}://{host}:{port}"));
            services.AddTransient<ISymbolService, SymbolService>();

            return services;
        }
    }
}

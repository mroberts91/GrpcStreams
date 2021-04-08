using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streams.Stocks;
using System;
using System.Linq;

namespace StreamClient.Data
{
    public static class GrpcChannelExtensions
    {
        public static IServiceCollection AddGrpcClient(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_HOST");
            var port = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_PORT");
            var protocol = configuration.GetValue<string>("BACKEND_HTTPS_SERVICE_PROTOCOL");

            // Add a transient GRPC channel to the IOC
            services.AddTransient(sp => GrpcChannel.ForAddress($"{protocol}://{host}:{port}"));
            services.AddScoped<GrpcClientFactory>();

            return services;
        }
    }
}

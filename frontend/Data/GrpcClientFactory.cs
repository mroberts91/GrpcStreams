using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Streams.Stocks;
using System;
using System.Linq;

namespace StreamClient.Data
{
    public class GrpcClientFactory
    {
        private readonly GrpcChannel _channel;
        private readonly ILogger<GrpcClientFactory> _logger;

        public GrpcClientFactory(GrpcChannel channel, ILogger<GrpcClientFactory> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        public StockSymbols.StockSymbolsClient StockSymbolsClient() => new(_channel);
    }
}

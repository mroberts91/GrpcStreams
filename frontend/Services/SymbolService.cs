using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Streams.Stocks;
using System.Threading;
using Grpc.Core;

namespace StreamClient.Services
{
    public class SymbolService : StockSymbols.StockSymbolsClient, ISymbolService
    {
        public SymbolService(GrpcChannel channel) : base(channel) { }

        public IAsyncEnumerable<SymbolResponse> SymbolStream(SymbolRequest request, CancellationToken cancellationToken = default) =>
            base.SymbolStream(request, cancellationToken: cancellationToken).ResponseStream.ReadAllAsync(cancellationToken);

        public async Task<IEnumerable<string>> SymbolsAsync(CancellationToken cancellationToken = default)
        {
            var response = await base.SymbolsAsync(new(), cancellationToken: cancellationToken).ResponseAsync;
            return response?.SymbolList?.ToList() ?? new List<string>();
        }

        public async Task<SymbolResponse> SymbolAsync(SymbolRequest request, CancellationToken cancellationToken = default) =>
            await base.SymbolAsync(request, cancellationToken: cancellationToken).ResponseAsync;
    }
}

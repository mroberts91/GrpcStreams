using Streams.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StreamClient.Services
{
    public interface ISymbolService
    {
        Task<SymbolResponse> SymbolAsync(SymbolRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> SymbolsAsync(CancellationToken cancellationToken = default);
        IAsyncEnumerable<SymbolResponse> SymbolStream(SymbolRequest request, CancellationToken cancellationToken = default);
    }
}

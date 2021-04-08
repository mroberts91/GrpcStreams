#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Streams.Stocks;

namespace Streams
{
    public class SymbolsService : StockSymbols.StockSymbolsBase
    {
        private readonly ILogger<SymbolsService> _logger;
        private readonly Random _random = new();
        private int _streamRecordCount = 0;
        private readonly object _lockObject = new();
        private readonly List<StockSymbol> _symbols = new(24)
        {
            new("AC", 7.09),
            new("NOK", 4.37),
            new("F", 11.37),
            new("GE", 11.45),
            new("XLF", 30.88),
            new("GE", 53.50),
            new("BAC", 32.50),
            new("SLV", 24.48),
            new("VGAC", 17.65),
            new("NIO", 57.60),
            new("SPY", 386.19),
            new("CCIV", 30.22),
            new("TZA", 4.79),
            new("EE", 55.76),
            new("ITUB", 5.23),
            new("X", 16.50),
            new("TE", 25.22),
            new("PFE", 34.89),
            new("BB", 12.15),
            new("BBD", 4.79),
            new("CCL", 21.05),
            new("SPCE", 54.72),
            new("SNAP", 58.31),
            new("T", 28.89),
        };

        public SymbolsService(ILogger<SymbolsService> logger)
        {
            _logger = logger;
        }

        public override async Task SymbolStream(SymbolRequest request, IServerStreamWriter<SymbolResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _streamRecordCount++;
                await responseStream.WriteAsync(GenerateSymbolResponse(request.Symbol) 
                    ?? new() 
                       { 
                            CurrentPrice = 0.00,
                            Symbol = request.Symbol,
                            Updated = DateTime.UtcNow.ToTimestamp() 
                        });
                await Task.Delay(1500);
            }

            _streamRecordCount = 0;
        }

        public override Task<SymbolsResponse> Symbols(Empty request, ServerCallContext context) =>
            Task.Run(() => new SymbolsResponse { SymbolList = { _symbols.Select(s => s.Symbol).ToList() }});

        public override Task<SymbolResponse> Symbol(SymbolRequest request, ServerCallContext context) =>
            Task.Run(() =>
                GenerateSymbolResponse(request.Symbol)
                    ?? new()
                    {
                        CurrentPrice = 0.00,
                        Symbol = request.Symbol,
                        Updated = DateTime.UtcNow.ToTimestamp()
                    });

        private SymbolResponse? GenerateSymbolResponse(string symbol)
        {
            var stockSymbol = GetSymbolResponseFromDataStore(symbol);
            if (stockSymbol is null) return null;

            int multiplier = _random.Next(0, 2) == 1 ? 1 : -1;
            double percentChance = (_streamRecordCount % 10 == 0 ? 10 : _random.Next(1, 3)) / 100.0;
            double priceChange = percentChance * multiplier;
            double newPrice = (stockSymbol.Price * priceChange) + stockSymbol.Price;

            stockSymbol = stockSymbol with { Price = newPrice };
            UpdateStockSymbolInDataStore(stockSymbol);

            return new()
            {
                CurrentPrice = newPrice,
                Symbol = stockSymbol.Symbol,
                Updated = DateTime.UtcNow.ToTimestamp(),
            };
        }

        private StockSymbol? GetSymbolResponseFromDataStore(string symbol) =>
            _symbols.FirstOrDefault(s => s?.Symbol?.Equals(symbol, StringComparison.OrdinalIgnoreCase) ?? false);

        private void UpdateStockSymbolInDataStore(StockSymbol symbol)
        {
            var record = GetSymbolResponseFromDataStore(symbol.Symbol);
            if (record is null) return;

            lock (_lockObject)
            {
                _symbols.Remove(record);
                _symbols.Add(symbol);
            }
        }

        private record StockSymbol(string Symbol, double Price);
    }
}

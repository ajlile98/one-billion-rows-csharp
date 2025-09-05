
using Microsoft.Extensions.DependencyInjection;
using one_billion_rows_csharp.Extensions;
using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Parser;
using one_billion_rows_csharp.Strategy;

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton<IParser, BasicParsingStrategy>();
serviceCollection.AddSingleton<IParser, ParallelParsingStrategy>();
serviceCollection.AddSingleton<IAggregateParser, ParallelAggregateParsingStrategy>();

serviceCollection.AddSingleton<IAnalyzer, WeatherRecordAnalyzer>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<IParser>();
var aggregateParser = serviceProvider.GetRequiredService<IAggregateParser>();
string filename = "./1brc/measurements_1_000_000_000.txt";
// string filename = "./1brc/measurements_100_000_000.txt";


// Parsing
// var start = DateTime.Now;
// var records = await parser.Parse(filename);
// var end = DateTime.Now;
// Console.WriteLine($"Parsing Completed in {(end - start).TotalMilliseconds} ms\n");
// Console.WriteLine(records.Count());

// Analysis
// var analyzer = serviceProvider.GetRequiredService<IAnalyzer>();
// start = DateTime.Now;
// analyzer.Analyze(records);
// end = DateTime.Now;
// Console.WriteLine($"Analysis Completed in {(end - start).TotalMilliseconds} ms");

// Aggregate Parsing
var start = DateTime.Now;
var stats = await aggregateParser.AggregateParseAsync(filename);
var end = DateTime.Now;
Console.WriteLine($"Parsing Completed in {(end - start).TotalMilliseconds} ms\n");

foreach (var stat in stats.Take(5))
{
    Console.WriteLine($"{stat.Key}/{stat.Value}");
}


using Microsoft.Extensions.DependencyInjection;
using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Parser;
using one_billion_rows_csharp.Strategy;

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton<IParser, BasicParsingStrategy>();
serviceCollection.AddSingleton<IParser, ParallelParsingStrategy>();

serviceCollection.AddSingleton<IAnalyzer, WeatherRecordAnalyzer>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<IParser>();


// Parsing
string filename = "./1brc/measurements_100_000_000.txt";
// filename = "./1brc/measurements.txt";
var start = DateTime.Now;
var records = await parser.Parse(filename);
Console.WriteLine(records.Count());
var end = DateTime.Now;
Console.WriteLine($"Parsing Completed in {(end - start).TotalMilliseconds} ms\n");

// Analysis
var analyzer = serviceProvider.GetRequiredService<IAnalyzer>();
start = DateTime.Now;
analyzer.Analyze(records);
end = DateTime.Now;
Console.WriteLine($"Analysis Completed in {(end - start).TotalMilliseconds} ms");
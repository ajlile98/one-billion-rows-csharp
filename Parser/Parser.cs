using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Records;
using one_billion_rows_csharp.Strategy;

namespace one_billion_rows_csharp.Parser;

public class WeatherRecordParser(IParsingStrategy strategy) : IParser
{
    public IEnumerable<WeatherRecord> Parse(string filename)
    {
        Console.WriteLine(strategy);
        // string filename = "./1brc/measurements_100_000_000.txt";
        // filename = "./1brc/measurements.txt";
        return strategy.Parse(filename);
    }
}

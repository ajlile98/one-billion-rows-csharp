using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Parser;

public class WeatherRecordAnalyzer : IAnalyzer
{
    public void Analyze(IEnumerable<WeatherRecord> records) 
    {
        var query = records.GroupBy(x => x.City)
                            .Select(g => new
                            {
                                g.Key,
                                Min = Double.Round(g.Min(m => m.Temp), 1),
                                Max = Double.Round(g.Max(m => m.Temp), 1),
                                Count = g.Count(),
                                Average = Double.Round(g.Average(m => m.Temp), 1),
                            });

        // Iterate over each anonymous type.
        foreach (var result in query.Where(x => x.Count > 1).Take(5))
        {
            Console.WriteLine($"{result.Key}/{result.Min.ToString("0.0")}/{result.Average.ToString("0.0")}/{result.Max.ToString("0.0")}");
        }

        Console.WriteLine($"Number of Cities: {query.Count()}");
    }
}

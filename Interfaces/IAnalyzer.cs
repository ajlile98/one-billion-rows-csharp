
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Interfaces;

public interface IAnalyzer
{
    void Analyze(IEnumerable<WeatherRecord> records);
}

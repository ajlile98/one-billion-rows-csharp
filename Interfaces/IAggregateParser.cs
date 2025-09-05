using System;
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Interfaces;

public interface IAggregateParser
{
    public Task<Dictionary<string, WeatherStats>> AggregateParseAsync(string filename);
}

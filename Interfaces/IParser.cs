using System;
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Interfaces;

public interface IParser
{
    IEnumerable<WeatherRecord> Parse(string filename);
}

using System;
using System.Collections.Generic;
using WeatherRecordType = one_billion_rows_csharp.Records.WeatherRecord;

namespace one_billion_rows_csharp.Strategy;

public interface IParsingStrategy
{
    public IEnumerable<WeatherRecordType> Parse(string filename);
}

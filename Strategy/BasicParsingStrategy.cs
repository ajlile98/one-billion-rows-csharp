using one_billion_rows_csharp.Interfaces;
using WeatherRecordType = one_billion_rows_csharp.Records.WeatherRecord;

namespace one_billion_rows_csharp.Strategy;

public class BasicParsingStrategy : IParser
{
    public IEnumerable<WeatherRecordType> Parse(string filename)
    {
        var records = new List<WeatherRecordType> { };
        try
        {
            using StreamReader reader = new(filename);
            var line = reader.ReadLine();
            var index = 0;
            while (line != null)
            {
                var split_line = line.Split(";");
                records.Add(new WeatherRecordType(
                    City: split_line[0],
                    Temp: Convert.ToDouble(split_line[1])
                ));
                if (index++ % 100_000_000 == 0) Console.WriteLine(index);
                line = reader.ReadLine();
            }
            // Console.WriteLine(records.Count);
            // Console.WriteLine(records.GroupBy(x => x.City));
            return records;



        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
            return records;
        }


    }
}
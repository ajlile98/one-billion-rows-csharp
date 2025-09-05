using System;
using System.Buffers;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using one_billion_rows_csharp.Extensions;
using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Strategy;

public class ParallelAggregateParsingStrategy : IAggregateParser 
{
    public async Task<Dictionary<string, WeatherStats>> AggregateParseAsync(string filename)
    {
        // var records = new List<WeatherRecord> { };
        long chunkSizeBytes = 128 * 1024 * 1024;
        var stats = new Dictionary<string, WeatherStats>();

        // Split file into x chunks based on filesize
        var ChunkBytes = GetFileChunks(filename, chunkSizeBytes).ToList();

        // parallelize the data load
        using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, "WeatherRecordFile"))
        {
            var tasks = new List<Task<Dictionary<string, WeatherStats>>>();
            for (int i = 0; i < ChunkBytes.Count() - 1; i++)
            {
                var byteStart = ChunkBytes[i];
                var byteEnd = ChunkBytes[i + 1];
                var task = Task.Run(() => ParseChunkFromMemoryMap(mmf, byteStart, byteEnd));
                tasks.Add(task);
            }

            // await tasks to finish and extend total records
            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                // records.AddRange(result);
                foreach (var city in result.Keys)
                {
                    if (!stats.ContainsKey(city))
                    {
                        stats.Add(city, new WeatherStats());
                    }
                    stats[city].Merge(result[city]);
                }
            }
        }

        // return records;
        return stats;
    }
    private IEnumerable<long> GetFileChunks(string filename, long chunkSize)
    {
        Console.WriteLine("GetFileChunks()");
        var chunkBytesStart = new List<long> { };

        FileInfo fileInfo = new FileInfo(filename);
        long fileSizeInBytes = fileInfo.Length;
        Console.WriteLine(fileSizeInBytes);
        int chunks = (int)(fileSizeInBytes / chunkSize);

        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            for (int i = 0; i < chunks; i++)
            {
                chunkBytesStart.Add(fs.Position);

                fs.Seek(chunkSize, SeekOrigin.Current);

                // Console.WriteLine($"{i}/{chunks} {(long)i * chunkSize}");
                int b = 0;
                do
                {
                    b = fs.ReadByte();
                    // Console.WriteLine($"b: {b}");
                } while (b != 10 && b != -1); // 10 is ASCII for '\n' (newline)
            }
            chunkBytesStart.Add(fileSizeInBytes);
        }

        // Console.WriteLine("Chunks Bytes Start:");
        // foreach (var i in chunkBytesStart)
        // {
        //     Console.WriteLine(i);
        // }
        return chunkBytesStart;

    }
    private Dictionary<string, WeatherStats> ParseChunkFromMemoryMap(MemoryMappedFile mmf, long byteStart, long byteEnd)
    {
        int estimatedCapacity = (int)((byteEnd - byteStart) / 27);
        // var records = new List<WeatherRecord>(estimatedCapacity);
        var chunkSize = (int)(byteEnd - byteStart);
        var stats = new Dictionary<string, WeatherStats>();

        var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);
        using (var accessor = mmf.CreateViewAccessor(byteStart, byteEnd - byteStart))
        {
            accessor.ReadArray(0, buffer, 0, chunkSize);
            int lineStart = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 10) // newline
                {
                    if (i > lineStart) // skip empty lines
                    {
                        var record = ParseLineFromBytes(buffer, lineStart, i);
                        if (record != null) {
                            // records.Add(record);
                            if (!stats.ContainsKey(record.City))
                            {
                                stats.Add(record.City, new WeatherStats());
                            }
                            stats[record.City].Update(record.Temp);
                            // Console.WriteLine($"{record.City}/{record.Temp} stats: {stats[record.City]}");
                        }
                    }
                    lineStart = i + 1;
                }
            }
            
            // Handle last line if no trailing newline
            if (lineStart < buffer.Length)
            {
                var record = ParseLineFromBytes(buffer, lineStart, buffer.Length);
                if (record != null)
                {
                    // records.Add(record);
                    if (!stats.ContainsKey(record.City))
                    {
                        stats.Add(record.City, new WeatherStats());
                    }
                    stats[record.City].Update(record.Temp);

                }
            }
        }
        // return records;
        return stats;
    }

    private WeatherRecord? ParseLineFromBytes(byte[] buffer, int start, int end)
    {
        // Find semicolon without string operations
        int semicolonIndex = -1;
        for (int i = start; i < end; i++)
        {
            if (buffer[i] == 59) // semicolon ASCII
            {
                semicolonIndex = i;
                break;
            }
        }
        
        if (semicolonIndex == -1) return null;
        
        // Extract city name
        var city = System.Text.Encoding.UTF8.GetString(buffer, start, semicolonIndex - start);

        // Parse temperature directly from bytes
        return new WeatherRecord(city, ParseTemperature(buffer, semicolonIndex + 1, end - semicolonIndex - 1));
    }

    private double ParseTemperature(byte[] buffer, int start, int length)
    {
        double result = 0;
        double sign = 1;
        bool foundDecimal = false;
        double decimalMultiplier = 0.1;

        for (int i = start; i < start + length; i++)
        {
            byte b = buffer[i];
            if (b == 45) // '-'
                sign = -1;
            else if (b == 46) // '.'
                foundDecimal = true;
            else if (b >= 48 && b <= 57) // '0'-'9'
            {
                if (foundDecimal)
                {
                    result += (b - 48) * decimalMultiplier;
                    decimalMultiplier *= 0.1;
                }
                else
                {
                    result = result * 10 + (b - 48);
                }
            }
        }

        // var str = System.Text.Encoding.UTF8.GetString(buffer, start, length);
        // Console.WriteLine($"'{str}' result: {result * sign}");
        return result * sign;
    }
}
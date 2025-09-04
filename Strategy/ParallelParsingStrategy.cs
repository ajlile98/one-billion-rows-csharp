using System;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using one_billion_rows_csharp.Extensions;
using one_billion_rows_csharp.Interfaces;
using one_billion_rows_csharp.Records;

namespace one_billion_rows_csharp.Strategy;

public class ParallelParsingStrategy : IParser
{
    public async Task<IEnumerable<WeatherRecord>> Parse(string filename)
    {
        var records = new List<WeatherRecord> { };

        // Split file into x chunks based on filesize
        var ChunkBytes = GetFileChunks(filename).ToList();

        // parallelize the data load
        using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, "WeatherRecordFile"))
        {
            var tasks = new List<Task<IEnumerable<WeatherRecord>>>();
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
                records.AddRange(result);
            }
        }

        // return
        return records;
    }
    private IEnumerable<long> GetFileChunks(string filename)
    {
        Console.WriteLine("GetFileChunks()");
        var chunkBytesStart = new List<long> { };
        int chunks = Environment.ProcessorCount * 2;
        FileInfo fileInfo = new FileInfo(filename);
        long fileSizeInBytes = fileInfo.Length;
        long chunkSize = fileSizeInBytes / chunks;
        Console.WriteLine(fileSizeInBytes);
        // long curr = 0;
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
    private IEnumerable<WeatherRecord> ParseChunkFromMemoryMap(MemoryMappedFile mmf, long byteStart, long byteEnd)
    {
        var records = new List<WeatherRecord>();
        var chunkSize = (int)(byteEnd - byteStart);
        // using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        using (var accessor = mmf.CreateViewAccessor(byteStart, byteEnd - byteStart))
        {
            // fs.Seek(byteStart, SeekOrigin.Begin);

            byte[] buffer = new byte[chunkSize];
            // await fs.ReadExactlyAsync(buffer, 0, buffer.Length);
            accessor.ReadArray(0, buffer, 0, chunkSize);
            string text = System.Text.Encoding.UTF8.GetString(buffer);
            // Console.WriteLine($"Text: {text}");
            foreach (var line in text.Split('\n'))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                records.Add(ParseWeatherRecord(line));
            }
        }
        return records;
    }
    private WeatherRecord ParseWeatherRecord(string line)
    {
        var split_line = line.Split(";");
        try
        {
            return new WeatherRecord(
                City: split_line[0],
                Temp: Convert.ToDouble(split_line[1])
            );
        }
        catch (System.Exception)
        {
            Console.WriteLine($"line: '{line}'");
            Console.WriteLine($"split_line: {split_line}");
            throw;
        }
    }
}
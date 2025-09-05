using System;

namespace one_billion_rows_csharp.Records;

public class WeatherStats
{
    public double Min { get; private set; }
    public double Max { get; private set; }
    public double Sum { get; private set; }
    public long Count { get; private set; }

    public void Update(double temp)
    {
        if (Count == 0)
        {
            Min = Max = Sum = temp;
            Count = 1;
        }
        else
        {
            if (temp < Min) Min = temp;
            if (temp > Max) Max = temp;
            Sum += temp;
            Count++;
        }
    }

    public void Merge(WeatherStats other)
    {
        if (Count == 0)
        {
            Min = other.Min;
            Max = other.Max;
            Sum = other.Max;
            Count = other.Count;
        }
        else
        {
            Min = Math.Min(Min, other.Min);
            Max = Math.Max(Max, other.Max);
            Sum += other.Sum;
            Count += other.Count;
        }
    }

    public double Average => Count > 0 ? Math.Round(Sum / Count, 2) : 0;

    public override string ToString()
    {
        return $"{Min}/{Average}/{Max}/{Count}";
    }
}

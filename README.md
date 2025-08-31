# One Billion Row Challenge - C#

A fun project implementing the [One Billion Row Challenge](https://github.com/gunnarmorling/1brc) using C#.

## About

The One Billion Row Challenge is a performance challenge where the goal is to process a text file containing one billion weather measurements as quickly as possible. Each row contains a weather station name and a temperature measurement in the format:

```
Station Name;Temperature
```

For example:
```
Hamburg;12.0
Bulawayo;8.9
Palembang;38.8
```

## Project Structure

```
├── Program.cs                          # Main entry point
├── WeatherRecord/
│   ├── Records/
│   │   └── WeatherRecord.cs           # Weather data record definition
│   ├── Interfaces/
│   │   ├── IAnalyzer.cs               # Interface for data analysis
│   │   └── IParsingStrategy.cs        # Interface for parsing strategies
│   ├── Parser.cs                      # Main parser implementation
│   ├── Analyzer.cs                    # Data analysis implementation
│   └── Strategy/
│       └── BasicParsingStrategy.cs    # Basic file parsing strategy
└── 1brc/                              # Test data and scripts
    ├── create_measurements_fast.sh     # Script to generate test data
    └── measurements_*.txt              # Generated measurement files
```

## Features

- **Modular Design**: Uses dependency injection with separate parsing strategies and analyzers
- **Configurable Parsing**: Different parsing strategies can be implemented and swapped
- **Performance Focused**: Designed to handle large datasets efficiently
- **Clean Architecture**: Separated concerns with interfaces and implementations

## Getting Started

### Prerequisites

- .NET 9.0 or later
- C# 12+ support

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run
```

The program will process the measurement file and output statistics including:
- Minimum, maximum, and average temperatures per weather station
- Total number of unique cities processed
- Processing time

## Performance

This is a for-fun exploration of different optimization techniques in C# including:
- Memory-efficient parsing strategies
- LINQ optimizations
- Parallel processing opportunities
- Custom data structures

## Contributing

This is a personal learning project, but feel free to fork and experiment with your own optimizations!

## Related

- [Original 1BRC Challenge](https://github.com/gunnarmorling/1brc)
- [Java implementations](https://github.com/gunnarmorling/1brc/tree/main/src/main/java/dev/morling/onebrc)

# Mockup - zero-reflection, compile-time mocking

C# Source Generators for efficient mocking

[![NuGet](https://img.shields.io/nuget/v/Mockup.svg)](https://www.nuget.org/packages/Mockup/)
[![NuGet](https://img.shields.io/nuget/v/Mockup.Analyzers.svg)](https://www.nuget.org/packages/Mockup.Analyzers/)

### Demo

Given an interface:


```csharp
public interface IObjectService
{   
    object ReadWriteProperty { get; set; }

    object SingleArgReturnMethod(object arg);
}
```

Mock it in your test code:

```csharp
using Mockup;
using Xunit;

[assembly: Mock(typeof(IObjectService))] // This will generate mock

namespace Mockup.Tests;

public class ObjectServiceMockTests
{
    [Fact]
    public void TestReadWriteProperty()
    {
        object value = "Value";
        
        var objectService = new ObjectServiceMock()
            .ReadWriteProperty(() => value, v => value = v)
            .Object; // This will produce IObjectService
        
        // Your test code...
    }

    [Fact]
    public void TestSingleArgReturnMethod()
    {
        var objectService = new ObjectServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Object; // This will produce IObjectService

        // Your test code...
    }
}
```

### Installation

Install Mockup via NuGet package: `Mockup`, `Mockup.Analyzers`

```xml
<PackageReference Include="Mockup" Version="0.5.0" />
<PackageReference Include="Mockup.Analyzers" Version="0.5.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

### Benchmarks

Return constant string from mocked method:

```
BenchmarkDotNet v0.13.12, macOS Ventura 13.6.6 (22G630) [Darwin 22.6.0]
Intel Core i5-7267U CPU 3.10GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.301 [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
```

| Method      | Mean        | Error      | StdDev     | Gen0   | Gen1   | Gen2   | Allocated |
|------------ |------------:|-----------:|-----------:|-------:|-------:|-------:|----------:|
| Mockup      |    34.60 ns |   0.599 ns |   0.531 ns | 0.0688 |      - |      - |     144 B |
| Moq         | 4,380.75 ns |  58.442 ns |  51.808 ns | 1.8616 |      - |      - |    3905 B |
| NSubstitute | 5,410.08 ns | 105.646 ns | 121.662 ns | 3.7384 |      - |      - |    7833 B |
| FakeItEasy  | 5,701.07 ns | 107.159 ns | 114.659 ns | 2.4109 | 0.0153 | 0.0076 |    5057 B |

Return argument string from mocked method:

```
BenchmarkDotNet v0.13.12, macOS Ventura 13.6.6 (22G630) [Darwin 22.6.0]
Intel Core i5-7267U CPU 3.10GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.301 [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
```

| Method      | Mean         | Error        | StdDev     | Gen0   | Gen1   | Gen2   | Allocated |
|------------ |-------------:|-------------:|-----------:|-------:|-------:|-------:|----------:|
| Mockup      |     16.39 ns |     0.392 ns |   0.347 ns | 0.0268 |      - |      - |      56 B |
| Moq         | 76,471.38 ns | 1,119.135 ns | 934.529 ns | 4.1504 | 1.2207 | 0.4883 |    9118 B |
| NSubstitute |  6,309.30 ns |   115.183 ns | 107.743 ns | 3.7537 |      - |      - |    7905 B |
| FakeItEasy  |  6,377.44 ns |   121.293 ns | 129.783 ns | 2.7771 | 0.0305 | 0.0305 |    5861 B |


### TODO

This is an early stage, experimental project. The following is not done:

* Mocking abstract classes
* Mocking generic types
* Custom names for mocks
* Arbitrary namespace for mocks

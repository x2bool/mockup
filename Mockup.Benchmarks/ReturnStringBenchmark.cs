using BenchmarkDotNet.Attributes;
using FakeItEasy;
using Moq;
using NSubstitute;

namespace Mockup.Benchmarks;

[MemoryDiagnoser]
public class ReturnStringBenchmark
{   
    [Benchmark]
    public string Mockup()
    {
        var value = "Value";
        
        var stringService = new StringServiceMock()
            .GetString(() => value)
            .Object;

        return stringService.GetString();
    }
    
    [Benchmark]
    public string Moq()
    {
        var value = "Value";

        var mock = new Mock<IStringService>();
        mock
            .Setup(s => s.GetString())
            .Returns(value);

        return mock.Object.GetString();
    }
    
    // [Benchmark]
    // public string Moq_Lambda()
    // {
    //     var value = "Value";
    //
    //     var mock = new Mock<IStringService>();
    //     mock
    //         .Setup(s => s.GetString())
    //         .Returns(() => value);
    //
    //     return mock.Object.GetString();
    // }

    [Benchmark]
    public string NSubstitute()
    {
        var value = "Value";

        var mock = Substitute.For<IStringService>();
        mock.GetString()
            .Returns(value);

        return mock.GetString();
    }

    // [Benchmark]
    // public string NSubstitute_Lambda()
    // {
    //     var value = "Value";
    //
    //     var mock = Substitute.For<IStringService>();
    //     mock.GetString().Returns(_ => value);
    //
    //     return mock.GetString();
    // }

    [Benchmark]
    public string FakeItEasy()
    {
        var value = "Value";

        var mock = A.Fake<IStringService>();
        A.CallTo(() => mock.GetString())
            .Returns(value);

        return mock.GetString();
    }
}

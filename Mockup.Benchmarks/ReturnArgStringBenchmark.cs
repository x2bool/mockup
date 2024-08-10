using BenchmarkDotNet.Attributes;
using FakeItEasy;
using Moq;
using NSubstitute;

namespace Mockup.Benchmarks;

[MemoryDiagnoser]
public class ReturnArgStringBenchmark
{
    [Benchmark]
    public string Mockup()
    {
        var value = "Value";
        
        var stringService = new StringServiceMock()
            .GetArgString(v => v)
            .Build();

        return stringService.GetArgString(value);
    }
    
    [Benchmark]
    public string Moq()
    {
        var value = "Value";

        var mock = new Mock<IStringService>();
        mock
            .Setup(s => s.GetArgString(It.IsAny<string>()))
            .Returns<string>(v => v);

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
        mock.GetArgString(Arg.Any<string>())
            .Returns(call => call.ArgAt<string>(0));
    
        return mock.GetArgString(value);
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
        A.CallTo(() => mock.GetArgString(A<string>._))
            .ReturnsLazily(call => call.Arguments.Get<string>(0));
    
        return mock.GetArgString(value);
    }
}

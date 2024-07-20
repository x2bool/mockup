using BenchmarkDotNet.Running;
using Mockup;
using Mockup.Benchmarks;

[assembly: Mock(typeof(IStringService))]

namespace Mockup.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}

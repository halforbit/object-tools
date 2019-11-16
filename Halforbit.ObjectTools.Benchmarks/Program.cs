using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace Halforbit.ObjectTools.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StringMapBenchmarks>(ManualConfig
                .CreateEmpty()
                .With(new ConsoleLogger())
                .With(DefaultColumnProviders.Job)
                .With(DefaultColumnProviders.Instance)
                .With(DefaultColumnProviders.Descriptor)
                .With(DefaultColumnProviders.Params)
                .With(DefaultColumnProviders.Metrics)
                .With(DefaultColumnProviders.Statistics)
                .With(Job.LongRun.With(CoreRuntime.Core22)));
                //.With(Job.ShortRun.With(ClrRuntime.CreateForLocalFullNetFrameworkBuild("net462")))
                //.With(Job.ShortRun.With(CoreRuntime.Core22))
                //.With(Job.ShortRun.With(CoreRuntime.Core30)));
        }
    }
}

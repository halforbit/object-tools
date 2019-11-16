using BenchmarkDotNet.Attributes;
using Halforbit.ObjectTools.ObjectStringMap.Implementation;
using System;
using System.Collections.Generic;

namespace Halforbit.ObjectTools.Benchmarks
{
    [MemoryDiagnoser]
    public class StringMapBenchmarks
    {
        [Benchmark]
        public void MapDictionary_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            var memberValues = new Dictionary<string, object>(1)
            {
                ["this"] = value
            };

            _ = map.Map(memberValues);
        }

        [Benchmark]
        public void MapDictionary_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var memberValues = new Dictionary<string, object>(1)
            {
                ["this"] = value
            };

            _ = map.Map(memberValues);
        }

        [Benchmark]
        public void MapDictionary_ImmutableDateTime_SpeedTest()
        {
            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            var memberValues = new Dictionary<string, object>(1)
            {
                ["Property"] = new DateTime(
                    2016, 7, 11,
                    2, 3, 4,
                    DateTimeKind.Utc)
            };

            _ = map.Map(memberValues);
        }

        [Benchmark]
        public void MapObject_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            _ = map.Map(value);
        }

        [Benchmark]
        public void MapObject_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");
            
            _ = map.Map(value);
        }

        [Benchmark]
        public void MapObject_ImmutableDateTime_SpeedTest()
        {
            var value = new Immutable<DateTime>(new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc));

            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            _ = map.Map(value);
        }

        [Benchmark]
        public void MapString_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            var str = $"alfa/{value:yyyy/MM/dd/hh/mm/ss}/bravo";
                
            _ = map.Map(str);
        }

        [Benchmark]
        public void MapString_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var str = $"alfa/{value:N}/bravo";

            _ = map.Map(str);
        }

        [Benchmark]
        public void MapString_ImmutableDateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            var str = $"alfa/{value:yyyy/MM/dd/hh/mm/ss}/bravo";

            _ =map.Map(str);
        }

        class Immutable<TType>
        {
            public Immutable(TType property)
            {
                Property = property;
            }

            public TType Property { get; }
        }
    }
}

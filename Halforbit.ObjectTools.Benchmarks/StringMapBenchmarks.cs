using BenchmarkDotNet.Attributes;
using Halforbit.ObjectTools.ObjectStringMap.Implementation;
using System;
using System.Collections.Generic;

namespace Halforbit.ObjectTools.Benchmarks
{
    [MemoryDiagnoser]
    public class StringMapBenchmarks
    {
        static readonly IReadOnlyDictionary<string, object> _dateTimeDictionary = new Dictionary<string, object>(1)
        {
            ["this"] = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc)
        };

        static readonly StringMap<DateTime> _dateTimeDictionaryMap = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapDictionary_DateTime()
        {
            _ = _dateTimeDictionaryMap.Map(_dateTimeDictionary);
        }

        static IReadOnlyDictionary<string, object> _guidDictionary = new Dictionary<string, object>(1)
        {
            ["this"] = Guid.NewGuid()
        };

        static readonly StringMap<Guid> _guidDictionaryMap = new StringMap<Guid>("alfa/{this}/bravo");

        [Benchmark]
        public void MapDictionary_Guid()
        {
            _ = _guidDictionaryMap.Map(_guidDictionary);
        }

        static readonly IReadOnlyDictionary<string, object> _immutableDateTimeDictionary = new Dictionary<string, object>(1)
        {
            ["Property"] = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc)
        };

        static readonly StringMap<Immutable<DateTime>> _immutableDateTimeDictionaryMap = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapDictionary_ImmutableDateTime()
        {
            _ = _immutableDateTimeDictionaryMap.Map(_immutableDateTimeDictionary);
        }

        static readonly DateTime _dateTime = new DateTime(
            2016, 7, 11,
            2, 3, 4,
            DateTimeKind.Utc);

        static readonly StringMap<DateTime> _dateTimeMap = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapObject_DateTime()
        {
            _ = _dateTimeMap.Map(_dateTime);
        }

        static readonly Guid _guid = Guid.NewGuid();

        static readonly StringMap<Guid> _guidMap = new StringMap<Guid>("alfa/{this}/bravo");

        [Benchmark]
        public void MapObject_Guid()
        {
            _ = _guidMap.Map(_guid);
        }

        static readonly Immutable<DateTime> _immutableDateTime = new Immutable<DateTime>(new DateTime(
            2016, 7, 11,
            2, 3, 4,
            DateTimeKind.Utc));

        static readonly StringMap<Immutable<DateTime>> _immutableDateTimeMap = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapObject_ImmutableDateTime()
        {
            _ = _immutableDateTimeMap.Map(_immutableDateTime);
        }

        static readonly string _dateTimeString = $"alfa/{_dateTime:yyyy/MM/dd/hh/mm/ss}/bravo";
                
        static readonly StringMap<DateTime> _dateTimeStringMap = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapString_DateTime()
        {
            _ = _dateTimeStringMap.Map(_dateTimeString);
        }

        static readonly string _guidString = $"alfa/{_guid:N}/bravo";
        
        static readonly StringMap<Guid> _guidStringMap = new StringMap<Guid>("alfa/{this}/bravo");
            
        [Benchmark]
        public void MapString_Guid()
        {
            _ = _guidStringMap.Map(_guidString);
        }

        static readonly string _immutableDateString = $"alfa/{_dateTime:yyyy/MM/dd/hh/mm/ss}/bravo";

        static readonly StringMap<Immutable<DateTime>> _immutableDateTimeStringMap = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

        [Benchmark]
        public void MapString_ImmutableDateTime()
        {
            _ = _immutableDateTimeStringMap.Map(_immutableDateString);
        }

        static readonly ImplicitConvertible _implicitConvertible = new ImplicitConvertible("bravo", "charlie");

        static readonly string _implicitConvertibleString = $"alfa/bravo|charlie/delta";

        static readonly StringMap<ImplicitConvertible> _implicitConvertibleMap = "alfa/{this}/delta";

        [Benchmark]
        public void MapObject_ImplicitConvertible()
        {
            _ = _implicitConvertibleMap.Map(_implicitConvertible);
        }

        [Benchmark]
        public void MapString_ImplicitConvertible()
        {
            _ = _implicitConvertibleMap.Map(_implicitConvertibleString);
        }

        class ImplicitConvertible
        {
            public ImplicitConvertible(
                string alfa,
                string bravo)
            {
                Alfa = alfa;

                Bravo = bravo;
            }

            public string Alfa { get; }

            public string Bravo { get; }

            public static implicit operator string(ImplicitConvertible source) => $"{source.Alfa}|{source.Bravo}";

            public static implicit operator ImplicitConvertible(string source)
            {
                var parts = source.Split('|');
            
                return new ImplicitConvertible(parts[0], parts[1]);
            }
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

using Halforbit.ObjectTools.ObjectStringMap.Implementation;
using System;
using System.Diagnostics;
using Xunit;

namespace Halforbit.ObjectTools.Tests.ObjectStringMap
{
    public class StringMapTests
    {
        const string TestString = "quick-brown-fox";

        const string DateFormat = "yyyy/MM/dd";

        readonly DateTime TestDate = new DateTime(
            2016, 7, 9, 
            0, 0, 0, 
            DateTimeKind.Utc);

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenString_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenString_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenString_WildcardTail_ThenSuccess()
        {
            var map = new StringMap<string>("alfa/{*this}");

            var str = map.Map("alfa/charlie/delta/bravo.jpg");

            Assert.Equal(
                "charlie/delta/bravo.jpg",
                str);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WildcardTail_ThenSuccess()
        {
            var key = "charlie/delta/bravo.jpg";

            var map = new StringMap<string>("alfa/{*this}");

            var str = map.Map(key, allowPartialMap: false);

            Assert.Equal(
                $"alfa/{key}",
                str);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenDateTime_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestDate, 
                TestDate.ToString(DateFormat), 
                DateFormat);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenDateTime_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                TestDate.ToString(DateFormat), 
                TestDate,
                DateFormat);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenInt_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                int.MaxValue, 
                int.MaxValue.ToString());
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenInt_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                int.MaxValue.ToString(),
                int.MaxValue);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenNullableInt_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                (int?)int.MaxValue,
                int.MaxValue.ToString());
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenNullableInt_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                int.MaxValue.ToString(),
                (int?)int.MaxValue);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenLong_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                long.MaxValue, 
                long.MaxValue.ToString());
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenLong_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                long.MaxValue.ToString(), 
                long.MaxValue);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenSimpleType_ThenSuccess(guid, guid.ToString("N"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenSimpleType_ThenSuccess(guid.ToString("N"), guid);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenSimpleType_ThenSuccess(guid, guid.ToString("D"), "D");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenSimpleType_ThenSuccess(guid.ToString("D"), guid);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenWrappedType_ThenSuccess(guid, guid.ToString("N"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenWrappedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenWrappedType_ThenSuccess(guid.ToString("N"), guid);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenWrappedType_ThenSuccess(guid, guid.ToString("D"), "D");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenWrappedDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenWrappedType_ThenSuccess(guid.ToString("D"), guid, "D");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenImmutableType_ThenSuccess(guid, guid.ToString("N"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenImmutableType_ThenSuccess(guid.ToString("N"), guid);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenInvalidValue_ThenDefault()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/not-a-guid/bravo";

            var obj = map.Map(str);

            Assert.Null(obj);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenPatternNotMatched_ThenDefault()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/ff14cea6a92c4a82bd8478c3d17220d2/charlie";

            var obj = map.Map(str);

            Assert.Null(obj);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenSlashInPattern_ThenProperMatch()
        {
            var map = new StringMap<ExchangeCurrencyPairTimeKey>(
                "order-books/{Exchange}/{CurrencyPair}/{Time:yyyy-MM-dd/HH-mm-ss}");

            var str = "order-books/bitstamp/btc-usd/2017-04-04/17-37-00";

            var obj = map.Map(str);

            Assert.Equal("bitstamp", obj.Exchange);

            Assert.Equal("btc-usd", obj.CurrencyPair);

            Assert.Equal(
                new DateTime(2017, 4, 4, 17, 37, 0, DateTimeKind.Utc), 
                obj.Time);
        }

        [Fact, Trait("Type", "Unit")]
        public void IsMatch_WhenIsMatch_ThenTrue()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/ff14cea6a92c4a82bd8478c3d17220d2/bravo";

            Assert.True(map.IsMatch(str));
        }

        [Fact, Trait("Type", "Unit")]
        public void IsMatch_WhenIsNotMatch_ThenFalse()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/ff14cea6a92c4a82bd8478c3d17220d2/charlie";

            Assert.False(map.IsMatch(str));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_NonFullMatch_ThenNull()
        {
            var map = new StringMap<ExchangeCurrencyPairTimeKey>("apples/{Exchange}/oranges");

            var result = map.Map("/apples/bitstamp/oranges");

            Assert.Null(result);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_ValueTuple_ThenSuccess()
        {
            var map = new StringMap<(double X, double Y)>("apples/{Item1}/{Item2}/oranges");

            var (x, y) = map.Map("apples/3.14/123/oranges");

            Assert.Equal(3.14, x);

            Assert.Equal(123, y);
        }

        [Fact, Trait("Type", "Speed")]
        public void MapObject_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            while(time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(value);

                count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapString_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var str = $"alfa/{value:N}/bravo";

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var obj = map.Map(str);

                count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapObject_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11, 
                2, 3, 4, 
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(value);

                count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapString_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            var str = $"alfa/{value:yyyy/MM/dd/hh/mm/ss}/bravo";

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var obj = map.Map(str);

                count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapObject_ImmutableDateTime_SpeedTest()
        {
            var value = new Immutable<DateTime>(new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc));

            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(value);

                count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapString_ImmutableDateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            var str = $"alfa/{value:yyyy/MM/dd/hh/mm/ss}/bravo";

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var obj = map.Map(str);

               count++;
            }

            Console.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            Console.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        static void MapObject_WhenSimpleType_ThenSuccess<TSimpleType>(
            TSimpleType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<TSimpleType>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{this}/bravo" :
                $"alfa/{{this:{format}}}/bravo");

            var str = map.Map(value);

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenSimpleType_ThenSuccess<TSimpleType>(
            string stringValue,
            TSimpleType value,
            string format = null)
        {
            var map = new StringMap<TSimpleType>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{this}/bravo" :
                $"alfa/{{this:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.Equal(
                value,
                obj);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapObject_WhenWrappedType_ThenSuccess<TType>(
            TType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Wrapper<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var obj = new Wrapper<TType>
            {
                Property = value
            };

            var str = map.Map(obj);

            Console.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenWrappedType_ThenSuccess<TType>(
            string stringValue,
            TType value,
            string format = null)
        {
            var map = new StringMap<Wrapper<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.Equal(
                value,
                obj.Property);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapObject_WhenImmutableType_ThenSuccess<TType>(
            TType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var obj = new Immutable<TType>(value);

            var str = map.Map(obj);

            Console.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenImmutableType_ThenSuccess<TType>(
            string stringValue,
            TType value,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.Equal(
                value,
                obj.Property);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        class Wrapper<TType> 
        {
            public TType Property { get; set; }
        }

        class Immutable<TType>
        {
            public Immutable(TType property)
            {
                Property = property;
            }

            public TType Property { get; }
        }

        public class ExchangeCurrencyPairTimeKey
        {
            public ExchangeCurrencyPairTimeKey(
                string exchange = default(string),
                string currencyPair = default(string),
                DateTime? time = default(DateTime?))
            {
                Exchange = exchange;

                CurrencyPair = currencyPair;

                Time = time;
            }

            public string Exchange { get; }

            public string CurrencyPair { get; }

            public DateTime? Time { get; }
        }
    }
}

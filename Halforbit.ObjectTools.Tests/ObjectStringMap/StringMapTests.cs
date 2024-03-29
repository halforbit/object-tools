﻿using Halforbit.ObjectTools.ObjectStringMap.Implementation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

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

        readonly ITestOutputHelper _testOutputHelper;

        public StringMapTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenString_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenEnum_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestEnumEcho.CharlieDelta,
                "charlie-delta");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenEnum_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                "charlie-delta",
                TestEnumEcho.CharlieDelta);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenEnum_PascalCase_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestEnumEcho.CharlieDelta,
                "CharlieDelta",
                "p");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenEnum_PascalCase_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                "CharlieDelta",
                TestEnumEcho.CharlieDelta,
                "p");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenEnum_CamelCase_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestEnumEcho.CharlieDelta,
                "charlieDelta",
                "c");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenEnum_CamelCase_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                "charlieDelta",
                TestEnumEcho.CharlieDelta,
                "c");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenEnum_Integer_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestEnumEcho.CharlieDelta,
                "2",
                "i");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenEnum_Integer_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                "2",
                TestEnumEcho.CharlieDelta,
                "i");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapDictionary_WhenString_ThenSuccess()
        {
            MapDictionary_WhenSimpleType_ThenSuccess<string>(
                new Dictionary<string, object>
                {
                    ["this"] = TestString
                },
                TestString);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenString_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        // TODO: Implement this

        //[Fact, Trait("Type", "Unit")]
        //public void MapString_WhenTuple_ThenSuccess()
        //{
        //    var key = (PartitionKey: "123", RecordId: "456");

        //    var map = new StringMap<(string PartitionKey, string RecordId)>("{PartitionKey}|things/{RecordId}");

        //    var str = map.Map(key);

        //    Assert.Equal(
        //        $"{key.PartitionKey}|things/{key.RecordId}",
        //        str);
        //}

        //[Fact, Trait("Type", "Unit")]
        //public void MapObject_WhenTuple_ThenSuccess()
        //{
        //    var key = (PartitionKey: "123", RecordId: "456");

        //    var str = $"{key.PartitionKey}|things/{key.RecordId}";

        //    var map = new StringMap<(string PartitionKey, string RecordId)>("{PartitionKey}|things/{RecordId}");

        //    var r = map.Map(str);

        //    Assert.Equal(key.PartitionKey, r.PartitionKey);

        //    Assert.Equal(key.RecordId, r.RecordId);
        //}

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenAnonymousTuple_ThenSuccess()
        {
            var key = ("123", "456");

            var map = new StringMap<(string, string)>("{Item1}|things/{Item2}");

            var str = map.Map(key);

            Assert.Equal(
                $"{key.Item1}|things/{key.Item2}",
                str);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenAnonymousTuple_ThenSuccess()
        {
            var key = ("123", "456");

            var str = $"{key.Item1}|things/{key.Item2}";

            var map = new StringMap<(string, string)>("{Item1}|things/{Item2}");

            var r = map.Map(str);

            Assert.Equal(key.Item1, r.Item1);

            Assert.Equal(key.Item2, r.Item2);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapDictionary_WhenString_WildcardTail_ThenSuccess()
        {
            var key = "charlie/delta/bravo.jpg";

            var map = new StringMap<string>("alfa/{*this}");

            var str = map.Map(new Dictionary<string, object>
            {
                ["this"] = key
            });

            Assert.Equal(
                $"alfa/{key}",
                str);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WildcardTail_ThenSuccess()
        {
            var key = "charlie/delta/bravo.jpg";

            var map = new StringMap<string>("alfa/{*this}");

            var str = map.Map(key, allowPartialMap: false);

            Assert.Equal(
                $"alfa/{key}",
                str);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenString_WildcardTail_ThenSuccess()
        {
            var map = new StringMap<string>("alfa/{*this}");

            var str = map.Map("alfa/charlie/delta/bravo.jpg");

            Assert.Equal(
                "charlie/delta/bravo.jpg",
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
        public void MapDictionary_WhenDateTime_ThenSuccess()
        {
            MapDictionary_WhenSimpleType_ThenSuccess<DateTime>(
                new Dictionary<string, object>
                {
                    ["this"] = TestDate
                },
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
        public void MapDictionary_WhenInt_ThenSuccess()
        {
            MapDictionary_WhenSimpleType_ThenSuccess<int>(
                new Dictionary<string, object>
                {
                    ["this"] = int.MaxValue
                },
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
        public void MapDictionary_WhenNullableInt_ThenSuccess()
        {
            MapDictionary_WhenSimpleType_ThenSuccess<int?>(
                new Dictionary<string, object>
                {
                    ["this"] = (int?)int.MaxValue
                },
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
        public void MapDictionary_WhenLong_ThenSuccess()
        {
            MapDictionary_WhenSimpleType_ThenSuccess<long>(
                new Dictionary<string, object>
                {
                    ["this"] = long.MaxValue
                },
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
        public void MapDictionary_WhenGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapDictionary_WhenSimpleType_ThenSuccess<Guid>(
                new Dictionary<string, object>
                {
                    ["this"] = guid
                },
                guid.ToString("N"));
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
        public void MapDictionary_WhenDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapDictionary_WhenSimpleType_ThenSuccess<Guid>(
                new Dictionary<string, object>
                {
                    ["this"] = guid
                },
                guid.ToString("D"), "D");
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
        public void MapDictionary_WhenWrappedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapDictionary_WhenWrappedType_ThenSuccess<Guid>(
                new Dictionary<string, object>
                {
                    ["Property"] = guid
                },
                guid.ToString("N"));
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
        public void MapString_WhenWrappedEnum_ThenSuccess()
        {
            MapString_WhenWrappedType_ThenSuccess("charlie-delta", TestEnumEcho.CharlieDelta);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedEnum_ThenSuccess()
        {
            MapObject_WhenWrappedType_ThenSuccess(TestEnumEcho.CharlieDelta, "charlie-delta");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenWrappedEnum_PascalCase_ThenSuccess()
        {
            MapString_WhenWrappedType_ThenSuccess("CharlieDelta", TestEnumEcho.CharlieDelta, "p");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedEnum_PascalCase_ThenSuccess()
        {
            MapObject_WhenWrappedType_ThenSuccess(TestEnumEcho.CharlieDelta, "CharlieDelta", "p");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenWrappedEnum_CamelCase_ThenSuccess()
        {
            MapString_WhenWrappedType_ThenSuccess("charlieDelta", TestEnumEcho.CharlieDelta, "c");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedEnum_CamelCase_ThenSuccess()
        {
            MapObject_WhenWrappedType_ThenSuccess(TestEnumEcho.CharlieDelta, "charlieDelta", "c");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenWrappedEnum_Integer_ThenSuccess()
        {
            MapString_WhenWrappedType_ThenSuccess("2", TestEnumEcho.CharlieDelta, "i");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenWrappedEnum_Integer_ThenSuccess()
        {
            MapObject_WhenWrappedType_ThenSuccess(TestEnumEcho.CharlieDelta, "2", "i");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapDictionary_WhenWrappedDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapDictionary_WhenWrappedType_ThenSuccess<Guid>(
                new Dictionary<string, object>
                {
                    ["Property"] = guid
                },
                guid.ToString("D"), "D");
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
        public void MapDictionary_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapDictionary_WhenImmutableType_ThenSuccess<Guid>(
                new Dictionary<string, object>
                {
                    ["Property"] = guid
                },
                guid.ToString("N"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenImmutableType_ThenSuccess(guid.ToString("N"), guid);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenInvalidValue_ThenDefault()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/not-a-guid/bravo";

            var obj = map.Map(str);

            Assert.Null(obj);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenPatternNotMatched_ThenDefault()
        {
            var map = new StringMap<Wrapper<Guid>>("alfa/{Property}/bravo");

            var str = "alfa/ff14cea6a92c4a82bd8478c3d17220d2/charlie";

            var obj = map.Map(str);

            Assert.Null(obj);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_WhenSlashInPattern_ThenProperMatch()
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
        public void MapString_NonFullMatch_ThenNull()
        {
            var map = new StringMap<ExchangeCurrencyPairTimeKey>("apples/{Exchange}/oranges");

            var result = map.Map("/apples/bitstamp/oranges");

            Assert.Null(result);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_ValueTuple_ThenSuccess()
        {
            var map = new StringMap<(double X, double Y)>("apples/{Item1}/{Item2}/oranges");

            var (x, y) = map.Map("apples/3.14/123/oranges");

            Assert.Equal(3.14, x);

            Assert.Equal(123, y);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_ValueTuple_ThenSuccess()
        {
            var map = new StringMap<(double X, double Y)>("/contoso-app/add-numbers/{Item1}/{Item2}");

            var s = map.Map((2, 3));

            Assert.Equal("/contoso-app/add-numbers/2/3", s);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapDictionary_ValueTuple_ThenSuccess()
        {
            var map = new StringMap<(double X, double Y)>("/contoso-app/add-numbers/{Item1}/{Item2}");

            var s = map.Map(new Dictionary<string, object>
            {
                ["Item1"] = 2,

                ["Item2"] = 3
            });

            Assert.Equal("/contoso-app/add-numbers/2/3", s);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_WhenNullableProperty_ThenSuccess()
        {
            var k = new ExchangeProductDateTimeKey(
                "coinbase-pro",
                "btc-usd",
                new DateTime(2019, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                null);

            var actual = new StringMap<ExchangeProductDateTimeKey>("order-book-snapshots/{ExchangeId}/{ProductId}/{Date:yyyy-MM-dd}/{Time:HH-mm-ss}").Map(k, true);

            Assert.Equal("order-book-snapshots/coinbase-pro/btc-usd/2019-09-30/", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_ImplicitOperatorConvertible_Success()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                new ImplicitConvertible("charlie", "delta"),
                "charlie|delta");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_ImplicitOperatorConvertible_Success()
        {
            MapString_WhenObjectType_ThenSuccess(
                "charlie|delta",
                new ImplicitConvertible("charlie", "delta"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_ExplicitOperatorConvertible_Success()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                new ExplicitConvertible("charlie", "delta"),
                "charlie|delta");
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_ExplicitOperatorConvertible_Success()
        {
            MapString_WhenObjectType_ThenSuccess(
                "charlie|delta",
                new ExplicitConvertible("charlie", "delta"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapObject_NullDateTime_Success()
        {
            StringMap<DateTime?> map = "snapshot-times/{this:yyyy/MM/dd/HH/mm/ss}";

            var actual = map.Map(null as DateTime?, true);

            Assert.Equal("snapshot-times/", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_NullDateTime_Success()
        {
            StringMap<DateTime?> map = "snapshot-times/{this:yyyy/MM/dd/HH/mm/ss}";

            var actual = map.Map("snapshot-times/");

            Assert.Equal(null, actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_InvalidProperty_UsefulError()
        {
            var map = new StringMap<ExchangeCurrencyPairTimeKey>("alfa/{DoesntExist}/bravo");

            Assert.Throws<ArgumentException>(() => map.Map("alfa/charlie/bravo"));
        }

        [Fact, Trait("Type", "Unit")]
        public void MapString_InvalidBraces_UsefulError()
        {
            Assert.Throws<ArgumentException>(() => 
                new StringMap<ExchangeCurrencyPairTimeKey>("alfa/Exchange}/bravo").Map("alfa/charlie/bravo"));

            Assert.Throws<ArgumentException>(() =>
                new StringMap<ExchangeCurrencyPairTimeKey>("alfa/{{Exchange}/bravo").Map("alfa/charlie/bravo"));

            Assert.Throws<ArgumentException>(() =>
                new StringMap<ExchangeCurrencyPairTimeKey>("alfa/{Exchange/bravo").Map("alfa/charlie/bravo"));

            Assert.Throws<ArgumentException>(() =>
                new StringMap<ExchangeCurrencyPairTimeKey>("alfa/{Exchange}}/bravo").Map("alfa/charlie/bravo"));
        }

        [Fact, Trait("Type", "Speed")]
        public void MapObject_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(value);

                count++;
            }

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapDictionary_Guid_SpeedTest()
        {
            var value = Guid.NewGuid();

            var map = new StringMap<Guid>("alfa/{this}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            var memberValues = new Dictionary<string, object>
            {
                ["this"] = value
            };

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(memberValues);

                count++;
            }

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
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

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
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

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapDictionary_DateTime_SpeedTest()
        {
            var value = new DateTime(
                2016, 7, 11,
                2, 3, 4,
                DateTimeKind.Utc);

            var map = new StringMap<DateTime>("alfa/{this:yyyy/MM/dd/hh/mm/ss}/bravo");

            var time = Stopwatch.StartNew();

            var count = 0;

            var memberValues = new Dictionary<string, object>
            {
                ["this"] = value
            };

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(memberValues);

                count++;
            }

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
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

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
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

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        [Fact, Trait("Type", "Speed")]
        public void MapDictionary_ImmutableDateTime_SpeedTest()
        {
            var map = new StringMap<Immutable<DateTime>>("alfa/{Property:yyyy/MM/dd/hh/mm/ss}/bravo");

            var memberValues = new Dictionary<string, object>
            {
                ["Property"] = new DateTime(
                    2016, 7, 11,
                    2, 3, 4,
                    DateTimeKind.Utc)
            };

            var count = 0;

            var time = Stopwatch.StartNew();

            while (time.Elapsed.TotalSeconds < 5)
            {
                var str = map.Map(memberValues);

                count++;
            }

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
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

            _testOutputHelper.WriteLine($"Rate: {count / time.Elapsed.TotalSeconds} / s");

            _testOutputHelper.WriteLine($"Avg: {time.Elapsed.TotalMilliseconds / count} ms");
        }

        void MapObject_WhenSimpleType_ThenSuccess<TSimpleType>(
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

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapDictionary_WhenSimpleType_ThenSuccess<TSimpleType>(
            IReadOnlyDictionary<string, object> dictionary,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<TSimpleType>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{this}/bravo" :
                $"alfa/{{this:{format}}}/bravo");

            var str = map.Map(dictionary);

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapString_WhenSimpleType_ThenSuccess<TSimpleType>(
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

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapString_WhenObjectType_ThenSuccess<TSimpleType>(
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
                JsonConvert.SerializeObject(value),
                JsonConvert.SerializeObject(obj));

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapObject_WhenWrappedType_ThenSuccess<TType>(
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

            _testOutputHelper.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapDictionary_WhenWrappedType_ThenSuccess<TType>(
            IReadOnlyDictionary<string, object> memberValues,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Wrapper<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = map.Map(memberValues);

            _testOutputHelper.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapString_WhenWrappedType_ThenSuccess<TType>(
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

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapObject_WhenImmutableType_ThenSuccess<TType>(
            TType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var obj = new Immutable<TType>(value);

            var str = map.Map(obj);

            _testOutputHelper.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapDictionary_WhenImmutableType_ThenSuccess<TType>(
            IReadOnlyDictionary<string, object> memberValues,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = map.Map(memberValues);

            _testOutputHelper.WriteLine($"str is {str}");

            Assert.Equal(
                $"alfa/{stringValue}/bravo",
                str);

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
        }

        void MapString_WhenImmutableType_ThenSuccess<TType>(
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

            _testOutputHelper.WriteLine($"map: {map}");

            _testOutputHelper.WriteLine($"str: {str}");
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

        public class ExchangeProductDateTimeKey
        {
            public ExchangeProductDateTimeKey(
                string exchangeId,
                string productId,
                DateTime date,
                DateTime? time)
            {
                ExchangeId = exchangeId;

                ProductId = productId;

                Date = date;

                Time = time;
            }

            public string ExchangeId { get; }

            public string ProductId { get; }

            public DateTime Date { get; }

            public DateTime? Time { get; }
        }

        enum TestEnumEcho
        {
            Unknown = 0,

            AlfaBravo = 1,

            CharlieDelta = 2
        }

        public class ImplicitConvertible
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

            public static implicit operator ImplicitConvertible(string source) => new ImplicitConvertible(
                source.Split('|')[0], 
                source.Split('|')[1]);
        }

        public class ExplicitConvertible
        {
            public ExplicitConvertible(
                string alfa,
                string bravo)
            {
                Alfa = alfa;
                Alfa = alfa;

                Bravo = bravo;
            }

            public string Alfa { get; }

            public string Bravo { get; }

            public static explicit operator string(ExplicitConvertible source) => $"{source.Alfa}|{source.Bravo}";

            public static explicit operator ExplicitConvertible(string source) => new ExplicitConvertible(
                source.Split('|')[0],
                source.Split('|')[1]);
        }
    }
}

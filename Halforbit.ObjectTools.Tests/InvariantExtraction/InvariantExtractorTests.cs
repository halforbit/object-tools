using Halforbit.ObjectTools.InvariantExtraction.Implementation;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Xunit;

namespace Halforbit.ObjectTools.Tests.InvariantExtraction
{
    public class InvariantExtractorTests
    {
        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariants_WhenAllInvariants_ThenAllValues()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10, 
                1, 2, 3, 
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariants(
                c => c.AccountId == accountId && c.CreateTime == createTime, 
                out redacted);

            Assert.Equal(accountId, invariant.AccountId);

            Assert.Equal(createTime, invariant.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariants_WhenSomeInvariant_ThenSomeInvariantValue()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariants(
                c => c.AccountId == accountId && c.CreateTime < createTime,
                out redacted);

            Assert.Equal(accountId, invariant.AccountId);

            Assert.Null(invariant.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariants_WhenNoInvariants_ThenNoValues()
        {
            var accountId = new Random().Next();//123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariants(
                c => c.AccountId != accountId && c.CreateTime < createTime,
                out redacted);

            Assert.Null(invariant.AccountId);

            Assert.Null(invariant.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariants_WhenSimpleTypeValue_ThenSimpleTypeResult()
        {
            var expectedValue = new Guid("5891874f63684768b10d17499a21756c");

            Expression<Func<Guid, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariants(
                c => c == expectedValue,
                out redacted);

            Assert.Equal(expectedValue, invariant);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariantDictionary_WhenAllInvariants_ThenAllValues()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariantDictionary(
                c => c.AccountId == accountId && c.CreateTime == createTime,
                out redacted);

            Assert.Equal(2, invariant.Count);

            Assert.Equal(accountId, invariant["AccountId"]);

            Assert.Equal(createTime, invariant["CreateTime"]);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariantDictionary_WhenSomeInvariant_ThenSomeInvariantValue()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariantDictionary(
                c => c.AccountId == accountId && c.CreateTime < createTime,
                out redacted);

            Assert.Equal(1, invariant.Count);

            Assert.Equal(accountId, invariant["AccountId"]);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariantDictionary_EnumPartitioned_WhenSomeInvariant_ThenSomeInvariantValue()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<EnumPartitionedFileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariantDictionary(
                c => c.PartitionId == TestEnum.Bravo && c.AccountId == accountId && c.CreateTime < createTime,
                out redacted);

            Assert.Equal(1, invariant.Count);

            Assert.Equal(accountId, invariant["AccountId"]);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariantDictionary_WhenNoInvariants_ThenNoValues()
        {
            var accountId = new Random().Next();

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var invariant = new InvariantExtractor().ExtractInvariantDictionary(
                c => c.AccountId != accountId && c.CreateTime < createTime,
                out redacted);

            Assert.Equal(0, invariant.Count);
        }

        [Fact, Trait("Type", "Unit")]
        public void ExtractInvariantDictionary_WhenSimpleTypeValue_ThenSimpleTypeResult()
        {
            var expectedValue = Guid.NewGuid();

            Expression<Func<Guid, bool>> redacted;

            var dictionary = new InvariantExtractor().ExtractInvariantDictionary(
                c => c == expectedValue,
                out redacted);

            Assert.Equal(1, dictionary.Count);

            Assert.Equal(expectedValue, dictionary["this"]);
        }

        [Fact, Trait("Type", "Speed")]
        public void ExtractInvariants_SpeedTest()
        {
            var accountId = 123;

            var createTime = new DateTime(
                2016, 7, 10,
                1, 2, 3,
                DateTimeKind.Utc);

            Expression<Func<FileKey, bool>> redacted;

            var time = Stopwatch.StartNew();

            var count = 0;

            while (time.Elapsed < TimeSpan.FromSeconds(5))
            {
                var invariant = new InvariantExtractor().ExtractInvariants(
                    c => c.AccountId == accountId && c.CreateTime == createTime,
                    out redacted);

                Assert.Equal(accountId, invariant.AccountId);

                Assert.Equal(createTime, invariant.CreateTime);

                count++;
            }

            var rate = count / time.Elapsed.TotalSeconds;

            var avgMs = time.Elapsed.TotalMilliseconds / count;

            Console.WriteLine($"Rate: {rate:0.00} / sec, Avg: {avgMs:0.00} ms");
        }

        class FileKey
        {
            public int? AccountId { get; set; }

            public DateTime? CreateTime { get; set; }
        }

        enum TestEnum
        { 
            Unknown = 0,
            Alfa,
            Bravo,
            Charlie
        }

        class EnumPartitionedFileKey
        {
            public TestEnum PartitionId { get; set; }

            public int? AccountId { get; set; }

            public DateTime? CreateTime { get; set; }
        }
    }
}



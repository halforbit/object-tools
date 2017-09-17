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
    }
}



using Halforbit.ObjectTools.ObjectBuild.Implementation;
using System;
using System.Diagnostics;
using Xunit;

namespace Halforbit.ObjectTools.Tests.ObjectBuild
{
    public class BuilderTests
    {
        readonly int TestAccountId = 123;

        readonly DateTime TestCreateTime = new DateTime(2015, 3, 4);

        [Fact, Trait("Type", "Speed")]
        public void Build_SpeedTest()
        {
            var fileKey = new Builder<ImmutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Set(k => k.CreateTime, TestCreateTime)
                .Build();

            var count = 0;

            var time = Stopwatch.StartNew();

            while (time.Elapsed.TotalSeconds < 5)
            {
                fileKey = new Builder<ImmutableFileKey>()
                    .Set(k => k.AccountId, TestAccountId)
                    .Set(k => k.CreateTime, TestCreateTime)
                    .Build();

                count++;
            }

            var totalMs = time.Elapsed.TotalMilliseconds;

            Console.WriteLine($"Rate: {count / (totalMs / 1000):0.00} / s, Avg: {totalMs / count:0.00} ms");
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenImmutableAllValues_ThenSuccess()
        {
            var fileKey = new Builder<ImmutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Set(k => k.CreateTime, TestCreateTime)
                .Build();

            Assert.Equal(TestAccountId, fileKey.AccountId);

            Assert.Equal(TestCreateTime, fileKey.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenImmutableSomeValues_ThenSuccess()
        {
            var fileKey = new Builder<ImmutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Build();

            Assert.Equal(TestAccountId, fileKey.AccountId);

            Assert.Null(fileKey.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenSemiMutableAllValues_ThenSuccess()
        {
            var fileKey = new Builder<SemiMutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Set(k => k.CreateTime, TestCreateTime)
                .Build();

            Assert.Equal(TestAccountId, fileKey.AccountId);

            Assert.Equal(TestCreateTime, fileKey.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenSemiMutableSomeValues_ThenSuccess()
        {
            var fileKey = new Builder<SemiMutableFileKey>()
                .Set(k => k.CreateTime, TestCreateTime)
                .Build();

            Assert.Null(fileKey.AccountId);

            Assert.Equal(TestCreateTime, fileKey.CreateTime);            
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenMutableAllValues_ThenSuccess()
        {
            var fileKey = new Builder<MutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Set(k => k.CreateTime, TestCreateTime)
                .Build();

            Assert.Equal(TestAccountId, fileKey.AccountId);

            Assert.Equal(TestCreateTime, fileKey.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenMutableSomeValues_ThenSuccess()
        {
            var fileKey = new Builder<MutableFileKey>()
                .Set(k => k.AccountId, TestAccountId)
                .Build();

            Assert.Equal(TestAccountId, fileKey.AccountId);

            Assert.Null(fileKey.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenClone_ThenSuccess()
        {
            var fileKeyA = new ImmutableFileKey(TestAccountId, TestCreateTime);

            var fileKeyB = new Builder<ImmutableFileKey>(fileKeyA).Build();

            Assert.Equal(fileKeyA.AccountId, fileKeyB.AccountId);

            Assert.Equal(fileKeyA.CreateTime, fileKeyB.CreateTime);
        }

        [Fact, Trait("Type", "Unit")]
        public void Build_WhenMutation_ThenSuccess()
        {
            var newAccountId = 234;

            var fileKeyA = new ImmutableFileKey(TestAccountId, TestCreateTime);

            var fileKeyB = new Builder<ImmutableFileKey>(fileKeyA)
                .Set(k => k.AccountId, newAccountId)
                .Build();

            Assert.Equal(newAccountId, fileKeyB.AccountId);

            Assert.Equal(fileKeyA.CreateTime, fileKeyB.CreateTime);
        }

        public class ImmutableFileKey
        {
            public ImmutableFileKey(
                int? accountId, 
                DateTime? createTime)
            {
                AccountId = accountId;

                CreateTime = createTime;
            }

            public int? AccountId { get; }

            public DateTime? CreateTime { get; }
        }

        public class SemiMutableFileKey
        {
            public SemiMutableFileKey(
                DateTime? createTime)
            {
                CreateTime = createTime;
            }

            public int? AccountId { get; set; }

            public DateTime? CreateTime { get; }
        }

        public class MutableFileKey
        {
            public int? AccountId { get; set; }

            public DateTime? CreateTime { get; set; }
        }
    }
}

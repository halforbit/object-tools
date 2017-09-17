using Halforbit.ObjectTools.InvariantExtraction.Extensions;
using System;
using Xunit;

namespace Halforbit.ObjectTools.Tests.InvariantExtraction
{
    public class MutateTests
    {
        [Fact, Trait("Type", "Unit")]
        public void Mutate_WhenSourceAndOverrides_ThenCorrectValues()
        {
            var original = new FileKey(
                1234,
                DateTime.UtcNow);

            var mutant = original.Mutate(k => k.AccountId == 2345);

            Assert.Equal(original.CreateTime, mutant.CreateTime);

            Assert.Equal(2345, mutant.AccountId);
        }

        class FileKey
        {
            public FileKey(
                int? accountId = default(int?),
                DateTime? createTime = default(DateTime?))
            {
                AccountId = accountId;

                CreateTime = createTime;
            }

            public int? AccountId { get; }

            public DateTime? CreateTime { get; }
        }
    }
}



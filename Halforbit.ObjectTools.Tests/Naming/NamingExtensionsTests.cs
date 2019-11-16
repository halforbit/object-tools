using Xunit;

namespace Halforbit.ObjectTools.Tests.Naming
{
    public class NamingExtensionsTests
    {
        [Fact, Trait("Type", "Unit")]
        public void TrainToPascalCase_Success()
        {
            var actual = "alfa-bravo.charlie-delta".TrainToPascalCase();

            Assert.Equal("AlfaBravo.CharlieDelta", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void PascalToTrainCase_Success()
        {
            var actual = "AlfaBravo.CharlieDelta".PascalToTrainCase();

            Assert.Equal("alfa-bravo.charlie-delta", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void TrainToCamelCase_Success()
        {
            var actual = "alfa-bravo.charlie-delta".TrainToCamelCase();

            Assert.Equal("alfaBravo.charlieDelta", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void CamelToTrainCase_Success()
        {
            var actual = "alfaBravo.charlieDelta".CamelToTrainCase();

            Assert.Equal("alfa-bravo.charlie-delta", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void PascalToCamelCase_Success()
        {
            var actual = "AlfaBravo.CharlieDelta".TrainToCamelCase();

            Assert.Equal("alfaBravo.charlieDelta", actual);
        }

        [Fact, Trait("Type", "Unit")]
        public void CamelToPascalCase_Success()
        {
            var actual = "alfaBravo.charlieDelta".CamelToPascalCase();

            Assert.Equal("AlfaBravo.CharlieDelta", actual);
        }
    }
}

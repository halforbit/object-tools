using Halforbit.ObjectTools.ObjectStringMap.Implementation;
using System;
using Xunit;

namespace Halforbit.ObjectTools.Tests.ObjectStringMap;

public class StringExpressionConverterTests
{
    [Fact, Trait("Type", "Unit")]
    public void ConstantString_Success()
    {
        var result = StringExpressionConverter.Convert<ForecastKey>(k => $"hello");

        Assert.Equal("hello", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_Properties_Success()
    {
        var result = StringExpressionConverter.Convert<ForecastKey>(k => $"forecasts/{k.PostalCode}/{k.Date:yyyy/MM/dd}");

        Assert.Equal("forecasts/{PostalCode}/{Date:yyyy/MM/dd}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_Self_Success()
    {
        var result = StringExpressionConverter.Convert<DateTime>(k => $"forecasts/{k:yyyy/MM/dd}");

        Assert.Equal("forecasts/{this:yyyy/MM/dd}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_BadShape_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            StringExpressionConverter.Convert<ForecastKey>(k => $"forecasts/{k.PostalCode + 123}/{k.Date:yyyy/MM/dd}"));
    }

    record struct ForecastKey(
        int PostalCode,
        DateTime Date);
}

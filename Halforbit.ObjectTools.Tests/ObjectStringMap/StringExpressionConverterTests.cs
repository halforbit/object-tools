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
        var result = StringExpressionConverter.Convert<ForecastKey>(k => $"forecasts/{k.PostalCode}/{k.City}/{k.Date:yyyy/MM/dd}");

        Assert.Equal("forecasts/{PostalCode}/{City}/{Date:yyyy/MM/dd}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_Tuple_Success()
    {
        var result = StringExpressionConverter.Convert<(int PostalCode, DateTime Date)>(k => $"forecasts/{k.PostalCode}/{k.Date:yyyy/MM/dd}");

        Assert.Equal("forecasts/{Item1}/{Item2:yyyy/MM/dd}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_Self_Success()
    {
        var result = StringExpressionConverter.Convert<DateTime>(k => $"forecasts/{k:yyyy/MM/dd}");

        Assert.Equal("forecasts/{this:yyyy/MM/dd}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_Large_Complex_Success()
    {        
        var result = StringExpressionConverter.Convert<StoreKey>(
            k => $"end-calls/{k.PartnerId}/{k.TemplateVersion}/{k.Year:D4}/{k.Month:D2}/{k.Day:D2}/{k.RecordId}");

        Assert.Equal("end-calls/{PartnerId}/{TemplateVersion}/{Year:D4}/{Month:D2}/{Day:D2}/{RecordId}", result);
    }

    [Fact, Trait("Type", "Unit")]
    public void InterpolatedString_BadShape_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            StringExpressionConverter.Convert<ForecastKey>(k => $"forecasts/{k.PostalCode + 123}/{k.Date:yyyy/MM/dd}"));
    }

    public class StoreKey
    {
        public StoreKey(
            Guid partnerId,
            string templateVersion,
            int year,
            int month,
            int day,
            Guid? recordId)
        {
            PartnerId = partnerId;

            TemplateVersion = templateVersion;
            Year = year;
            Month = month;
            Day = day;

            RecordId = recordId;
        }

        public Guid PartnerId { get; }

        public string TemplateVersion { get; }

        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public Guid? RecordId { get; }
    }

    record struct ForecastKey(
        int PostalCode,
        string City,
        DateTime Date);
}

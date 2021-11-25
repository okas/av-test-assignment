using Xunit;

namespace Backend.WebApi.Tests;

public class UnitTest1
{
    public static bool IsEven(int candidate)
    {
        return candidate % 2 == 0;
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(2)]
    public void IsEvenEvenValues_ReturnTrue(int value)
    {
        var result = IsEven(value);
        Assert.True(result, $"\"{value}\": Should be even number.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(3)]
    public void IsEven_OddValues_ReturnFalse(int value)
    {
        var result = IsEven(value);
        Assert.False(result, $"\"{value}\": Should be even number.");
    }
}
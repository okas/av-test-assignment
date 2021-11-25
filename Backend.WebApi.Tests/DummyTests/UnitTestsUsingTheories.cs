using Xunit;

namespace Backend.WebApi.Tests;

public class UnitTestsUsingTheories
{
    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(2)]
    public void IsEven_EvenNumber_ReturnTrue(int value)
    {
        var subject = new LogicToTest();
        var result = subject.IsEven(value);
        Assert.True(result, $"\"{value}\": Should be even number.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(3)]
    public void IsEven_OddNumber_ReturnFalse(int value)
    {
        var subject = new LogicToTest();
        var result = subject.IsEven(value);
        Assert.False(result, $"\"{value}\": Should be even number.");
    }
}

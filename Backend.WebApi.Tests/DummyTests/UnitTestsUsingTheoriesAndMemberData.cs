using System.Collections.Generic;
using Xunit;

namespace Backend.WebApi.Tests.DummyTests;

public class UnitTestsUsingTheoriesAndMemberData
{
    [Theory]
    [MemberData(nameof(Data))]
    public void IsEven_ModulusBy2_ReturnTrue(int candidate, int expected)
    {
        var result = candidate % 2;
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { 1, 1 },
            new object[] { 0, 0 },
            new object[] { 3, 1 },
            new object[] { 4, 0 },
        };
}

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Backend.WebApi.Tests;

public class UnitTestsUsingFixture : IClassFixture<MyTestFixture>
{
    public UnitTestsUsingFixture(MyTestFixture importantInstanceToTest) => ImportantInstanceToTest = importantInstanceToTest;

    public MyTestFixture ImportantInstanceToTest { get; private set; }

    [Fact]
    public void IsEven_EvenValues_ReturnTrue()
    {
        var subject = new LogicToTest();
        var result = ImportantInstanceToTest.Instance.EvenNumbers.All(subject.IsEven);
        Assert.True(result, $"{{{string.Join(", ", ImportantInstanceToTest.Instance.EvenNumbers)}}}, but all should be even numbers.");
    }

    [Fact]
    public void IsEven_OddValues_ReturnTrue()
    {
        var subject = new LogicToTest();
        var result = !ImportantInstanceToTest.Instance.EvenNumbers.All(subject.IsEven);
        Assert.False(result, $"{{{string.Join(", ", ImportantInstanceToTest.Instance.OddNumbers)}}}, but all should be odd numbers.");
    }
}


public class MyTestFixture : IDisposable
{
    public MyTestFixture()
    {
        // Test fixture setup code
        Instance = new SomeExpensiveTestResourceToShare(
            new int[] { -2, 0, 2 },
            new int[] { -1, 1, 3 }
        );
    }

    public SomeExpensiveTestResourceToShare Instance { get; private set; }

    public void Dispose()
    {
        // Test fixture teardown code
        Instance.EvenNumbers.Clear();
        Instance.OddNumbers.Clear();
    }
}

public class SomeExpensiveTestResourceToShare
{
    public SomeExpensiveTestResourceToShare(int[] evenNumbers, int[] oddNumbers)
    {
        EvenNumbers = new List<int>(evenNumbers);
        OddNumbers = new List<int>(oddNumbers);
    }

    public List<int> EvenNumbers { get; private set; }

    public List<int> OddNumbers { get; private set; }
}

public class LogicToTest
{
    public bool IsEven(int candidate) => candidate % 2 == 0;
}
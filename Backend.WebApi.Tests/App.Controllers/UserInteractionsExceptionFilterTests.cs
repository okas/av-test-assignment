using System.Reflection;
using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Backend.WebApi.Tests.App.Controllers;

public class UserInteractionsExceptionFilterTests
{
    private static readonly Type _controllerType;

    static UserInteractionsExceptionFilterTests() => _controllerType = typeof(UserInteractionsController);

    [Theory]
    [MemberData(nameof(GetActionMethods))]
    public void ActionMethodsWithCUDOperationsExceptionFilter_DecoratedCorrectly_ShouldSucceed(
        // Arrange
        MethodInfo actionMethod)
    {
        // Act
        // Assert
        actionMethod.Should().BeDecoratedWith<ServiceFilterAttribute>()
            .Which.ServiceType.Should().Be<CUDOperationsExceptionFilter>();
    }

    public static IEnumerable<object[]> GetActionMethods()
    {
        return new List<object[]>
        {
            new object[] { _controllerType.GetMethod(nameof(UserInteractionsController.PostUserInteraction))! },
            new object[] { _controllerType.GetMethod(nameof(UserInteractionsController.PatchUserInteraction))! },
        };
    }
}

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.WebApi.App.Controllers;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static Backend.WebApi.Tests.UserInteractionUtilities;

namespace Backend.WebApi.Tests.App.Controllers;

[Collection("IntegrationTestFixture")]
public sealed class UserInteractionQueryTests
{
    private readonly (Guid Id, bool IsOpen)[] _knownEntitesIdIsOpen;
    private readonly UserInteractionsController _sutController;

    /// <summary>
    /// Uses DbTransaction to roll back changes after each test
    /// </summary>
    /// <param name="fixture"></param>
    public UserInteractionQueryTests(IntegrationTestFixture fixture)
    {
        _knownEntitesIdIsOpen = GenerateKnownData(5);
        SeedData(fixture, _knownEntitesIdIsOpen);
        _sutController = new UserInteractionsController(
            fixture.ScopedServiceProvider!.GetService<IMediator>()!);
    }

    [Fact]
    public async Task Get_CanGetAll_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: default,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeAssignableTo<IEnumerable<UserInteractionDto>>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnOkObjectResultAndNonNullValue()
    {
        // Arrange+
        UserInteractionGetByIdQuery query = new()
        {
            Id = _knownEntitesIdIsOpen[0].Id,
        };

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                query: query,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                .And.BeOfType<UserInteractionDto>();
    }

    [Fact]
    public async Task Get_CanGetSingleById_ReturnDtoWithCorrectId()
    {
        // Arrange+
        UserInteractionGetByIdQuery query = new()
        {
            Id = _knownEntitesIdIsOpen[0].Id,
        };

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                query,
                ct: default
                );

        // Assert
        response.Result.As<OkObjectResult>().Value.As<UserInteractionDto>()
                .Id.Should().Be(query.Id);
    }

    [Fact]
    public async Task Get_CannotGetUsingNonExistingId_ReturnNotFoundResult()
    {
        // Arrange
        UserInteractionGetByIdQuery query = new()
        {
            Id = Guid.NewGuid(),
        };

        // Act
        ActionResult<UserInteractionDto> response =
            await _sutController.GetUserInteraction(
                query,
                ct: default
                );

        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Get_CanGetUnFiltered_ReturnSome()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: default,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenTrue_ReturnSome()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: true,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CanFilteredIsOpenFalse_ReturnSome()
    {
        // Arrange
        // Act
        ActionResult<IEnumerable<UserInteractionDto>> response =
            await _sutController.GetUserInteractions(
                isOpen: false,
                ct: default
                );

        // Assert
        response.Result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        response.Result.As<OkObjectResult>().Value.Should().NotBeNull()
                 .And.BeAssignableTo<IEnumerable<UserInteractionDto>>()
                 .Which.Should().HaveCountGreaterThan(0);
    }
}

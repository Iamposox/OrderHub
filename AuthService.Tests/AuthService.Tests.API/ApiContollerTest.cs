using AuthService.API.Controllers;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace AuthService.Tests.API;
public class ApiContollerTest : IAsyncLifetime
{
    private ApiController _apiController;
    private Mock<IUserService> _userService;
    public async Task InitializeAsync()
    {
        _userService = new Mock<IUserService>();
        var loggerApi = NullLogger<ApiController>.Instance;
        _apiController = new ApiController(loggerApi, _userService.Object);
    }
    public async Task DisposeAsync()
    {
    }
    [Fact]
    public async Task RegisterUserAsyncDone()
    {
        var dto = new UserCreateDto("Danila", "Test", "Admin");
        _userService.Setup(u => u.RegisterUserAsync(dto)).Returns(Task.FromResult(dto));

        var result = await _apiController.RegisterAsync(dto);

        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
    }
}

using Xunit;
using Moq;
using Microsoft.JSInterop;
using TeslaAPI;
using System;
using System.Threading.Tasks;

namespace TeslaAPITestUnit;

public class UnitTest1
{
    [Fact]
    public async Task StoreToken_SetsTokenInLocalStorage()
    {
        // Arrange
        var mockJsRuntime = new Mock<IJSRuntime>();
        var tokenResponse = new TokenResponse
        {
            access_token = "testAccessToken",
            expires_in = 3600 // 1 hour for example
        };

        // Setup mock to capture calls to IJSRuntime
        // Note: Depending on the implementation details of LocalStorageService and how it interacts with IJSRuntime,
        // you might need to adjust the setup to match actual method calls.
        mockJsRuntime.Setup(js =>
            js.InvokeAsync<object>(
                "localStorage.setItem",
                It.IsAny<object[]>())
            ).Returns(Task.FromResult((object)null));

        // Act
        var result = await TeslaAPI.Commands.StoreToken(tokenResponse, mockJsRuntime.Object);

        // Assert
        // Verify that localStorage.setItem was called with the correct parameters
        mockJsRuntime.Verify(js =>
            js.InvokeAsync<object>(
                "localStorage.setItem",
                It.Is<object[]>(args => args[0].ToString() == "UserToken" && args[1].ToString() == tokenResponse.access_token)),
            Times.Once());

        mockJsRuntime.Verify(js =>
            js.InvokeAsync<object>(
                "localStorage.setItem",
                It.Is<object[]>(args => args[0].ToString() == "UserTokenExpiresAt")),
            Times.Once());

        Assert.Equal(string.Empty, result);
    }
}

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using TeslaAPI.Component;
using Microsoft.JSInterop;
using TeslaAPI.Pages;

public class TestInterface
{
    [Fact]
    public void AddFavorite_ButtonClick_AddsFavoriteCommand()
    {
        // Arrange
        using var ctx = new TestContext();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        ctx.Services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);
        ctx.Services.AddSingleton<FavoriteCommands>();

        var ServiceCommand = ctx.Services.GetService<FavoriteCommands>();

        var component = ctx.RenderComponent<Interface>();
        var btn = component.Find("button");
        btn.Click();
        Assert.Equal(1, ServiceCommand.Commands.Count);
    }
}
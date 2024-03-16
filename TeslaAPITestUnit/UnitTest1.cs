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
        var favoriteCommandsMock = new Mock<FavoriteCommands>();
        favoriteCommandsMock.Setup(m => m.BaseTasks).Returns(new List<string> { "Command1", "Command2" });
        favoriteCommandsMock.Setup(m => m.Commands).Returns(new List<Command>());

        ctx.Services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);
        ctx.Services.AddSingleton<FavoriteCommands>(favoriteCommandsMock.Object);

     
        var component = ctx.RenderComponent<Interface>();
        component.Find("button.specific-class").Click();
        favoriteCommandsMock.Verify(m => m.Commands.Add(It.IsAny<Command>()), Times.Once);
    }
}

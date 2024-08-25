using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using TeslaAPI.Component;
using Microsoft.JSInterop;
using TeslaAPI.Pages;
using TeslaAPI.Data;
using Azure;
using Microsoft.AspNetCore.Routing;
using TeslaAPI.Component.Commands;

public class TestInterface
{
    [Fact]
    public void AddFavorite_ButtonClick_AddsFavoriteCommand()
    {
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
    [Fact]
    public void RemoveFavorite_ButtonClick_RemovesFavoriteCommand()
    {
        using var ctx = new TestContext();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        ctx.Services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);
        ctx.Services.AddSingleton<FavoriteCommands>();

        var ServiceCommand = ctx.Services.GetService<FavoriteCommands>();

        var component = ctx.RenderComponent<Interface>();
        var btn = component.Find("button");
        btn.Click();
        btn.Click();
        Assert.Equal(0, ServiceCommand.Commands.Count);
    }

    [Fact]
    public void SetData_UpdatesPropertiesCorrectly()
    {
        using var ctx = new TestContext();
        ctx.Services.AddSingleton<VehicleDataResponse>();
        ctx.Services.AddSingleton<FavoriteCommands>();
        var vehicleDataResponse = new VehicleDataResponse
        {
            response = new ResponseData
            {
                charge_state = new ChargeState
                {
                    battery_level = 80,
                    preconditioning_times = "None",
                    preconditioning_enable = false,
                    charge_port_door_open = true,
                    charging_state = "Charging",
                    managed_charging_active = true
                },
                vehicle_state = new VehicleState
                {
                    sentry_mode = true,
                    sentry_mode_available = true,
                    preconditioning_enable = true,
                    remote_start = false,
                    locked = false
                },
                user_id = 12345,
                vehicle_id = 67890,
                vin = "123VINTEST"
            }
        };
        ctx.Services.AddSingleton<VehicleDataResponse>(vehicleDataResponse);
        var ServiceCommand = ctx.Services.GetService<VehicleDataResponse>();
        var FavCommands = ctx.Services.GetService<FavoriteCommands>();
        var testComponent = ctx.RenderComponent<TeslaAPI.Pages.Index>();
        testComponent.Instance.SetData(ServiceCommand);
        Assert.Equal(80, ServiceCommand.response.charge_state.battery_level);
        Assert.Equal("None", ServiceCommand.response.charge_state.preconditioning_times);
        Assert.True(ServiceCommand.response.vehicle_state.preconditioning_enable);
        Assert.True(ServiceCommand.response.vehicle_state.sentry_mode);
        Assert.False(ServiceCommand.response.vehicle_state.locked);
    }
}

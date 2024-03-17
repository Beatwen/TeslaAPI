using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using TeslaAPI.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;
using TeslaAPI.Component;
using TeslaAPI.Component.Commands;



var builder = WebApplication.CreateBuilder(args);
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<FavoriteCommands>();
builder.Services.AddSingleton<VehicleDataResponse>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.MapGet("/.well-known/appspecific/com.tesla.3p.public-key.pem", async context =>
{
    var publicKeyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/.well-known/appspecific/com.tesla.3p.public-key.pem");
    if (File.Exists(publicKeyFilePath))
    {
        var publicKeyContent = await File.ReadAllTextAsync(publicKeyFilePath);
        context.Response.Headers.Add("Content-Type", "application/x-pem-file");
        await context.Response.WriteAsync(publicKeyContent);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

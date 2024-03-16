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

// Add a route for serving .pem files
app.MapGet("/.well-known/appspecific/com.tesla.3p.public-key.pem", async context =>
{
    // Path to your public key file
    var publicKeyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/.well-known/appspecific/com.tesla.3p.public-key.pem");
    Debug.Print("public key file path : " + publicKeyFilePath);
    if (File.Exists(publicKeyFilePath))
    {
        Debug.Print("Public key file found");
        // Read the content of the .pem file
        var publicKeyContent = await File.ReadAllTextAsync(publicKeyFilePath);

        // Set the content type header
        context.Response.Headers.Add("Content-Type", "application/x-pem-file");
       // Write the .pem file content to the response
        await context.Response.WriteAsync(publicKeyContent);
    }
    else
    {
        Debug.Print("Public key file not found");
        // .pem file not found, return a 404 response
        context.Response.StatusCode = 404;
    }
});

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<EnvHost>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/nginx", async ([FromServices] HttpClient client) => 
{
    var url = "http://nginx";
    var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();

    return body;
});

app.MapGet("/", (EnvHost hoster) => 
{
    return $"Version II - Hello from {hoster.GetHostName}";
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public class EnvHost
{
    private string name = $"Apka nr.: {Random.Shared.Next(1, 99)}";
    public string GetHostName => name;
}
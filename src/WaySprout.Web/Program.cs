using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton<IJobApplicationRepository, InMemoryJobApplicationRepository>();
builder.Services.AddScoped<GetJobApplicationsHandler>();

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/api/v1/applications", async (GetJobApplicationsHandler handler) =>
{
    var result = await handler.HandleAsync();
    return Results.Ok(result);
});

app.Run();
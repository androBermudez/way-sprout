using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Application.UseCases.GetJobApplicationById;
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
builder.Services.AddScoped<GetJobApplicationByIdHandler>();

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/api/v1/applications", async (GetJobApplicationsHandler handler) =>
{
    var result = await handler.HandleAsync();
    return Results.Ok(result);
});

app.MapGet("/api/v1/applications/{id:guid}", async (Guid id, GetJobApplicationByIdHandler handler) =>
{
    var result = await handler.HandleAsync(id);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.Run();
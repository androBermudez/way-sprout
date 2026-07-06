using WaySprout.Application.Ports;
using WaySprout.Application.Services;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Application.UseCases.GetJobApplicationById;
using WaySprout.Domain.Enums;
using WaySprout.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<DateRangePresetResolver>();
builder.Services.AddSingleton<IJobApplicationRepository, InMemoryJobApplicationRepository>();
builder.Services.AddScoped<GetJobApplicationsHandler>();
builder.Services.AddScoped<GetJobApplicationByIdHandler>();

var app = builder.Build();

app.UseCors("Frontend");

var apiGroup = app.MapGroup("/api/v1");

apiGroup.MapGet("/applications", async (GetJobApplicationsHandler handler) =>
{
    // TODO: parse status/searchText/appliedRange/sortBy/direction from the query string.
    var query = new JobApplicationQuery(
        SearchText: null,
        Statuses: new HashSet<ApplicationStatus>(),
        AppliedRange: null,
        SortBy: null,
        Direction: null);

    var result = await handler.HandleAsync(query);
    return Results.Ok(result);
});

apiGroup.MapGet("/applications/{id:guid}", async (Guid id, GetJobApplicationByIdHandler handler) =>
{
    var result = await handler.HandleAsync(id);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.Run();
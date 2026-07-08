using Microsoft.AspNetCore.Mvc;
using WaySprout.Application.Enums;
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

builder.Services.AddProblemDetails();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<DateRangePresetResolver>();
builder.Services.AddScoped<IJobApplicationRepository, InMemoryJobApplicationRepository>();
builder.Services.AddScoped<GetJobApplicationsHandler>();
builder.Services.AddScoped<GetJobApplicationByIdHandler>();

var app = builder.Build();

app.UseCors("Frontend");

var apiGroup = app.MapGroup("/api/v1");

apiGroup.MapGet("/applications", async (
    string? q,
    string[]? status,
    [FromQuery(Name = "applied-range")] string? appliedRange,
    [FromQuery(Name = "sort-by")] string? sortBy,
    string? direction,
    GetJobApplicationsHandler handler) =>
{
  var statuses = new HashSet<ApplicationStatus>();

  if (status is not null)
  {
    foreach (var value in status)
    {
      if (!TryParseEnum<ApplicationStatus>(value, out var parsedStatus))
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          ["status"] = [$"Invalid status value: '{value}'."],
        });
      }

      statuses.Add(parsedStatus!.Value);
    }
  }

  if (!TryParseEnum<DateRangePreset>(appliedRange, out var parsedAppliedRange))
  {
    return Results.ValidationProblem(new Dictionary<string, string[]>
    {
      ["applied-range"] = [$"Invalid applied-range value: '{appliedRange}'."],
    });
  }

  if (!TryParseEnum<JobApplicationSortCriteria>(sortBy, out var parsedSortBy))
  {
    return Results.ValidationProblem(new Dictionary<string, string[]>
    {
      ["sort-by"] = [$"Invalid sort-by value: '{sortBy}'."],
    });
  }

  if (!TryParseEnum<SortDirection>(direction, out var parsedDirection))
  {
    return Results.ValidationProblem(new Dictionary<string, string[]>
    {
      ["direction"] = [$"Invalid direction value: '{direction}'."],
    });
  }

  var query = new JobApplicationQuery(q, statuses, parsedAppliedRange, parsedSortBy, parsedDirection);
  var result = await handler.HandleAsync(query);
  return Results.Ok(result);
});

apiGroup.MapGet("/applications/{id:guid}", async (Guid id, GetJobApplicationByIdHandler handler) =>
{
  var result = await handler.HandleAsync(id);
  return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.Run();

// Absent (null/empty) input parses to `null` successfully. An unrecognized value fails.
static bool TryParseEnum<TEnum>(string? value, out TEnum? result) where TEnum : struct, Enum
{
  if (string.IsNullOrEmpty(value))
  {
    result = null;
    return true;
  }

  if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
  {
    result = parsed;
    return true;
  }

  result = null;
  return false;
}

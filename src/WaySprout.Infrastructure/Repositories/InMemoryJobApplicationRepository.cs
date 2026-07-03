using WaySprout.Application.Ports;
using WaySprout.Domain.Entities;

namespace WaySprout.Infrastructure.Repositories;

public class InMemoryJobApplicationRepository : IJobApplicationRepository
{
  private static readonly Guid SeedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

  private static readonly IReadOnlyList<JobApplication> Seed =
  [
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
      SeedUserId,
      "Nébula Soft",
      "Backend Engineer",
      """
      Buscamos una persona para el equipo de plataforma, enfocada en el diseño de
      APIs internas y la evolución de nuestro core de facturación. Trabajarás con
      arquitectura hexagonal, colas de mensajes y bases de datos relacionales.
      """,
      new DateOnly(2026, 5, 12)
    ),
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
      SeedUserId,
      "Cronos Digital",
      "Frontend Engineer",
      """
      Equipo de producto buscando reforzar el frontend de nuestra suite de
      analítica. Stack principal en React y TypeScript, con foco en accesibilidad
      y componentes reutilizables.
      """,
      new DateOnly(2026, 6, 2)
    ),
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
      SeedUserId,
      "Vertice Labs",
      "Data Analyst",
      """
      Rol orientado a construir dashboards y modelos de reporting para el área
      comercial. Se valora experiencia con SQL avanzado y herramientas de
      visualización de datos.
      """,
      new DateOnly(2026, 6, 20)
    ),
  ];

  public Task<IReadOnlyList<JobApplication>> GetAllAsync()
  {
    return Task.FromResult(Seed);
  }
}

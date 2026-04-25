# Integration Testing Template (.NET) - EnglishSeed Engine

Esta guia define una base de integration testing profesional para API con foco en:

1. Realismo: pruebas contra PostgreSQL real en contenedor.
2. Aislamiento: reset de estado entre tests con Respawn.
3. Velocidad: una sola instancia de contenedor por coleccion.
4. Mantenibilidad: clase base reutilizable y convenciones claras.

## Paquetes NuGet recomendados

En `tests/EnglishSeedEngine.IntegrationTests/EnglishSeedEngine.IntegrationTests.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.5" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
  <PackageReference Include="xunit" Version="2.8.1" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.1" />
  <PackageReference Include="FluentAssertions" Version="6.12.0" />
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />
  <PackageReference Include="Respawn" Version="6.2.1" />
</ItemGroup>
```

## Estructura recomendada

```text
tests/EnglishSeedEngine.IntegrationTests/
  Infrastructure/
    PostgresContainerFixture.cs
    RespawnCheckpoint.cs
    IntegrationTestWebApplicationFactory.cs
    ApiIntegrationTestFixture.cs
    ApiTestCollection.cs
  Students/
    StudentsEndpointsTests.cs
```

## Flujo del harness

1. Arranca contenedor PostgreSQL una vez por coleccion.
2. Inyecta connection string en `WebApplicationFactory`.
3. Ejecuta migraciones al iniciar fixture.
4. Crea checkpoint Respawn.
5. Antes de cada test: `ResetDatabaseAsync()`.

## Convenciones

1. Nombre de test: `Action_Condition_ExpectedResult`.
2. No mockear EF ni repositorio en integration tests.
3. Mockear solo bordes externos (IA, reloj, proveedor auth externo).
4. Un test valida una ruta de comportamiento completa.

## Comandos de uso

```bash
dotnet test tests/EnglishSeedEngine.IntegrationTests
dotnet test --filter "FullyQualifiedName~StudentsEndpointsTests"
```

## Nota de adaptacion

Los archivos plantilla en `templates/dotnet-integration-testing` usan placeholders:

- Namespace: `EnglishSeedEngine.*`
- DbContext: `AppDbContext`
- Servicio externo: `IAiContentGenerator`

Sustituyelos por los nombres reales al bootstrap del proyecto.


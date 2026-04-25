# Dotnet Integration Testing Starter

Este starter es un punto de partida para `tests/EnglishSeedEngine.IntegrationTests`.

## Pasos de adaptacion

1. Copia `Infrastructure/` y `Students/` al proyecto de tests real.
2. Cambia namespaces para que coincidan con tu solucion.
3. Ajusta referencias:
   - `EnglishSeedEngine.Infrastructure.Persistence`
   - `AppDbContext`
4. En la API, expone `Program` para test host:

```csharp
public partial class Program;
```

5. Si usas proveedores externos (IA, reloj, auth), sustituyelos por doubles en `IntegrationTestWebApplicationFactory`.

## Comando de validacion

```bash
dotnet test tests/EnglishSeedEngine.IntegrationTests
```


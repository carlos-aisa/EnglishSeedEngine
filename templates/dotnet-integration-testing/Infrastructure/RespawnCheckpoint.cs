using System;
using System.Threading.Tasks;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace EnglishSeedEngine.IntegrationTests.Infrastructure;

public sealed class RespawnCheckpoint
{
    private Respawner? _respawner;

    public async Task InitializeAsync(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
            });
    }

    public async Task ResetAsync(string connectionString)
    {
        if (_respawner is null)
        {
            throw new InvalidOperationException("Respawn is not initialized.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}

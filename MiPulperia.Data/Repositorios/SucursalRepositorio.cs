using MiPulperia.Model;
using Npgsql;

namespace MiPulperia.Data.Repositorios;

public class SucursalRepositorio
{
    private readonly ConexionPostgres _conexion;

    public SucursalRepositorio(ConexionPostgres conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Sucursal>> ListarAsync()
    {
        var sucursales = new List<Sucursal>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync();

        const string sql = """
            SELECT id_sucursal, nombre, direccion, telefono, activa
            FROM user_12.mp_sucursales
            ORDER BY id_sucursal;
            """;

        await using var comando = new NpgsqlCommand(sql, conexion);
        await using var lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            sucursales.Add(new Sucursal
            {
                IdSucursal = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Direccion = lector.GetString(2),
                Telefono = lector.IsDBNull(3)
                    ? null
                    : lector.GetString(3),
                Activa = lector.GetBoolean(4)
            });
        }

        return sucursales;
    }

public async Task CrearAsync(Sucursal sucursal)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        INSERT INTO user_12.mp_sucursales
            (nombre, direccion, telefono, activa)
        VALUES
            (@nombre, @direccion, @telefono, @activa);
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue(
        "nombre", sucursal.Nombre);

    comando.Parameters.AddWithValue(
        "direccion", sucursal.Direccion);

    comando.Parameters.AddWithValue(
        "telefono", (object?)sucursal.Telefono ?? DBNull.Value);

    comando.Parameters.AddWithValue(
        "activa", sucursal.Activa);

    await comando.ExecuteNonQueryAsync();
}

public async Task<Sucursal?> ObtenerPorIdAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        SELECT id_sucursal, nombre, direccion, telefono, activa
        FROM user_12.mp_sucursales
        WHERE id_sucursal = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);
    comando.Parameters.AddWithValue("id", id);

    await using var lector = await comando.ExecuteReaderAsync();

    if (await lector.ReadAsync())
    {
        return new Sucursal
        {
            IdSucursal = lector.GetInt32(0),
            Nombre = lector.GetString(1),
            Direccion = lector.GetString(2),
            Telefono = lector.IsDBNull(3)
                ? null
                : lector.GetString(3),
            Activa = lector.GetBoolean(4)
        };
    }

    return null;
}

public async Task<bool> ActualizarAsync(Sucursal sucursal)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        UPDATE user_12.mp_sucursales
        SET nombre = @nombre,
            direccion = @direccion,
            telefono = @telefono,
            activa = @activa
        WHERE id_sucursal = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue(
        "id", sucursal.IdSucursal);

    comando.Parameters.AddWithValue(
        "nombre", sucursal.Nombre);

    comando.Parameters.AddWithValue(
        "direccion", sucursal.Direccion);

    comando.Parameters.AddWithValue(
        "telefono",
        (object?)sucursal.Telefono ?? DBNull.Value);

    comando.Parameters.AddWithValue(
        "activa", sucursal.Activa);

    int filasActualizadas = await comando.ExecuteNonQueryAsync();

    return filasActualizadas > 0;
    
}

public async Task<bool> EliminarAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        DELETE FROM user_12.mp_sucursales
        WHERE id_sucursal = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue("id", id);

    int filasEliminadas = await comando.ExecuteNonQueryAsync();

    return filasEliminadas > 0;
}

}

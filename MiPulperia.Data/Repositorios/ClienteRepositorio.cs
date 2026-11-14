using MiPulperia.Model;
using Npgsql;

namespace MiPulperia.Data.Repositorios;

public class ClienteRepositorio
{
    private readonly ConexionPostgres _conexion;

    public ClienteRepositorio(ConexionPostgres conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Cliente>> ListarAsync()
    {
        var clientes = new List<Cliente>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync();

        const string sql = """
            SELECT id_cliente,
                   identificacion,
                   nombre,
                   correo,
                   telefono
            FROM user_12.mp_clientes
            ORDER BY id_cliente;
            """;

        await using var comando = new NpgsqlCommand(sql, conexion);
        await using var lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            clientes.Add(new Cliente
            {
                IdCliente = lector.GetInt32(0),
                Identificacion = lector.GetString(1),
                Nombre = lector.GetString(2),
                Correo = lector.GetString(3),
                Telefono = lector.IsDBNull(4)
                    ? null
                    : lector.GetString(4)
            });
        }

        return clientes;
    }

public async Task CrearAsync(Cliente cliente)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        INSERT INTO user_12.mp_clientes
            (identificacion, nombre, correo, telefono)
        VALUES
            (@identificacion, @nombre, @correo, @telefono);
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue(
        "identificacion", cliente.Identificacion);

    comando.Parameters.AddWithValue(
        "nombre", cliente.Nombre);

    comando.Parameters.AddWithValue(
        "correo", cliente.Correo);

    comando.Parameters.AddWithValue(
        "telefono",
        (object?)cliente.Telefono ?? DBNull.Value);

    await comando.ExecuteNonQueryAsync();
}

public async Task<Cliente?> ObtenerPorIdAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        SELECT id_cliente,
               identificacion,
               nombre,
               correo,
               telefono
        FROM user_12.mp_clientes
        WHERE id_cliente = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);
    comando.Parameters.AddWithValue("id", id);

    await using var lector = await comando.ExecuteReaderAsync();

    if (await lector.ReadAsync())
    {
        return new Cliente
        {
            IdCliente = lector.GetInt32(0),
            Identificacion = lector.GetString(1),
            Nombre = lector.GetString(2),
            Correo = lector.GetString(3),
            Telefono = lector.IsDBNull(4)
                ? null
                : lector.GetString(4)
        };
    }

    return null;
}

public async Task<bool> ActualizarAsync(Cliente cliente)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        UPDATE user_12.mp_clientes
        SET identificacion = @identificacion,
            nombre = @nombre,
            correo = @correo,
            telefono = @telefono
        WHERE id_cliente = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue(
        "id", cliente.IdCliente);

    comando.Parameters.AddWithValue(
        "identificacion", cliente.Identificacion);

    comando.Parameters.AddWithValue(
        "nombre", cliente.Nombre);

    comando.Parameters.AddWithValue(
        "correo", cliente.Correo);

    comando.Parameters.AddWithValue(
        "telefono",
        (object?)cliente.Telefono ?? DBNull.Value);

    int filasActualizadas = await comando.ExecuteNonQueryAsync();

    return filasActualizadas > 0;
}

public async Task<bool> EliminarAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        DELETE FROM user_12.mp_clientes
        WHERE id_cliente = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue("id", id);

    int filasEliminadas = await comando.ExecuteNonQueryAsync();

    return filasEliminadas > 0;
}
}

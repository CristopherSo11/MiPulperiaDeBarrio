using MiPulperia.Model;
using Npgsql;

namespace MiPulperia.Data.Repositorios;

public class ProductoRepositorio
{
    private readonly ConexionPostgres _conexion;

    public ProductoRepositorio(ConexionPostgres conexion)
    {
        _conexion = conexion;
    }

    public async Task<List<Producto>> ListarAsync()
    {
        var productos = new List<Producto>();

        await using var conexion = _conexion.CrearConexion();
        await conexion.OpenAsync();

        const string sql = """
            SELECT id_producto,
                   codigo,
                   nombre,
                   descripcion,
                   precio,
                   atributos::text,
                   maximo_reserva,
                   horas_reserva,
                   existencia_minima
            FROM user_12.mp_productos
            ORDER BY id_producto;
            """;

        await using var comando = new NpgsqlCommand(sql, conexion);
        await using var lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            productos.Add(new Producto
            {
                IdProducto = lector.GetInt32(0),
                Codigo = lector.GetString(1),
                Nombre = lector.GetString(2),
                Descripcion = lector.IsDBNull(3)
                    ? null
                    : lector.GetString(3),
                Precio = lector.GetDecimal(4),
                Atributos = lector.GetString(5),
                MaximoReserva = lector.GetInt32(6),
                HorasReserva = lector.GetInt32(7),
                ExistenciaMinima = lector.GetInt32(8)
            });
        }

        return productos;
    }

public async Task CrearAsync(Producto producto)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        INSERT INTO user_12.mp_productos
        (
            codigo,
            nombre,
            descripcion,
            precio,
            atributos,
            maximo_reserva,
            horas_reserva,
            existencia_minima
        )
        VALUES
        (
            @codigo,
            @nombre,
            @descripcion,
            @precio,
            @atributos,
            @maximo_reserva,
            @horas_reserva,
            @existencia_minima
        );
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue(
        "codigo", producto.Codigo);

    comando.Parameters.AddWithValue(
        "nombre", producto.Nombre);

    comando.Parameters.AddWithValue(
        "descripcion",
        (object?)producto.Descripcion ?? DBNull.Value);

    comando.Parameters.AddWithValue(
        "precio", producto.Precio);

    comando.Parameters.Add(
        "atributos",
        NpgsqlTypes.NpgsqlDbType.Jsonb
    ).Value = producto.Atributos;

    comando.Parameters.AddWithValue(
        "maximo_reserva", producto.MaximoReserva);

    comando.Parameters.AddWithValue(
        "horas_reserva", producto.HorasReserva);

    comando.Parameters.AddWithValue(
        "existencia_minima", producto.ExistenciaMinima);

    await comando.ExecuteNonQueryAsync();
}

public async Task<Producto?> ObtenerPorIdAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        SELECT id_producto,
               codigo,
               nombre,
               descripcion,
               precio,
               atributos::text,
               maximo_reserva,
               horas_reserva,
               existencia_minima
        FROM user_12.mp_productos
        WHERE id_producto = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);
    comando.Parameters.AddWithValue("id", id);

    await using var lector = await comando.ExecuteReaderAsync();

    if (await lector.ReadAsync())
    {
        return new Producto
        {
            IdProducto = lector.GetInt32(0),
            Codigo = lector.GetString(1),
            Nombre = lector.GetString(2),
            Descripcion = lector.IsDBNull(3)
                ? null
                : lector.GetString(3),
            Precio = lector.GetDecimal(4),
            Atributos = lector.GetString(5),
            MaximoReserva = lector.GetInt32(6),
            HorasReserva = lector.GetInt32(7),
            ExistenciaMinima = lector.GetInt32(8)
        };
    }

    return null;
}

public async Task<bool> ActualizarAsync(Producto producto)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        UPDATE user_12.mp_productos
        SET codigo = @codigo,
            nombre = @nombre,
            descripcion = @descripcion,
            precio = @precio,
            atributos = @atributos,
            maximo_reserva = @maximo_reserva,
            horas_reserva = @horas_reserva,
            existencia_minima = @existencia_minima
        WHERE id_producto = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);

    comando.Parameters.AddWithValue("id", producto.IdProducto);
    comando.Parameters.AddWithValue("codigo", producto.Codigo);
    comando.Parameters.AddWithValue("nombre", producto.Nombre);

    comando.Parameters.AddWithValue(
        "descripcion",
        (object?)producto.Descripcion ?? DBNull.Value);

    comando.Parameters.AddWithValue("precio", producto.Precio);

    comando.Parameters.Add(
        "atributos",
        NpgsqlTypes.NpgsqlDbType.Jsonb
    ).Value = producto.Atributos;

    comando.Parameters.AddWithValue(
        "maximo_reserva", producto.MaximoReserva);

    comando.Parameters.AddWithValue(
        "horas_reserva", producto.HorasReserva);

    comando.Parameters.AddWithValue(
        "existencia_minima", producto.ExistenciaMinima);

    return await comando.ExecuteNonQueryAsync() > 0;
}

public async Task<bool> EliminarAsync(int id)
{
    await using var conexion = _conexion.CrearConexion();
    await conexion.OpenAsync();

    const string sql = """
        DELETE FROM user_12.mp_productos
        WHERE id_producto = @id;
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);
    comando.Parameters.AddWithValue("id", id);

    return await comando.ExecuteNonQueryAsync() > 0;
}
}

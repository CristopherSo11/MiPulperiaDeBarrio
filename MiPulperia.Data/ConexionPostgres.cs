using Npgsql;

namespace MiPulperia.Data;

public class ConexionPostgres
{
    private readonly string _cadenaConexion;

    public ConexionPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

public async Task<bool> ProbarConexionAsync()
{
    await using var conexion = CrearConexion();

    await conexion.OpenAsync();

    await using var comando = new NpgsqlCommand(
        "SELECT 1", conexion);

    var resultado = await comando.ExecuteScalarAsync();

    return Convert.ToInt32(resultado) == 1;
}

    public NpgsqlConnection CrearConexion()
    {
        return new NpgsqlConnection(_cadenaConexion);
    }
}


namespace MiPulperia.Model;

public class Cliente
{
    public int IdCliente { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string? Telefono { get; set; }
}

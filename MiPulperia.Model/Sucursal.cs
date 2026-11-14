namespace MiPulperia.Model;

public class Sucursal
{
    public int IdSucursal { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public bool Activa { get; set; } = true;
}

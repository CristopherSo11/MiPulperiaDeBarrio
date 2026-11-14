namespace MiPulperia.Model;

public class Producto
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public string Atributos { get; set; } = "{}";

    public int MaximoReserva { get; set; } = 1;

    public int HorasReserva { get; set; } = 24;

    public int ExistenciaMinima { get; set; } = 0;
}

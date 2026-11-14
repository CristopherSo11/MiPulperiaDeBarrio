using Microsoft.AspNetCore.Mvc;
using MiPulperia.Data.Repositorios;
using MiPulperia.Model;
using Npgsql;
using System.Text.Json;

namespace MiPulperia.Mvc.Controllers;

public class ProductosController : Controller
{
    private readonly ProductoRepositorio _repositorio;

    public ProductosController(ProductoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // CONSULTAR PRODUCTOS

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productos = await _repositorio.ListarAsync();
        return View(productos);
    }

    // CREAR PRODUCTO

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new Producto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Producto producto)
    {
        ValidarProducto(producto);

        if (!ModelState.IsValid)
        {
            return View(producto);
        }

        try
        {
            await _repositorio.CrearAsync(producto);

            TempData["Mensaje"] =
                "Producto registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex)
            when (ex.SqlState == "23505")
        {
            ModelState.AddModelError(
                nameof(producto.Codigo),
                "Ya existe un producto con este código.");

            return View(producto);
        }
    }

    // EDITAR PRODUCTO

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _repositorio.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id, Producto producto)
    {
        if (id != producto.IdProducto)
        {
            return BadRequest();
        }

        ValidarProducto(producto);

        if (!ModelState.IsValid)
        {
            return View(producto);
        }

        try
        {
            bool actualizado =
                await _repositorio.ActualizarAsync(producto);

            if (!actualizado)
            {
                return NotFound();
            }

            TempData["Mensaje"] =
                "Producto actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex)
            when (ex.SqlState == "23505")
        {
            ModelState.AddModelError(
                nameof(producto.Codigo),
                "Ya existe un producto con este código.");

            return View(producto);
        }
    }

    // ELIMINAR PRODUCTO

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _repositorio.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    [HttpPost]
    [ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        try
        {
            bool eliminado =
                await _repositorio.EliminarAsync(id);

            if (!eliminado)
            {
                return NotFound();
            }

            TempData["Mensaje"] =
                "Producto eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex)
            when (ex.SqlState == "23503" ||
                  ex.SqlState == "23001")
        {
            TempData["Error"] =
                "No se puede eliminar este producto porque tiene registros relacionados.";

            return RedirectToAction(nameof(Index));
        }
    }

    // VALIDACIONES DEL PRODUCTO

    private void ValidarProducto(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Codigo))
        {
            ModelState.AddModelError(
                nameof(producto.Codigo),
                "Debe ingresar el código del producto.");
        }

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            ModelState.AddModelError(
                nameof(producto.Nombre),
                "Debe ingresar el nombre del producto.");
        }

        if (producto.Precio <= 0)
        {
            ModelState.AddModelError(
                nameof(producto.Precio),
                "El precio debe ser mayor que cero.");
        }

        if (producto.MaximoReserva <= 0)
        {
            ModelState.AddModelError(
                nameof(producto.MaximoReserva),
                "El máximo de reserva debe ser mayor que cero.");
        }

        if (producto.HorasReserva <= 0)
        {
            ModelState.AddModelError(
                nameof(producto.HorasReserva),
                "Las horas de reserva deben ser mayores que cero.");
        }

        if (producto.ExistenciaMinima < 0)
        {
            ModelState.AddModelError(
                nameof(producto.ExistenciaMinima),
                "La existencia mínima no puede ser negativa.");
        }

        if (string.IsNullOrWhiteSpace(producto.Atributos))
        {
            ModelState.AddModelError(
                nameof(producto.Atributos),
                "Debe ingresar los atributos JSON.");

            return;
        }

        try
        {
            using var documento =
                JsonDocument.Parse(producto.Atributos);

            if (documento.RootElement.ValueKind !=
                JsonValueKind.Object)
            {
                ModelState.AddModelError(
                    nameof(producto.Atributos),
                    "Los atributos deben ser un objeto JSON.");
            }
        }
        catch (JsonException)
        {
            ModelState.AddModelError(
                nameof(producto.Atributos),
                "El formato JSON no es válido.");
        }
    }
}

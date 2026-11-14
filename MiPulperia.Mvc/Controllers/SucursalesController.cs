using Microsoft.AspNetCore.Mvc;
using MiPulperia.Data.Repositorios;
using Npgsql;

namespace MiPulperia.Mvc.Controllers;

public class SucursalesController : Controller
{
    private readonly SucursalRepositorio _repositorio;

    public SucursalesController(SucursalRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index()
    {
        var sucursales = await _repositorio.ListarAsync();

        return View(sucursales);
    }

[HttpGet]
public IActionResult Crear()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Crear(
    MiPulperia.Model.Sucursal sucursal)
{
    if (!ModelState.IsValid)
    {
        return View(sucursal);
    }

    await _repositorio.CrearAsync(sucursal);

    TempData["Mensaje"] = "Sucursal registrada correctamente.";

    return RedirectToAction(nameof(Index));
}

[HttpGet]
public async Task<IActionResult> Editar(int id)
{
    var sucursal = await _repositorio.ObtenerPorIdAsync(id);

    if (sucursal == null)
    {
        return NotFound();
    }

    return View(sucursal);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Editar(
    int id,
    MiPulperia.Model.Sucursal sucursal)
{
    if (id != sucursal.IdSucursal)
    {
        return BadRequest();
    }

    if (!ModelState.IsValid)
    {
        return View(sucursal);
    }

    bool actualizado = await _repositorio.ActualizarAsync(sucursal);

    if (!actualizado)
    {
        return NotFound();
    }

    TempData["Mensaje"] = "Sucursal actualizada correctamente.";

    return RedirectToAction(nameof(Index));
}

[HttpGet]
public async Task<IActionResult> Eliminar(int id)
{
    var sucursal = await _repositorio.ObtenerPorIdAsync(id);

    if (sucursal == null)
    {
        return NotFound();
    }

    return View(sucursal);
}

[HttpPost]
[ActionName("Eliminar")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConfirmarEliminar(int id)
{
    try
    {
        bool eliminado = await _repositorio.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        TempData["Mensaje"] = "Sucursal eliminada correctamente.";
    }
    
    catch (PostgresException ex)
        when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation
        || ex.SqlState == "23001")

    {
        TempData["Error"] =
            "No se puede eliminar esta sucursal porque tiene registros relacionados.";
    }

    return RedirectToAction(nameof(Index));
}

}

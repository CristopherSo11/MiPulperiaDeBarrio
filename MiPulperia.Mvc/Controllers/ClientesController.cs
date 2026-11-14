using Microsoft.AspNetCore.Mvc;
using MiPulperia.Data.Repositorios;

namespace MiPulperia.Mvc.Controllers;

public class ClientesController : Controller
{
    private readonly ClienteRepositorio _repositorio;

    public ClientesController(ClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _repositorio.ListarAsync();

        return View(clientes);
    }

[HttpGet]
public IActionResult Crear()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Crear(
    MiPulperia.Model.Cliente cliente)
{
    if (!ModelState.IsValid)
    {
        return View(cliente);
    }

    await _repositorio.CrearAsync(cliente);

    TempData["Mensaje"] = "Cliente registrado correctamente.";

    return RedirectToAction(nameof(Index));
}

[HttpGet]
public async Task<IActionResult> Editar(int id)
{
    var cliente = await _repositorio.ObtenerPorIdAsync(id);

    if (cliente == null)
    {
        return NotFound();
    }

    return View(cliente);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Editar(
    int id,
    MiPulperia.Model.Cliente cliente)
{
    if (id != cliente.IdCliente)
    {
        return BadRequest();
    }

    if (!ModelState.IsValid)
    {
        return View(cliente);
    }

    bool actualizado = await _repositorio.ActualizarAsync(cliente);

    if (!actualizado)
    {
        return NotFound();
    }

    TempData["Mensaje"] = "Cliente actualizado correctamente.";

    return RedirectToAction(nameof(Index));
}

[HttpGet]
public async Task<IActionResult> Eliminar(int id)
{
    var cliente = await _repositorio.ObtenerPorIdAsync(id);

    if (cliente == null)
    {
        return NotFound();
    }

    return View(cliente);
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

        TempData["Mensaje"] =
            "Cliente eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
    catch (Npgsql.PostgresException ex)
        when (ex.SqlState == "23503" ||
              ex.SqlState == "23001")
    {
        TempData["Error"] =
            "No se puede eliminar este cliente porque tiene reservas relacionadas.";

        return RedirectToAction(nameof(Index));
    }
}
}

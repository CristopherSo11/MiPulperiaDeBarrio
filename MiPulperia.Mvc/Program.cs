using MiPulperia.Data;
using MiPulperia.Data.Repositorios;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration
    .GetConnectionString("PostgreSQL");

if (string.IsNullOrWhiteSpace(cadenaConexion)
    || cadenaConexion == "PENDIENTE")
{
    throw new InvalidOperationException(
        "La conexión PostgreSQL no está configurada.");
}

builder.Services.AddSingleton(
    new ConexionPostgres(cadenaConexion));
    builder.Services.AddScoped<SucursalRepositorio>();
    builder.Services.AddScoped<ClienteRepositorio>();
    builder.Services.AddScoped<ProductoRepositorio>();



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapGet("/probar-conexion", async (
    ConexionPostgres conexion) =>
{
    try
    {
        bool conectado = await conexion.ProbarConexionAsync();

        return conectado
            ? Results.Ok("Conexion con PostgreSQL exitosa.")
            : Results.Problem("PostgreSQL no respondio correctamente.");
    }
    catch (Exception)
    {
        return Results.Problem(
            "No se pudo conectar con PostgreSQL. Revisa la configuracion y los registros del servidor.");
    }
});

app.Run();

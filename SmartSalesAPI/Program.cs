using SmartSales.Business.Services;
using SmartSales.Data.ClienteRepository;
using SmartSales.Data.ProductoRepository;
using SmartSales.Data.UsuarioRepository;
using SmartSales.Business.Interfaces;
using SmartSales.Data.VentaRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Agrega el servicio para ClienteRepository y ClienteServices
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();// Forma correcta: AddScoped<Interfaz, Implementación>()
builder.Services.AddScoped<ClienteServices>(); // Tu servicio se queda igual (a menos que también le hayas creado una interfaz IClienteServices)

// Agrega el servicio para ProductoRepository y ProductServices
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ProductServices>();

// Agrega el servicio para UsuarioRepository y UsuarioServices
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioServices>();

builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<VentaServices>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

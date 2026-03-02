using SmartSales.Business.Services;
using SmartSales.Data.ClienteRepository;
using SmartSales.Data.ProductoRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Agrega el servicio para ClienteRepository y ClienteServices
builder.Services.AddScoped<SmartSales.Business.Interfaces.IClienteRepository, ClienteRepository>();// Forma correcta: AddScoped<Interfaz, Implementación>()
builder.Services.AddScoped<ClienteServices>(); // Tu servicio se queda igual (a menos que también le hayas creado una interfaz IClienteServices)

// Agrega el servicio para ProductoRepository y ProductServices
builder.Services.AddScoped<SmartSales.Business.Interfaces.IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ProductServices>();

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

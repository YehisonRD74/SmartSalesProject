using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using SmartSales.Business.Services;
using SmartSales.Data.ClienteRepository;
using SmartSales.Data.Configuration;
using SmartSales.Data.ProductoRepository;
using SmartSales.Data.UsuarioRepository;
using SmartSales.Data.VentaRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

builder.Services.AddSingleton<IDbConnectionStringProvider>(_ => new DbConnectionStringProvider(defaultConnectionString));
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ClienteServices>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioServices>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<VentaServices>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing. Configure Jwt:Key, Jwt:Issuer and Jwt:Audience in appsettings.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var connectionProvider = scope.ServiceProvider.GetRequiredService<IDbConnectionStringProvider>();
    await connectionProvider.EnsureDatabaseExistsAsync();

    var usuarioServices = scope.ServiceProvider.GetRequiredService<UsuarioServices>();
    var defaultAdmin = await usuarioServices.BuscarUsuarioPorNombreExacto("admin");
    if (defaultAdmin is null)
    {
        await usuarioServices.CrearUsuario(new Usuario
        {
            Nombre = "admin",
            Email = "admin@smartsales.local",
            ContrasenaHash = "123456",
            Rol = "Administrador",
            Estado = true
        });
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Interfaces;

namespace SmartSalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IDbConnectionStringProvider _connectionStringProvider;

        public ConfiguracionController(IDbConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        [HttpGet]
        public IActionResult ObtenerConfiguracion()
        {
            return Ok(new
            {
                apiBaseUrl = $"{Request.Scheme}://{Request.Host}",
                connectionString = _connectionStringProvider.ConnectionString
            });
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarConfiguracion([FromBody] ActualizarConfiguracionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Configuración inválida.");
            }

            if (!string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                var newConnection = request.ConnectionString.Trim();
                await _connectionStringProvider.EnsureDatabaseExistsAsync(newConnection);
                _connectionStringProvider.ConnectionString = newConnection;
            }

            return NoContent();
        }
    }

    public class ActualizarConfiguracionRequest
    {
        public string? ConnectionString { get; set; }
    }
}

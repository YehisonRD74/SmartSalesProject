using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Entidades;
using SmartSales.Business.Services;
using SmartSales.Data.ClienteRepository;
using SmartSalesAPI.DTO.Cliente;
using SmartSalesAPI.Mappers;

namespace SmartSalesAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteServices _clienteServices;
        public ClienteController(ClienteServices clienteServices)
        {
            _clienteServices = clienteServices;
        }
        
        //Crear Cliente
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDTO cliente)
        {
            var clienteEntity = ClienteMapper.ToEntity(cliente);
            if (string.IsNullOrEmpty(clienteEntity.Nombre) || string.IsNullOrEmpty(clienteEntity.Telefono))
            {
                return BadRequest("Debes de llenar estos campos");
            }
            await _clienteServices.CrearCliente(clienteEntity);
            return StatusCode(201, new { message = "Cliente creado exitosamente.", data = ClienteMapper.ToDTO(clienteEntity)});
        }

        //Modificar cliente
        [HttpPut("{id}")] 
        public async Task<IActionResult> ModificarCliente([FromRoute] int id, [FromBody] ModificarClienteDTO cliente)
        {
            if (id != cliente.Id) return BadRequest("El ID de la ruta no coincide con el del cuerpo.");

            // AQUÍ VA TU VALIDACIÓN ESTRELLA:
            var existing = await _clienteServices.BuscarClientePorID(id);
            if (existing == null)
            {
                return NotFound("No se encontró el cliente para modificar"); // NotFound (404) es más correcto aquí que BadRequest
            }

            var clienteEntity = ClienteMapper.ToEntity(cliente);

            await _clienteServices.ModificarCliente(clienteEntity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de cliente no válido");
            }
            await _clienteServices.EliminarCliente(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> MostrarClientes()
        {
            var existing = await _clienteServices.MostrarClientes();
            var clienteDTO = ClienteMapper.ToDTOList(existing);
            return Ok(clienteDTO);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarClientePorID([FromRoute] int id)
        {
            if (id <= 0 || id == null)
            {
                return BadRequest("ID de cliente no válido");
            }
            var cliente = await _clienteServices.BuscarClientePorID(id);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }
            var clienteDTO = ClienteMapper.ToDTO(cliente);
            return Ok(clienteDTO);
        }

        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarClientePorNombre([FromQuery] string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest();
            }
            var existing = await _clienteServices.BuscarClientePorNombre(nombre);
            if (existing == null)
            {
                return NotFound("Cliente no encontrado");
            }
            var clienteDto = ClienteMapper.ToDTO(existing);
            return Ok(clienteDto);
        }

    }
}

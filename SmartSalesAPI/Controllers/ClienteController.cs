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
    public class ClienteController : Controller
    {
        private readonly ClienteServices _clienteServices;
        public ClienteController()
        {
            _clienteServices = new ClienteServices(new ClienteRepository());
        }

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDTO cliente)
        {
            var clienteEntity = ClienteMapper.ToEntity(cliente);
            if (string.IsNullOrEmpty(clienteEntity.Nombre) || string.IsNullOrEmpty(clienteEntity.Email) || string.IsNullOrEmpty(clienteEntity.Telefono))
            {
                return BadRequest("No se pudo crear el cliente");
            }
            await _clienteServices.CrearCliente(clienteEntity);
            return Created();
        }

        [HttpPatch]
        public async Task<IActionResult> ModificarCliente([FromBody] ModificarClienteDTO cliente)
        {
            var clienteEntity = ClienteMapper.ToEntity(cliente);
            if (clienteEntity == null)
            {
                return BadRequest("No se pudo modificar el cliente");
            }
            else if (clienteEntity.IdCliente == null || clienteEntity.IdCliente <= 0)
            {
                return BadRequest("ID de cliente no válido");
            }
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

        [HttpGet("MostrarClientes")]
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

        [HttpGet]
        public async Task<IActionResult> BuscarClientePorNombre([FromQuery] string nombre)
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

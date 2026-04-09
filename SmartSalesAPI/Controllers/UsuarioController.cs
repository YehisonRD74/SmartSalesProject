using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.Usuario;
using SmartSalesAPI.Mappers;

namespace SmartSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioServices _usuarioServices;
        public UsuarioController(UsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO crearUsuario)
        {
            var usuarioEntity = UsuarioMapper.ToUsuario(crearUsuario);
            if (string.IsNullOrEmpty(usuarioEntity.Nombre))
            {
                return BadRequest("Debes de llenar este campo");
            }
            await _usuarioServices.CrearUsuario(usuarioEntity);
            return StatusCode(201, new { message = "Usuario creado exitosamente.", data = UsuarioMapper.ToResponseDTO(usuarioEntity) });
        }

        [HttpPut("Modificar/{id}")]
        public async Task<IActionResult> ModificarUsuario([FromRoute] int id, [FromBody] ModificarUsuarioDTO usuarioDTO)
        {
            if (id != usuarioDTO.IdUsuario) return BadRequest("El id del usuario no coincide");
            var usuarioEntity = UsuarioMapper.ToUsuario(usuarioDTO);
            await _usuarioServices.ModificarUsuario(usuarioEntity);
            return NoContent();
        }

        [HttpPut("CambiarEstado/{id}")]
        public async Task<IActionResult> CambiarEstadoUsuario([FromRoute] int id, [FromBody] bool estado)
        {
            await _usuarioServices.CambiarEstadoUsuario(id, estado);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> MostrarUsuarios()
        {
            var usuariosEntity = await _usuarioServices.MostrarUsuarios();
            if (usuariosEntity == null || !usuariosEntity.Any())
            {
                return NotFound("No se encontraron usuarios registrados.");
            }
            var usuariosDTO = UsuarioMapper.ToDTOList(usuariosEntity);
            return Ok(usuariosDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscaUsuarioPorID ([FromRoute] int id)
        {
            var usuarioEntity = await _usuarioServices.BuscarUsuarioPorID(id);
            if (usuarioEntity == null) return NotFound("Usuario no encontrado.");

            var usuario = UsuarioMapper.ToResponseDTO(usuarioEntity);
            return Ok(usuario);
        }

        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarUsuarioPorNombre([FromQuery] string? nombre)
        {
            
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            var usuarioEntity = await _usuarioServices.BuscarUsuarioPorNombre(nombre);
            if (usuarioEntity == null)
            {
                return NotFound("No se encontró ningún usuario con ese nombre.");
            }
            var usuario = UsuarioMapper.ToDTOList(usuarioEntity);
            return Ok(usuario);
        }
    }
}

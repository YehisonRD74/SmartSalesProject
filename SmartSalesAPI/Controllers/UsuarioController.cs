using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Interfaces;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.Usuario;
using SmartSalesAPI.Mappers;

namespace SmartSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UsuarioServices _usuarioServices;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioServices 
        }

        [HttpPost]
        public IActionResult CrearUsuario([FromBody]CrearUsuarioDTO crearUsuario)
        {
            var usuarioEntity = UsuarioMapper.ToUsuario(crearUsuario);
            
            return ();
        }
    }
}

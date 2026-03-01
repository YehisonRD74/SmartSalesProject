using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.Producto;
using SmartSalesAPI.Mappers;
using SmartSales.Data.ProductoRepository;

namespace SmartSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : Controller
    {
        private readonly ProductServices _productServices;
        public ProductoController()
        {
            _productServices = new ProductServices(new ProductoRepository());
        }
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] CrearProductoDTO crearProductoDTO)
        {
            var producto = ProductoMapper.ToProducto(crearProductoDTO);
            await _productServices.CrearProductoAsync(producto);
            return CreatedAtAction(nameof(BuscarProductoPorId), new { id = producto.IdProducto }, new { message = "Producto creado exitosamente.", data = ProductoMapper.ToResponderProductoDTO(producto) });
        }

        [HttpPut]
        public async Task<IActionResult> ModificarProducto([FromBody] ModificarProductoDTO modificarProductoDTO)
        {

            if (modificarProductoDTO.Id == 0)
            {
                return BadRequest(new { message = "El ID del producto es requerido." });
            }
            var producto = ProductoMapper.ToProducto(modificarProductoDTO);
            await _productServices.ModificarProductoAsync(producto);
            return Ok( "Producto modificado exitosamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del producto es requerido." });
            }
            await _productServices.EliminarProductoAsync(id);
            return Ok("Producto eliminado exitosamente.");
        }

        [HttpGet]
        public async Task<IActionResult> MostrarProductos()
        {
            var productos = ProductoMapper.ToDTOList(await _productServices.MostrarProductosAsync());
            return Ok(new { message = "Productos obtenidos exitosamente.", data = productos });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del producto es requerido." });
            }
            var producto = ProductoMapper.ToResponderProductoDTO(await _productServices.BuscarProductoPorIDAsync(id));
            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
            return Ok(new { message = "Producto obtenido exitosamente.", data = producto });
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorNombre(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest(new { message = "El nombre del producto es requerido." });
            }                               
            var producto = ProductoMapper.ToResponderProductoDTO(await _productServices.BuscarProductoPorNombreAsync(nombre));
            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
            return Ok(new { message = "Producto obtenido exitosamente.", data = producto });
        }
    }
}

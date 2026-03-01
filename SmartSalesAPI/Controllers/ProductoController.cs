using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.Producto;
using SmartSalesAPI.Mappers;
using SmartSales.Data.ProductoRepository;

namespace SmartSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductServices _productServices;
        public ProductoController(ProductServices productServices)
        {
            _productServices = productServices;
        }

        //Crear Productos
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] CrearProductoDTO crearProductoDTO)
        {
            if (string.IsNullOrEmpty(crearProductoDTO.Nombre))
            {
                return BadRequest("El campo de nombre no puede estar vacio.");
            } else if (crearProductoDTO.Precio <= 0)
            {
                return BadRequest("El valor ingresado en el campo de precio es invalido.");
            } else if (crearProductoDTO.Stock <= 0)
            {
                return BadRequest("La cantidad ingresada en el campo de stock es invalido.");
            }
            var producto = ProductoMapper.ToProducto(crearProductoDTO);
            await _productServices.CrearProductoAsync(producto);
            return StatusCode(201, new
            {
                message = "Producto creado exitosamente.",
                data = ProductoMapper.ToResponderProductoDTO(producto)
            });
        }

        //Modificar Productos
        [HttpPut]
        public async Task<IActionResult> ModificarProducto([FromBody] ModificarProductoDTO modificarProductoDTO)
        {
            if (modificarProductoDTO.Id == 0 || modificarProductoDTO.Id <= 0)
            {
                return BadRequest("El ID del producto es requerido.");
            }
            var producto = ProductoMapper.ToProducto(modificarProductoDTO);
            await _productServices.ModificarProductoAsync(producto);
            return Ok("Producto modificado exitosamente.");
        }

        //Eliminar produtos
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

        //Mostrar todos los produtos
        [HttpGet]
        public async Task<IActionResult> MostrarProductos()
        {
            var productos = ProductoMapper.ToDTOList(await _productServices.MostrarProductosAsync());
            return Ok(new { message = "Productos obtenidos exitosamente.", data = productos });
        }

        //Buscar producto por id
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarProductoPorId([FromRoute]int id)
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

        //Buscar producto por su nombre
        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarProductoPorNombre([FromQuery]string? nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest(new { message = "El nombre del producto es requerido." });
            }                               
            var productos = ProductoMapper.ToDTOList(await _productServices.BuscarProductoPorNombreAsync(nombre));

            // 3. Verificamos si la lista está vacía
            if (productos == null || !productos.Any())
            {
                return NotFound($"No se encontraron productos que contengan: '{nombre}'");
            }
            if (productos == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
            return Ok(new { message = "Producto obtenido exitosamente.", data = productos });
        }
    }
}

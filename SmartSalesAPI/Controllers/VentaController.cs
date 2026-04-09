using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSales.Business.Services;
using SmartSalesAPI.DTO.DetallesVenta;
using SmartSalesAPI.DTO.Venta;
using SmartSalesAPI.Mappers;

namespace SmartSalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : Controller
    {
        private readonly VentaServices _ventaServices;
        public VentaController(VentaServices ventaServices)
        {
            this._ventaServices = ventaServices;
        }

        [HttpGet]
        public async Task<ActionResult> MostraVentas()
        {
            var ventas = await _ventaServices.MostrarVentas();
            if (ventas == null || !ventas.Any())
            {
                return NotFound("No se encontraron ventas registradas.");
            }

            return Ok(VentaMapper.MapToMostrarDTO(ventas));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> BuscarVentaPorId([FromRoute] int id)
        {
            var venta = await _ventaServices.BuscarVentaPorId(id);
            if (venta == null)
            {
                return NotFound("No se encontró la venta.");
            }

            return Ok(VentaMapper.MapToMostrarDTO(venta));
        }

        [HttpPost]
        public async Task<ActionResult> CrearVenta([FromBody] CrearVentaDTO crearVentaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var VentaEntity = VentaMapper.MapToEntity(crearVentaDTO);
            VentaEntity.CalcularTotal();

            await _ventaServices.CrearVentaCompleta(VentaEntity);

            return StatusCode(201, new { message = "Venta creada exitosamente", data = VentaMapper.MapToDTO(VentaEntity)});
        }


        [HttpGet("buscar/{id}")]
        public async Task<ActionResult> BuscarDetalleVentaPorID([FromRoute] int id)
        {
            var Detalles = await _ventaServices.MostrarDetallesPorVenta(id);
            if (Detalles == null)
            {
                return NotFound("No se encontró Los detalles de la venta");
            }

            return Ok(VentaMapper.MapToMostrarDTO(Detalles));
        }

        
    }
}

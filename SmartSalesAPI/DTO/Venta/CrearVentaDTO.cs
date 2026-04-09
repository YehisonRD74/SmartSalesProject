using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.DetallesVenta;

namespace SmartSalesAPI.DTO.Venta
{
    public class CrearVentaDTO
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        
        public List<CrearDetalleVentaDTO> Detalles { get; set; } = new List<CrearDetalleVentaDTO>();
    }
}

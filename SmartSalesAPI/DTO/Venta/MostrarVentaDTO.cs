using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.DetallesVenta;

namespace SmartSalesAPI.DTO.Venta;

public class MostrarVentaDTO
{
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }
    public string? NombreCliente { get; set; } 
    public int IdUsuario { get; set; }
    public string? NombreVendedor { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public List<MostrarDetalleVentaDTO> Detalles { get; set; } = new List<MostrarDetalleVentaDTO>();
}

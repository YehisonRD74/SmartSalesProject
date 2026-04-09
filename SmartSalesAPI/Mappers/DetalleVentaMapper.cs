using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.DetallesVenta;

namespace SmartSalesAPI.Mappers
{
    public class DetalleVentaMapper
    {
        public static List<DetalleVenta> MapToEntityList(List<CrearDetalleVentaDTO> detalleVentaDTOs)
        {
            return detalleVentaDTOs.Select(detalleVentaDTO => new DetalleVenta
            {
                IdProducto = detalleVentaDTO.IdProducto,
                Cantidad = detalleVentaDTO.Cantidad,
                PrecioUnitario = detalleVentaDTO.PrecioUnitario,
                Subtotal = detalleVentaDTO.Subtotal
            }).ToList();
        }

        public static List<MostrarDetalleVentaDTO> MapToDTOList(List<DetalleVenta> detalleVentas)
        {
            return detalleVentas.Select(detalleVenta => new MostrarDetalleVentaDTO
            {
                IdProducto = detalleVenta.IdProducto,
                Cantidad = detalleVenta.Cantidad,
                PrecioUnitario = detalleVenta.PrecioUnitario,
                Subtotal = detalleVenta.Subtotal
            }).ToList();
        }
    }
}

          
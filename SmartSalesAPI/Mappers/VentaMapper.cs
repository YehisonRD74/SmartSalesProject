using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.DetallesVenta;
using SmartSalesAPI.DTO.Venta;

namespace SmartSalesAPI.Mappers
{
    public class VentaMapper
    {
        public static CrearVentaDTO MapToDTO(Venta venta)
        {
            return new CrearVentaDTO
            {
                IdCliente = venta.IdCliente,
                IdUsuario = venta.IdUsuario,
                
            };
        }

        public static Venta MapToEntity(CrearVentaDTO ventaDTO)
        {
            // 1. Creamos la venta base y preparamos la lista vacía
            var venta = new Venta
            {
                IdCliente = ventaDTO.IdCliente,
                IdUsuario = ventaDTO.IdUsuario,
                Detalles = new List<DetalleVenta>() // ¡Crucial: Inicializamos la lista!
            };

            // 2. Pasamos los productos del DTO a la Entidad
            if (ventaDTO.Detalles != null)
            {
                foreach (var detalleDTO in ventaDTO.Detalles)
                {
                    venta.Detalles.Add(new DetalleVenta
                    { 
                        IdProducto = detalleDTO.IdProducto,
                        Cantidad = detalleDTO.Cantidad,
                        PrecioUnitario = detalleDTO.PrecioUnitario,
                        Subtotal = detalleDTO.Subtotal
                    });
                }
            }

            return venta;
        }

        public static List<MostrarVentaDTO> MapToMostrarDTO(List<Venta> ventas)
        {
            var mostrarVentas = new List<MostrarVentaDTO>();
            foreach (var venta in ventas)
            {
                mostrarVentas.Add(new MostrarVentaDTO
                {
                    IdVenta = venta.IdVenta,
                    IdCliente = venta.IdCliente,
                    IdUsuario = venta.IdUsuario,
                    Total = venta.Total
                });
            }
            return mostrarVentas;
        }

        public static MostrarVentaDTO MapToMostrarDTO(Venta venta)
        {
            // 1. Mapeamos los datos generales de la Venta
            var dto = new MostrarVentaDTO
            {
                IdVenta = venta.IdVenta,
                IdCliente = venta.IdCliente,
                NombreCliente = venta.NombreCliente,   // ¡Aprovechamos el JOIN de SQL!
                IdUsuario = venta.IdUsuario,
                NombreVendedor = venta.NombreVendedor, // ¡Aprovechamos el JOIN de SQL!
                Fecha = venta.Fecha,
                Total = venta.Total,

                // Inicializamos la lista de salida vacía
                Detalles = new List<MostrarDetalleVentaDTO>()
            };

            // 2. Mapeamos la lista de productos (De Entidad a DTO)
            if (venta.Detalles != null)
            {
                foreach (var detalle in venta.Detalles)
                {
                    dto.Detalles.Add(new MostrarDetalleVentaDTO
                    {
                        IdDetalle = detalle.IdDetalle,
                        IdProducto = detalle.IdProducto,
                        NombreProducto = detalle.NombreProducto, // ¡El nombre del producto!
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal
                    });
                }
            }

            return dto;
        }
    }
}

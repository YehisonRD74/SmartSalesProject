using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.Producto;

namespace SmartSalesAPI.Mappers
{
    public class ProductoMapper
    {
        public static ResponderProductoDTO ToResponderProductoDTO(Producto producto)
        {
            return new ResponderProductoDTO
            {
                Id = producto.IdProducto,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }
    
        public static Producto ToProducto(CrearProductoDTO crearProductoDTO)
        {
            return new Producto
            {
                Nombre = crearProductoDTO.Nombre,
                Precio = crearProductoDTO.Precio,
                Stock = crearProductoDTO.Stock
            };
        }
        public static Producto ToProducto(ModificarProductoDTO modificarProductoDTO)
        {
            return new Producto
            {
                IdProducto = modificarProductoDTO.Id,
                Nombre = modificarProductoDTO.Nombre,
                Precio = modificarProductoDTO.Precio,
                Stock = modificarProductoDTO.Stock
            };
        }

        public static List<ResponderProductoDTO> ToDTOList(List<Producto> productos)
        {
            return productos.Select(p => ToResponderProductoDTO(p)).ToList();
        }
    
    }
}


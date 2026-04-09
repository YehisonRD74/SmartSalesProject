namespace SmartSalesAPI.DTO.DetallesVenta
{
    public class CrearDetalleVentaDTO
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

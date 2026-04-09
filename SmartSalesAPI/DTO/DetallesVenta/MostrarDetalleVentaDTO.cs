namespace SmartSalesAPI.DTO.DetallesVenta
{
    public class MostrarDetalleVentaDTO
    {
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; } = null;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

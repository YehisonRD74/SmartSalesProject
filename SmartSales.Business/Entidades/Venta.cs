using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string? NombreCliente { get; set; } // Propiedad del JOIN
        public int IdUsuario { get; set; }
        public string? NombreVendedor { get; set; } // Propiedad del JOIN
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; } = 0;
        // Relaciones del diagrama
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        public decimal CalcularTotal()
        {
            decimal acumulado = 0;
            foreach (var detalle in Detalles)
            {
                acumulado += detalle.CalcularSubtotal();
            }
            this.Total = acumulado;
            return this.Total;
        }

        public void AgregarProducto(Producto producto, int cantidad) { /* Lógica */ }
        public void EliminarProducto(int idProducto) { /* Lógica */ }
    }
}

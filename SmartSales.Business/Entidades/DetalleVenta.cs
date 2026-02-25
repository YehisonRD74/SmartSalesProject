using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class DetalleVenta
    {
        public int IdDetalle { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public decimal CalcularSubtotal()
        {
            this.Subtotal = Cantidad * PrecioUnitario;
            return this.Subtotal;
        }
    }
}

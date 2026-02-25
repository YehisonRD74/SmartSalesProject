using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public decimal Total { get; set; }

        public void GenerarPDF() { /* Lógica de exportación */ }
        public void EnviarPorEmail() { /* Lógica de envío */ }
    }
}

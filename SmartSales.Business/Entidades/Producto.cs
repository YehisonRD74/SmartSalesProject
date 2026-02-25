using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public void ActualizarStock(int cantidad)
        {
            this.Stock += cantidad;
        }

        public bool VerificarDisponibilidad(int cantidad)
        {
            return this.Stock >= cantidad;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class Cliente
    {
        public int? IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }


        public Cliente BuscarPorEmail(string email)
        {
            // Lógica de búsqueda
            return this;
        }
    }
}

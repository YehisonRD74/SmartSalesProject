using SmartSales.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Interfaces
{
    public interface IClienteRepository
    {
        public void CrearCliente();
        public void ActualizarCliente();
        public void EliminarCliente();
        public List<Cliente> MostrarClientes();

    }
}

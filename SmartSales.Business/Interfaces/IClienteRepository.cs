using SmartSales.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Interfaces
{
    public interface IClienteRepository
    {
        Task CrearClienteAsync(Cliente cliente);
        Task ActualizarClienteAsync(Cliente cliente);
        Task EliminarClienteAsync(int id);
        Task<List<Cliente>> MostrarClientesAsync();
        Task<Cliente> BuscarClientePorNombreAsync(string nombre);
        Task<Cliente> BuscarClientePorIDAsync(int id);
    }
}

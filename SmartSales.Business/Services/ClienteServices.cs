using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Services
{
    public class ClienteServices
    {
        private readonly IClienteRepository _clienteRepository;
        public ClienteServices(IClienteRepository clienteRepository)
        {
            this._clienteRepository = clienteRepository;
        }

        public async Task CrearCliente(Cliente cliente)
        {
            await _clienteRepository.CrearClienteAsync(cliente);
        }
        public async Task ModificarCliente(Cliente cliente)
        {
            await _clienteRepository.ActualizarClienteAsync(cliente);
        }
        public async Task EliminarCliente(int id)
        {
            await _clienteRepository.EliminarClienteAsync(id);
        }
        public async Task<List<Cliente>> MostrarClientes()
        {
            return await _clienteRepository.MostrarClientesAsync();
        }
        public async Task<Cliente> BuscarClientePorNombre(string nombre)
        {
            return await _clienteRepository.BuscarClientePorNombreAsync(nombre);
        }
        public async Task<Cliente> BuscarClientePorID(int id)
        {
            return await _clienteRepository.BuscarClientePorIDAsync(id);
        }
    }
}

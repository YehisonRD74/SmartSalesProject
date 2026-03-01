using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Services
{
    public class ProductServices
    {
        private readonly IProductoRepository _productoRepository;
        public ProductServices(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task CrearProductoAsync(Producto producto)
        {
            await _productoRepository.CrearProductoAsync(producto);
        }
        public async Task ModificarProductoAsync(Producto producto)
        {
            await _productoRepository.ModificarProductoAsync(producto);
        }
        public async Task EliminarProductoAsync(int id)
        {
            await _productoRepository.EliminarProductoAsync(id);
        }
        public async Task<List<Producto>> MostrarProductosAsync()
        {
            return await _productoRepository.MostrarProductosAsync();
        }
        public async Task<List<Producto>> BuscarProductoPorNombreAsync(string nombre)
        {
            return await _productoRepository.BuscarProductoPorNombreAsync(nombre);
        }
        public async Task<Producto> BuscarProductoPorIDAsync(int id)
        {
            return await _productoRepository.BuscarProductoPorIDAsync(id);
        }
    }
}

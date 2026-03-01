using SmartSales.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Interfaces
{
    public interface IProductoRepository
    {
        public Task CrearProductoAsync(Producto producto);
        public Task ModificarProductoAsync(Producto producto);
        public Task EliminarProductoAsync(int id);
        public Task<List<Producto>> MostrarProductosAsync();
        public Task<List<Producto>> BuscarProductoPorNombreAsync(string nombre);
        public Task<Producto> BuscarProductoPorIDAsync(int id);

    }
}

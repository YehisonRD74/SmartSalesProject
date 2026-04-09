using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSales.Business.Entidades;

namespace SmartSales.Business.Interfaces
{
    public interface IVentaRepository
    {
        Task<int> CrearVentaCompletaAsync(Venta venta);
        Task<List<Venta>> MostrarVentasAsync();
        Task<Venta> BuscarVentaPorIdAsync(int id);
        Task<Venta> MostrarDetallesPorVentaAsync(int idVenta);
    }
}

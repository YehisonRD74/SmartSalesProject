using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Services
{
    public class VentaServices
    {
        private readonly IVentaRepository ventaRepository;
        public VentaServices(IVentaRepository ventaRepository)
        {
            this.ventaRepository = ventaRepository;
        }

        public async Task<int> CrearVentaCompleta(Venta venta)
        {
            return await ventaRepository.CrearVentaCompletaAsync(venta);
        }

        public async Task<List<Venta>> MostrarVentas()
        {
            return await ventaRepository.MostrarVentasAsync();
        }

        public async Task<Venta> BuscarVentaPorId(int id)
        {
            return await ventaRepository.BuscarVentaPorIdAsync(id);
        }

        public async Task<Venta> MostrarDetallesPorVenta(int idVenta)
        {
            return await ventaRepository.MostrarDetallesPorVentaAsync(idVenta);

        }
    }
}

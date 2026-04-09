using Microsoft.Data.SqlClient;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using SmartSales.Business.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSales.Data.VentaRepository
{
    public class VentaRepository : IVentaRepository
    {
        private readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        public async Task<int> CrearVentaCompletaAsync(Venta venta)
        {
            using (var cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                // Iniciamos una Transacción: Si algo falla con un producto, no se guarda la venta a medias.
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        int idVentaGenerado = 0;

                        // 1. GUARDAR EL PADRE (La Venta)
                        using (var cmd = new SqlCommand("Sp_CreateVenta", cnn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdCliente", venta.IdCliente);
                            cmd.Parameters.AddWithValue("@IdUsuario", venta.IdUsuario);
                            cmd.Parameters.AddWithValue("@Total", venta.Total);

                            // ATENCIÓN: Usamos ExecuteScalar para atrapar el ID que SQL acaba de generar
                            var result = await cmd.ExecuteScalarAsync();
                            idVentaGenerado = Convert.ToInt32(result);
                        }

                        // 2. GUARDAR LOS HIJOS (La lista de productos)
                        foreach (var detalle in venta.Detalles)
                        {
                            using (var cmd = new SqlCommand("Sp_CreateDetalleVenta", cnn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // ¡Le pasamos el ID de la venta que acabamos de crear arriba!
                                cmd.Parameters.AddWithValue("@NuevoIdVenta", idVentaGenerado);

                                cmd.Parameters.AddWithValue("@IdProducto", detalle.IdProducto);
                                cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                                cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal); // (Corregí un pequeño typo que tenías aquí)

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // Si llegamos hasta aquí, todo salió perfecto. Confirmamos el guardado en SQL.
                        transaction.Commit();

                        return idVentaGenerado; // Devolvemos el número de factura exitoso
                    }
                    catch (Exception ex)
                    {
                        // Si hubo un error, cancelamos todo (Rollback) para no dejar datos huérfanos
                        transaction.Rollback();
                        throw new Exception("Error al procesar la venta: " + ex.Message);
                    }
                }
            }
        }

        public async Task<List<Venta>> MostrarVentasAsync()
        {
            var ventas = new List<Venta>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_GetAllVentas", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var venta = new Venta
                            {
                                // 3. Leemos exactamente los nombres de las columnas que devuelve tu SP
                                IdVenta = reader.GetInt32(reader.GetOrdinal("IdVenta")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),

                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                // Esta columna viene del INNER JOIN con Cliente
                                NombreCliente = reader.GetString(reader.GetOrdinal("NombreCliente")),

                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                // Esta columna viene del INNER JOIN con Usuario
                                NombreVendedor = reader.GetString(reader.GetOrdinal("NombreVendedor"))
                            };

                            ventas.Add(venta);
                        }
                    }
                }
            }
            return ventas;
        }

        public async Task<Venta> BuscarVentaPorIdAsync(int id)
        {
            Venta venta = null;
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_GetVentaById", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            venta = new Venta
                            {
                                // 3. Leemos exactamente los nombres de las columnas que devuelve tu SP
                                IdVenta = reader.GetInt32(reader.GetOrdinal("IdVenta")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),

                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                // Esta columna viene del INNER JOIN con Cliente
                                NombreCliente = reader.GetString(reader.GetOrdinal("NombreCliente")),

                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                // Esta columna viene del INNER JOIN con Usuario
                                NombreVendedor = reader.GetString(reader.GetOrdinal("NombreVendedor"))
                            };

                        }
                    }
                }
            }
            return venta;
        }

        // 1. CAMBIO IMPORTANTE: Ahora devolvemos 'Task<Venta>'
        public async Task<Venta> MostrarDetallesPorVentaAsync(int idVenta)
        {
            var venta = new Venta();
            var detalles = new List<DetalleVenta>();

            // Usamos esta variable para saber si ya leímos al cliente y al vendedor
            bool encabezadoYaLeido = false;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_GetDetallesByVenta", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        // El while recorre todas las filas que devuelve el SQL
                        while (await reader.ReadAsync())
                        {
                            // 2. Leemos los datos generales SOLO en la primera vuelta
                            if (!encabezadoYaLeido)
                            {
                                venta.IdVenta = idVenta; // Ya lo tienes del parámetro
                                venta.Total = reader.GetDecimal(reader.GetOrdinal("Total"));
                                venta.NombreCliente = reader.GetString(reader.GetOrdinal("NombreCliente"));
                                venta.NombreVendedor = reader.GetString(reader.GetOrdinal("NombreVendedor"));

                                // OJO: Asegúrate de que la Fecha viene en tu Stored Procedure. 
                                // En el script SQL que me pasaste antes, NO estaba la Fecha en el SELECT.
                                // Si no la pusiste en SQL, comenta la línea de abajo o dará error:
                                // venta.Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha"));

                                encabezadoYaLeido = true; // Cambiamos la bandera para que no lo vuelva a leer
                            }

                            // 3. Leemos el producto y lo agregamos a la lista en CADA vuelta
                            var detalle = new DetalleVenta
                            {
                                IdDetalle = reader.GetInt32(reader.GetOrdinal("IdDetalle")),
                                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                                // Recuerda atrapar el NombreProducto para que no salga en blanco
                                NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                                Subtotal = reader.GetDecimal(reader.GetOrdinal("SubTotal"))
                            };

                            detalles.Add(detalle);
                        }
                    }
                }
            }

            // 4. Metemos la lista de detalles dentro de nuestro objeto Venta
            venta.Detalles = detalles;

            // 5. ¡Devolvemos el ticket completo!
            return venta;
        }
    }
}

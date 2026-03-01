using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;

namespace SmartSales.Data.ProductoRepository
{
    public class ProductoRepository : IProductoRepository
    {
        // Usamos la misma cadena de conexión de tus otras clases
        private readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        public async Task CrearProductoAsync(Producto producto)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_CreateProduct", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@Nombre", (object)producto.Nombre ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", (object)producto.Precio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stock", (object)producto.Stock ?? DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ModificarProductoAsync(Producto producto)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_UpdateProduct", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@Nombre", (object)producto.Nombre ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", (object)producto.Precio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stock", (object)producto.Stock ?? DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EliminarProductoAsync(int id)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_DeleteProduct", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@IdProducto", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Producto>> MostrarProductosAsync()
        {
            var list = new List<Producto>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetAllProducts", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(MapearProducto(reader));
                        }
                    }
                }
            }
            return list;
        }

        public async Task<Producto> BuscarProductoPorIDAsync(int id)
        {
            Producto producto = null;
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetProductById", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@IdProducto", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        // Usamos 'if' porque solo esperamos un resultado
                        if (await reader.ReadAsync())
                        {
                            producto = MapearProducto(reader);
                        }
                    }
                }
            }
            return producto; // Devolverá null si no lo encuentra
        }

        public async Task<Producto> BuscarProductoPorNombreAsync(string nombre)
        {
            Producto producto = null;
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetProductByName", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@Nombre", nombre);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            producto = MapearProducto(reader);
                        }
                    }
                }
            }
            return producto;
        }

        // --- MÉTODO DE APOYO (DRY: Don't Repeat Yourself) ---
        // Extraemos la lectura a un método privado para no repetir código en Mostrar y Buscar
        private Producto MapearProducto(SqlDataReader reader)
        {
            return new Producto
            {
                IdProducto = reader.IsDBNull(reader.GetOrdinal("IdProducto")) ? null : reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? null : reader.GetDecimal(reader.GetOrdinal("Precio")),
                Stock = reader.IsDBNull(reader.GetOrdinal("Stock")) ? null : reader.GetInt32(reader.GetOrdinal("Stock"))
            };
        }
    }
}

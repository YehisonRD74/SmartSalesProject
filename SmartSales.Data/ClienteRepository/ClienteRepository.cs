using Microsoft.Data.SqlClient;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSales.Data.ClienteRepository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
        public async Task ActualizarClienteAsync(Cliente cliente)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_UpdateCustomer", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Email", cliente.Email);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Cliente> BuscarClientePorIDAsync(int? id)
        {
            var cliente = new Cliente();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetCustomerById", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) { 
                            cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                            };
                        }
                        return cliente;
                    }
                }
            }
        }

        public async Task<List<Cliente>> BuscarClientePorNombreAsync(string nombre)
        {
            List<Cliente> clientes = new List<Cliente>();

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetCustomersByName", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@Nombre", nombre);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                            };
                            clientes.Add(cliente);
                        }
                        return clientes;
                    }
                }
            }
        }


        public async Task CrearClienteAsync(Cliente cliente)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_CreateCustomer", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();
                    
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Email", cliente.Email);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EliminarClienteAsync(int id)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_DeleteCustomer", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Cliente>> MostrarClientesAsync()
        {
            var list = new List<Cliente>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_GetAllCustomers", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cnn.OpenAsync();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            var cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                            };
                            list.Add(cliente);
                        }
                        return list;
                    }
                }
            }
        }
    }
}

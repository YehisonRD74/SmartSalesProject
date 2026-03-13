using Microsoft.Data.SqlClient;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System.Data;

namespace SmartSales.Data.UsuarioRepository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        public async Task<Usuario> BuscarUsuarioPorIDAsync(int id)
        {
            var usuario = new Usuario();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd =  new SqlCommand("Sp_GetUserById", cnn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new Usuario()
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Rol = reader.GetString(reader.GetOrdinal("Rol"))
                            };
                        }
                        return usuario;
                    }
                }
            }
        }

        public async Task<Usuario> BuscarUsuarioPorNombreAsync(string nombre)
        {
            var usuario = new Usuario();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("Sp_GetUserByName", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new Usuario()
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Rol = reader.GetString(reader.GetOrdinal("Rol"))
                            };
                        }
                        return usuario;
                    }
                }
            }
        }

        public async Task CambiarEstadoUsuarioAsync(int id, bool estado)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_setUserStatus", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_CreateUser", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.ContrasenaHash);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ModificarUsuarioAsync(Usuario usuario)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_CreateUser", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.ContrasenaHash);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Usuario>> MostrarUsuariosAsync()
        {
            var listaUsuario = new List<Usuario>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("Sp_GetUserByName", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var usuario = new Usuario()
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Rol = reader.GetString(reader.GetOrdinal("Rol"))
                            };
                            listaUsuario.Add(usuario);
                        }
                        return listaUsuario;
                    }
                }
            }
        }
    }
}

using Microsoft.Data.SqlClient;
using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System.Data;

namespace SmartSales.Data.UsuarioRepository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string ConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=SmartSales; Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        // Buscar usuario por ID
        public async Task<Usuario> BuscarUsuarioPorIDAsync(int id)
        {
            Usuario usuario = null;
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd =  new SqlCommand("Sp_GetUserById", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
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

        // Buscar usuario por nombre
        public async Task<List<Usuario>> BuscarUsuarioPorNombreAsync(string nombre)
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("Sp_GetUserByName", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", nombre);
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
                            usuarios.Add(usuario);
                        }
                        return usuarios;
                    }
                }
            }
        }

        // Cambiar estado del usuario (activo/inactivo)
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

        // Crear nuevo usuario
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

        // Modificar usuario existente
        public async Task ModificarUsuarioAsync(Usuario usuario)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Sp_UpdateUser", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.ContrasenaHash);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Mostrar todos los usuarios
        public async Task<List<Usuario>> MostrarUsuariosAsync()
        {
            var listaUsuario = new List<Usuario>();
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                await cnn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("Sp_GetAllUser", cnn))
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
                                Rol = reader.GetString(reader.GetOrdinal("Rol")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado"))
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

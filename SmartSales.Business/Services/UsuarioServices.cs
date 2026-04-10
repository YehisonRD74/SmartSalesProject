using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;

namespace SmartSales.Business.Services
{
    public class UsuarioServices
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioServices(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task CrearUsuario(Usuario usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.ContrasenaHash))
            {
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
            }

            await _usuarioRepository.CrearUsuarioAsync(usuario);
        }

        public async Task ModificarUsuario(Usuario usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.ContrasenaHash) &&
                !usuario.ContrasenaHash.StartsWith("$2"))
            {
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
            }

            await _usuarioRepository.ModificarUsuarioAsync(usuario);
        }

        public Task CambiarEstadoUsuario(int id, bool estado) =>
            _usuarioRepository.CambiarEstadoUsuarioAsync(id, estado);

        public Task<List<Usuario>> MostrarUsuarios() =>
            _usuarioRepository.MostrarUsuariosAsync();

        public Task<List<Usuario>> BuscarUsuarioPorNombre(string nombre) =>
            _usuarioRepository.BuscarUsuarioPorNombreAsync(nombre);

        public Task<Usuario?> BuscarUsuarioPorNombreExacto(string nombre) =>
            _usuarioRepository.BuscarUsuarioPorNombreExactoAsync(nombre);

        public Task<Usuario> BuscarUsuarioPorID(int id) =>
            _usuarioRepository.BuscarUsuarioPorIDAsync(id);

        public Task<Usuario?> BuscarUsuarioPorEmail(string email) =>
            _usuarioRepository.BuscarUsuarioPorEmailAsync(email);
    }
}

using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
            await _usuarioRepository.CrearUsuarioAsync(usuario);
        }
        public async Task ModificarUsuario(Usuario usuario)
        {
            await _usuarioRepository.ModificarUsuarioAsync(usuario);
        }
        public async Task CambiarEstadoUsuario(int id, bool estado)
        {
            await _usuarioRepository.CambiarEstadoUsuarioAsync(id, estado);
        }
        public async Task<List<Usuario>> MostrarUsuarios()
        {
            return await _usuarioRepository.MostrarUsuariosAsync();
        }
        public async Task<Usuario> BuscarUsuarioPorNombre(string nombre)
        {
            return await _usuarioRepository.BuscarUsuarioPorNombreAsync(nombre);
        }
        public async Task<Usuario> BuscarUsuarioPorID(int id)
        {
            return await _usuarioRepository.BuscarUsuarioPorIDAsync(id);
        }

    }
}

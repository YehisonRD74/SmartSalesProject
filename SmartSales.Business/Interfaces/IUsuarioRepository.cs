using SmartSales.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Interfaces
{
    public interface IUsuarioRepository
    {
        public Task CrearUsuarioAsync(Usuario usuario);
        public Task ModificarUsuarioAsync(Usuario usuario);
        public Task CambiarEstadoUsuarioAsync(int id, bool estado);
        public Task<List<Usuario>> MostrarUsuariosAsync();
        public Task<List<Usuario>> BuscarUsuarioPorNombreAsync(string nombre);
        public Task<Usuario?> BuscarUsuarioPorNombreExactoAsync(string nombre);
        public Task<Usuario> BuscarUsuarioPorIDAsync(int id);
        public Task<Usuario?> BuscarUsuarioPorEmailAsync(string email);
    }
}

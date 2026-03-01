using SmartSales.Business.Entidades;
using SmartSales.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Data.UsuarioRepository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public Task<Usuario> BuscarUsuarioPorIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> BuscarUsuarioPorNombreAsync(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task CambiarEstadoUsuarioAsync(int id, bool estado)
        {
            throw new NotImplementedException();
        }

        public Task CrearUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task ModificarUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> MostrarUsuariosAsync()
        {
            throw new NotImplementedException();
        }
    }
}

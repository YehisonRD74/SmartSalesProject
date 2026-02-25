using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSales.Business.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string Rol { get; set; }

        public bool IniciarSesion(string email, string password)
        {
            // Lógica de autenticación
            return true;
        }

        public void CambiarContrasena(string nueva)
        {
            this.ContrasenaHash = nueva;
        }
    }
}

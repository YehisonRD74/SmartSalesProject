using SmartSales.Business.Entidades;
using SmartSalesAPI.DTO.Producto;
using SmartSalesAPI.DTO.Usuario;

namespace SmartSalesAPI.Mappers
{
    public class UsuarioMapper
    {
        public static ResponderUsuarioDTO ToResponseDTO(Usuario usuario)
        {
            return new ResponderUsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Estado = usuario.Estado
                
            };
        }

        public static Usuario ToUsuario(CrearUsuarioDTO crearUsuarioDTO)
        {
            return new Usuario
            {
                Nombre = crearUsuarioDTO.Nombre,
                Email = crearUsuarioDTO.Email,
                ContrasenaHash = crearUsuarioDTO.ContrasenaHash,
                Rol = crearUsuarioDTO.Rol
            };
        }
        public static Usuario ToUsuario(ModificarUsuarioDTO modificarUsuarioDTO)
        {
            return new Usuario
            {   
                IdUsuario = modificarUsuarioDTO.IdUsuario,
                Nombre = modificarUsuarioDTO.Nombre,
                Email = modificarUsuarioDTO.Email,
                ContrasenaHash = modificarUsuarioDTO.ContrasenaHash,
                Rol = modificarUsuarioDTO.Rol
            };
        }

        public static List<ResponderUsuarioDTO> ToDTOList(List<Usuario> usuarios)
        {
            return usuarios.Select(u => ToResponseDTO(u)).ToList();
        }
    }
}

namespace SmartSalesAPI.DTO.Usuario
{
    public class ModificarUsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? ContrasenaHash { get; set; }
        public string? Rol { get; set; }
    }
}

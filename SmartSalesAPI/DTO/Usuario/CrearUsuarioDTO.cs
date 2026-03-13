namespace SmartSalesAPI.DTO.Usuario
{
    public class CrearUsuarioDTO
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? ContrasenaHash { get; set; }
        public string? Rol { get; set; }
    }
}

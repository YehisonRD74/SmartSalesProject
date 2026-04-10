namespace SmartSalesAPI.DTO.Auth
{
    public sealed class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
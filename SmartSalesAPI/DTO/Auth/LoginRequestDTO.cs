namespace SmartSalesAPI.DTO.Auth
{
    public sealed class LoginRequestDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
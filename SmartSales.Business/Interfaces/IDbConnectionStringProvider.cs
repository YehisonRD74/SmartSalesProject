namespace SmartSales.Business.Interfaces
{
    public interface IDbConnectionStringProvider
    {
        string ConnectionString { get; set; }

        Task EnsureDatabaseExistsAsync(string? connectionString = null);
    }
}

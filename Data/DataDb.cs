using Microsoft.EntityFrameworkCore;
using testProd.auth;



public class DataContext : DbContext
{

    private readonly IConfiguration _config;

    public DataContext(IConfiguration config)
    {
        _config = config;
    }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString); // Настройка для использования MSSQL
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired();

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAddOrUpdate();
        });

        base.OnModelCreating(modelBuilder);
    }
}
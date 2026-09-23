using Microsoft.EntityFrameworkCore;
using SalesDashboard.Api.Entities;

namespace SalesDashboard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Ограничения длин
        mb.Entity<Manager>().Property(m => m.FullName).HasMaxLength(120);
        mb.Entity<Manager>().Property(m => m.Team).HasMaxLength(64);
        mb.Entity<Manager>().Property(m => m.Position).HasMaxLength(64);
        mb.Entity<Manager>().Property(m => m.AvatarColor).HasMaxLength(16);

        mb.Entity<Customer>().Property(c => c.Name).HasMaxLength(120);
        mb.Entity<Customer>().Property(c => c.Company).HasMaxLength(200);
        mb.Entity<Customer>().Property(c => c.Segment).HasMaxLength(32);

        mb.Entity<Category>().Property(c => c.Name).HasMaxLength(64);

        mb.Entity<Product>().Property(p => p.Name).HasMaxLength(160);
        mb.Entity<Product>().Property(p => p.BasePrice).HasPrecision(18, 2);
        mb.Entity<Product>().Property(p => p.BaseCost).HasPrecision(18, 2);

        mb.Entity<SaleItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        mb.Entity<SaleItem>().Property(i => i.UnitCost).HasPrecision(18, 2);

        // Индексы под аналитические запросы
        mb.Entity<Sale>().HasIndex(s => s.Date);
        mb.Entity<Sale>().HasIndex(s => new { s.Status, s.Date });
        mb.Entity<Sale>().HasIndex(s => s.ManagerId);
        mb.Entity<SaleItem>().HasIndex(i => i.SaleId);
        mb.Entity<SaleItem>().HasIndex(i => i.ProductId);
        mb.Entity<Product>().HasIndex(p => p.CategoryId);

        // Seed-данные справочников (детерминированные)
        mb.Entity<Category>().HasData(SeedData.Categories);
        mb.Entity<Product>().HasData(SeedData.Products);
        mb.Entity<Manager>().HasData(SeedData.Managers);
        mb.Entity<Customer>().HasData(SeedData.Customers);
    }
}
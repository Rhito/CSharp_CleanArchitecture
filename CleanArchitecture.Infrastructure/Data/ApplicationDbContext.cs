using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CleanArchitecture.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Tắt cảnh báo PendingModelChanges để có thể Update-Database
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // 1. Seed Category trước (vì Product phụ thuộc vào Category)
            modelBuilder.Entity<Category>().HasData(MockData.Categories);

            // 2. Seed Product
            modelBuilder.Entity<Product>().HasData(MockData.Products);

            // 3. Seed Customer
            modelBuilder.Entity<Customer>().HasData(MockData.Customers);

            // 4. Seed Order (Phụ thuộc Customer)
            modelBuilder.Entity<Order>().HasData(MockData.Orders);

            // 5. Seed OrderDetail (Phụ thuộc Order và Product)
            modelBuilder.Entity<OrderDetail>().HasData(MockData.OrderDetails);

            // Cấu hình Fluent API tại đây thay vì dùng Annotation trong Domain
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");

            // Cấu hình cho Product.Price: 18 số, lấy 2 số thập phân
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Cấu hình cho Order.TotalAmount
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Cấu hình cho OrderDetail.UnitPrice
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal(18,2)");

        }
    }
}

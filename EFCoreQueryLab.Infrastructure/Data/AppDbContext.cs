using EFCoreQueryLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryLab.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Price)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Stock)
                    .IsRequired();

                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Products)
                    .HasForeignKey(x => x.CategoryId);

                entity.HasIndex(x => x.CategoryId);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.OrderDate)
                    .IsRequired();

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.Orders)
                    .HasForeignKey(x => x.CustomerId);

                entity.HasIndex(x => x.CustomerId);
                entity.HasIndex(x => x.OrderNumber);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Quantity)
                    .IsRequired();

                entity.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                entity.HasOne(x => x.Order)
                    .WithMany(x => x.OrderItems)
                    .HasForeignKey(x => x.OrderId);

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.OrderItems)
                    .HasForeignKey(x => x.ProductId);

                entity.HasIndex(x => x.OrderId);
                entity.HasIndex(x => x.ProductId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Amount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.PaymentDate)
                    .IsRequired();

                entity.HasOne(x => x.Order)
                    .WithOne(x => x.Payment)
                    .HasForeignKey<Payment>(x => x.OrderId);

                entity.HasIndex(x => x.OrderId)
                    .IsUnique();
            });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Configurando o mapeamento de Order
            modelBuilder.Entity<Order>(builder =>
            {
                builder.ToTable("Orders");
                builder.HasKey(o => o.Id);

                builder.Property(o => o.CustomerId)
                    .IsRequired();

                builder.Property(o => o.Status)
                    .IsRequired()
                    .HasConversion<int>(); // Salva o Enum como número inteiro no banco

                builder.Property(o => o.CreatedAt)
                    .IsRequired();

                // Configurando o relacionamento 1 para N (Order -> OrderItem)
                builder.HasMany(o => o.Items)
                    .WithOne()
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade); // Se o pedido for deletado, os itens também serão

                // Regra Sênior: Ensina ao EF Core a acessar a lista privada backing field "_items"
                var navigation = builder.Metadata.FindNavigation(nameof(Order.Items));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            // 2. Configurando o mapeamento de OrderItem
            modelBuilder.Entity<OrderItem>(builder =>
            {
                builder.ToTable("OrderItems");
                builder.HasKey(i => i.Id);

                builder.Property(i => i.ProductName)
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(i => i.Quantity)
                    .IsRequired();

                builder.Property(i => i.UnitPrice)
                    .IsRequired()
                    .HasColumnType("TEXT"); // SQLite lida melhor com decimais mapeando como TEXT ou REAL
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

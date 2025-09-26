using Microsoft.EntityFrameworkCore;
using Subs.Domain.Models;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Data;

public class SubsDbContext(DbContextOptions<SubsDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>(b =>
        {
            b.HasKey(s => s.Id);

            b.Property(s => s.CreatedAt).IsRequired();
            b.Property(s => s.UpdatedAt).IsRequired();

            b.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            b.HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey("ClientId")
                .IsRequired();

            b.OwnsOne(s => s.Payment, payment =>
            {
                payment.Property(p => p.Method).HasConversion<string>().IsRequired();
                payment.Property(p => p.Frequency).HasConversion<string>().IsRequired();
                payment.Property(p => p.Amount).HasColumnType("numeric(18,2)");

                payment.OwnsOne(p => p.Discount, discount =>
                {
                    discount.Property(d => d.Value).HasColumnType("numeric(18,2)");
                    discount.Property(d => d.Type).HasConversion<string>();
                });

                payment.OwnsOne(p => p.Currency, currency =>
                {
                    currency.Property(c => c.Type).HasMaxLength(50);
                    currency.Property(c => c.Rate).HasColumnType("numeric(18,6)");
                    currency.Property(c => c.Reference);
                });
            });
        });

        modelBuilder.Entity<Client>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            b.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            b.Property(c => c.Email).HasMaxLength(200).IsRequired();
            b.Property(c => c.Phone).HasMaxLength(20);
        });
    }
}
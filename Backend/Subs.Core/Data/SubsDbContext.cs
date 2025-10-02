using Microsoft.EntityFrameworkCore;
using Subs.Domain.Models;
using Subs.Domain.Models.History;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Data;

public class SubsDbContext(DbContextOptions<SubsDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<SubscriptionEventHistory> SubscriptionsEventHistories { get; set; } = null!;
    public DbSet<ClientEventHistory> ClientEventHistories { get; set; } = null!;
    public DbSet<ClientMessage> ClientMessages { get; set; }
    public DbSet<SubscriptionMessage> SubscriptionMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region | Models
        modelBuilder.Entity<Subscription>(b =>
        {
            b.HasKey(s => s.Id);

            b.Property(s => s.CreatedAt).IsRequired();
            b.Property(s => s.UpdatedAt).IsRequired();

            b.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            b.HasOne(s => s.Client)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            b.OwnsOne(s => s.Payment, payment =>
            {
                payment.Property(p => p.Method)
                    .HasConversion<string>()
                    .IsRequired();

                payment.Property(p => p.Frequency)
                    .HasConversion<string>()
                    .IsRequired();

                payment.Property(p => p.Amount)
                    .HasColumnType("numeric(18,2)");

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

            b.Property(c => c.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(c => c.LastName)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(c => c.Email)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(c => c.Phone)
                .HasMaxLength(20);
        });
        #endregion

        #region | History
        modelBuilder.Entity<SubscriptionEventHistory>(b =>
        {
            b.HasKey(e => e.Id);

            b.Property(e => e.CreatedAt)
                .IsRequired();

            b.Property(e => e.Operation)
                .HasConversion<string>()
                .IsRequired();

            b.Property(e => e.Note)
                .HasMaxLength(500);

            b.Property(e => e.StatusAtEvent)
                .HasConversion<string>()
                .IsRequired();

            b.HasOne(e => e.Subscription)
                .WithMany()
                .HasForeignKey(e => e.SubscriptionId)
                .IsRequired();
        });

        modelBuilder.Entity<ClientEventHistory>(b =>
        {
            b.HasKey(e => e.Id);

            b.Property(e => e.CreatedAt)
                .IsRequired();

            b.Property(e => e.Operation)
                .HasConversion<string>()
                .IsRequired();

            b.Property(e => e.Note)
                .HasMaxLength(500);

            b.Property(e => e.StatusAtEvent)
                .HasConversion<string>()
                .IsRequired();

            b.HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .IsRequired();
        });
        #endregion

        #region | Messages
        modelBuilder.Entity<SubscriptionMessage>(b =>
        {
            b.HasKey(m => m.Id);

            b.Property(m => m.SubscriptionId)
                .IsRequired();

            b.Property(m => m.Operation)
                .HasConversion<string>()
                .IsRequired();

            b.Property(m => m.CreatedAt)
                .IsRequired();

            b.Property(m => m.Status)
                .HasConversion<string>()
                .IsRequired();

            b.Property(m => m.ProcessedAt);
        });

        modelBuilder.Entity<ClientMessage>(b =>
        {
            b.HasKey(m => m.Id);

            b.Property(m => m.ClientId)
                .IsRequired();

            b.Property(m => m.ClientJson)
                .IsRequired();

            b.Property(m => m.Operation)
                .HasConversion<string>()
                .IsRequired();

            b.Property(m => m.CreatedAt)
                .IsRequired();

            b.Property(m => m.Status)
                .HasConversion<string>()
                .IsRequired();

            b.Property(m => m.ProcessedAt);
        });
        #endregion
    }
}
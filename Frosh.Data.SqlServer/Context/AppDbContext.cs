using Microsoft.EntityFrameworkCore;
using Frosh.Core.Entities;

namespace Frosh.Data.SqlServer.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ارتباط مشتری و پروژه
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Member)
            .WithMany()
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // ارتباط پروژه و پیش‌فاکتور
        modelBuilder.Entity<Quote>()
            .HasOne(q => q.Project)
            .WithMany()
            .HasForeignKey(q => q.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // ارتباط پیش‌فاکتور و اقلام آن
        modelBuilder.Entity<QuoteItem>()
            .HasOne(qi => qi.Quote)
            .WithMany(q => q.Items)
            .HasForeignKey(qi => qi.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        // ارتباط کالا و اقلام پیش‌فاکتور
        modelBuilder.Entity<QuoteItem>()
            .HasOne(qi => qi.Product)
            .WithMany()
            .HasForeignKey(qi => qi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Microsoft.EntityFrameworkCore;
using PolicyManagement.Models;

namespace PolicyManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Policy> Policies { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<PolicyEnrollment> PolicyEnrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship: User -> PolicyEnrollments
        modelBuilder.Entity<User>()
            .HasMany(u => u.PolicyEnrollments)
            .WithOne(pe => pe.User)
            .HasForeignKey(pe => pe.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship: Policy -> PolicyEnrollments
        modelBuilder.Entity<Policy>()
            .HasMany(p => p.PolicyEnrollments)
            .WithOne(pe => pe.Policy)
            .HasForeignKey(pe => pe.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure unique constraint for User email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure unique constraint for User phone number + country code combination
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.CountryCode, u.PhoneNumber })
            .IsUnique()
            .HasFilter("\"country_code\" IS NOT NULL AND \"phone_number\" IS NOT NULL");

        // Configure unique constraint for Policy code
        modelBuilder.Entity<Policy>()
            .HasIndex(p => p.PolicyCode)
            .IsUnique();
    }
}
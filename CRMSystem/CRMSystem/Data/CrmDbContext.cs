using CRM.Models;
using CRMProject.Models;
using CRMSystem.Models;
using MarketingAutomation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesAutomation.Models;

namespace CRMSystem.Data
{
    // CrmDbContext inherits from IdentityDbContext to support Identity features like roles and users
    public class CrmDbContext : IdentityDbContext<IdentityUser>
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options)
            : base(options)
        {
        }

        // Existing Report table
        public DbSet<Report> Reports { get; set; } = null!;

        // ✅ Add DbSets for other tables based on your SQL structure
        public DbSet<CustomerProfile> CustomerProfile { get; set; } = null!;
        public DbSet<CustomerResponse> CustomerResponse { get; set; } = null!;
        public DbSet<SalesOpportunity> SalesOpportunity { get; set; } = null!;
        public DbSet<SupportTicket> SupportTicket { get; set; } = null!;
        public DbSet<Campaign> Campaign { get; set; } = null!;
        public DbSet<Lead> Leads { get; set; } = null!;
        //public object CustomerProfile { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Remove CustomerProfile → SalesOpportunity relationship
            // Add Lead → SalesOpportunity relationship
            modelBuilder.Entity<SalesOpportunity>()
                .HasOne(s => s.Lead)
                .WithMany()
                .HasForeignKey(s => s.LeadId)
                .OnDelete(DeleteBehavior.SetNull); // Optional: SetNull or Restrict

            modelBuilder.Entity<SalesOpportunity>()
                .Property(s => s.EstimatedValue)
                .HasPrecision(18, 2); // ✅ Fixes the warning

            // ✅ Map to correct table names
            modelBuilder.Entity<SalesOpportunity>().ToTable("SalesOpportunity");
            modelBuilder.Entity<Lead>().ToTable("Lead");
        }
    }
}



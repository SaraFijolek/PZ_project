using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Models;

namespace Pharmacy_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Lek> LEKI { get; set; }
        public DbSet<Klient> KLIENT { get; set; }
        public DbSet<Faktura> FAKTURA { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           
            builder.Entity<Lek>().ToTable("LEKI");
            builder.Entity<Klient>().ToTable("KLIENT");
            builder.Entity<Faktura>().ToTable("FAKTURA");
            builder.Entity<ApplicationUser>().ToTable("PRACOWNICY");

            
            builder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");
            builder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens");
           
            builder.Entity<Faktura>()
                .HasOne(f => f.Klient)
                .WithMany(k => k.Faktury)
                .HasForeignKey(f => f.ID_Klienta)
                .OnDelete(DeleteBehavior.SetNull);

            
            builder.Entity<Faktura>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Faktury)
                .HasForeignKey(f => f.ID_Pracownika)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    }

    



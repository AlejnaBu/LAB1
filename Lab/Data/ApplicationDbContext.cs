using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Lab.Models;
using Lab.Models.Entities;

namespace Lab.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doktori> Doktoret { get; set; }
        public DbSet<Pacienti> Pacientet { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kjo duhet të qëndrojë e para për të siguruar konfigurimet e identitetit
            base.OnModelCreating(modelBuilder);

            // Konfigurimi i lidhjes me tabelën `AspNetUsers`
            modelBuilder.Entity<User>()
                .HasMany<Doktori>()
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany<Pacienti>()
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lidhja midis Appointment dhe Doktorit
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doktori)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoktoriId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lidhja midis Appointment dhe Pacientit
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Pacienti)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PacientiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

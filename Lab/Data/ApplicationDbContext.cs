using Microsoft.EntityFrameworkCore;
using Lab.Models.Entities;

namespace Lab.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doktori> Doktoret { get; set; }
        public DbSet<Pacienti> Pacientet { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            base.OnModelCreating(modelBuilder);
        }
    }
}

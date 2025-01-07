using CozyNest.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CozyNest.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ReservationService> ReservationServices { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();
            builder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithOne()
                .HasForeignKey<Reservation>(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ReservationService>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationServices)
                .HasForeignKey(rs => rs.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ReservationService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.ReservationServices)
                .HasForeignKey(rs => rs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Room>()
                .Property(r => r.RoomNumber)
                .HasMaxLength(10)
                .IsRequired();
            builder.Entity<Reservation>()
                .HasIndex(r => r.RoomId);
        }

    }
}

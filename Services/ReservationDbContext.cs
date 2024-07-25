using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Services
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(a => a.UserRoles)
                .WithOne(b => b.User)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<UserRole>()
                .Property(a => a.Role)
                .HasConversion(
                b => (int)b,
                c => (RoleEnum)c);

            modelBuilder.Entity<ApiKey>()
                .HasOne(a => a.CreatedByUser)
                .WithMany(b => b.ApiKeys)
                .HasForeignKey(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Provider)
                .WithMany(b => b.ProviderAppointments)
                .HasForeignKey(c => c.ProviderUserId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(b => b.ClientAppointments)
                .HasForeignKey(c => c.ClientUserId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Brandon" },
                new User { Id = 2, Name = "Lora" },
                new User { Id = 3, Name = "Zach" },
                new User { Id = 4, Name = "Colette" },
                new User { Id = 5, Name = "Noah" },
                new User { Id = 6, Name = "Sharla" }
            );

            modelBuilder.Entity<ApiKey>().HasData(
                new ApiKey { Id = 1, Key = "efe33b8d-9311-4858-8cb5-ab38dd97604b", Expiration = new DateTime(2026, 07, 25), CreatedByUserId = 1 },
                new ApiKey { Id = 2, Key = "2011ea9a-ae39-8be4-7656-f46f48d289ff", Expiration = new DateTime(2026, 07, 25), CreatedByUserId = 2 },
                new ApiKey { Id = 3, Key = "2dc26fe3-df8c-7ff7-7f12-c72f3973f651", Expiration = new DateTime(2023, 07, 25), CreatedByUserId = 3 },
                new ApiKey { Id = 4, Key = "e28e8a47-3938-1836-367a-1c4fd9f9997d", Expiration = new DateTime(2026, 07, 25), CreatedByUserId = 4 },
                new ApiKey { Id = 5, Key = "f5fe1945-97e2-f352-e7ff-43ff0dc78580", Expiration = new DateTime(2026, 07, 25), CreatedByUserId = 5 },
                new ApiKey { Id = 6, Key = "8948c70e-6805-e5d1-4b1c-c387cd8e3c7f", Expiration = new DateTime(2026, 07, 25), CreatedByUserId = 6 }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, Role = RoleEnum.Admin },
                new UserRole { Id = 2, UserId = 2, Role = RoleEnum.Provider },
                new UserRole { Id = 3, UserId = 3, Role = RoleEnum.Client },
                new UserRole { Id = 4, UserId = 4, Role = RoleEnum.Client },
                new UserRole { Id = 5, UserId = 5, Role = RoleEnum.Client },
                new UserRole { Id = 6, UserId = 6, Role = RoleEnum.Provider },
                new UserRole { Id = 7, UserId = 6, Role = RoleEnum.Client }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}

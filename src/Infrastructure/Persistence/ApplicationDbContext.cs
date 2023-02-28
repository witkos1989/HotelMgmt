using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Duende.IdentityServer.EntityFramework.Options;
using Infrastructure.Identity;
using Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly IMediator _mediator;
        private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IMediator mediator,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
            : base(options)
        {
            _mediator = mediator;
            _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        }

        public DbSet<Address> Addresses => Set<Address>();

        public DbSet<Company> Companies => Set<Company>();

        public DbSet<Country> Countries => Set<Country>();

        public DbSet<Furniture> Furnitures => Set<Furniture>();

        public DbSet<Guest> Guests => Set<Guest>();

        public DbSet<GuestInvoice> GuestInvoices => Set<GuestInvoice>();

        public DbSet<Hotel> Hotels => Set<Hotel>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<ReservationStatus> ReservationStatuses => Set<ReservationStatus>();

        public DbSet<ReservationStatusName> ReservationStatusName => Set<ReservationStatusName>();

        public DbSet<ReservedRoom> ReservedRoom => Set<ReservedRoom>();

        public DbSet<Room> Room => Set<Room>();

        public DbSet<RoomType> RoomType => Set<RoomType>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens");
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEvents(this);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
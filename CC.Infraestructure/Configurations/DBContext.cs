using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Configurations
{
    public class DBContext : IdentityDbContext<User, Role, Guid>, IQueryableUnitOfWork
    {
        public DBContext(DbContextOptions<DBContext> options)
        : base(options)
        {
        }

        /// <summary>
        /// Permission
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// PermissionRole
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// WorkArea
        /// </summary>
        public DbSet<WorkArea> WorkAreas { get; set; }

        /// <summary>
        /// Workstation
        /// </summary>
        public DbSet<Workstation> Workstations { get; set; }

        /// <summary>
        /// UserWorkstations
        /// </summary>
        public DbSet<UserWorkstation> UserWorkstations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<User>().Property(p => p.FirstName).HasMaxLength(20);
            modelBuilder.Entity<User>().Property(p => p.LastName).HasMaxLength(20);

            modelBuilder.Entity<Role>().HasKey(c => c.Id);
            modelBuilder.Entity<Role>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Permission>().HasKey(c => c.Id);
            modelBuilder.Entity<Permission>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Permission>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<RolePermission>().HasKey(c => c.Id);
            modelBuilder.Entity<RolePermission>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<RolePermission>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<WorkArea>().HasKey(c => c.Id);
            modelBuilder.Entity<WorkArea>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<WorkArea>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Workstation>().HasKey(c => c.Id);
            modelBuilder.Entity<Workstation>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Workstation>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<UserWorkstation>().HasKey(c => c.Id);
            modelBuilder.Entity<UserWorkstation>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<UserWorkstation>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.HasDefaultSchema("Management");
            DisableCascadingDelete(modelBuilder);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationship = modelBuilder.Model.GetEntityTypes()
                .Where(e => !e.ClrType.Namespace.StartsWith("Microsoft.AspNetCore.Identity"))
                .SelectMany(e => e.GetForeignKeys());

            foreach (var r in relationship)
            {
                r.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await ex.Entries.Single().ReloadAsync().ConfigureAwait(false);
            }
        }

        public void DetachLocal<TEntity>(TEntity entity, EntityState state) where TEntity : class
        {
            if (entity is null)
            {
                return;
            }

            var local = Set<TEntity>().Local.ToList();

            if (local?.Any() ?? false)
            {
                local.ForEach(item =>
                {
                    Entry(item).State = EntityState.Detached;
                });
            }

            Entry(entity).State = state;
        }

        public DbContext GetContext()
        {
            return this;
        }

        public DbSet<TEntity> GetSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }
    }
}
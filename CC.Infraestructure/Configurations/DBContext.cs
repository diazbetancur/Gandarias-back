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

        /// <summary>
        /// HireTypes
        /// </summary>
        public DbSet<HireType> HireTypes { get; set; }

        /// <summary>
        /// UserActivityLog
        /// </summary>
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        /// <summary>
        /// ShiftType
        /// </summary>
        public DbSet<ShiftType> ShiftTypes { get; set; }

        /// <summary>
        /// HybridWorkstation
        /// </summary>
        public DbSet<HybridWorkstation> HybridWorkstations { get; set; }

        /// <summary>
        /// EmployeeScheduleRestriction
        /// </summary>
        public DbSet<EmployeeScheduleRestriction> EmployeeScheduleRestrictions { get; set; }

        /// <summary>
        /// AbsenteeismType
        /// </summary>
        public DbSet<AbsenteeismType> AbsenteeismTypes { get; set; }

        /// <summary>
        /// UserAbsenteeism
        /// </summary>
        public DbSet<UserAbsenteeism> UserAbsenteeisms { get; set; }

        /// <summary>
        /// EmployeeScheduleException
        /// </summary>
        public DbSet<EmployeeScheduleException> EmployeeScheduleExceptions { get; set; }

        /// <summary>
        /// LawRestriction
        /// </summary>
        public DbSet<LawRestriction> LawRestrictions { get; set; }

        /// <summary>
        /// WorkstationDemandTemplate
        /// </summary>
        public DbSet<WorkstationDemandTemplate> WorkstationDemandTemplates { get; set; }

        /// <summary>
        /// WorkstationDemand
        /// </summary>
        public DbSet<WorkstationDemand> WorkstationDemands { get; set; }

        /// <summary>
        /// EmployeeShiftTypeRestriction
        /// </summary>
        public DbSet<EmployeeShiftTypeRestriction> EmployeeShiftTypeRestrictions { get; set; }

        /// <summary>
        /// Schedule
        /// </summary>
        public DbSet<Schedule> Schedules { get; set; }

        /// <summary>
        /// UserShift
        /// </summary>
        public DbSet<UserShift> UserShifts { get; set; }

        /// <summary>
        /// SigningCofiguration
        /// </summary>
        public DbSet<SigningCofiguration> SigningCofigurations { get; set; }

        /// <summary>
        /// SigningCofiguration
        /// </summary>
        public DbSet<Signing> Signings { get; set; }

        // TODO: Revisar si es necesario esto

        /// <summary>
        /// License
        /// </summary>
        public DbSet<License> Licenses { get; set; }

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

            modelBuilder.Entity<HireType>().HasKey(c => c.Id);
            modelBuilder.Entity<HireType>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<HireType>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<UserActivityLog>().HasKey(c => c.Id);
            modelBuilder.Entity<UserActivityLog>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<UserActivityLog>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<ShiftType>().HasKey(c => c.Id);
            modelBuilder.Entity<ShiftType>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<ShiftType>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<HybridWorkstation>().HasKey(c => c.Id);
            modelBuilder.Entity<HybridWorkstation>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<HybridWorkstation>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<HybridWorkstation>().HasKey(c => c.Id);
            modelBuilder.Entity<HybridWorkstation>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<HybridWorkstation>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<EmployeeScheduleRestriction>().HasKey(c => c.Id);
            modelBuilder.Entity<EmployeeScheduleRestriction>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<EmployeeScheduleRestriction>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<AbsenteeismType>().HasKey(c => c.Id);
            modelBuilder.Entity<AbsenteeismType>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<AbsenteeismType>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<UserAbsenteeism>().HasKey(c => c.Id);
            modelBuilder.Entity<UserAbsenteeism>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<UserAbsenteeism>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<EmployeeScheduleException>().HasKey(c => c.Id);
            modelBuilder.Entity<EmployeeScheduleException>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<EmployeeScheduleException>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<LawRestriction>().HasKey(c => c.Id);
            modelBuilder.Entity<LawRestriction>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<LawRestriction>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<WorkstationDemandTemplate>().HasKey(c => c.Id);
            modelBuilder.Entity<WorkstationDemandTemplate>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<WorkstationDemandTemplate>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<WorkstationDemand>().HasKey(c => c.Id);
            modelBuilder.Entity<WorkstationDemand>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<WorkstationDemand>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<EmployeeShiftTypeRestriction>().HasKey(c => c.Id);
            modelBuilder.Entity<EmployeeShiftTypeRestriction>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<EmployeeShiftTypeRestriction>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Schedule>().HasKey(c => c.Id);
            modelBuilder.Entity<Schedule>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Schedule>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Schedule>().HasKey(c => c.Id);
            modelBuilder.Entity<Schedule>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Schedule>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Signing>().HasKey(c => c.Id);
            modelBuilder.Entity<Signing>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Signing>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<SigningCofiguration>().HasKey(c => c.Id);
            modelBuilder.Entity<SigningCofiguration>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<SigningCofiguration>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            // TODO: Revisar si es necesario esto

            modelBuilder.Entity<UserShift>().HasKey(c => c.Id);
            modelBuilder.Entity<UserShift>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<UserShift>().Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

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
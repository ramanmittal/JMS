using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JMS.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JMS.Entity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        //public ApplicationDbContext() : base(){ }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<JournalAdmin> JournalAdmins { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<JournalSetting> JournalSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne(x => x.Tenant).WithMany(x => x.ApplicationUsers).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalAdmin>().HasOne(x => x.ApplicationUser).WithOne(x => x.JournalAdmin).HasForeignKey<JournalAdmin>(x => x.UserId);
            builder.Entity<ApplicationUser>().HasIndex(x => new { x.TenantId, x.UserName }).IsUnique(true);
            builder.Entity<ApplicationUser>().HasIndex(x => new { x.UserName }).IsUnique(false);
            builder.Entity<ApplicationUser>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<Tenant>().HasIndex(x => x.JournalPath).IsUnique(true);
            builder.Entity<Tenant>().HasQueryFilter(m => m.Deleted == null);
            builder.Entity<Tenant>().HasMany(x => x.JournalAdmins).WithOne(x => x.Tenant).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalSetting>().HasOne(x => x.Tenant).WithMany(x => x.JournalSettings).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalSetting>().HasIndex(x => new { x.TenantId, x.Key }).IsUnique(true);
            builder.Entity<IdentityUserLogin<long>>().HasKey(x => new { x.ProviderKey, x.LoginProvider });
            builder.Entity<IdentityUserRole<long>>().HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<IdentityUserToken<long>>().HasKey(x => new { x.UserId, x.Name, x.LoginProvider });
        }
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            ((Trackable)entry.Entity).Added = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            ((Trackable)entry.Entity).Changed = DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            ((Trackable)entry.Entity).Deleted = DateTime.UtcNow;
                            break;
                    }
                } 
            }
        }


    }
}

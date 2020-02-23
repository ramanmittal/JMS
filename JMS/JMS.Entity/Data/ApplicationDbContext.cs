using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JMS.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            builder.Entity<ApplicationUser>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<Tenant>().HasIndex(x => x.JournalPath).IsUnique(true);
            builder.Entity<Tenant>().HasMany(x => x.JournalAdmins).WithOne(x => x.Tenant).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalSetting>().HasOne(x => x.Tenant).WithMany(x => x.JournalSettings).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalSetting>().HasIndex(x => new { x.TenantId, x.Key }).IsUnique(true);
            builder.Entity<IdentityUserLogin<long>>().HasKey(x => new { x.ProviderKey, x.LoginProvider });
            builder.Entity<IdentityUserRole<long>>().HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<IdentityUserToken<long>>().HasKey(x => new { x.UserId, x.Name, x.LoginProvider });
        }
        
    }
}

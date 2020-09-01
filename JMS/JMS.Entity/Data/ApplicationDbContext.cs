using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
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
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<JournalSetting> JournalSettings { get; set; }
        public DbSet<Submission> Submission { get; set; }
        public DbSet<TenantArticleComponent> TenantArticleComponent { get; set; }

        public DbSet<SubmisssionFile> SubmisssionFile { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ReviewRequest> ReviewRequest { get; set; }
        public DbSet<SubmissionHistory> SubmissionHistory { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne(x => x.Tenant).WithMany(x => x.ApplicationUsers).HasForeignKey(x => x.TenantId);
            builder.Entity<ApplicationUser>().HasIndex(x => new { x.TenantId, x.UserName }).IsUnique(true);
            builder.Entity<ApplicationUser>().HasIndex(x => new { x.UserName }).IsUnique(false);
            builder.Entity<ApplicationUser>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<Tenant>().HasIndex(x => x.JournalPath).IsUnique(true);
            builder.Entity<Tenant>().HasQueryFilter(m => m.Deleted == null);
            builder.Entity<JournalSetting>().HasOne(x => x.Tenant).WithMany(x => x.JournalSettings).HasForeignKey(x => x.TenantId);
            builder.Entity<JournalSetting>().HasIndex(x => new { x.TenantId, x.Key }).IsUnique(true);
            builder.Entity<IdentityUserLogin<long>>().HasKey(x => new { x.ProviderKey, x.LoginProvider });
            builder.Entity<IdentityUserRole<long>>().HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<IdentityUserToken<long>>().HasKey(x => new { x.UserId, x.Name, x.LoginProvider });
            builder.Entity<Submission>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserID);
            builder.Entity<TenantArticleComponent>().HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId);
            builder.Entity<SubmisssionFile>().HasOne(x => x.Submission).WithMany().HasForeignKey(x => x.SubmissionId);
            builder.Entity<SubmisssionFile>().HasOne(x => x.TenantArticleComponent).WithMany().HasForeignKey(x => x.ArticleComponentId);
            builder.Entity<Contributor>().HasOne(x => x.Submission).WithMany().HasForeignKey(x => x.SubmissionId);
            builder.Entity<Author>().HasOne(x => x.User).WithOne().HasForeignKey<Author>(x => x.Id);
            builder.Entity<Submission>().HasOne(x => x.Editor).WithMany().HasForeignKey(x => x.EditorId);
            builder.Entity<ReviewRequest>().HasOne(x => x.Submission).WithMany().HasForeignKey(x => x.SubmissionId);
            builder.Entity<ReviewRequest>().HasOne(x => x.Reviewer).WithMany().HasForeignKey(x => x.ReviewerID);
            builder.Entity<SubmissionHistory>().HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenanatID);
            var cascadeFKs = builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

                foreach (var fk in cascadeFKs)
                    fk.DeleteBehavior = DeleteBehavior.Restrict;

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

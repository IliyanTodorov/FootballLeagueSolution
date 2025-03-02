namespace FootballLeague.Data
{
    using Microsoft.EntityFrameworkCore;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Common.Models;
    using System.Linq;
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    public class FootballLeagueDbContext : DbContext
    {
        public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>().HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Match>()
               .HasOne(m => m.HomeTeam)
               .WithMany(t => t.HomeMatches)
               .HasForeignKey(m => m.HomeTeamId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditInfoRules();
            ApplySoftDeleteRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ApplyAuditInfoRules();
            ApplySoftDeleteRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyAuditInfoRules()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditInfo &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var auditEntity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    auditEntity.CreatedOn = DateTime.UtcNow;
                }
                auditEntity.ModifiedOn = DateTime.UtcNow;
            }
        }

        private void ApplySoftDeleteRules()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is IDeletableEntity && e.State == EntityState.Deleted))
            {
                var deletable = (IDeletableEntity)entry.Entity;
                deletable.IsDeleted = true;
                deletable.DeletedOn = DateTime.UtcNow;
                entry.State = EntityState.Modified;
            }
        }
    }
}

using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.DataLayer
{
    public class BattleSimDbContext : DbContext
    {
        public BattleSimDbContext(DbContextOptions<BattleSimDbContext> options) : base(options) { }

        public DbSet<Fighter> Fighter { get; set; }
        public DbSet<Attack> Attack { get; set; }
        public DbSet<ElementType> ElementType { get; set; }  // Fixed namespace
        public DbSet<FighterType> FighterType { get; set; }  // Added FighterType
        public DbSet<FighterAttack> FighterAttack { get; set; } // Many-to-Many table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fighter -> ElementType (One-to-Many)
            modelBuilder.Entity<Fighter>()
                .HasOne(f => f.ElementType)
                .WithMany()
                .HasForeignKey(f => f.ElementTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fighter -> FighterType (One-to-Many)
            modelBuilder.Entity<Fighter>()
                .HasOne(f => f.FighterType)
                .WithMany()
                .HasForeignKey(f => f.FighterTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<FighterAttack>()
                .HasKey(fa => fa.Id);

            modelBuilder.Entity<FighterAttack>()
                .HasOne(fa => fa.Fighter)
                .WithMany(f => f.FighterAttacks)
                .HasForeignKey(fa => fa.FighterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FighterAttack>()
                .HasOne(fa => fa.Attack)
                .WithMany(a => a.FighterAttacks)
                .HasForeignKey(fa => fa.AttackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attack -> ElementType (One-to-Many)
            modelBuilder.Entity<Attack>()
                .HasOne(a => a.ElementType)
                .WithMany()
                .HasForeignKey(a => a.ElementTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }

}

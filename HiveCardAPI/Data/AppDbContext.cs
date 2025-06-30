using HiveCardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace HiveCardAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<CreditCardProduct> CreditCardProducts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<TransactionDetails> TransactionDetail { get; set; }
        public DbSet<PdfFile> PdfFiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Correct FK → non-PK unique join:
            modelBuilder.Entity<Statement>()
                .HasOne(s => s.CreditCard)
                .WithMany(c => c.Statements)
                .HasPrincipalKey(c => c.CardNumber)      // Use CardNumber as the principal key
                .HasForeignKey(s => s.CreditCardId);     // This is the FK in Statements
        }
    }
}
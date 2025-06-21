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
    }
}
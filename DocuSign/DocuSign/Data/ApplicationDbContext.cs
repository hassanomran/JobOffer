using DocuSign.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DocuSign.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<JobOffer> JobOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobOffer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RecipientName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RecipientEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.JobOfferContent).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.PdfFileName).HasMaxLength(255);
                entity.Property(e => e.DocuSignEnvelopeId).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Base;

namespace ProjectInvoices.API.Data
{
    /// <summary>
    /// Represents the application database context for managing project invoices 
    /// and payments and all relevant entities
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : 
            base(options)
        {}

        public DbSet<Project> Projects { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Contractor> Contractors { get; set; }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<ProjectInvoice> ProjectInvoices { get; set; }
        public DbSet<ProjectInvoiceItem> ProjectInvoiceItems { get; set; }

        public DbSet<ProjectInvoicePaymentGroup> ProjectInvoicePaymentGroups { get; set; }
        public DbSet<ProjectInvoicePayment> ProjectInvoicePayments { get; set; }
        public DbSet<CashOutMovement> CashOutMovements { get; set; }
        public DbSet<CheckOutMovement> CheckOutMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure Project entity
            modelBuilder.Entity<Project>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Project>().HasIndex(p => p.Name).IsUnique();

            //Configure Item entity
            modelBuilder.Entity<Item>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Item>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Item>().Property(p => p.Unit).HasMaxLength(50);

            //Configure Supplier entity
            modelBuilder.Entity<Supplier>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Supplier>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Supplier>().Property(p => p.Phone).HasMaxLength(50);
            modelBuilder.Entity<Supplier>().Property(p => p.Email).HasMaxLength(50);
            modelBuilder.Entity<Supplier>().Property(p => p.Address).HasMaxLength(50);

            //Configure Contractor entity
            modelBuilder.Entity<Contractor>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Contractor>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Contractor>().Property(p => p.Phone).HasMaxLength(50);
            modelBuilder.Entity<Contractor>().Property(p => p.Email).HasMaxLength(50);
            modelBuilder.Entity<Contractor>().Property(p => p.Address).HasMaxLength(50);

            //Configure Bank entity
            modelBuilder.Entity<Bank>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Bank>().HasIndex(p => p.Name).IsUnique();

            //Configure Bank Account entity
            modelBuilder.Entity<BankAccount>().Property(p => p.AccountNumber).HasMaxLength(50);
            modelBuilder.Entity<BankAccount>().HasIndex(p => p.AccountNumber).IsUnique();

            modelBuilder.Entity<BankAccount>().Property(p => p.AccountName).HasMaxLength(50);
            modelBuilder.Entity<BankAccount>().HasIndex(p => p.AccountName).IsUnique();

            modelBuilder.Entity<BankAccount>().HasOne(x => x.Bank).WithMany().HasForeignKey(p => p.BankId).OnDelete(DeleteBehavior.Restrict);

            //Configure ProjectInvoice entity
            modelBuilder.Entity<ProjectInvoice>().Property(p => p.ReferenceNumber).HasMaxLength(50);
            modelBuilder.Entity<ProjectInvoice>().HasIndex(p => p.ReferenceNumber).IsUnique();

            modelBuilder.Entity<ProjectInvoice>().HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProjectInvoice>().HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);

            //Configure ProjectInvoiceItem entity
            modelBuilder.Entity<ProjectInvoiceItem>().HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProjectInvoiceItem>().HasOne<ProjectInvoice>().WithMany(x => x.Items).HasForeignKey(x => x.ProjectInvoiceId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProjectInvoiceItem>().Property(p => p.Unit).HasMaxLength(50);

            //Configure ProjectInvoicePayment entity
            modelBuilder.Entity<ProjectInvoicePayment>().Property(p => p.Done).HasDefaultValue(false);
            modelBuilder.Entity<ProjectInvoicePayment>().Property(p => p.IsGroup).HasDefaultValue(false);

            modelBuilder.Entity<ProjectInvoicePayment>().HasOne<ProjectInvoice>().WithMany().HasForeignKey(x => x.ProjectInvoiceId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProjectInvoicePayment>().HasOne<ProjectInvoicePaymentGroup>().WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Restrict);

            //Configure CashOutMovement entity
            modelBuilder.Entity<CashOutMovement>().HasIndex(p => p.PaymentId);

            //Configure CheckOutMovement entity
            modelBuilder.Entity<CheckOutMovement>().HasIndex(p => p.PaymentId);
            modelBuilder.Entity<CheckOutMovement>().Property(p => p.CheckNumber).HasMaxLength(50);
            modelBuilder.Entity<CheckOutMovement>().HasOne<BankAccount>().WithMany().HasForeignKey(x => x.BankAcountId).OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges()
        {
            //Set CreateDate for new entries, LastModifiedDate for new and updated entries
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.CreateDate = DateTime.Now;
                        entity.LastModifiedDate = DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.LastModifiedDate = DateTime.Now;
                    }
                }    
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Set CreateDate for new entries, LastModifiedDate for new and updated entries
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.CreateDate = DateTime.Now;
                        entity.LastModifiedDate = DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.LastModifiedDate = DateTime.Now;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

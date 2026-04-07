using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data.Models;

namespace PersonalAccount.Data;

public partial class PersonalAccountContext : DbContext
{
    public PersonalAccountContext()
    {
    }

    public PersonalAccountContext(DbContextOptions<PersonalAccountContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Emploee> Emploees { get; set; }

    public virtual DbSet<LinksUserCompany> LinksUserCompanies { get; set; }

    public virtual DbSet<Nomenclature> Nomenclatures { get; set; }

    public virtual DbSet<Schemaversion> Schemaversions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<Models.JournalRow> JournalRows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.Company).WithMany(p => p.Categories)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("categories_company_id_fk");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companies_pkey");

            entity.ToTable("companies");

            entity.HasIndex(e => e.Inn, "company_inn_ix").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.LoadOptions)
                .HasColumnType("jsonb")
                .HasColumnName("load_options");
        });

        modelBuilder.Entity<Emploee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("emploees_pkey");

            entity.ToTable("emploees");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");

            entity.HasOne(d => d.Company).WithMany(p => p.Emploees)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("emploees_company_id_fk");
        });

        modelBuilder.Entity<LinksUserCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("links_user_company_pkey");

            entity.ToTable("links_user_company");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.LinksUserCompanies)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("links_user_company_company_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.LinksUserCompanies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("links_user_company_user_id_fk");
        });

        modelBuilder.Entity<Nomenclature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("nomenclatures_pkey");

            entity.ToTable("nomenclatures");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.Category).WithMany(p => p.Nomenclatures)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("nomenclatures_category_id_fk");
        });

        modelBuilder.Entity<Schemaversion>(entity =>
        {
            entity.HasKey(e => e.Schemaversionsid).HasName("PK_schemaversions_Id");

            entity.ToTable("schemaversions");

            entity.Property(e => e.Schemaversionsid).HasColumnName("schemaversionsid");
            entity.Property(e => e.Applied)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("applied");
            entity.Property(e => e.Scriptname)
                .HasMaxLength(255)
                .HasColumnName("scriptname");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ChangePeriod).HasColumnName("change_period");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Discount)
                .HasPrecision(15, 2)
                .HasColumnName("discount");
            entity.Property(e => e.EmloeeId).HasColumnName("emloee_id");
            entity.Property(e => e.NomenclatureId).HasColumnName("nomenclature_id");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasPrecision(15, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.TransactionType).HasColumnName("transaction_type");

            entity.HasOne(d => d.Company).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions_company_id_fk");

            entity.HasOne(d => d.Emloee).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.EmloeeId)
                .HasConstraintName("transactions_emloee_id_fk");

            entity.HasOne(d => d.Nomenclature).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.NomenclatureId)
                .HasConstraintName("transactions_nomenclature_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

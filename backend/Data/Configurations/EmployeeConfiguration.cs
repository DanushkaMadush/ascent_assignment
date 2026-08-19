using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Table
            builder.ToTable("Employees");

            // Primary Key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(450)
                .ValueGeneratedNever();

            // Employee Code
            builder.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeCode)
                .IsUnique();

            // First Name
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            // Last Name
            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Email
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            // Phone Number
            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(30);

            // Is Active
            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Created At
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            //Created By
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(450);

            builder.HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Manager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.ManagerId);
        }
    }
}

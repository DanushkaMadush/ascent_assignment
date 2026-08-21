using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations
{
    public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            // Table
            builder.ToTable("Leave");

            // Primary Key
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedOnAdd();

            // Employee Id
            builder.Property(l => l.EmployeeId)
                .IsRequired()
                .HasMaxLength(450);

            // Leave Type
            builder.Property(l => l.LeaveType)
                .IsRequired()
                .HasMaxLength(50);

            // Start Date
            builder.Property(l => l.StartDate)
                .IsRequired()
                .HasColumnType("date");

            // End Date
            builder.Property(l => l.EndDate)
                .IsRequired()
                .HasColumnType("date");

            // Reason
            builder.Property(l => l.Reason)
                .HasMaxLength(1000);

            // Status
            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            // Approved By
            builder.Property(l => l.ApprovedById)
                .HasMaxLength(450);

            // Applied On
            builder.Property(l => l.AppliedOn)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for queries filtering data by employee id
            builder.HasIndex(l => l.EmployeeId);

            // Index for queries filtering data by status
            builder.HasIndex(l => l.Status);

            // Index for queries filtering data by start date and end date
            builder.HasIndex(l => new
            {
                l.StartDate,
                l.EndDate
            });
        }
    }
}

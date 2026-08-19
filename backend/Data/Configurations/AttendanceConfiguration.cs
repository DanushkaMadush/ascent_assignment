using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            // Table
            builder.ToTable("Attendance");

            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            // Employee Id
            builder.Property(a => a.EmployeeId)
                .IsRequired()
                .HasMaxLength(450);

            // Check In
            builder.Property(a => a.CheckInTime)
                .IsRequired()
                .HasColumnType("datetime2");

            // Check Out
            builder.Property(a => a.CheckOutTime)
                .HasColumnType("datetime2");

            // Work Date
            builder.Property(a => a.WorkDate)
                .IsRequired()
                .HasColumnType("date");

            // Status
            builder.Property(a => a.Status)
                .HasMaxLength(30);

            // Device Type
            builder.Property(a => a.DeviceType)
                .HasMaxLength(30);

            builder.HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => new
            {
                a.EmployeeId,
                a.WorkDate
            })
            .IsUnique();

            builder.HasIndex(a => a.WorkDate);
        }
    }
}

using AnyoneForTennis.OldDatas.Models;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.OldDatas.Data;

public partial class Hitdb1Context : DbContext
{
    public Hitdb1Context()
    {
    }

    public Hitdb1Context(DbContextOptions<Hitdb1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Coach> Coaches { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("data source=scithitdb1.cducloud.cdu.edu.au;initial catalog=HITDB1;user id=Student1Login;password=g85{K4OzKF3<;encrypt=optional");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coach>(entity =>
        {
            entity.Property(e => e.CoachId).ValueGeneratedNever();
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsFixedLength();
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.MemberId).ValueGeneratedNever();
            entity.Property(e => e.Email)
                .HasMaxLength(400)
                .IsFixedLength();
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsFixedLength();
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.Property(e => e.ScheduleId).ValueGeneratedNever();
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

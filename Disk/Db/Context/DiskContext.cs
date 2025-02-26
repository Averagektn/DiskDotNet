using Disk.Entities;
using Disk.Properties.Config;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.IO;

namespace Disk.Db.Context;

public partial class DiskContext : DbContext
{
    public DiskContext() { }

    public DiskContext(DbContextOptions<DiskContext> options) : base(options) { }

    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<Map> Maps { get; set; }
    public virtual DbSet<PathInTarget> PathInTargets { get; set; }
    public virtual DbSet<PathToTarget> PathToTargets { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<SessionResult> SessionResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlite(AppConfig.DbConnectionString);
    }

    public void EnsureDatabaseExists()
    {   
        if (!Directory.Exists("./Db"))
        {
            Log.Fatal("Db folder not found");
            _ = Directory.CreateDirectory("./Db");
            Log.Information("Created new db folder");
        }

        if (!File.Exists(AppConfig.DbConnectionString))
        {
            Log.Fatal("Db file not found");
            _ = Database.EnsureCreated();
            Log.Information("Created new db file");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Appointment>(entity =>
        {
            _ = entity.HasKey(e => e.Id);

            _ = entity.ToTable("appointment");

            _ = entity.Property(e => e.Id).HasColumnName("app_id");
            _ = entity.Property(e => e.DateTime).HasColumnName("app_date_time");
            _ = entity.Property(e => e.Patient).HasColumnName("app_patient");

            _ = entity.HasOne(d => d.PatientNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Patient)
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<Map>(entity =>
        {
            _ = entity.ToTable("map");

            _ = entity.HasIndex(e => e.Name, "IX_map_map_name").IsUnique();

            _ = entity.Property(e => e.Id).HasColumnName("map_id");
            _ = entity.Property(e => e.CoordinatesJson).HasColumnName("map_coordinates_json");
            _ = entity.Property(e => e.CreatedAtDateTime).HasColumnName("map_created_at_date_time");
            _ = entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnName("map_name");
        });

        _ = modelBuilder.Entity<PathInTarget>(entity =>
        {
            _ = entity.HasKey(e => new { e.Session, e.TargetId });

            _ = entity.ToTable("path_in_target");

            _ = entity.Property(e => e.Session).HasColumnName("pit_session");
            _ = entity.Property(e => e.TargetId).HasColumnName("pit_target_id");
            _ = entity.Property(e => e.CoordinatesJson).HasColumnName("pit_coordinates_json");
            _ = entity.Property(e => e.Precision).HasColumnName("pit_precision");

            _ = entity.HasOne(d => d.SessionNavigation).WithMany(p => p.PathInTargets)
                .HasForeignKey(d => d.Session)
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<PathToTarget>(entity =>
        {
            _ = entity.HasKey(e => new { e.Session, e.TargetNum });

            _ = entity.ToTable("path_to_target");

            _ = entity.Property(e => e.Session).HasColumnName("ptt_session");
            _ = entity.Property(e => e.TargetNum).HasColumnName("ptt_target_num");
            _ = entity.Property(e => e.AngleDistance).HasColumnName("ptt_ange_distance");
            _ = entity.Property(e => e.AngleSpeed).HasColumnName("ptt_angle_speed");
            _ = entity.Property(e => e.ApproachSpeed).HasColumnName("ptt_approach_speed");
            _ = entity.Property(e => e.CoordinatesJson).HasColumnName("ptt_coordinates_json");
            _ = entity.Property(e => e.Time).HasColumnName("ptt_time");

            _ = entity.HasOne(d => d.SessionNavigation).WithMany(p => p.PathToTargets)
                .HasForeignKey(d => d.Session)
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<Patient>(entity =>
        {
            _ = entity.HasKey(e => e.Id);

            _ = entity.ToTable("patient");

            _ = entity.Property(e => e.Id).HasColumnName("pat_id");
            _ = entity.Property(e => e.DateOfBirth).HasColumnName("pat_date_of_birth");
            _ = entity.Property(e => e.Name)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            _ = entity.Property(e => e.Patronymic)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            _ = entity.Property(e => e.PhoneHome).HasColumnName("pat_phone_home");
            _ = entity.Property(e => e.PhoneMobile).HasColumnName("pat_phone_mobile");
            _ = entity.Property(e => e.Surname)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");
        });

        _ = modelBuilder.Entity<Session>(entity =>
        {
            _ = entity.HasKey(e => e.Id);

            _ = entity.ToTable("session");

            _ = entity.HasIndex(e => e.LogFilePath, "IX_session_ses_log_file_path").IsUnique();

            _ = entity.Property(e => e.Id).HasColumnName("ses_id");
            _ = entity.Property(e => e.Appointment).HasColumnName("ses_appointment");
            _ = entity.Property(e => e.DateTime).HasColumnName("ses_date_time");
            _ = entity.Property(e => e.LogFilePath).HasColumnName("ses_log_file_path");
            _ = entity.Property(e => e.Map).HasColumnName("ses_map");
            _ = entity.Property(e => e.MaxXAngle).HasColumnName("ses_max_x_angle");
            _ = entity.Property(e => e.MaxYAngle).HasColumnName("ses_max_y_angle");

            _ = entity.HasOne(d => d.AppointmentNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.Appointment)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(d => d.MapNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.Map)
                .OnDelete(DeleteBehavior.Restrict);
        });

        _ = modelBuilder.Entity<SessionResult>(entity =>
        {
            _ = entity.HasKey(e => e.Id);

            _ = entity.ToTable("session_result");

            _ = entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("sres_id");
            _ = entity.Property(e => e.Deviation).HasColumnName("sres_deviation");
            _ = entity.Property(e => e.Dispersion).HasColumnName("sres_dispersion");
            _ = entity.Property(e => e.MathExp).HasColumnName("sres_math_exp");
            _ = entity.Property(e => e.Score).HasColumnName("sres_score");

            _ = entity.HasOne(d => d.Sres).WithOne(p => p.SessionResult)
                .HasForeignKey<SessionResult>(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

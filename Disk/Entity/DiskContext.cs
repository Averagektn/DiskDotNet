using Microsoft.EntityFrameworkCore;

namespace Disk.Entity;

public partial class DiskContext : DbContext
{
    public DiskContext()
    {
    }

    public DiskContext(DbContextOptions<DiskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Diagnosis> Diagnoses { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<M2mCardDiagnosis> M2mCardDiagnoses { get; set; }

    public virtual DbSet<Map> Maps { get; set; }

    public virtual DbSet<PathInTarget> PathInTargets { get; set; }

    public virtual DbSet<PathToTarget> PathToTargets { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionResult> SessionResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=Db/disk.db;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddrId);

            entity.ToTable("address");

            entity.Property(e => e.AddrId).HasColumnName("addr_id");
            entity.Property(e => e.AddrApartment).HasColumnName("addr_apartment");
            entity.Property(e => e.AddrCorpus)
                .HasDefaultValue(1)
                .HasColumnName("addr_corpus");
            entity.Property(e => e.AddrDistrict).HasColumnName("addr_district");
            entity.Property(e => e.AddrHouse).HasColumnName("addr_house");
            entity.Property(e => e.AddrRegion).HasColumnName("addr_region");
            entity.Property(e => e.AddrStreet).HasColumnName("addr_street");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.CrdId);

            entity.ToTable("card");

            entity.HasIndex(e => e.CrdNumber, "IX_card_crd_number").IsUnique();

            entity.Property(e => e.CrdId).HasColumnName("crd_id");
            entity.Property(e => e.CrdNumber)
                .HasColumnType("TEXT (9)")
                .HasColumnName("crd_number");
            entity.Property(e => e.CrdPatient).HasColumnName("crd_patient");

            entity.HasOne(d => d.CrdPatientNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.CrdPatient)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.DiaId);

            entity.ToTable("diagnosis");

            entity.HasIndex(e => e.DiaId, "IX_diagnosis_dia_id").IsUnique();

            entity.HasIndex(e => e.DiaName, "IX_diagnosis_dia_name").IsUnique();

            entity.Property(e => e.DiaId).HasColumnName("dia_id");
            entity.Property(e => e.DiaName).HasColumnName("dia_name");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DocId);

            entity.ToTable("doctor");

            entity.HasIndex(e => e.DocId, "IX_doctor_doc_id").IsUnique();

            entity.Property(e => e.DocId).HasColumnName("doc_id");
            entity.Property(e => e.DocName).HasColumnName("doc_name");
            entity.Property(e => e.DocPassword).HasColumnName("doc_password");
            entity.Property(e => e.DocPatronymic).HasColumnName("doc_patronymic");
            entity.Property(e => e.DocSurname).HasColumnName("doc_surname");
        });

        modelBuilder.Entity<M2mCardDiagnosis>(entity =>
        {
            entity.HasKey(e => new { e.C2dCard, e.C2dDiagnosis });

            entity.ToTable("m2m_card_diagnosis");

            entity.Property(e => e.C2dCard).HasColumnName("c2d_card");
            entity.Property(e => e.C2dDiagnosis).HasColumnName("c2d_diagnosis");
            entity.Property(e => e.C2dDiagnosisFinish).HasColumnName("c2d_diagnosis_finish");
            entity.Property(e => e.C2dDiagnosisStart).HasColumnName("c2d_diagnosis_start");

            entity.HasOne(d => d.C2dCardNavigation).WithMany(p => p.M2mCardDiagnoses).HasForeignKey(d => d.C2dCard);

            entity.HasOne(d => d.C2dDiagnosisNavigation).WithMany(p => p.M2mCardDiagnoses)
                .HasForeignKey(d => d.C2dDiagnosis)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Map>(entity =>
        {
            entity.ToTable("map");

            entity.HasIndex(e => e.MapId, "IX_map_map_id").IsUnique();

            entity.Property(e => e.MapId).HasColumnName("map_id");
            entity.Property(e => e.MapCoordinatesJson).HasColumnName("map_coordinates_json");
            entity.Property(e => e.MapCreatedAt).HasColumnName("map_created_at");
            entity.Property(e => e.MapCreatedBy).HasColumnName("map_created_by");

            entity.HasOne(d => d.MapCreatedByNavigation).WithMany(p => p.Maps).HasForeignKey(d => d.MapCreatedBy);
        });

        modelBuilder.Entity<PathInTarget>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("path_in_target");

            entity.Property(e => e.PitCoordinatesJson).HasColumnName("pit_coordinates_json");
            entity.Property(e => e.PitSession).HasColumnName("pit_session");
            entity.Property(e => e.PitTargetId).HasColumnName("pit_target_id");

            entity.HasOne(d => d.PitSessionNavigation).WithMany()
                .HasForeignKey(d => d.PitSession)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PathToTarget>(entity =>
        {
            entity.HasKey(e => new { e.PttSession, e.PttNum });

            entity.ToTable("path_to_target");

            entity.Property(e => e.PttSession).HasColumnName("ptt_session");
            entity.Property(e => e.PttNum).HasColumnName("ptt_num");
            entity.Property(e => e.PttCoordinatesJson).HasColumnName("ptt_coordinates_json");

            entity.HasOne(d => d.PttSessionNavigation).WithMany(p => p.PathToTargets)
                .HasForeignKey(d => d.PttSession)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatId);

            entity.ToTable("patient");

            entity.Property(e => e.PatId).HasColumnName("pat_id");
            entity.Property(e => e.PatAddress).HasColumnName("pat_address");
            entity.Property(e => e.PatDateOfBirth).HasColumnName("pat_date_of_birth");
            entity.Property(e => e.PatName)
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            entity.Property(e => e.PatPatronymic)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            entity.Property(e => e.PatPhoneHome).HasColumnName("pat_phone_home");
            entity.Property(e => e.PatPhoneMobile).HasColumnName("pat_phone_mobile");
            entity.Property(e => e.PatSurname)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");

            entity.HasOne(d => d.PatAddressNavigation).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PatAddress)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SesId);

            entity.ToTable("session");

            entity.HasIndex(e => e.SesId, "IX_session_ses_id").IsUnique();

            entity.HasIndex(e => e.SesLogFilePath, "IX_session_ses_log_file_path").IsUnique();

            entity.Property(e => e.SesId).HasColumnName("ses_id");
            entity.Property(e => e.SesDate).HasColumnName("ses_date");
            entity.Property(e => e.SesDoctor).HasColumnName("ses_doctor");
            entity.Property(e => e.SesLogFilePath).HasColumnName("ses_log_file_path");
            entity.Property(e => e.SesMap).HasColumnName("ses_map");
            entity.Property(e => e.SesPatient).HasColumnName("ses_patient");

            entity.HasOne(d => d.SesDoctorNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SesDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SesMapNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SesMap)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SesPatientNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SesPatient)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SessionResult>(entity =>
        {
            entity.HasKey(e => e.SresId);

            entity.ToTable("session_result");

            entity.HasIndex(e => e.SresId, "IX_session_result_sres_id").IsUnique();

            entity.Property(e => e.SresId)
                .ValueGeneratedNever()
                .HasColumnName("sres_id");
            entity.Property(e => e.SresDeviation).HasColumnName("sres_deviation");
            entity.Property(e => e.SresDispersion).HasColumnName("sres_dispersion");
            entity.Property(e => e.SresMathExp).HasColumnName("sres_math_exp");

            entity.HasOne(d => d.Sres).WithOne(p => p.SessionResult)
                .HasForeignKey<SessionResult>(d => d.SresId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

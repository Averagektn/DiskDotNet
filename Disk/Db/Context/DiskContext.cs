using Disk.Entity;
using Disk.Properties.Config;
using Microsoft.EntityFrameworkCore;

namespace Disk.Db.Context;

public partial class DiskContext : DbContext
{
    public DiskContext() {}

    public DiskContext(DbContextOptions<DiskContext> options) : base(options) {}

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<CardDiagnosis> CardDiagnoses { get; set; }

    public virtual DbSet<Contraindication> Contraindications { get; set; }

    public virtual DbSet<Diagnosis> Diagnoses { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorAppointment> DoctorAppointments { get; set; }

    public virtual DbSet<DoctorCabinet> DoctorCabinets { get; set; }

    public virtual DbSet<M2mCardDiagnosis> M2mCardDiagnoses { get; set; }

    public virtual DbSet<Map> Maps { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<PathInTarget> PathInTargets { get; set; }

    public virtual DbSet<PathToTarget> PathToTargets { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientCard> PatientCards { get; set; }

    public virtual DbSet<Procedure> Procedures { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionResult> SessionResults { get; set; }

    public virtual DbSet<TargetFile> TargetFiles { get; set; }

    public virtual DbSet<Xray> Xrays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(Config.Default.DB_CONNECTION);

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
            entity.Property(e => e.AddrHouse).HasColumnName("addr_house");
            entity.Property(e => e.AddrRegion).HasColumnName("addr_region");
            entity.Property(e => e.AddrStreet)
                .UseCollation("RTRIM")
                .HasColumnName("addr_street");

            entity.HasOne(d => d.AddrRegionNavigation).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.AddrRegion)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppId);

            entity.ToTable("appointment");

            entity.HasIndex(e => e.AppTime, "IDX_time").IsDescending();

            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.AppDoctor).HasColumnName("app_doctor");
            entity.Property(e => e.AppPatient).HasColumnName("app_patient");
            entity.Property(e => e.AppTime)
                .UseCollation("RTRIM")
                .HasColumnName("app_time");

            entity.HasOne(d => d.AppDoctorNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AppDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.AppPatientNavigation).WithMany(p => p.Appointments).HasForeignKey(d => d.AppPatient);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.CrdId);

            entity.ToTable("card");

            entity.HasIndex(e => e.CrdNumber, "IDX_number");

            entity.HasIndex(e => e.CrdNumber, "UNQ_IDX_number").IsUnique();

            entity.Property(e => e.CrdId).HasColumnName("crd_id");
            entity.Property(e => e.CrdNumber)
                .UseCollation("RTRIM")
                .HasColumnType("TEXT (9)")
                .HasColumnName("crd_number");
            entity.Property(e => e.CrdPatient).HasColumnName("crd_patient");

            entity.HasOne(d => d.CrdPatientNavigation).WithMany(p => p.Cards).HasForeignKey(d => d.CrdPatient);
        });

        modelBuilder.Entity<CardDiagnosis>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("card_diagnosis");

            entity.Property(e => e.C2dDiagnosisFinish).HasColumnName("c2d_diagnosis_finish");
            entity.Property(e => e.C2dDiagnosisStart).HasColumnName("c2d_diagnosis_start");
            entity.Property(e => e.CrdId).HasColumnName("crd_id");
            entity.Property(e => e.CrdNumber)
                .HasColumnType("TEXT (9)")
                .HasColumnName("crd_number");
            entity.Property(e => e.DiaId).HasColumnName("dia_id");
            entity.Property(e => e.DiaName).HasColumnName("dia_name");
        });

        modelBuilder.Entity<Contraindication>(entity =>
        {
            entity.HasKey(e => e.ConId);

            entity.ToTable("contraindication");

            entity.Property(e => e.ConId).HasColumnName("con_id");
            entity.Property(e => e.ConCard).HasColumnName("con_card");
            entity.Property(e => e.ConName)
                .UseCollation("RTRIM")
                .HasColumnName("con_name");

            entity.HasOne(d => d.ConCardNavigation).WithMany(p => p.Contraindications).HasForeignKey(d => d.ConCard);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.DiaId);

            entity.ToTable("diagnosis");

            entity.HasIndex(e => e.DiaName, "IX_diagnosis_dia_name").IsUnique();

            entity.Property(e => e.DiaId).HasColumnName("dia_id");
            entity.Property(e => e.DiaName)
                .UseCollation("RTRIM")
                .HasColumnName("dia_name");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DstId);

            entity.ToTable("district");

            entity.HasIndex(e => e.DstName, "IX_district_dst_name").IsUnique();

            entity.Property(e => e.DstId).HasColumnName("dst_id");
            entity.Property(e => e.DstName)
                .UseCollation("RTRIM")
                .HasColumnName("dst_name");
            entity.Property(e => e.DstRegion).HasColumnName("dst_region");

            entity.HasOne(d => d.DstRegionNavigation).WithMany(p => p.Districts)
                .HasForeignKey(d => d.DstRegion)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DocId);

            entity.ToTable("doctor");

            entity.Property(e => e.DocId).HasColumnName("doc_id");
            entity.Property(e => e.DocName)
                .UseCollation("RTRIM")
                .HasColumnName("doc_name");
            entity.Property(e => e.DocPassword)
                .UseCollation("RTRIM")
                .HasColumnName("doc_password");
            entity.Property(e => e.DocPatronymic)
                .UseCollation("RTRIM")
                .HasColumnName("doc_patronymic");
            entity.Property(e => e.DocSurname)
                .UseCollation("RTRIM")
                .HasColumnName("doc_surname");
        });

        modelBuilder.Entity<DoctorAppointment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("doctor_appointment");

            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.AppTime).HasColumnName("app_time");
            entity.Property(e => e.DocId).HasColumnName("doc_id");
            entity.Property(e => e.DocName).HasColumnName("doc_name");
            entity.Property(e => e.DocPatronymic).HasColumnName("doc_patronymic");
            entity.Property(e => e.DocSurname).HasColumnName("doc_surname");
            entity.Property(e => e.PatId).HasColumnName("pat_id");
            entity.Property(e => e.PatName)
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            entity.Property(e => e.PatPatronymic)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            entity.Property(e => e.PatSurname)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");
        });

        modelBuilder.Entity<DoctorCabinet>(entity =>
        {
            entity.HasKey(e => e.DcId);

            entity.ToTable("doctor_cabinet");

            entity.HasIndex(e => new { e.DcFloor, e.DcCabinetNum }, "IX_doctor_cabinet_dc_floor_dc_cabinet_num").IsUnique();

            entity.Property(e => e.DcId).HasColumnName("dc_id");
            entity.Property(e => e.DcCabinetNum).HasColumnName("dc_cabinet_num");
            entity.Property(e => e.DcFloor).HasColumnName("dc_floor");
            entity.Property(e => e.DcName)
                .UseCollation("RTRIM")
                .HasColumnName("dc_name");
        });

        modelBuilder.Entity<M2mCardDiagnosis>(entity =>
        {
            entity.HasKey(e => new { e.C2dCard, e.C2dDiagnosis });

            entity.ToTable("m2m_card_diagnosis");

            entity.Property(e => e.C2dCard).HasColumnName("c2d_card");
            entity.Property(e => e.C2dDiagnosis).HasColumnName("c2d_diagnosis");
            entity.Property(e => e.C2dDiagnosisFinish)
                .UseCollation("RTRIM")
                .HasColumnName("c2d_diagnosis_finish");
            entity.Property(e => e.C2dDiagnosisStart)
                .UseCollation("RTRIM")
                .HasColumnName("c2d_diagnosis_start");

            entity.HasOne(d => d.C2dCardNavigation).WithMany(p => p.M2mCardDiagnoses).HasForeignKey(d => d.C2dCard);

            entity.HasOne(d => d.C2dDiagnosisNavigation).WithMany(p => p.M2mCardDiagnoses)
                .HasForeignKey(d => d.C2dDiagnosis)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Map>(entity =>
        {
            entity.ToTable("map");

            entity.HasIndex(e => e.MapName, "IX_map_map_name").IsUnique();

            entity.Property(e => e.MapId).HasColumnName("map_id");
            entity.Property(e => e.MapCoordinatesJson).HasColumnName("map_coordinates_json");
            entity.Property(e => e.MapCreatedAt)
                .UseCollation("RTRIM")
                .HasColumnName("map_created_at");
            entity.Property(e => e.MapCreatedBy).HasColumnName("map_created_by");
            entity.Property(e => e.MapName)
                .UseCollation("RTRIM")
                .HasColumnName("map_name");

            entity.HasOne(d => d.MapCreatedByNavigation).WithMany(p => p.Maps)
                .HasForeignKey(d => d.MapCreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NtId);

            entity.ToTable("note");

            entity.Property(e => e.NtId).HasColumnName("nt_id");
            entity.Property(e => e.NtDoctor).HasColumnName("nt_doctor");
            entity.Property(e => e.NtPatient).HasColumnName("nt_patient");
            entity.Property(e => e.NtText).HasColumnName("nt_text");

            entity.HasOne(d => d.NtDoctorNavigation).WithMany(p => p.Notes)
                .HasForeignKey(d => d.NtDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.NtPatientNavigation).WithMany(p => p.Notes).HasForeignKey(d => d.NtPatient);
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.OpId);

            entity.ToTable("operation");

            entity.Property(e => e.OpId).HasColumnName("op_id");
            entity.Property(e => e.OpAsingnedBy).HasColumnName("op_asingned_by");
            entity.Property(e => e.OpCabinet).HasColumnName("op_cabinet");
            entity.Property(e => e.OpCard).HasColumnName("op_card");
            entity.Property(e => e.OpDateTime)
                .UseCollation("NOCASE")
                .HasColumnName("op_date_time");
            entity.Property(e => e.OpName)
                .UseCollation("RTRIM")
                .HasColumnName("op_name");

            entity.HasOne(d => d.OpAsingnedByNavigation).WithMany(p => p.Operations)
                .HasForeignKey(d => d.OpAsingnedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.OpCabinetNavigation).WithMany(p => p.Operations)
                .HasForeignKey(d => d.OpCabinet)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.OpCardNavigation).WithMany(p => p.Operations).HasForeignKey(d => d.OpCard);
        });

        modelBuilder.Entity<PathInTarget>(entity =>
        {
            entity.HasKey(e => new { e.PitSession, e.PitTargetId });

            entity.ToTable("path_in_target");

            entity.Property(e => e.PitSession).HasColumnName("pit_session");
            entity.Property(e => e.PitTargetId).HasColumnName("pit_target_id");
            entity.Property(e => e.PitCoordinatesJson).HasColumnName("pit_coordinates_json");

            entity.HasOne(d => d.PitSessionNavigation).WithMany(p => p.PathInTargets).HasForeignKey(d => d.PitSession);
        });

        modelBuilder.Entity<PathToTarget>(entity =>
        {
            entity.HasKey(e => new { e.PttSession, e.PttNum });

            entity.ToTable("path_to_target");

            entity.Property(e => e.PttSession).HasColumnName("ptt_session");
            entity.Property(e => e.PttNum).HasColumnName("ptt_num");
            entity.Property(e => e.PttAngleDistance).HasColumnName("ptt_angle_distance");
            entity.Property(e => e.PttAngleSpeed).HasColumnName("ptt_angle_speed");
            entity.Property(e => e.PttApproachSpeed).HasColumnName("ptt_approach_speed");
            entity.Property(e => e.PttCoordinatesJson).HasColumnName("ptt_coordinates_json");
            entity.Property(e => e.PttTime).HasColumnName("ptt_time");

            entity.HasOne(d => d.PttSessionNavigation).WithMany(p => p.PathToTargets).HasForeignKey(d => d.PttSession);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatId);

            entity.ToTable("patient");

            entity.HasIndex(e => new { e.PatName, e.PatSurname, e.PatPatronymic }, "IDX_name");

            entity.Property(e => e.PatId).HasColumnName("pat_id");
            entity.Property(e => e.PatAddress).HasColumnName("pat_address");
            entity.Property(e => e.PatDateOfBirth)
                .UseCollation("NOCASE")
                .HasColumnName("pat_date_of_birth");
            entity.Property(e => e.PatName)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            entity.Property(e => e.PatPatronymic)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            entity.Property(e => e.PatPhoneHome)
                .UseCollation("NOCASE")
                .HasColumnName("pat_phone_home");
            entity.Property(e => e.PatPhoneMobile)
                .UseCollation("NOCASE")
                .HasColumnName("pat_phone_mobile");
            entity.Property(e => e.PatSurname)
                .UseCollation("NOCASE")
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");

            entity.HasOne(d => d.PatAddressNavigation).WithMany(p => p.Patients).HasForeignKey(d => d.PatAddress);
        });

        modelBuilder.Entity<PatientCard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("patient_card");

            entity.Property(e => e.CrdId).HasColumnName("crd_id");
            entity.Property(e => e.CrdNumber)
                .HasColumnType("TEXT (9)")
                .HasColumnName("crd_number");
            entity.Property(e => e.PatId).HasColumnName("pat_id");
            entity.Property(e => e.PatName)
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            entity.Property(e => e.PatPatronymic)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            entity.Property(e => e.PatSurname)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");
        });

        modelBuilder.Entity<Procedure>(entity =>
        {
            entity.HasKey(e => e.ProId);

            entity.ToTable("procedure");

            entity.Property(e => e.ProId).HasColumnName("pro_id");
            entity.Property(e => e.ProAssignedBy).HasColumnName("pro_assigned_by");
            entity.Property(e => e.ProAssignedTo).HasColumnName("pro_assigned_to");
            entity.Property(e => e.ProCabinet).HasColumnName("pro_cabinet");
            entity.Property(e => e.ProDateTime)
                .UseCollation("NOCASE")
                .HasColumnName("pro_date_time");
            entity.Property(e => e.ProName)
                .UseCollation("NOCASE")
                .HasColumnName("pro_name");

            entity.HasOne(d => d.ProAssignedByNavigation).WithMany(p => p.Procedures)
                .HasForeignKey(d => d.ProAssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ProAssignedToNavigation).WithMany(p => p.Procedures).HasForeignKey(d => d.ProAssignedTo);

            entity.HasOne(d => d.ProCabinetNavigation).WithMany(p => p.Procedures)
                .HasForeignKey(d => d.ProCabinet)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RgnId);

            entity.ToTable("region");

            entity.HasIndex(e => e.RgnName, "IX_region_rgn_name").IsUnique();

            entity.Property(e => e.RgnId).HasColumnName("rgn_id");
            entity.Property(e => e.RgnName)
                .UseCollation("NOCASE")
                .HasColumnName("rgn_name");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("report");

            entity.Property(e => e.AppTime).HasColumnName("app_time");
            entity.Property(e => e.DocName).HasColumnName("doc_name");
            entity.Property(e => e.DocPatronymic).HasColumnName("doc_patronymic");
            entity.Property(e => e.DocSurname).HasColumnName("doc_surname");
            entity.Property(e => e.MapName).HasColumnName("map_name");
            entity.Property(e => e.PatName)
                .HasColumnType("TEXT (20)")
                .HasColumnName("pat_name");
            entity.Property(e => e.PatPatronymic)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_patronymic");
            entity.Property(e => e.PatSurname)
                .HasColumnType("TEXT (30)")
                .HasColumnName("pat_surname");
            entity.Property(e => e.SesDate).HasColumnName("ses_date");
            entity.Property(e => e.SresDeviation).HasColumnName("sres_deviation");
            entity.Property(e => e.SresDispersion).HasColumnName("sres_dispersion");
            entity.Property(e => e.SresId).HasColumnName("sres_id");
            entity.Property(e => e.SresMathExp).HasColumnName("sres_math_exp");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SesId);

            entity.ToTable("session");

            entity.HasIndex(e => e.SesLogFilePath, "IX_session_ses_log_file_path").IsUnique();

            entity.Property(e => e.SesId).HasColumnName("ses_id");
            entity.Property(e => e.SesAppointment).HasColumnName("ses_appointment");
            entity.Property(e => e.SesDate)
                .UseCollation("NOCASE")
                .HasColumnName("ses_date");
            entity.Property(e => e.SesLogFilePath)
                .UseCollation("RTRIM")
                .HasColumnName("ses_log_file_path");
            entity.Property(e => e.SesMap).HasColumnName("ses_map");

            entity.HasOne(d => d.SesAppointmentNavigation).WithMany(p => p.Sessions).HasForeignKey(d => d.SesAppointment);

            entity.HasOne(d => d.SesMapNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SesMap)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SessionResult>(entity =>
        {
            entity.HasKey(e => e.SresId);

            entity.ToTable("session_result");

            entity.Property(e => e.SresId)
                .ValueGeneratedNever()
                .HasColumnName("sres_id");
            entity.Property(e => e.SresDeviation).HasColumnName("sres_deviation");
            entity.Property(e => e.SresDispersion).HasColumnName("sres_dispersion");
            entity.Property(e => e.SresMathExp).HasColumnName("sres_math_exp");
            entity.Property(e => e.SresScore).HasColumnName("sres_score");

            entity.HasOne(d => d.Sres).WithOne(p => p.SessionResult).HasForeignKey<SessionResult>(d => d.SresId);
        });

        modelBuilder.Entity<TargetFile>(entity =>
        {
            entity.HasKey(e => e.TfId);

            entity.ToTable("target_file");

            entity.HasIndex(e => e.TfFilepath, "IX_target_file_tf_filepath").IsUnique();

            entity.Property(e => e.TfId).HasColumnName("tf_id");
            entity.Property(e => e.TfAddedBy).HasColumnName("tf_added_by");
            entity.Property(e => e.TfFilepath)
                .UseCollation("RTRIM")
                .HasColumnName("tf_filepath");

            entity.HasOne(d => d.TfAddedByNavigation).WithMany(p => p.TargetFiles)
                .HasForeignKey(d => d.TfAddedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Xray>(entity =>
        {
            entity.HasKey(e => e.XrId);

            entity.ToTable("xray");

            entity.HasIndex(e => e.XrFilePath, "IX_xray_xr_file_path").IsUnique();

            entity.Property(e => e.XrId).HasColumnName("xr_id");
            entity.Property(e => e.XrCard).HasColumnName("xr_card");
            entity.Property(e => e.XrDate)
                .UseCollation("NOCASE")
                .HasColumnName("xr_date");
            entity.Property(e => e.XrDescription).HasColumnName("xr_description");
            entity.Property(e => e.XrFilePath)
                .UseCollation("RTRIM")
                .HasColumnName("xr_file_path");

            entity.HasOne(d => d.XrCardNavigation).WithMany(p => p.Xrays).HasForeignKey(d => d.XrCard);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

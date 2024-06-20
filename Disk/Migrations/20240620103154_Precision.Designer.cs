﻿// <auto-generated />
using Disk.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Disk.Migrations
{
    [DbContext(typeof(DiskContext))]
    [Migration("20240620103154_Precision")]
    partial class Precision
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("Disk.Entities.Appointment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("app_id");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("app_date_time");

                    b.Property<long>("Doctor")
                        .HasColumnType("INTEGER")
                        .HasColumnName("app_doctor");

                    b.Property<long>("Patient")
                        .HasColumnType("INTEGER")
                        .HasColumnName("app_patient");

                    b.HasKey("Id");

                    b.HasIndex("Doctor");

                    b.HasIndex("Patient");

                    b.ToTable("appointment", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Doctor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("doc_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("doc_name")
                        .UseCollation("NOCASE");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("doc_password");

                    b.Property<string>("Patronymic")
                        .HasColumnType("TEXT")
                        .HasColumnName("doc_patronymic")
                        .UseCollation("NOCASE");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("doc_surname")
                        .UseCollation("NOCASE");

                    b.HasKey("Id");

                    b.ToTable("doctor", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Map", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("map_id");

                    b.Property<string>("CoordinatesJson")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("map_coordinates_json");

                    b.Property<string>("CreatedAtDateTime")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("map_created_at_date_time");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("map_created_by");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("map_name")
                        .UseCollation("NOCASE");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex(new[] { "Name" }, "IX_map_map_name")
                        .IsUnique();

                    b.ToTable("map", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.PathInTarget", b =>
                {
                    b.Property<long>("Session")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pit_session");

                    b.Property<long>("TargetId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pit_target_id");

                    b.Property<string>("CoordinatesJson")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("pit_coordinates_json");

                    b.Property<float>("Precision")
                        .HasColumnType("REAL")
                        .HasColumnName("pit_precision");

                    b.HasKey("Session", "TargetId");

                    b.ToTable("path_in_target", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.PathToTarget", b =>
                {
                    b.Property<long>("Session")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ptt_session");

                    b.Property<long>("TargetNum")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ptt_target_num");

                    b.Property<double>("AngleDistance")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_ange_distance");

                    b.Property<double>("AngleSpeed")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_angle_speed");

                    b.Property<double>("ApproachSpeed")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_approach_speed");

                    b.Property<string>("CoordinatesJson")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ptt_coordinates_json");

                    b.Property<double>("Time")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_time");

                    b.HasKey("Session", "TargetNum");

                    b.ToTable("path_to_target", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Patient", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("pat_id");

                    b.Property<string>("DateOfBirth")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("pat_date_of_birth");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT (20)")
                        .HasColumnName("pat_name")
                        .UseCollation("NOCASE");

                    b.Property<string>("Patronymic")
                        .HasColumnType("TEXT (30)")
                        .HasColumnName("pat_patronymic")
                        .UseCollation("NOCASE");

                    b.Property<string>("PhoneHome")
                        .HasColumnType("TEXT")
                        .HasColumnName("pat_phone_home");

                    b.Property<string>("PhoneMobile")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("pat_phone_mobile");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT (30)")
                        .HasColumnName("pat_surname")
                        .UseCollation("NOCASE");

                    b.HasKey("Id");

                    b.ToTable("patient", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Session", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ses_id");

                    b.Property<long>("Appointment")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ses_appointment");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ses_date_time");

                    b.Property<string>("LogFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ses_log_file_path");

                    b.Property<long>("Map")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ses_map");

                    b.Property<float>("MaxXAngle")
                        .HasColumnType("REAL")
                        .HasColumnName("ses_max_x_angle");

                    b.Property<float>("MaxYAngle")
                        .HasColumnType("REAL")
                        .HasColumnName("ses_max_y_angle");

                    b.HasKey("Id");

                    b.HasIndex("Appointment");

                    b.HasIndex("Map");

                    b.HasIndex(new[] { "LogFilePath" }, "IX_session_ses_log_file_path")
                        .IsUnique();

                    b.ToTable("session", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.SessionResult", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sres_id");

                    b.Property<double>("Deviation")
                        .HasColumnType("REAL")
                        .HasColumnName("sres_deviation");

                    b.Property<double>("Dispersion")
                        .HasColumnType("REAL")
                        .HasColumnName("sres_dispersion");

                    b.Property<double>("MathExp")
                        .HasColumnType("REAL")
                        .HasColumnName("sres_math_exp");

                    b.Property<long>("Score")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sres_score");

                    b.HasKey("Id");

                    b.ToTable("session_result", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Appointment", b =>
                {
                    b.HasOne("Disk.Entities.Doctor", "DoctorNavigation")
                        .WithMany("Appointments")
                        .HasForeignKey("Doctor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Disk.Entities.Patient", "PatientNavigation")
                        .WithMany("Appointments")
                        .HasForeignKey("Patient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DoctorNavigation");

                    b.Navigation("PatientNavigation");
                });

            modelBuilder.Entity("Disk.Entities.Map", b =>
                {
                    b.HasOne("Disk.Entities.Doctor", "CreatedByNavigation")
                        .WithMany("Maps")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedByNavigation");
                });

            modelBuilder.Entity("Disk.Entities.PathInTarget", b =>
                {
                    b.HasOne("Disk.Entities.Session", "SessionNavigation")
                        .WithMany("PathInTargets")
                        .HasForeignKey("Session")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SessionNavigation");
                });

            modelBuilder.Entity("Disk.Entities.PathToTarget", b =>
                {
                    b.HasOne("Disk.Entities.Session", "SessionNavigation")
                        .WithMany("PathToTargets")
                        .HasForeignKey("Session")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SessionNavigation");
                });

            modelBuilder.Entity("Disk.Entities.Session", b =>
                {
                    b.HasOne("Disk.Entities.Appointment", "AppointmentNavigation")
                        .WithMany("Sessions")
                        .HasForeignKey("Appointment")
                        .IsRequired();

                    b.HasOne("Disk.Entities.Map", "MapNavigation")
                        .WithMany("Sessions")
                        .HasForeignKey("Map")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AppointmentNavigation");

                    b.Navigation("MapNavigation");
                });

            modelBuilder.Entity("Disk.Entities.SessionResult", b =>
                {
                    b.HasOne("Disk.Entities.Session", "Sres")
                        .WithOne("SessionResult")
                        .HasForeignKey("Disk.Entities.SessionResult", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Sres");
                });

            modelBuilder.Entity("Disk.Entities.Appointment", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Disk.Entities.Doctor", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Maps");
                });

            modelBuilder.Entity("Disk.Entities.Map", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Disk.Entities.Patient", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("Disk.Entities.Session", b =>
                {
                    b.Navigation("PathInTargets");

                    b.Navigation("PathToTargets");

                    b.Navigation("SessionResult");
                });
#pragma warning restore 612, 618
        }
    }
}
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
    [Migration("20250318130351_MathExpToShiftRename")]
    partial class MathExpToShiftRename
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("Disk.Entities.Attempt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("att_id");

                    b.Property<int>("CursorRadius")
                        .HasColumnType("INTEGER")
                        .HasColumnName("att_cursor_radius");

                    b.Property<string>("DateTime")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("att_date_time");

                    b.Property<string>("LogFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("att_log_file_path");

                    b.Property<float>("MaxXAngle")
                        .HasColumnType("REAL")
                        .HasColumnName("att_max_x_angle");

                    b.Property<float>("MaxYAngle")
                        .HasColumnType("REAL")
                        .HasColumnName("att_max_y_angle");

                    b.Property<int>("SamplingInterval")
                        .HasColumnType("INTEGER")
                        .HasColumnName("att_sampling_interval");

                    b.Property<long>("Session")
                        .HasColumnType("INTEGER")
                        .HasColumnName("att_session");

                    b.Property<int>("TargetRadius")
                        .HasColumnType("INTEGER")
                        .HasColumnName("att_target_radius");

                    b.HasKey("Id");

                    b.HasIndex("Session");

                    b.HasIndex(new[] { "LogFilePath" }, "IX_UNQ_attempt_att_log_file_path")
                        .IsUnique();

                    b.ToTable("attempt", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.AttemptResult", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ares_id");

                    b.Property<double>("DeviationX")
                        .HasColumnType("REAL")
                        .HasColumnName("ares_deviation_x");

                    b.Property<double>("DeviationY")
                        .HasColumnType("REAL")
                        .HasColumnName("ares_deviation_y");

                    b.Property<double>("MathExpX")
                        .HasColumnType("REAL")
                        .HasColumnName("ares_shift_x");

                    b.Property<double>("MathExpY")
                        .HasColumnType("REAL")
                        .HasColumnName("ares_shift_y");

                    b.Property<long>("Score")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ares_score");

                    b.HasKey("Id");

                    b.ToTable("attempt_result", (string)null);
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

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasColumnName("map_description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("map_name")
                        .UseCollation("NOCASE");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Name" }, "IX_UNQ_map_map_name")
                        .IsUnique();

                    b.ToTable("map", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.PathInTarget", b =>
                {
                    b.Property<long>("Attempt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pit_attempt");

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

                    b.HasKey("Attempt", "TargetId");

                    b.ToTable("path_in_target", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.PathToTarget", b =>
                {
                    b.Property<long>("Attempt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ptt_attempt");

                    b.Property<long>("TargetNum")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ptt_target_id");

                    b.Property<double>("ApproachSpeed")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_approach_speed");

                    b.Property<double>("AverageSpeed")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_average_speed");

                    b.Property<string>("CoordinatesJson")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ptt_coordinates_json");

                    b.Property<double>("Distance")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_distance");

                    b.Property<double>("Time")
                        .HasColumnType("REAL")
                        .HasColumnName("ptt_time");

                    b.HasKey("Attempt", "TargetNum");

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

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ses_date");

                    b.Property<long>("Map")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ses_map");

                    b.Property<long>("Patient")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ses_patient");

                    b.HasKey("Id");

                    b.HasIndex("Map");

                    b.HasIndex("Patient");

                    b.HasIndex(new[] { "Date", "Map", "Patient" }, "IX_UNQ_session_date_map_id_patient_id")
                        .IsUnique();

                    b.ToTable("session", (string)null);
                });

            modelBuilder.Entity("Disk.Entities.Attempt", b =>
                {
                    b.HasOne("Disk.Entities.Session", "SessionNavigation")
                        .WithMany("Attempts")
                        .HasForeignKey("Session")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SessionNavigation");
                });

            modelBuilder.Entity("Disk.Entities.AttemptResult", b =>
                {
                    b.HasOne("Disk.Entities.Attempt", "Attempt")
                        .WithOne("AttemptResult")
                        .HasForeignKey("Disk.Entities.AttemptResult", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attempt");
                });

            modelBuilder.Entity("Disk.Entities.PathInTarget", b =>
                {
                    b.HasOne("Disk.Entities.Attempt", "AttemptNavigation")
                        .WithMany("PathInTargets")
                        .HasForeignKey("Attempt")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AttemptNavigation");
                });

            modelBuilder.Entity("Disk.Entities.PathToTarget", b =>
                {
                    b.HasOne("Disk.Entities.Attempt", "AttemptNavigation")
                        .WithMany("PathToTargets")
                        .HasForeignKey("Attempt")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AttemptNavigation");
                });

            modelBuilder.Entity("Disk.Entities.Session", b =>
                {
                    b.HasOne("Disk.Entities.Map", "MapNavigation")
                        .WithMany("Sessions")
                        .HasForeignKey("Map")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Disk.Entities.Patient", "PatientNavigation")
                        .WithMany("Sessions")
                        .HasForeignKey("Patient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MapNavigation");

                    b.Navigation("PatientNavigation");
                });

            modelBuilder.Entity("Disk.Entities.Attempt", b =>
                {
                    b.Navigation("AttemptResult");

                    b.Navigation("PathInTargets");

                    b.Navigation("PathToTargets");
                });

            modelBuilder.Entity("Disk.Entities.Map", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Disk.Entities.Patient", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Disk.Entities.Session", b =>
                {
                    b.Navigation("Attempts");
                });
#pragma warning restore 612, 618
        }
    }
}

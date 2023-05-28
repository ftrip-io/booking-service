﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ftrip.io.booking_service.Persistance;

namespace ftrip.io.booking_service.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230528194740_added accommodation configuration")]
    partial class addedaccommodationconfiguration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ftrip.io.booking_service.AccommodationConfiguration.Domain.Accommodation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AccommodationId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsManualAccept")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Accommodations");
                });

            modelBuilder.Entity("ftrip.io.booking_service.ReservationRequests.Domain.ReservationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AccomodationId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("GuestId")
                        .HasColumnType("char(36)");

                    b.Property<int>("GuestNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ReservationRequests");
                });

            modelBuilder.Entity("ftrip.io.booking_service.Reservations.Domain.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AccomodationId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("GuestId")
                        .HasColumnType("char(36)");

                    b.Property<int>("GuestNumber")
                        .HasColumnType("int");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("ftrip.io.booking_service.ReservationRequests.Domain.ReservationRequest", b =>
                {
                    b.OwnsOne("ftrip.io.booking_service.Common.Domain.DatePeriod", "DatePeriod", b1 =>
                        {
                            b1.Property<Guid>("ReservationRequestId")
                                .HasColumnType("char(36)");

                            b1.Property<DateTime>("DateFrom")
                                .HasColumnType("datetime(6)");

                            b1.Property<DateTime>("DateTo")
                                .HasColumnType("datetime(6)");

                            b1.HasKey("ReservationRequestId");

                            b1.ToTable("ReservationRequests");

                            b1.WithOwner()
                                .HasForeignKey("ReservationRequestId");
                        });
                });

            modelBuilder.Entity("ftrip.io.booking_service.Reservations.Domain.Reservation", b =>
                {
                    b.OwnsOne("ftrip.io.booking_service.Common.Domain.DatePeriod", "DatePeriod", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("char(36)");

                            b1.Property<DateTime>("DateFrom")
                                .HasColumnType("datetime(6)");

                            b1.Property<DateTime>("DateTo")
                                .HasColumnType("datetime(6)");

                            b1.HasKey("ReservationId");

                            b1.ToTable("Reservations");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using house_manager.Utilities;

#nullable disable

namespace house_manager.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240626135347_ParkingSpacesChanges2")]
    partial class ParkingSpacesChanges2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApartmentLodger", b =>
                {
                    b.Property<int>("ApartmentsId")
                        .HasColumnType("int");

                    b.Property<int>("OwnersId")
                        .HasColumnType("int");

                    b.HasKey("ApartmentsId", "OwnersId");

                    b.HasIndex("OwnersId");

                    b.ToTable("ApartmentLodger");
                });

            modelBuilder.Entity("CarLodger", b =>
                {
                    b.Property<int>("CarsId")
                        .HasColumnType("int");

                    b.Property<int>("OwnersId")
                        .HasColumnType("int");

                    b.HasKey("CarsId", "OwnersId");

                    b.HasIndex("OwnersId");

                    b.ToTable("CarLodger");
                });

            modelBuilder.Entity("house_manager.Models.Apartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResidentsNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.ToTable("Apartments");
                });

            modelBuilder.Entity("house_manager.Models.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("house_manager.Models.House", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ParkingPlacesNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Houses");
                });

            modelBuilder.Entity("house_manager.Models.Lodger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("HouseId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassportNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pathronymic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.ToTable("Lodgers");
                });

            modelBuilder.Entity("house_manager.Models.OwnedApartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApartmentId")
                        .HasColumnType("int");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<float>("OwnershipPercentage")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ApartmentId");

                    b.HasIndex("OwnerId");

                    b.ToTable("OwnedApartments");
                });

            modelBuilder.Entity("house_manager.Models.OwnedCar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("OwnerId");

                    b.ToTable("OwnedCars");
                });

            modelBuilder.Entity("house_manager.Models.OwnedParkingSpace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<int>("ParkingSpaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParkingSpaceId");

                    b.ToTable("OwnedParkingSpaces");
                });

            modelBuilder.Entity("house_manager.Models.ParkingSpace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("ParkingSpaces");
                });

            modelBuilder.Entity("ApartmentLodger", b =>
                {
                    b.HasOne("house_manager.Models.Apartment", null)
                        .WithMany()
                        .HasForeignKey("ApartmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("house_manager.Models.Lodger", null)
                        .WithMany()
                        .HasForeignKey("OwnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarLodger", b =>
                {
                    b.HasOne("house_manager.Models.Car", null)
                        .WithMany()
                        .HasForeignKey("CarsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("house_manager.Models.Lodger", null)
                        .WithMany()
                        .HasForeignKey("OwnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("house_manager.Models.Apartment", b =>
                {
                    b.HasOne("house_manager.Models.House", "House")
                        .WithMany("Apartments")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("House");
                });

            modelBuilder.Entity("house_manager.Models.Lodger", b =>
                {
                    b.HasOne("house_manager.Models.House", null)
                        .WithMany("Lodgers")
                        .HasForeignKey("HouseId");
                });

            modelBuilder.Entity("house_manager.Models.OwnedApartment", b =>
                {
                    b.HasOne("house_manager.Models.Apartment", "Apartment")
                        .WithMany("OwnersOwned")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("house_manager.Models.Lodger", "Owner")
                        .WithMany("OwnedApartments")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("house_manager.Models.OwnedCar", b =>
                {
                    b.HasOne("house_manager.Models.Car", "Car")
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("house_manager.Models.Lodger", "Owner")
                        .WithMany("OwnedCars")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("house_manager.Models.OwnedParkingSpace", b =>
                {
                    b.HasOne("house_manager.Models.Lodger", "Owner")
                        .WithMany("OwnedParkingSpaces")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("house_manager.Models.ParkingSpace", "ParkingSpace")
                        .WithMany()
                        .HasForeignKey("ParkingSpaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("ParkingSpace");
                });

            modelBuilder.Entity("house_manager.Models.ParkingSpace", b =>
                {
                    b.HasOne("house_manager.Models.House", "House")
                        .WithMany("ParkingSpaces")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("House");
                });

            modelBuilder.Entity("house_manager.Models.Apartment", b =>
                {
                    b.Navigation("OwnersOwned");
                });

            modelBuilder.Entity("house_manager.Models.House", b =>
                {
                    b.Navigation("Apartments");

                    b.Navigation("Lodgers");

                    b.Navigation("ParkingSpaces");
                });

            modelBuilder.Entity("house_manager.Models.Lodger", b =>
                {
                    b.Navigation("OwnedApartments");

                    b.Navigation("OwnedCars");

                    b.Navigation("OwnedParkingSpaces");
                });
#pragma warning restore 612, 618
        }
    }
}

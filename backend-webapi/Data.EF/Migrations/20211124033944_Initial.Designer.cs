﻿// <auto-generated />
using System;
using Backend.WebApi.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.WebApi.Data.EF.Migrations;

[DbContext(typeof(ApiDbContext))]
[Migration("20211124033944_Initial")]
partial class Initial
{
    protected void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "6.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

        modelBuilder.Entity("Backend.WebApi.Model.UserInteraction", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("Created")
                    .HasColumnType("datetime2");

                b.Property<DateTime>("Deadline")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("IsOpen")
                    .HasColumnType("bit");

                b.HasKey("Id");

                b.ToTable("UserInteraction");
            });
#pragma warning restore 612, 618
    }
}


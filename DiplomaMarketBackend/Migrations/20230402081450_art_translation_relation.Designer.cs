﻿// <auto-generated />
using System;
using DiplomaMarketBackend.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    [DbContext(typeof(BaseContext))]
    [Migration("20230402081450_art_translation_relation")]
    partial class art_translation_relation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleCharacteristic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int?>("NameId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<int?>("TitleId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("NameId");

                    b.HasIndex("TitleId");

                    b.ToTable("ArticleCharacteristics");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BrandId")
                        .HasColumnType("integer");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DescriptionId")
                        .HasColumnType("integer");

                    b.Property<int>("DocketId")
                        .HasColumnType("integer");

                    b.Property<decimal>("OldPrice")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("SellStatus")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<int>("TitleId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("rztk_art_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DescriptionId");

                    b.HasIndex("DocketId");

                    b.HasIndex("TitleId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.BrandModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("LogoURL")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("rztk_brand_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.BreadcrumbsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleModelId")
                        .HasColumnType("integer");

                    b.Property<int?>("TitleId")
                        .HasColumnType("integer");

                    b.Property<string>("href")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArticleModelId");

                    b.HasIndex("TitleId");

                    b.ToTable("Breadcrumbs");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.CategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("DescriptionId")
                        .HasColumnType("integer");

                    b.Property<int?>("NameId")
                        .HasColumnType("integer");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("integer");

                    b.Property<int?>("rztk_cat_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DescriptionId");

                    b.HasIndex("NameId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.CharacteristicValueModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleCharacteristicId")
                        .HasColumnType("integer");

                    b.Property<int?>("TitleId")
                        .HasColumnType("integer");

                    b.Property<string>("href")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArticleCharacteristicId");

                    b.HasIndex("TitleId");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ImageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleModelId")
                        .HasColumnType("integer");

                    b.Property<int?>("base_actionId")
                        .HasColumnType("integer");

                    b.Property<int?>("bigId")
                        .HasColumnType("integer");

                    b.Property<int?>("big_tileId")
                        .HasColumnType("integer");

                    b.Property<int?>("largeId")
                        .HasColumnType("integer");

                    b.Property<int?>("mediumId")
                        .HasColumnType("integer");

                    b.Property<int?>("mobile_largeId")
                        .HasColumnType("integer");

                    b.Property<int?>("mobile_mediumId")
                        .HasColumnType("integer");

                    b.Property<int?>("originalId")
                        .HasColumnType("integer");

                    b.Property<int?>("previewId")
                        .HasColumnType("integer");

                    b.Property<int>("smallId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleModelId");

                    b.HasIndex("base_actionId");

                    b.HasIndex("bigId");

                    b.HasIndex("big_tileId");

                    b.HasIndex("largeId");

                    b.HasIndex("mediumId");

                    b.HasIndex("mobile_largeId");

                    b.HasIndex("mobile_mediumId");

                    b.HasIndex("originalId");

                    b.HasIndex("previewId");

                    b.HasIndex("smallId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.Language", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.PictureModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("height")
                        .HasColumnType("integer");

                    b.Property<string>("url")
                        .HasColumnType("text");

                    b.Property<int?>("width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.TagModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleModelId")
                        .HasColumnType("integer");

                    b.Property<int?>("NameId")
                        .HasColumnType("integer");

                    b.Property<int?>("Priority")
                        .HasColumnType("integer");

                    b.Property<int?>("TitleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleModelId");

                    b.HasIndex("NameId");

                    b.HasIndex("TitleId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.TextContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("OriginalLanguageId")
                        .HasColumnType("text");

                    b.Property<string>("OriginalText")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OriginalLanguageId");

                    b.ToTable("textContents");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.Translation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("LanguageId")
                        .HasColumnType("text");

                    b.Property<int>("TextContentId")
                        .HasColumnType("integer");

                    b.Property<string>("TranslationString")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("TextContentId");

                    b.ToTable("translations");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.VideoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleModelId")
                        .HasColumnType("integer");

                    b.Property<string>("ExternalId")
                        .HasColumnType("text");

                    b.Property<string>("Order")
                        .HasColumnType("text");

                    b.Property<string>("PreviewURL")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArticleModelId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.VideoTypeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("VideoTypes");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.WarningModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleModelId")
                        .HasColumnType("integer");

                    b.Property<int?>("MessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleModelId");

                    b.HasIndex("MessageId");

                    b.ToTable("Warnings");
                });

            modelBuilder.Entity("Lessons3.Entity.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RegDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleCharacteristic", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", "Article")
                        .WithMany("ArticleCharacteristics")
                        .HasForeignKey("ArticleId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.CategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Name")
                        .WithMany()
                        .HasForeignKey("NameId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId");

                    b.Navigation("Article");

                    b.Navigation("Category");

                    b.Navigation("Name");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.BrandModel", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.CategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Description")
                        .WithMany()
                        .HasForeignKey("DescriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Docket")
                        .WithMany()
                        .HasForeignKey("DocketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");

                    b.Navigation("Description");

                    b.Navigation("Docket");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.BreadcrumbsModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", null)
                        .WithMany("Breadcrumbs")
                        .HasForeignKey("ArticleModelId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.CategoryModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Description")
                        .WithMany()
                        .HasForeignKey("DescriptionId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Name")
                        .WithMany()
                        .HasForeignKey("NameId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.CategoryModel", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("Description");

                    b.Navigation("Name");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.CharacteristicValueModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleCharacteristic", null)
                        .WithMany("Values")
                        .HasForeignKey("ArticleCharacteristicId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ImageModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", null)
                        .WithMany("Images")
                        .HasForeignKey("ArticleModelId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "base_action")
                        .WithMany()
                        .HasForeignKey("base_actionId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "big")
                        .WithMany()
                        .HasForeignKey("bigId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "big_tile")
                        .WithMany()
                        .HasForeignKey("big_tileId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "large")
                        .WithMany()
                        .HasForeignKey("largeId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "medium")
                        .WithMany()
                        .HasForeignKey("mediumId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "mobile_large")
                        .WithMany()
                        .HasForeignKey("mobile_largeId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "mobile_medium")
                        .WithMany()
                        .HasForeignKey("mobile_mediumId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "original")
                        .WithMany()
                        .HasForeignKey("originalId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "preview")
                        .WithMany()
                        .HasForeignKey("previewId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.PictureModel", "small")
                        .WithMany()
                        .HasForeignKey("smallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("base_action");

                    b.Navigation("big");

                    b.Navigation("big_tile");

                    b.Navigation("large");

                    b.Navigation("medium");

                    b.Navigation("mobile_large");

                    b.Navigation("mobile_medium");

                    b.Navigation("original");

                    b.Navigation("preview");

                    b.Navigation("small");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.TagModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", null)
                        .WithMany("Tags")
                        .HasForeignKey("ArticleModelId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Name")
                        .WithMany()
                        .HasForeignKey("NameId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId");

                    b.Navigation("Name");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.TextContent", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.Language", "OriginalLanguage")
                        .WithMany()
                        .HasForeignKey("OriginalLanguageId");

                    b.Navigation("OriginalLanguage");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.Translation", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "TextContent")
                        .WithMany("Translations")
                        .HasForeignKey("TextContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("TextContent");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.VideoModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", null)
                        .WithMany("Video")
                        .HasForeignKey("ArticleModelId");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.WarningModel", b =>
                {
                    b.HasOne("DiplomaMarketBackend.Entity.Models.ArticleModel", null)
                        .WithMany("Warning")
                        .HasForeignKey("ArticleModelId");

                    b.HasOne("DiplomaMarketBackend.Entity.Models.TextContent", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleCharacteristic", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.ArticleModel", b =>
                {
                    b.Navigation("ArticleCharacteristics");

                    b.Navigation("Breadcrumbs");

                    b.Navigation("Images");

                    b.Navigation("Tags");

                    b.Navigation("Video");

                    b.Navigation("Warning");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.CategoryModel", b =>
                {
                    b.Navigation("ChildCategories");
                });

            modelBuilder.Entity("DiplomaMarketBackend.Entity.Models.TextContent", b =>
                {
                    b.Navigation("Translations");
                });
#pragma warning restore 612, 618
        }
    }
}

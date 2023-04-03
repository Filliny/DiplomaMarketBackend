using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //this.Down(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LogoURL = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    RegDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "textContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalText = table.Column<string>(type: "text", nullable: true),
                    OriginalLanguageId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_textContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_textContents_Languages_OriginalLanguageId",
                        column: x => x.OriginalLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<int>(type: "integer", nullable: true),
                    DescriptionId = table.Column<int>(type: "integer", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    rztk_cat_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Categories_textContents_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "textContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categories_textContents_NameId",
                        column: x => x.NameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TranslationString = table.Column<string>(type: "text", nullable: true),
                    LanguageId = table.Column<string>(type: "text", nullable: true),
                    TextContentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_translations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_translations_textContents_TextContentId",
                        column: x => x.TextContentId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    DescriptionId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    OldPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SellStatus = table.Column<string>(type: "text", nullable: true),
                    Warning = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_textContents_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "textContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleCharacteristics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    NameId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ArticleId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCharacteristics_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCharacteristics_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCharacteristics_textContents_NameId",
                        column: x => x.NameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCharacteristics_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Breadcrumbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: false),
                    href = table.Column<string>(type: "text", nullable: false),
                    ArticleModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breadcrumbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Breadcrumbs_Articles_ArticleModelId",
                        column: x => x.ArticleModelId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Breadcrumbs_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    originalId = table.Column<int>(type: "integer", nullable: false),
                    base_actionId = table.Column<int>(type: "integer", nullable: false),
                    previewId = table.Column<int>(type: "integer", nullable: false),
                    smallId = table.Column<int>(type: "integer", nullable: false),
                    mediumId = table.Column<int>(type: "integer", nullable: false),
                    largeId = table.Column<int>(type: "integer", nullable: false),
                    big_tileId = table.Column<int>(type: "integer", nullable: false),
                    bigId = table.Column<int>(type: "integer", nullable: false),
                    mobile_mediumId = table.Column<int>(type: "integer", nullable: false),
                    mobile_largeId = table.Column<int>(type: "integer", nullable: false),
                    ArticleModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Articles_ArticleModelId",
                        column: x => x.ArticleModelId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Images_Pictures_base_actionId",
                        column: x => x.base_actionId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_bigId",
                        column: x => x.bigId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_big_tileId",
                        column: x => x.big_tileId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_largeId",
                        column: x => x.largeId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_mediumId",
                        column: x => x.mediumId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_mobile_largeId",
                        column: x => x.mobile_largeId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_mobile_mediumId",
                        column: x => x.mobile_mediumId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_originalId",
                        column: x => x.originalId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_previewId",
                        column: x => x.previewId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Pictures_smallId",
                        column: x => x.smallId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    ArticleModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Articles_ArticleModelId",
                        column: x => x.ArticleModelId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    URL = table.Column<string>(type: "text", nullable: true),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    PreviewURL = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    ArticleModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Articles_ArticleModelId",
                        column: x => x.ArticleModelId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Videos_VideoTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "VideoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    href = table.Column<string>(type: "text", nullable: true),
                    ArticleCharacteristicId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_ArticleCharacteristics_ArticleCharacteristicId",
                        column: x => x.ArticleCharacteristicId,
                        principalTable: "ArticleCharacteristics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Values_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_ArticleId",
                table: "ArticleCharacteristics",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_CategoryId",
                table: "ArticleCharacteristics",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_NameId",
                table: "ArticleCharacteristics",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_TitleId",
                table: "ArticleCharacteristics",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_BrandId",
                table: "Articles",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DescriptionId",
                table: "Articles",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TitleId",
                table: "Articles",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Breadcrumbs_ArticleModelId",
                table: "Breadcrumbs",
                column: "ArticleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Breadcrumbs_TitleId",
                table: "Breadcrumbs",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DescriptionId",
                table: "Categories",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameId",
                table: "Categories",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ArticleModelId",
                table: "Images",
                column: "ArticleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_base_actionId",
                table: "Images",
                column: "base_actionId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_big_tileId",
                table: "Images",
                column: "big_tileId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_bigId",
                table: "Images",
                column: "bigId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_largeId",
                table: "Images",
                column: "largeId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_mediumId",
                table: "Images",
                column: "mediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_mobile_largeId",
                table: "Images",
                column: "mobile_largeId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_mobile_mediumId",
                table: "Images",
                column: "mobile_mediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_originalId",
                table: "Images",
                column: "originalId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_previewId",
                table: "Images",
                column: "previewId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_smallId",
                table: "Images",
                column: "smallId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ArticleModelId",
                table: "Tags",
                column: "ArticleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_textContents_OriginalLanguageId",
                table: "textContents",
                column: "OriginalLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_translations_LanguageId",
                table: "translations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_translations_TextContentId",
                table: "translations",
                column: "TextContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_ArticleCharacteristicId",
                table: "Values",
                column: "ArticleCharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_TitleId",
                table: "Values",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ArticleModelId",
                table: "Videos",
                column: "ArticleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TypeId",
                table: "Videos",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Breadcrumbs");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "translations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "ArticleCharacteristics");

            migrationBuilder.DropTable(
                name: "VideoTypes");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "textContents");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}

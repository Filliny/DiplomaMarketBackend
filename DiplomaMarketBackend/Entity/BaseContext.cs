using DiplomaMarketBackend.Entity.Models;
using Lessons3.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Entity
{
    public sealed class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options)
             : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<ArticleCharacteristic> ArticleCharacteristics { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<BreadcrumbsModel> Breadcrumbs { get; set; }
        public DbSet<CharacteristicValueModel> Values { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<PictureModel> Pictures { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<VideoTypeModel> VideoTypes { get; set; }
        public DbSet<WarningModel> Warnings { get; set; }

        //Translations
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> translations { get; set; }
        public DbSet<TextContent> textContents { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //optionsBuilder.UseMySql(
            //    "server=127.0.0.1;user=root;password=;database=oauth;",
            //    new MySqlServerVersion(new Version(8, 0, 0)));
        }
    }
}

﻿using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Entity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Entity
{
    public partial class BaseContext : IdentityDbContext<UserModel>
    {
        public BaseContext(DbContextOptions<BaseContext> options)
             : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //Database.EnsureCreated();
        }

        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<PermissionKeysModel> PermissionKeys { get; set; }
        public DbSet<CustomerGroupModel> CustomerGroups { get; set; }
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<ArticleCharacteristic> ArticleCharacteristics { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<BreadcrumbsModel> Breadcrumbs { get; set; }
        public DbSet<ValueModel> CharacteristicValues { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<PictureModel> Pictures { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<VideoTypeModel> VideoTypes { get; set; }
        public DbSet<WarningModel> Warnings { get; set; }
        public DbSet<CharacteristicGroupModel> CharacteristicGroups { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<SellerModel> Sellers { get; set; }
        public DbSet<ActionModel> Actions { get; set; }
        public DbSet<CertificateModel> Certificates { get; set; }
        public DbSet<PromoCodeModel> PromoCodes { get; set; }
        public DbSet<ReceiverModel> Receivers { get; set; } 
        public DbSet<PaymentTypesModel> PaymentTypes { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<BannerModel> Banners { get; set; }

        //Fixed Filters
        public DbSet<FixedFilterSettingsModel> FixedFilterSettings { get; set; }

        //Translations
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> translations { get; set; }
        public DbSet<TextContent> textContents { get; set; }



        //deliveries
        public DbSet<AreaModel> Areas { get; set; }
        public DbSet<CityModel> Cities { get; set; }
        public DbSet<DeliveryModel> Deliveries { get; set; }
        public DbSet<BranchModel> Branches { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //optionsBuilder.UseMySql(
            //    "server=127.0.0.1;user=root;password=;database=oauth;",
            //    new MySqlServerVersion(new Version(8, 0, 0)));
        }
    }
}

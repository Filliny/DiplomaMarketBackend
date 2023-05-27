using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DiplomaMarketBackend.Entity.Seeder
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new BaseContext(serviceProvider.GetRequiredService<DbContextOptions<BaseContext>>()))
            {
                await InitDeliveries(context);
                await InitRoles(serviceProvider, context);
                await InitPayments(context);
                await InitStaticFilters(context);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task InitRoles(IServiceProvider serviceProvider, BaseContext context)
        {
            if (context.Roles.Any()) return;

            //Generate permissions table and fill corresponding roles
            var permission_dictionary = new Dictionary<string, string>
            {
                ["Categories"] = "Категорії",
                ["Articles"] = "Товари",
                ["Filters"] = "Фільтри",
                ["Reviews"] = "Відгуки про товари",
                ["Orders"] = "Замовлення",
                ["Payments"] = "Періодичні",
                ["Returns"] = "Повернення",
                ["Certificates"] = "Сертифікати",
                ["Loyality"] = "Програма лояльності",
                ["Actions"] = "Акції",
                ["Clients"] = "Зареєстровані клієнти",
                ["ClientsGroups"] = "Групи клієнтів",
                ["Storage"] = "Склад замовлення",
                ["Customers"] = "Користувачі",
            };

            foreach(var line in permission_dictionary)
            {
                var premission = await context.Permissions.FirstOrDefaultAsync(p => p.Name.Equals(line.Key));
                if (premission == null){

                    premission = new PermissionModel
                    {
                        Name = line.Key,
                        Description = line.Value
                    };

                    context.Permissions.Add(premission);
                    context.SaveChanges();
                }


                var read_id =  await EnsureRole(serviceProvider, line.Key + "Read");

                if (read_id != null)
                {
                    var key = new PermissionKeysModel
                    {
                        Allowed = AllowedKey.read,
                        RoleId = read_id.Id,
                        Permission = premission
                    };

                    context.PermissionKeys.Add(key);
                    context.SaveChanges();
                }

                var write_id =   await EnsureRole(serviceProvider, line.Key + "Write");
                if (write_id != null)
                {
                    var key = new PermissionKeysModel
                    {
                        Allowed = AllowedKey.write,
                        RoleId = write_id.Id,
                        Permission = premission
                    };

                    context.PermissionKeys.Add(key);    
                    context.SaveChanges();  
                }

            }

            var roles = new string[] { "User", "Admin", };

            foreach (var role in roles)
            {
                await EnsureRole(serviceProvider, role);
            }
        }


        private static async Task<IdentityRole?> EnsureRole(IServiceProvider serviceProvider, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR = null;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            else
            {
                return await roleManager.FindByNameAsync(role);
            }

            if (IR.Succeeded )
            {
                return await roleManager.FindByNameAsync(role);
            }



            throw new Exception("Role create error");
        }


        private static async Task InitDeliveries(BaseContext context)
        {
            if (context != null)
            {

                if (context.Deliveries.Count() == 0)
                {

                    var np = CreateDelivery(context, "Нова пошта", "Новая почта");
                    context.Deliveries.Add(np);


                    var mist = CreateDelivery(context, "Meest", "Meest");
                    context.Deliveries.Add(mist);


                    var post = CreateDelivery(context, "Укрпошта", "Укрпочта");
                    context.Deliveries.Add(post);

                    await context.SaveChangesAsync();
                }

            }
        }

        private static DeliveryModel CreateDelivery(BaseContext context, string name_uk, string name_ru)
        {
            var content = TextContentHelper.CreateFull(context, name_uk, name_ru);
            //context.SaveChanges();
            //TextContentHelper.UpdateTextContent(context, name_ru, content.Id, "RU");

            var delivery = new DeliveryModel()
            {
                Name = content
            };

            return delivery;

        }


        private static async Task InitPayments(BaseContext context)
        {
            if (await context.PaymentTypes.AnyAsync()) return;

            int id = EnsurePayment(context, "Оплатити зараз", "Оплатить сейчас", null, null, null);
            EnsurePayment(context, "Карткою", "Картой", null, null, id);
            EnsurePayment(context, "Google Pay", "Google Pay", null, null, id);
            EnsurePayment(context, "Оплатити онлайн карткою “єПідтримка”", "Оплатить онлайн картой “єПідтримка”", null, null, id);

            EnsurePayment(context, "Безготівковими для юридичних осіб", "Безналичными для юридических лиц",
                "Оплата для юридичних осіб через розрахунковий рахунок", "Оплата для юридических лиц через расчетный счет", null);
            EnsurePayment(context, "Оплата під час отримання товару", "Оплата при получении товара", null, null, null);
            EnsurePayment(context, "Оплатити онлайн соціальною картою ”Пакунок малюка”", "Оплатить онлайн социальной картой “Пакунок малюка”", null, null, null);
            EnsurePayment(context, "Кредит та оплата частинами", "Кредит и оплата частями", "Оформлення кредитів у банках партнерів", "Оформление кредитов в банках партнеров", null);
        }

        private static int EnsurePayment(BaseContext context, string name_ua, string name_ru, string? desc_ua, string? desc_ru, int? parent_id)
        {
            var payment = new PaymentTypesModel
            {
                Name = TextContentHelper.CreateFull(context, name_ua, name_ru),
                Description = desc_ua == null || desc_ru == null ? null : TextContentHelper.CreateFull(context, desc_ua, desc_ru),
                ParentId = parent_id

            };

            context.PaymentTypes.Add(payment);
            context.SaveChanges();

            return payment.Id;
        }


        private async static Task InitStaticFilters(BaseContext context)
        {
            if (context.ArticleCharacteristics.Any(c => c.CategoryId == null)) return;

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Бренд", "Бренд"),
                CategoryId = null,
                filterType = FilterType.brand

            });

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Ціна", "Цена"),
                CategoryId = null,
                filterType = FilterType.price

            });

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Готовий до відправлення", "Готов к отправке"),
                CategoryId = null,
                show_in_filter = true,
                filterType = FilterType.shipping_ready,
                Values = new List<ValueModel> { new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Готовий до відправлення", "Готов к отправке"),
                }}

            });

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Товари з акціями", "Aкционные товары"),
                CategoryId = null,
                show_in_filter = true,
                filterType = FilterType.action,
                Values = new List<ValueModel> { new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Акція", "Акция"),
                }}
            });

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Програма лояльності", "Программа лояльности"),
                CategoryId = null,
                show_in_filter = true,
                filterType = FilterType.bonuses,
                Values = new List<ValueModel> { new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "З бонусами", "C бонусами"),
                }}
            });

            context.ArticleCharacteristics.Add(new ArticleCharacteristic
            {
                Title = TextContentHelper.CreateFull(context, "Статус товару", "Статус товара"),
                CategoryId = null,
                show_in_filter = true,
                filterType = FilterType.status,
                Values = new List<ValueModel> {
                new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Немає в наявності", "Нет в наличии"),
                },
                new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Є в наявності", "Есть в наличии"),
                },
                new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Закінчився", "Кончился"),
                },
                new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Закінчується", "Заканчивается"),
                },
                new ValueModel{
                    Title = TextContentHelper.CreateFull(context, "Очікується", "Ожидается"),
                },
                }
            });

            await context.SaveChangesAsync();
        }
    }
}

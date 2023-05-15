using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            }
        }

        private static async Task InitRoles(IServiceProvider serviceProvider, BaseContext context)
        {
            if (context.Roles.Any()) return;

            var roles = new string[] { "User", "Admin", "Manager" };

            foreach (var role in roles)
            {
                await EnsureRole(serviceProvider, role);
            }

        }


        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string role)
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

            if (IR != null)
            {
                return IR;
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
            if ( await context.PaymentTypes.AnyAsync()) return;

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
    }
}

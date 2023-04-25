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
            var content = TextContentHelper.CreateTextContent(context, name_uk, "UK");
            context.SaveChanges();
            TextContentHelper.UpdateTextContent(context, name_ru, content.Id, "RU");

            var delivery = new DeliveryModel()
            {
                Name = content
            };

            return delivery;

        }

    }
}
